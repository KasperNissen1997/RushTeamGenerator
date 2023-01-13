using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    public class Team
    {
        private static int identifierCount = 0;
        public int Identifier;

        public List<Player> Players { get; }
        public int Capacity { get; }
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

        public Team(int capacity)
        {
            Identifier = identifierCount++;

            Players = new();
            Capacity = capacity;
        }

        public void AddPlayerGroup(PlayerGroup playerGroup)
        {
            foreach (Player player in playerGroup.Players)
                Players.Add(player);
        }
    }
}
