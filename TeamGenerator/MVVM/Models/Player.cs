using System;
using System.Collections.Generic;
using TeamGenerator.Enums;

namespace TeamGenerator.MVVM.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public List<Language> KnownLanguages { get; set; }
        public int Rating { get; set; }

        Relation inclusions { get; set; }
        Relation exclusions { get; set; }

        public Player(string name, string nickname, List<Language> knownLanguages, int rating)
        {
            Name = name;
            Nickname = nickname;
            KnownLanguages = knownLanguages;
            Rating = rating;

            inclusions = new Relation(RelationType.Inclusion);
            exclusions = new Relation(RelationType.Exclusion);
        }

        public void AddInclusion(Player player)
        {
            if (inclusions.Participants.Contains(player))
                return; // can't add two of the same player to inclusions

            if (exclusions.Participants.Contains(player))
                throw new InvalidOperationException(); // can't add a player in exclusion to inclusion

            if (player.exclusions.Participants.Contains(this))
                throw new InvalidOperationException(); // can't add a player who excludes "this" to inclusions

            inclusions.Participants.Add(player);
            player.AddInclusion(this);
        }

        public void RemoveInclusion(Player player)
        {
            if (!inclusions.Participants.Contains(player))
                throw new ArgumentException(); // the player is not in the inclusions of "this"

            inclusions.Participants.Remove(player);
            player.RemoveInclusion(this);
        }

        public void AddExclusion(Player player)
        {
            if (exclusions.Participants.Contains(player))
                return; // can't add two of the same player to exclusions

            if (inclusions.Participants.Contains(player))
                throw new InvalidOperationException(); // can't add a player in inclusion to exclusion

            if (player.inclusions.Participants.Contains(this))
                throw new InvalidOperationException(); // can't add a player who includes "this" to exclusions

            exclusions.Participants.Add(player);
        }

        public void RemoveExclusion(Player player)
        {
            if (!exclusions.Participants.Contains(player))
                throw new ArgumentException(); // the player is not in the exclusions of "this"

            exclusions.Participants.Remove(player);
        }
    }
}
