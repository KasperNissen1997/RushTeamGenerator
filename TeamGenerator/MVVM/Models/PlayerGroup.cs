using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    /// <summary>
    /// A <see cref="PlayerGroup"/> represents a group of <see cref="Player"/> instances.
    /// </summary>
    public class PlayerGroup : IComparable<PlayerGroup>
    {
        /// <summary>
        /// The <see cref="Player"/>s that make up the <see cref="PlayerGroup"/>.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// The amount of <see cref="Player"/>s that are in the group.
        /// </summary>
        public int Size
        {
            get
            {
                return Players.Count;
            }
        }
        /// <summary>
        /// The cumulative rating of all the <see cref="Player"/>s in the group.
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
        /// If all the <see cref="Player"/>s in the group speak danish.
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
        /// If all the <see cref="Player"/>s in the group speak english.
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
        /// Creates a new empty instance of a <see cref="PlayerGroup"/>.
        /// </summary>
        public PlayerGroup()
        {
            Players = new List<Player>();
        }

        /// <summary>
        /// Compares two instances of <see cref="PlayerGroup"/>s, based on the groups sum rating.
        /// </summary>
        /// <param name="other">The other <see cref="PlayerGroup"/> instance.</param>
        /// <returns>The inverse of a normal CompareTo method call. If a group has a higher rating than another group, it is considered smaller.</returns>
        /// <exception cref="NotImplementedException">Thrown if <paramref name="other"/> is not <see langword="null"/> nor of type <see cref="PlayerGroup"/>.</exception>
        public int CompareTo(PlayerGroup? other)
        {
            if (other is null)
                return 1;

            if (other is PlayerGroup playerGroup)
                return (Rating.CompareTo(playerGroup.Rating)) * -1; // inverse this so it it sorts in descending order via List.Sort()

            throw new NotImplementedException();
        }
    }
}
