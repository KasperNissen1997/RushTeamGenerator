using System;
using System.Collections.Generic;
using TeamGenerator.Enums;

namespace TeamGenerator.MVVM.Models
{
    public class Team
    {
        public List<Player> Players { get; private set; }
        public List<Language> Languages { get; private set; }
        public float Rating { get; private set; }

        public Team()
        {
            Players = new List<Player>();
            Languages = new List<Language>();
            Rating = 0f;
        }

        public bool TryAddPlayer(Player player)
        {
            if (Players.Count == 5)
                throw new InvalidOperationException(); // adding another player will exceed the maximum of 5 players

            if (TryGetAvailableLanguages(player, out List<Language> availableLanguages)) // attempt to get the available languages
                throw new ArgumentException(); // adding this player will result in no available language

            Players.Add(player); // add the player

            Languages = availableLanguages; // update language
            UpdateRating(); // update rating

            return true;
        }

        bool TryGetAvailableLanguages(Player possiblePlayer, out List<Language> availableLanguages)
        {
            List<Player> possiblePlayers = Players;
            possiblePlayers.Add(possiblePlayer);

            List<Language> allLanguages = new()
            {
                Language.Danish,
                Language.English
            };

            availableLanguages = allLanguages;

            foreach (Player player in possiblePlayers)
            {
                foreach (Language language in allLanguages)
                {
                    if (!player.KnownLanguages.Contains(language) && availableLanguages.Contains(language))
                        availableLanguages.Remove(language);
                }
            }

            if (availableLanguages.Count == 0)
                return false;

            return true;
        }

        void UpdateRating()
        {
            int sum = 0;

            foreach (Player player in Players)
            {
                sum += player.Rating;
            }

            Rating = sum / Players.Count;
        }
    }
}
