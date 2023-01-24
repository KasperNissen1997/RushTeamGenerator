using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    /// <summary>
    /// A team represents a group of Players, often limited in size.
    /// </summary>
    public class Team
    {
        private static int identifierCount = 0;
        /// <summary>
        /// A unique integer that is used to differentiate between different instances of <see cref="Team"/>s.
        /// </summary>
        public int Identifier;

        /// <summary>
        /// The <see cref="Player"/>s that make up the team.
        /// </summary>
        public List<Player> Players { get; }
        /// <summary>
        /// How many <see cref="Player"/>s there are room for in the team.
        /// </summary>
        public int Capacity { get; }
        /// <summary>
        /// The current amount of <see cref="Player"/>s that are in the team.
        /// </summary>
        public int Size
        {
            get
            {
                return Players.Count;
            }
        }
        /// <summary>
        /// The cumulative rating of all <see cref="Player"/>s in the team.
        /// </summary>
        public int Rating 
        { 
            get
            {
                int rating = 0;

                foreach (Player player in Players)
                    rating += player.Rating;

                return rating;
            }
        }

        /// <summary>
        /// If all the <see cref="Player"/>s in the team speak danish.
        /// </summary>
        public bool SpeaksDanish
        {
            get
            {
                bool speaksDanish = true;

                foreach (Player player in Players)
                    if (!player.SpeaksDanish)
                        speaksDanish = false;

                return speaksDanish;
            }
        }
        /// <summary>
        /// If all the <see cref="Player"/>s in the team speak english.
        /// </summary>
        public bool SpeaksEnglish
        {
            get
            {
                bool speaksEnglish = true;

                foreach (Player player in Players)
                    if (!player.SpeaksEnglish)
                        speaksEnglish = false;

                return speaksEnglish;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="Team"/>.
        /// </summary>
        /// <param name="capacity">The maximum amount of <see cref="Player"/>s that the team can have.</param>
        public Team(int capacity)
        {
            Identifier = identifierCount++;

            Players = new();
            Capacity = capacity;
        }

        /// <summary>
        /// Adds all the <see cref="Player"/>s in the <paramref name="playerGroup"/> to the team.
        /// </summary>
        /// <param name="playerGroup">The <see cref="PlayerGroup"/> with all the <see cref="Player"/>s that should be added to the team.</param>
        public void AddPlayerGroup(PlayerGroup playerGroup)
        {
            foreach (Player player in playerGroup.Players)
                Players.Add(player);
        }
    }
}
