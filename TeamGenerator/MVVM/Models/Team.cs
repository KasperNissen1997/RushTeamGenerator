﻿using System;
using System.Collections.Generic;
using TeamGenerator.MVVM.Models.Repositories;

namespace TeamGenerator.MVVM.Models
{
    /// <summary>
    /// A team represents a group of Players, often limited in size.
    /// </summary>
    public class Team : IComparable<Team>
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
        /// The average rating of all <see cref="Player"/>s in the team.
        /// </summary>
        public double AveragePlayerRating
        {
            get
            {
                double ratingSum = 0;

                foreach (Player player in Players)
                    ratingSum += player.Rating;

                return ratingSum / Players.Count;
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
        /// Creates a new instance of <see cref="Team"/>.
        /// </summary>
        /// <param name="capacity">The maximum amount of <see cref="Player"/>s that the team can have.</param>
        /// <param name="players">The <see cref="Player"/>s that make up the team.</param>
        public Team(int capacity, List<Player> players)
        {
            Identifier = identifierCount++;

            Players = players;
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

        public int CompareTo(Team? other)
        {
            if (other is null)
                return 1;

            if (other is Team otherTeam)
            {
                return AveragePlayerRating.CompareTo(otherTeam.AveragePlayerRating); // inverse this so it it sorts in descending order via List.Sort()
            }

            throw new NotImplementedException();
        }
    }
}
