using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using TeamGenerator.MVVM.ViewModels;
using TeamGenerator.MVVM.Models.Repositories;
using System.Text;

namespace TeamGenerator.MVVM.Models
{
    /// <summary>
    /// This class is used to generate groups of <see cref="Team"/>s, given a dataset of <see cref="Player"/>s.
    /// </summary>
    public static class Generator
    {
        public struct GenerationResults
        {
            public bool success;
            public bool partialTeams;
            public int optimizationIterationsPerformed;

            public int lowestTeamRating;
            public double averageTeamRating;
            public int highestTeamRating;

            public int largestTeamSize;
            public int smallestTeamSize;

            public GenerationResults(List<Team> teams, int allowedRatingDeviance, int optimizationIterationsPerformed)
            {
                partialTeams = false;
                this.optimizationIterationsPerformed = optimizationIterationsPerformed;

                int teamSize = teams[0].Size;

                List<int> teamRatings = new();

                largestTeamSize = int.MinValue;
                smallestTeamSize = int.MaxValue;

                foreach (Team team in teams)
                {
                    if (team.Size != teamSize)
                        partialTeams = true;

                    teamRatings.Add(team.Rating);

                    if (team.Size > largestTeamSize)
                        largestTeamSize = team.Size;

                    if (team.Size < smallestTeamSize)
                        smallestTeamSize = team.Size;
                }

                lowestTeamRating = teamRatings.Min();
                averageTeamRating = teamRatings.Average();
                highestTeamRating = teamRatings.Max();

                if (lowestTeamRating + allowedRatingDeviance >= highestTeamRating)
                    success = true;
                else
                    success = false;
            }

            public override string ToString()
            {
                StringBuilder sb = new();

                if (success)
                {
                    sb.AppendLine($"The team generation was a success!");
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine($"The team generation was unfortunately unsuccessful!");
                    sb.AppendLine();
                }

                sb.AppendLine($"A total of {optimizationIterationsPerformed} optimization iterations were performed, before this result was reached.");
                sb.AppendLine();

                sb.AppendLine($"Lowest Team Rating: {lowestTeamRating}");
                sb.AppendLine($"Average Team Rating: {Math.Round(averageTeamRating, 2)}");
                sb.AppendLine($"Highest Team Rating: {highestTeamRating}");
                sb.AppendLine();

                if (partialTeams)
                {
                    sb.AppendLine("Be aware that not all teams generated are of equal size.");
                    sb.AppendLine();
                    sb.AppendLine($"Smallest Team Size: {smallestTeamSize}");
                    sb.AppendLine($"Largest Team Size: {largestTeamSize}");
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Attempts to generate a group of <see cref="Team"/>s from <paramref name="players"/>. <br/>
        /// If succesfull, the new teams are stored in <paramref name="teams"/>, and this returns <see langword="true"/>.
        /// </summary>
        /// <param name="players">The <see cref="Player"/>s the <see cref="Team"/>s should be made up of.</param>
        /// <param name="teamCapacity">The capacity, aka maximum size, of each <see cref="Team"/>.</param>
        /// <param name="allowedRatingDeviance">How much difference in rating the lowest rated generated team is allowed to deviate from the highest rated generated team.</param>
        /// <param name="teams">The generated <see cref="Team"/>s if generation was succesfull; otherwise an incomplete list of <see cref="Team"/>s.</param>
        /// <param name="optimizationIterations">The amount of attempts to optimizie teams the algorithm should perform. The higher the number, the longer it takes to run, but the more 'even' teams become.</param>
        /// <returns>A struct of type <see cref="GenerationResults"/>, which will hold various analytical analysis performed on the resulting generated teams.</returns>
        public static GenerationResults TryGenerateTeams(List<Player> players, int teamCapacity, int allowedRatingDeviance, out List<Team> teams, int optimizationIterations)
        {
            if (players.Count < teamCapacity)
            {
                teams = new List<Team>()
                {
                    new Team(teamCapacity, players)
                };

                return new GenerationResults(teams, allowedRatingDeviance, 0);
            }

            // Initialize the playerGroups
            List<PlayerGroup> singlePlayerGroups = CreatePlayerGroups(players);
            singlePlayerGroups.Sort(); // sort players by rating, low to high

            List<PlayerGroup> multiplePlayerGroups = new();

            foreach (PlayerGroup playerGroup in new List<PlayerGroup>(singlePlayerGroups))
                if (playerGroup.Size > 1)
                {
                    multiplePlayerGroups.Add(playerGroup);

                    singlePlayerGroups.Remove(playerGroup);
                }

            // Initialize the teams
            int teamCount = players.Count / teamCapacity;

            teams = new List<Team>();

            for (int i = 0; i < teamCount; i++)
                teams.Add(TeamRepository.Instance.Create(teamCapacity));

            List<Team> finalTeams = new(teams); // Save a copy of the teams list

            // TODO: Check to see if multiple bad playerGroups are teamed together
            foreach (PlayerGroup multiplePlayerGroup in multiplePlayerGroups)
            {
                // get worst team by average player rating
                teams.Sort();

                if (teams[0].Size + multiplePlayerGroup.Size <= teamCapacity)
                    teams[0].AddPlayerGroup(multiplePlayerGroup);
                else
                    Trace.WriteLine("SEE MEEEEEEEEEE");
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

            Team lowestAverageRatedTeam;
            Team highestAverageRatedTeam;
            while (singlePlayerGroups.Count != 0)
            {
                if (teams.Count == 0)
                {
                    break;
                }

                // get team with lowest average
                teams.Sort();

                lowestAverageRatedTeam = teams[0];

                if (TryFindEligiblePlayerGroup(lowestAverageRatedTeam, singlePlayerGroups, out PlayerGroup eligiblePlayerGroup))
                {
                    lowestAverageRatedTeam.AddPlayerGroup(eligiblePlayerGroup);

                    singlePlayerGroups.Remove(eligiblePlayerGroup);

                    if (lowestAverageRatedTeam.Size == teamCapacity)
                        teams.Remove(lowestAverageRatedTeam);
                }
                else
                    teams.Remove(lowestAverageRatedTeam);
            }

            teams = finalTeams;
            int optimizationsPerformed = 0;
            for (; optimizationsPerformed < optimizationIterations; optimizationsPerformed++)
                if (!OptimizeTeams(teams))
                    break;

            return new GenerationResults(teams, allowedRatingDeviance, optimizationsPerformed);
        }

        /// <summary>
        /// Creates a list of <see cref="PlayerGroup"/> containing all players in <paramref name="players"/>.
        /// </summary>
        /// <param name="players">The list of players.</param>
        /// <returns>A <see cref="List{T}"/> with <see cref="PlayerGroup"/>s containing all provided <see cref="Player"/>s from <paramref name="players"/>.</returns>
        private static List<PlayerGroup> CreatePlayerGroups(List<Player> players)
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

        private static int GetAcquaintenceCount(Team team, PlayerGroup playerGroup)
        {
            int acquaintenceCount = 0;

            foreach (Player teamPlayer in team.Players)
                foreach (Player groupPlayer in playerGroup.Players)
                    if (teamPlayer.Acquaintences.Contains(groupPlayer))
                        acquaintenceCount++;

            return acquaintenceCount;
        }

        /// <summary>
        /// Attempts to find an eligible <see cref="PlayerGroup"/> to add to <paramref name="team"/>.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> the <see cref="PlayerGroup"/>s should be checked for eligibility against.</param>
        /// <param name="playerGroups">The <see cref="PlayerGroup"/>s that will be checked for eligibility.</param>
        /// <param name="eligiblePlayerGroup">If an eligible <see cref="PlayerGroup"/> is found, then this will be it; otherwise, it will hold an invalid <see cref="PlayerGroup"/>.</param>
        /// <returns><see langword="true"/> if an eligible <see cref="PlayerGroup"/> was found; otherwise <see langword="false"/>.</returns>
        private static bool TryFindEligiblePlayerGroup(Team team, List<PlayerGroup> playerGroups, out PlayerGroup eligiblePlayerGroup)
        {
            List<PlayerGroup> eligiblePlayerGroups = new List<PlayerGroup>();

            eligiblePlayerGroup = null;

            foreach (PlayerGroup playerGroup in playerGroups)
            {
                if (CheckPlayerGroupEligibility(team, playerGroup))
                {
                    eligiblePlayerGroups.Add(playerGroup);
                }
            }

            if (eligiblePlayerGroups.Count > 0)
            {
                eligiblePlayerGroup = eligiblePlayerGroups.First();
                int highestAcquaintenceCount = GetAcquaintenceCount(team, eligiblePlayerGroup);

                foreach (PlayerGroup playerGroup in eligiblePlayerGroups)
                    if (GetAcquaintenceCount(team, playerGroup) > highestAcquaintenceCount)
                    {
                        eligiblePlayerGroup = playerGroup;
                        highestAcquaintenceCount = GetAcquaintenceCount(team, eligiblePlayerGroup);
                    }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if <paramref name="playerGroup"/> can join <paramref name="team"/> without any conflicts.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> that <paramref name="playerGroup"/> should be checked for eligibility against.</param>
        /// <param name="playerGroup">The <see cref="PlayerGroup"/> that will be checked for eligibility.</param>
        /// <returns><see langword="true"/> if <paramref name="playerGroup"/> is eligible; otherwise <see langword="false"/>.</returns>
        private static bool CheckPlayerGroupEligibility(Team team, PlayerGroup playerGroup)
        {
            // is there room for any more players?
            if (team.Size >= team.Capacity)
                return false;

            // is there room for the playerGroup?
            if (team.Size + playerGroup.Size > team.Capacity)
                return false;

            // can they communicate?
            if (!((team.SpeaksDanish && playerGroup.SpeaksDanish)
                || (team.SpeaksEnglish && playerGroup.SpeaksEnglish)))
                return false;

            // are any of the groupPlayers excluded by any of the teamPlayers?
            foreach (Player teamPlayer in team.Players)
                foreach (Player groupPlayer in playerGroup.Players)
                    if (teamPlayer.Exclusions.Contains(groupPlayer))
                        return false;

            // all requirements are satisfied!
            return true;
        }

        private static bool OptimizeTeams(List<Team> teams)
        {
            for (int i = 0; i < teams.Count / 2; i++)
            {
                for (int j = 0; j < teams.Count - 1 - i; j++)
                {
                    teams.Sort();

                    Team worseTeam = teams[i];
                    Team betterTeam = teams[teams.Count - 1 - j];

                    int currentRatingDeviance = Math.Abs(betterTeam.Rating - worseTeam.Rating);

                    List<Player> availableWorsePlayers = new();
                    List<Player> availableBetterPlayers = new();

                    foreach (Player player in worseTeam.Players)
                        if (CanRemovePlayer(worseTeam, player))
                            availableWorsePlayers.Add(player);

                    foreach (Player player in betterTeam.Players)
                        if (CanRemovePlayer(betterTeam, player))
                            availableBetterPlayers.Add(player);

                    if (availableWorsePlayers.Count == 0 || availableBetterPlayers.Count == 0)
                        continue;

                    Player worstPlayer = availableWorsePlayers[0];
                    Player bestPlayer = availableBetterPlayers[0];

                    foreach (Player player in availableWorsePlayers)
                        if (player.Rating < worstPlayer.Rating)
                            worstPlayer = player;

                    foreach (Player player in availableBetterPlayers)
                        if (player.Rating > bestPlayer.Rating)
                            bestPlayer = player;

                    Team worseTeamCopy = new(worseTeam.Capacity, new(worseTeam.Players));
                    Team betterTeamCopy = new(betterTeam.Capacity, new(betterTeam.Players));
                    worseTeamCopy.Players.Remove(worstPlayer);
                    betterTeamCopy.Players.Remove(bestPlayer);

                    PlayerGroup worstSinglePlayerGroup = new();
                    PlayerGroup bestSinglePlayerGroup = new();
                    worstSinglePlayerGroup.Players.Add(worstPlayer);
                    bestSinglePlayerGroup.Players.Add(bestPlayer);

                    if (CheckPlayerGroupEligibility(worseTeamCopy, bestSinglePlayerGroup) && CheckPlayerGroupEligibility(betterTeamCopy, worstSinglePlayerGroup))
                    {
                        worseTeamCopy.AddPlayerGroup(bestSinglePlayerGroup);
                        betterTeamCopy.AddPlayerGroup(worstSinglePlayerGroup);

                        int possibleRatingDeviance = Math.Abs(betterTeamCopy.Rating - worseTeamCopy.Rating);
                        if (possibleRatingDeviance < currentRatingDeviance)
                        {
                            Trace.WriteLine("Improved rating deviance by " + (currentRatingDeviance - possibleRatingDeviance) + "!");

                            worseTeam.Players.Remove(worstPlayer);
                            betterTeam.Players.Remove(bestPlayer);

                            worseTeam.AddPlayerGroup(bestSinglePlayerGroup);
                            betterTeam.AddPlayerGroup(worstSinglePlayerGroup);

                            return true;
                        }
                    }

                }
            }

            return false;
        }

        private static bool CanRemovePlayer(Team team, Player player)
        {
            if (player.Inclusions.Count > 0)
                return false;
            
            Team teamCopy = new Team(team.Capacity, new(team.Players));

            teamCopy.Players.Remove(player);

            if (!teamCopy.SpeaksDanish && !teamCopy.SpeaksEnglish)
                return false;

            return true;
        }
    }
}
