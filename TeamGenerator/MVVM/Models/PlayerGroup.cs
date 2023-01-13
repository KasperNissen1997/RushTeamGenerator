using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    public class PlayerGroup : IComparable<PlayerGroup>
    {
        public List<Player> Players { get; set; }

        public int Size
        {
            get
            {
                return Players.Count;
            }
        }
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

        public PlayerGroup()
        {
            Players = new List<Player>();
        }

        public int CompareTo(PlayerGroup? other)
        {
            if (other is null)
                return 1;

            if (other is PlayerGroup playerGroup)
                return (Rating.CompareTo(playerGroup.Rating)) * -1;

            throw new NotImplementedException();
        }
    }
}
