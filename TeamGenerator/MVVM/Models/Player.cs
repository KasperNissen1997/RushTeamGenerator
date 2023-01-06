using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    public class Player
    {
        private static int identifierCount = 0;
        public int Identifier;

        public string Name { get; set; }
        public string Nickname { get; set; }
        public int Rating { get; set; }

        public bool SpeaksDanish { get; set; }
        public bool SpeaksEnglish { get; set; }

        public List<Player> Inclusions { get; set; }
        public List<Player> Exclusions { get; set; }

        /// <summary>
        /// TODO: Add summary, and code documentation in general
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="nickname">The nickname of the player. This is what is used in game.</param>
        /// <param name="rating">A number based evaluation of the players skill, ranging from 1 as the lowest, up to 12 as the highest.</param>
        /// <param name="speaksDanish">Does the player speak danish?</param>
        /// <param name="speaksEnglish">Does the player speak english?</param>
        public Player(string name, string nickname, int rating, bool speaksDanish, bool speaksEnglish)
        {
            Identifier = identifierCount++;

            Name = name;
            Nickname = nickname;
            Rating = rating;

            SpeaksDanish = speaksDanish;
            SpeaksEnglish = speaksEnglish;

            Inclusions = new List<Player>();
            Exclusions = new List<Player>();
        }

        public void AddInclusion(Player player)
        {
            if (Inclusions.Contains(player))
                return; // can't add two of the same player to inclusions

            if (Exclusions.Contains(player))
                throw new InvalidOperationException(); // can't add a player in exclusion to inclusion

            if (player.Exclusions.Contains(this))
                throw new InvalidOperationException(); // can't add a player who excludes "this" to inclusions

            Inclusions.Add(player);
            player.AddInclusion(this);
        }

        public void RemoveInclusion(Player player)
        {
            if (!Inclusions.Contains(player))
                throw new ArgumentException(); // the player is not in the inclusions of "this"

            Inclusions.Remove(player);
            player.RemoveInclusion(this);
        }

        public void AddExclusion(Player player)
        {
            if (Exclusions.Contains(player))
                return; // can't add two of the same player to exclusions

            if (Inclusions.Contains(player))
                throw new InvalidOperationException(); // can't add a player in inclusion to exclusion

            if (player.Inclusions.Contains(this))
                throw new InvalidOperationException(); // can't add a player who includes "this" to exclusions

            Exclusions.Add(player);
        }

        public void RemoveExclusion(Player player)
        {
            if (!Exclusions.Contains(player))
                throw new ArgumentException(); // the player is not in the exclusions of "this"

            Exclusions.Remove(player);
        }
    }
}
