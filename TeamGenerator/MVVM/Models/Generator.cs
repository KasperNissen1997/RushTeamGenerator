using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TeamGenerator.MVVM.Models
{
    public class Generator
    {
        public bool TryGenerateTeams(List<Player> players, int teamCapacity, int allowedRatingDeviance, out List<Team> teams)
        {
            List<PlayerGroup> playerGroups = CreatePlayerGroups(players);
            playerGroups.Sort(); // sort players by rating, low to high

            int teamCount = players.Count / teamCapacity; // the amount of teams that can be made

            teams = new List<Team>();

            for (int i = 0; i < teamCount; i++)
                teams.Add(new Team(teamCapacity));

            int ratingSum = 0;

            for (int i = 0; i < teamCount * teamCapacity; i++)
                ratingSum += players[i].Rating;

            int targetRating = ratingSum / teamCount;

            int currentTargetSize = 1; // the expected starting size of teams at this stage
            int currentTargetRating = targetRating / teamCapacity; // the expected starting rating of teams at this stage

            for (int i = 1; playerGroups.Count > 0; i++)
            {
                if (i == 1)
                {
                    foreach (Team team in teams)
                    {
                        team.AddPlayerGroup(playerGroups[0]);

                        playerGroups.RemoveAt(0);
                    }

                    continue;
                }

                if (i <= teamCapacity)
                {
                    currentTargetSize = i;
                    currentTargetRating = (int) (targetRating * ((double) i / teamCapacity)); // the expected rating of teams at this stage
                }

                foreach (Team team in teams)
                    if (TryFindEligiblePlayerGroup(team, playerGroups, currentTargetRating, allowedRatingDeviance, currentTargetSize, out PlayerGroup eligiblePlayerGroup))
                    {
                        team.AddPlayerGroup(eligiblePlayerGroup);

                        playerGroups.Remove(eligiblePlayerGroup);
                    }

                if (i >= teamCapacity + 3) // change 3 to a higher number to continue searching
                    return false;
            }

            #region Analytics
            List<int> teamRatings = new List<int>();

            foreach (Team team in teams)
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

        private bool TryFindEligiblePlayerGroup(Team team, List<PlayerGroup> playerGroups, int targetRating, int allowedRatingDeviance, int targetSize, out PlayerGroup eligiblePlayerGroup)
        {
            eligiblePlayerGroup = null;

            foreach (PlayerGroup playerGroup in new List<PlayerGroup>(playerGroups))
            {
                // check if the playerGroup satisfies all requirements
                if (CheckPlayerGroupEligibility(team, playerGroup, targetRating, allowedRatingDeviance, targetSize))
                {
                    // if they satisfy all the requirements, return them!
                    eligiblePlayerGroup = playerGroup;
                    return true;
                }
            }

            // no eligible playerGroup found
            return false;
        }

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
    }
}
