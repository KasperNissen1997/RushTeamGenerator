using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    public class Player : IComparable<Player>
    {
        private static int identifierCount = 0;
        /// <summary>
        /// A unique integer that is used to differentiate between different instances of <see cref="Player"/>.
        /// </summary>
        public int Identifier;

        /// <summary>
        /// The real name of the player.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The nickname of the player.
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// The amount of skill of the player, where 1 is the lowest amount, and 12 is the highest.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Indicates if the player speaks danish.
        /// </summary>
        public bool SpeaksDanish { get; set; }
        /// <summary>
        /// Indicates if the player speaks english.
        /// </summary>
        public bool SpeaksEnglish { get; set; }

        /// <summary>
        /// A collection of all the other players that should be part of the team this player is part of.
        /// </summary>
        public List<Player> Inclusions { get; set; }
        /// <summary>
        /// A collection of all the other players that should not be part of teams this player is part of.
        /// </summary>
        public List<Player> Exclusions { get; set; }

        /// <summary>
        /// A player.
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

        #region Relation logic
        /// <summary>
        /// Attempt to add <paramref name="player"/> to <see cref="Inclusions"/>.
        /// If <see cref="Inclusions"/> already contains an instance of <paramref name="player"/>, nothing will happen.
        /// </summary>
        /// <param name="player">The player who should be added to <see cref="Inclusions"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="player"/> can't be added to <see cref="Inclusions"/>.</exception>
        public void AddInclusion(Player player)
        {
            if (Inclusions.Contains(player)) // is the player already an inclusion?
                return;

            if (Exclusions.Contains(player)) // is the player an exclusion?
                throw new InvalidOperationException();

            if (player.Exclusions.Contains(this)) // does the player exclude "this"?
                throw new InvalidOperationException();

            Inclusions.Add(player);

            if (!player.Inclusions.Contains(this)) // does the player have "this" as an inclusion?
                player.AddInclusion(this);
        }

        /// <summary>
        /// Attempts to remove <paramref name="player"/> from <see cref="Inclusions"/>.
        /// </summary>
        /// <param name="player">The player who should be removed from <see cref="Inclusions"/>.</param>
        /// <exception cref="ArgumentException">Thrown if <see cref="Inclusions"/> does not contain <paramref name="player"/>.</exception>
        public void RemoveInclusion(Player player)
        {
            if (!Inclusions.Contains(player)) // is the player an inclusion?
                throw new ArgumentException();

            Inclusions.Remove(player);

            if (player.Inclusions.Contains(this)) // does the player have "this" as an inclusion?
                player.RemoveInclusion(this);
        }

        /// <summary>
        /// Attemps to add <paramref name="player"/> to <see cref="Exclusions"/>.
        /// If <see cref="Exclusions"/> already contains an instance of <paramref name="player"/>, nothing will happen.
        /// </summary>
        /// <param name="player">The player who should be added to <see cref="Exclusions"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="player"/> can't be added to <see cref="Exclusions"/>.</exception>
        public void AddExclusion(Player player)
        {
            if (Exclusions.Contains(player)) // is the player already an exclusion?
                return;

            if (Inclusions.Contains(player)) // is the player an inclusion?
                throw new InvalidOperationException();

            if (player.Inclusions.Contains(this)) // does the player include "this"?
                throw new InvalidOperationException();

            Exclusions.Add(player);

            if (player.Exclusions.Contains(this)) // does the player have "this" as an exclusion?
                player.AddExclusion(this);
        }

        /// <summary>
        /// Attempts to remove <paramref name="player"/> from <see cref="Exclusions"/>.
        /// </summary>
        /// <param name="player">The player who should be removed from <see cref="Exclusions"/>.</param>
        /// <exception cref="ArgumentException">Thrown if <see cref="Exclusions"/> does not contain <paramref name="player"/>.</exception>
        public void RemoveExclusion(Player player)
        {
            if (!Exclusions.Contains(player)) // is the player an exclusion?
                throw new ArgumentException();

            Exclusions.Remove(player);

            if (player.Exclusions.Contains(this)) // does the player have "this" as an exclusion?
                player.RemoveExclusion(this);
        }
        #endregion

        /// <summary>
        /// Compares two <see cref="Player"/> instances with each other based on <see cref="Rating"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Player"/> instance.</param>
        /// <returns>The base comparison of <see cref="Rating"/>. If <paramref name="other"/> is <see langword="null"/>, returns 1.</returns>
        /// <exception cref="NotImplementedException">Thrown if <paramref name="other"/> is not <see langword="null"/> or <see cref="Player"/>.</exception>
        public int CompareTo(Player? other)
        {
            if (other is null)
                return 1;

            if (other is Player)
                return Identifier.CompareTo(other.Identifier);

            throw new NotImplementedException();
        }
    }
}
