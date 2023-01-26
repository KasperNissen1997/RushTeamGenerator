﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Models
{
    /// <summary>
    /// This class is used to generate groups of <see cref="Team"/>s, given a dataset of <see cref="Player"/>s.
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Attempts to generate a group of <see cref="Team"/>s from <paramref name="players"/>. <br/>
        /// If succesfull, the new teams are stored in <paramref name="teams"/>, and this returns <see langword="true"/>.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s the <see cref="Team"/>s should be made up of.</param>
        /// <param name="teamCapacity">The capacity, aka maximum size, of each <see cref="Team"/>.</param>
        /// <param name="allowedRatingDeviance">How much the rating can deviate from the calcualted targeted rating.</param>
        /// <param name="teams">The generated <see cref="Team"/>s if generation was succesfull; otherwise an incomplete list of <see cref="Team"/>s.</param>
        /// <returns><see langword="true"/> if the generation of teams succeeded; otherwise <see langword="false"/>.</returns>
        public bool TryGenerateTeams(List<Player> players, int teamCapacity, int allowedRatingDeviance, out List<Team> teams)
        {
            List<PlayerGroup> singlePlayerGroups = CreatePlayerGroups(players);
            singlePlayerGroups.Sort(); // sort players by rating, low to high

            List<PlayerGroup> multiplePlayerGroups = new();

            foreach (PlayerGroup playerGroup in new List<PlayerGroup>(singlePlayerGroups))
                if (playerGroup.Size > 1)
                {
                    multiplePlayerGroups.Add(playerGroup);

                    singlePlayerGroups.Remove(playerGroup);
                }

            int teamCount = players.Count / teamCapacity; // the amount of teams that can be made

            teams = new List<Team>();

            for (int i = 0; i < teamCount; i++)
                teams.Add(new Team(teamCapacity));

            List<Team> finalTeams = new(teams); // TODO: CHECK UP

            int ratingSum = 0; // the cumulative sum of all players that are going to be part of teams

            for (int i = 0; i < teamCount * teamCapacity; i++) // use for-loop to only count the amount of players that are going to be part of teams
                ratingSum += players[i].Rating;

            int targetRating = ratingSum / teamCount; // the targeted rating that each team will want to have

            int currentTargetSize = 1; // the expected starting size of teams at this stage
            int currentTargetRating = targetRating / teamCapacity; // the expected starting rating of teams at this stage

            #region Old Algorithm
            //for (int i = 1; playerGroups.Count > 0; i++) // start with i = 1, and increment it for each loop, untill all playerGroups have been assigned a team
            //{
            //    if (i == teamCapacity + 3) // change 3 to a higher number to continue searching
            //        return false;

            //    if (i == 1) // for the first iteration of the loop...
            //    {
            //        foreach (Team team in teams) // ... naively assign the largest playerGroups to the teams
            //        {
            //            team.AddPlayerGroup(playerGroups[0]);

            //            playerGroups.RemoveAt(0);
            //        }

            //        continue; // start second iteration
            //    }

            //    if (i <= teamCapacity) // if we are still limiting the amount of players assigned to each team, as we do in the start of the loop, then ...
            //    {
            //        currentTargetSize = i; // ... increase our targeted size for our teams, so that they can accept more players
            //        currentTargetRating = (int) (targetRating * ((double) i / teamCapacity)); // ... recalculate the expected rating of teams at this stage
            //    }

            //    foreach (Team team in teams) // for each team ...
            //        if (i >= teamCapacity + 2)
            //        {
            //            if (TryFindEligiblePlayerGroup(team, playerGroups, currentTargetRating, currentTargetRating / 2, teamCapacity, out PlayerGroup eligiblePlayerGroup))
            //            {
            //                team.AddPlayerGroup(eligiblePlayerGroup);

            //                playerGroups.Remove(eligiblePlayerGroup);
            //            }
            //        }
            //        else
            //        {
            //            // attempt to find and assign an eligible player amongst the players that haven't been assigned a team yet
            //            if (TryFindEligiblePlayerGroup(team, playerGroups, currentTargetRating, allowedRatingDeviance, currentTargetSize, out PlayerGroup eligiblePlayerGroup))
            //            {
            //                team.AddPlayerGroup(eligiblePlayerGroup);

            //                playerGroups.Remove(eligiblePlayerGroup);
            //            }
            //        }
            //}
            #endregion

            // TODO: Fool-proof this loop baby boi <3
            for (int i = 0; i < multiplePlayerGroups.Count; i++)
            {
                teams[i % teamCapacity].AddPlayerGroup(multiplePlayerGroups[i]);
            }

            foreach (Team team in teams)
            {
                if (team.Size == 0)
                {
                    PlayerGroup lowestAverageRatedPlayerGroup = singlePlayerGroups[singlePlayerGroups.Count - 1];

                    team.AddPlayerGroup(lowestAverageRatedPlayerGroup);

                    singlePlayerGroups.Remove(lowestAverageRatedPlayerGroup);
                }
            }

            while (singlePlayerGroups.Count != 0)
            {
                // get team with lowest average
                teams.Sort();

                Team lowestAverageRatedTeam = teams[0];

                if (TryFindEligiblePlayerGroup(lowestAverageRatedTeam, singlePlayerGroups, out PlayerGroup eligiblePlayerGroup))
                {
                    lowestAverageRatedTeam.AddPlayerGroup(eligiblePlayerGroup);

                    singlePlayerGroups.Remove(eligiblePlayerGroup);

                    if (lowestAverageRatedTeam.Size == teamCapacity)
                        teams.Remove(lowestAverageRatedTeam);
                }
                else if (lowestAverageRatedTeam.Size != lowestAverageRatedTeam.Capacity)
                    throw new ArgumentException();
            }

            #region Analytics
            List<int> teamRatings = new List<int>();

            foreach (Team team in finalTeams)
                teamRatings.Add(team.Rating);

            int lowestTeamRating = teamRatings.Min();
            double averageTeamRating = teamRatings.Average();
            int highestTeamRating = teamRatings.Max();

            // TODO: Calculate in-team player rating deviation as int and percentage

            Trace.WriteLine("Teams generated succesfully! Analytics below:\n\n" +
                $"Lowest team rating: {lowestTeamRating}\n" +
                $"Average team rating: {averageTeamRating}\n" +
                $"Highest team rating: {highestTeamRating}");
            #endregion

            return true;
        }

        /// <summary>
        /// Creates a list of <see cref="PlayerGroup"/> containing all players in <paramref name="players"/>.
        /// </summary>
        /// <param name="players">The list of players.</param>
        /// <returns>A <see cref="List{T}"/> with <see cref="PlayerGroup"/>s containing all provided <see cref="Player"/>s from <paramref name="players"/>.</returns>
        private List<PlayerGroup> CreatePlayerGroups(List<Player> players)
        {
            List<PlayerGroup> playerGroups = new();

            List<Player> playersCopy = new(players);

            foreach (Player player in new List<Player>(playersCopy))
            {
                if (!playersCopy.Contains(player))
                    continue;

                PlayerGroup playerGroup = new();

                playerGroup.Players.Add(player);
                playersCopy.Remove(player);

                foreach (Player includedPlayer in player.GetPlayerInclusions())
                {
                    playerGroup.Players.Add(includedPlayer);
                    playersCopy.Remove(includedPlayer);
                }

                playerGroups.Add(playerGroup);
            }

            return playerGroups;
        }

        /// <summary>
        /// Attempts to find an eligible <see cref="PlayerGroup"/> to add to <paramref name="team"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> the <see cref="PlayerGroup"/>s should be checked for eligibility against.</param>
        /// <param name="playerGroups">The <see cref="PlayerGroup"/>s that will be checked for eligibility.</param>
        /// <param name="targetRating">The targeted rating of <paramref name="team"/>.</param>
        /// <param name="allowedRatingDeviance">The maximum allowed deviation from <paramref name="targetRating"/>.</param>
        /// <param name="targetSize">The targeted size of <paramref name="team"/>.</param>
        /// <param name="eligiblePlayerGroup">If an eligible <see cref="PlayerGroup"/> is found, then this will be it; otherwise, it will hold an invalid <see cref="PlayerGroup"/>.</param>
        /// <returns><see langword="true"/> if an eligible <see cref="PlayerGroup"/> was found; otherwise <see langword="false"/>.</returns>
        private bool TryFindEligiblePlayerGroup(Team team, List<PlayerGroup> playerGroups, int targetRating, int allowedRatingDeviance, int targetSize, out PlayerGroup eligiblePlayerGroup)
        {
            eligiblePlayerGroup = null;

            List<PlayerGroup> eligiblePlayerGroups = new();

            foreach (PlayerGroup playerGroup in new List<PlayerGroup>(playerGroups))
            {
                // if they satisfy all the requirements ...
                if (CheckPlayerGroupEligibility(team, playerGroup, targetRating, allowedRatingDeviance, targetSize))
                {
                    // ... then add them to our possiblePlayerGroups
                    eligiblePlayerGroups.Add(playerGroup);
                }
            }

            if (eligiblePlayerGroups.Count != 0)
            {
                int highestAcquaintenceCount = 0;

                foreach (PlayerGroup playerGroup in eligiblePlayerGroups)
                {
                    int acquaintenceCount = GetAcquaintenceCount(team, playerGroup);

                    if (acquaintenceCount > highestAcquaintenceCount || highestAcquaintenceCount == 0)
                    {
                        eligiblePlayerGroup = playerGroup;
                        highestAcquaintenceCount = acquaintenceCount;
                    }
                }

                return true;
            }

            // no eligible playerGroup found
            return false;
        }

        /// <summary>
        /// Checks if <paramref name="playerGroup"/> can join <paramref name="team"/> without any conflicts.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> that <paramref name="playerGroup"/> should be checked for eligibility against.</param>
        /// <param name="playerGroup">The <see cref="PlayerGroup"/> that will be checked for eligibility.</param>
        /// <param name="targetRating">The targeted rating of <paramref name="team"/>.</param>
        /// <param name="allowedRatingDeviance">The maximum allowed deviation from <paramref name="targetRating"/>.</param>
        /// <param name="targetSize">The targeted size of <paramref name="team"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="playerGroup"/> is eligible; otherwise <see langword="false"/>.</returns>
        private bool CheckPlayerGroupEligibility(Team team, PlayerGroup playerGroup, int targetRating, int allowedRatingDeviance, int targetSize)
        {
            if (team.Size >= targetSize)
                return false;

            // is there room for any more players?
            if (team.Size >= team.Capacity)
                return false;

            // is there room for the playerGroup?
            if (team.Size + playerGroup.Size > team.Capacity)
                return false;

            if (team.Size + playerGroup.Size > targetSize)
                return false;

            // can they communicate?
            if ((!team.SpeaksDanish && !playerGroup.SpeaksEnglish) 
                || (!team.SpeaksEnglish && !playerGroup.SpeaksDanish))
                return false;

            // are any of the groupPlayers excluded by any of the teamPlayers?
            foreach (Player teamPlayer in team.Players)
                foreach (Player groupPlayer in playerGroup.Players)
                    if (teamPlayer.Exclusions.Contains(groupPlayer))
                        return false;

            // is the groups added rating within the allowed range of deviation?
            if (team.Rating + playerGroup.Rating < targetRating - allowedRatingDeviance 
                || team.Rating + playerGroup.Rating > targetRating + allowedRatingDeviance)
                return false;

            // all requirements are satisfied!
            return true;
        }

        private int GetAcquaintenceCount(Team team, PlayerGroup playerGroup)
        {
            int acquaintenceCount = 0;

            foreach (Player teamPlayer in team.Players)
                foreach (Player groupPlayer in playerGroup.Players)
                    if (teamPlayer.Acquaintences.Contains(groupPlayer))
                        acquaintenceCount++;

            return acquaintenceCount;
        }

        private bool TryFindEligiblePlayerGroup(Team team, List<PlayerGroup> playerGroups, out PlayerGroup eligiblePlayerGroup)
        {
            eligiblePlayerGroup = null;

            foreach (PlayerGroup playerGroup in playerGroups)
            {
                if (CheckPlayerGroupEligibility(team, playerGroup))
                {
                    eligiblePlayerGroup = playerGroup;
                    return true;
                }
            }

            return false;
        }

        private bool CheckPlayerGroupEligibility(Team team, PlayerGroup playerGroup)
        {
            // is there room for any more players?
            if (team.Size >= team.Capacity)
                return false;

            // is there room for the playerGroup?
            if (team.Size + playerGroup.Size > team.Capacity)
                return false;

            // can they communicate?
            if ((!team.SpeaksDanish && !playerGroup.SpeaksEnglish)
                || (!team.SpeaksEnglish && !playerGroup.SpeaksDanish))
                return false;

            // are any of the groupPlayers excluded by any of the teamPlayers?
            foreach (Player teamPlayer in team.Players)
                foreach (Player groupPlayer in playerGroup.Players)
                    if (teamPlayer.Exclusions.Contains(groupPlayer))
                        return false;

            // all requirements are satisfied!
            return true;
        }
    }
}
