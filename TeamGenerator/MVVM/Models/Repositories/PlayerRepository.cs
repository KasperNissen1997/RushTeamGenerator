using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace TeamGenerator.MVVM.Models.Repositories
{
    public class PlayerRepository
    {
        #region Singleton
        private static PlayerRepository _instance;
        public static PlayerRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerRepository();
                    return _instance;
                }
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        private PlayerRepository()
        {
            players = new List<Player>();
            Load();
        }
        #endregion

        private string filePath = Path.GetFullPath(@"..\..\..\Data\Players.xml");

        private List<Player> players;

        #region Persistance
        public void Save()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            XmlWriterSettings settings = new()
            {
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Players");
                foreach (Player player in players)
                {
                    writer.WriteStartElement("Player");

                    writer.WriteElementString("Name", player.Name); // Name
                    writer.WriteElementString("Nickname", player.Nickname); // Nickname
                    writer.WriteElementString("Rating", player.Rating.ToString()); // Rating
                    writer.WriteElementString("SpeaksDanish", player.SpeaksDanish.ToString().ToLower()); // SpeaksDanish
                    writer.WriteElementString("SpeaksEnglish", player.SpeaksDanish.ToString().ToLower()); // SpeaksEnglish

                    writer.WriteStartElement("Inclusions"); // Inclusions
                    foreach (Player includedPlayer in player.Inclusions)
                    {
                        writer.WriteElementString("PlayerIdentifier", includedPlayer.Identifier.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Exclusions"); // Exclusions
                    foreach (Player excludedPlayer in player.Exclusions)
                    {
                        writer.WriteElementString("PlayerIdentifier", excludedPlayer.Identifier.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void Load()
        {
            XmlReaderSettings settings = new()
            {
                IgnoreWhitespace = true,
                IgnoreComments = true // probably unnecessary
            };

            using (XmlReader reader = XmlReader.Create(filePath, settings))
            {
                reader.ReadToFollowing("Player");
                do
                {
                    reader.ReadToFollowing("Name");

                    string name = reader.ReadElementContentAsString(); // Name
                    string nickname = reader.ReadElementContentAsString(); // Nickname                   
                    int rating = reader.ReadElementContentAsInt(); // Rating
                    bool speaksDanish = reader.ReadElementContentAsBoolean(); // SpeaksDanish
                    bool speaksEnglish = reader.ReadElementContentAsBoolean(); // SpeaksEnglish

                    /*  
                     *  Here we add a player with the limited information we have read so far.
                     *  
                     *  We still need to add the correct inclusion and exclusion relations, so to do that
                     *  we must once again read through the data, but this time we only focus on the relations.
                     */

                    players.Add(new Player(name, nickname, rating, speaksDanish, speaksEnglish));
                }
                while (reader.ReadToFollowing("Player"));
            }

            using (XmlReader reader = XmlReader.Create(filePath, settings)) // second read of the data - focus on relations
            {
                int playerCount = 0;

                reader.ReadToFollowing("Player");
                do
                {
                    reader.ReadToFollowing("Inclusions"); // Inclusion preparation

                    List<Player> inclusions = new List<Player>();
                    XmlReader subtreeReader = reader.ReadSubtree();

                    subtreeReader.ReadToFollowing("PlayerIdentifier");
                    do
                    {
                        try
                        {
                            inclusions.Add(Retrieve(subtreeReader.ReadElementContentAsInt())); // Inclusion
                        }
                        catch (InvalidOperationException)
                        {
                            subtreeReader.Close();
                            break;
                        }
                    } while (!subtreeReader.EOF);

                    reader.ReadToFollowing("Exclusions"); // Exclusion preparation

                    List<Player> exclusions = new List<Player>();
                    subtreeReader = reader.ReadSubtree();

                    subtreeReader.ReadToFollowing("PlayerIdentifier");
                    do
                    {
                        try
                        {
                            exclusions.Add(Retrieve(subtreeReader.ReadElementContentAsInt())); // Exclusion
                        }
                        catch (InvalidOperationException)
                        {
                            subtreeReader.Close();
                            break;
                        }
                    } while (!subtreeReader.EOF);

                    Retrieve(playerCount).Inclusions = inclusions;
                    Retrieve(playerCount).Exclusions = exclusions;

                    playerCount++;
                }
                while (reader.ReadToFollowing("Player"));
            }

            Trace.WriteLine("Player loading finished. Phew '>.>");
        }
        #endregion

        #region CRUD
        public Player Create(string name, string nickname, int rating, bool speaksDanish, bool speaksEnglish)
        {
            Player player = new Player(name, nickname, rating, speaksDanish, speaksEnglish);

            players.Add(player);

            return player;
        }

        public Player Retrieve(int identifier)
        {
            foreach (Player player in players)
            {
                if (player.Identifier == identifier)
                    return player;
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public List<Player> RetrieveAll()
        {
            return players;
        }

        public void UpdateName(int identifier, string name)
        {
            foreach (Player player in players)
            {
                if (player.Identifier == identifier)
                {
                    player.Name = name;
                    return;
                }
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public void UpdateNickname(int identifier, string nickname)
        {
            foreach (Player player in players)
            {
                if (player.Identifier == identifier)
                {
                    player.Nickname = nickname;
                    return;
                }
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public void UpdateLanguages(int identifier, bool speaksDanish, bool speaksEnglish)
        {
            foreach (Player player in players)
            {
                if (player.Identifier == identifier)
                {
                    player.SpeaksDanish = speaksDanish;
                    player.SpeaksEnglish = speaksEnglish;
                    return;
                }
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public void UpdateRating(int identifier, int rating)
        {
            foreach (Player player in players)
            {
                if (player.Identifier == identifier)
                {
                    player.Rating = rating;
                    return;
                }
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public void Delete(int identifier)
        {
            players.Remove(Retrieve(identifier));
        }
        #endregion
    }
}
