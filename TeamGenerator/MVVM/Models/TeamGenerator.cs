using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace TeamGenerator.MVVM.Models
{
    public class TeamGenerator
    {
        public bool TryGenerateTeams(List<Player> players, int teamCapacity, out List<Team> teams)
        {
            List<PlayerGroup> playerGroups = CreatePlayerGroups(players);
            playerGroups.Sort(); // sort players by rating, high to low

            int teamCount = players.Count / teamCapacity; // the amount of teams that can be made

            teams = new List<Team>();

            for (int i = 0; i < teamCount; i++)
                teams.Add(new Team(teamCapacity));

            int idealRating = GetPlayerRatingSum(players) / teamCount; // TODO: Change this to account for cutoff players

            int loopCount = 0;
            while (playerGroups.Count > 0)
            {
                int teamIteration = loopCount / teamCount + 1; // how many times have we looped the teams, starting from 1?
                int currentIdealRating = idealRating * (1 / teamIteration);

                Team currentTeam = teams[loopCount % teamCount];
                PlayerGroup currentPlayerGroup = playerGroups[loopCount % playerGroups.Count];

                if (teamIteration == 0) // first iteration only
                {
                    if (TryAddPlayerGroup(currentTeam, currentPlayerGroup)) // can we add the currentPlayerGroup to the currentTeam
                        playerGroups.Remove(currentPlayerGroup); // if yes, then remove the currentPlayerGroup
                    
                    continue;
                }

                if (currentTeam.Size >= teamIteration && currentTeam.Rating < currentIdealRating)
                {
                    if (TryAddPlayerGroup(currentTeam, playerGroups[loopCount]))
                        playerGroups.Remove(currentPlayerGroup);

                    continue;
                }

                if (currentTeam.Size < teamIteration)
                {
                    if (TryAddPlayerGroup(currentTeam, playerGroups[loopCount]))
                        playerGroups.Remove(currentPlayerGroup);

                    continue;
                }

                loopCount++;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a list of <see cref="PlayerGroup"/> containing all players in <paramref name="players"/>.
        /// </summary>
        /// <param name="players">The list of players.</param>
        /// <returns>A <see cref="List{T}"/> with <see cref="PlayerGroup"/>s containing all provided <see cref="Player"/>s from <paramref name="players"/>.</returns>
        private List<PlayerGroup> CreatePlayerGroups(List<Player> players)
        {
            List<PlayerGroup> playerGroups = new();

            foreach (Player player in new List<Player>(players))
            {
                PlayerGroup playerGroup = new();

                playerGroup.Players.Add(player);
                players.Remove(player);

                foreach (Player includedPlayer in player.Inclusions)
                {
                    playerGroup.Players.Add(includedPlayer);
                    players.Remove(includedPlayer);
                }

                playerGroups.Add(playerGroup);
            }

            return playerGroups;
        }

        private int GetPlayerRatingSum(List<Player> players)
        {
            int ratingSum = 0;

            foreach (Player player in players) 
                ratingSum += player.Rating;

            return ratingSum;
        }

        private bool TryAddPlayerGroup(Team team, PlayerGroup playerGroup)
        {
            if (team.Size >= team.Capacity) // is there room for any more players?
                return false;

            if (team.Size + playerGroup.Size > team.Capacity) // is there room for the playerGroup?
                return false;

            if ((!team.SpeaksDanish && !playerGroup.SpeaksEnglish) || (!team.SpeaksEnglish && !playerGroup.SpeaksDanish)) // can they communicate?
                return false;

            foreach (PlayerGroup teamPlayerGroup in team.PlayerGroups)
                foreach (Player teamPlayer in teamPlayerGroup.Players)
                    foreach (Player groupPlayer in playerGroup.Players)
                        if (teamPlayer.Exclusions.Contains(groupPlayer)) // are any of the playerGroup players excluded by a player on the team?
                            return false;

            team.PlayerGroups.Add(playerGroup);
            return true;
        }
    }
}
