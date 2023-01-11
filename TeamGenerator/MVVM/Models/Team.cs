using System;
using System.Collections.Generic;

namespace TeamGenerator.MVVM.Models
{
    public class Team
    {
        private static int identifierCount = 0;
        public int Identifier;

        public List<PlayerGroup> PlayerGroups { get; private set; }
        public int Capacity { get; private set; }
        public int Size
        {
            get
            {
                int size = 0;

                foreach (PlayerGroup playerGroup in PlayerGroups)
                    size += playerGroup.Size;

                return size;
            }
        }
        public float Rating { get; private set; }

        public bool SpeaksDanish { get; private set; }
        public bool SpeaksEnglish { get; private set; }

        public Team(int capacity)
        {
            Identifier = identifierCount++;

            PlayerGroups = new List<Player>();
            Rating = 0f;

            SpeaksDanish = true;
            SpeaksEnglish = true;
            Capacity = capacity;
        }

        public bool TryAddPlayer(Player player)
        {
            if (PlayerGroups.Count == 5)
                throw new InvalidOperationException(); // adding another player will exceed the maximum of 5 players

            if (TryGetAvailableLanguages(player, out bool speaksDanish, out bool speaksEnglish)) // attempt to get the available languages
                throw new ArgumentException(); // adding this player will result in no available language

            PlayerGroups.Add(player); // add the player

            UpdateRating(); // update rating

            SpeaksDanish = speaksDanish; // does all members of the team still speak danish?
            SpeaksEnglish = SpeaksEnglish; // how about english?

            return true;
        }

        bool TryGetAvailableLanguages(Player possiblePlayer, out bool danish, out bool english)
        {
            List<Player> possiblePlayers = PlayerGroups;
            possiblePlayers.Add(possiblePlayer);

            danish = true; 
            english = true;

            foreach (Player player in possiblePlayers)
            {
                if (!player.SpeaksDanish && danish)
                    danish = false;

                if (!player.SpeaksEnglish && english)
                    english = false;
            }

            if (!danish && !english)
                return false;

            return true;
        }

        void UpdateRating()
        {
            int sum = 0;

            foreach (Player player in PlayerGroups)
            {
                sum += player.Rating;
            }

            Rating = sum / PlayerGroups.Count;
        }
    }
}
