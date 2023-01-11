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
                int teamLoopCount = loopCount / teamCount + 1; // the amount of times we have looped over each team, starting from 1
                int currentIdealRating = (int) (idealRating * ((float) teamCount / teamLoopCount)); // the rating we would expect a team to have at this point

                Team currentTeam = teams[loopCount % teamCount];
                PlayerGroup currentPlayerGroup = playerGroups[loopCount % playerGroups.Count];



                if (teamLoopCount == 0) // first iteration only
                {
                    if (TryAddPlayerGroup(currentTeam, currentPlayerGroup)) // can we add the currentPlayerGroup to the currentTeam?
                        playerGroups.Remove(currentPlayerGroup); // if yes, then remove the currentPlayerGroup
                    
                    continue;
                }

                if (currentTeam.Size >= teamLoopCount && currentTeam.Rating < currentIdealRating)
                {
                    if (TryAddPlayerGroup(currentTeam, playerGroups[loopCount]))
                        playerGroups.Remove(currentPlayerGroup);

                    continue;
                }

                if (currentTeam.Size < teamLoopCount)
                {
                    if (TryAddPlayerGroup(currentTeam, playerGroups[loopCount]))
                        playerGroups.Remove(currentPlayerGroup);

                    continue;
                }

                loopCount++;
            }

            for (int i = 0; playerGroups.Count > 0; i++)
            {
                int teamLoopCount = i / teamCount; // the amount of times we have looped over each team

                // assign first playerGroups

                int currentIdealRating = (int) (idealRating * ((float) teamLoopCount / teamCapacity)); // the rating we would expect a team to have at this point in the loop

                foreach (Team team in teams)
                {
                    foreach (PlayerGroup playerGroup in playerGroups)
                        if (TryAddPlayerGroup(team, playerGroup))

                }
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
