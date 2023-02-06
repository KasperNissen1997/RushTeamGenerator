using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace TeamGenerator.MVVM.Models.Repositories
{
    public class TeamRepository
    {
        #region Singleton
        private static TeamRepository _instance;
        public static TeamRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TeamRepository();
                    return _instance;
                }
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        private TeamRepository()
        {
            teams = new List<Team>();
            Load();
        }
        #endregion

        private string filePath = Path.GetFullPath(@"..\..\..\Data\Teams.xml");

        private List<Team> teams;

        #region Persistance
        public void Save()
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            XmlWriterSettings settings = new()
            {
                Indent = true
            };

            // teams.Sort();

            for (int i = 0; i < teams.Count; i++)
                teams[i].Identifier = i;

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Teams");
                foreach (Team team in teams)
                {
                    writer.WriteStartElement("Team");

                    writer.WriteElementString("Capacity", team.Capacity.ToString()); // Capacity

                    writer.WriteStartElement("Players"); // Players
                    foreach (Player player in team.Players)
                    {
                        writer.WriteElementString("PlayerIdentifier", player.Identifier.ToString());
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
                reader.ReadToFollowing("Team");

                if (reader.EOF)
                    return;

                do
                {
                    reader.ReadToFollowing("Capacity");

                    int capacity = reader.ReadElementContentAsInt(); // Capacity

                    // reader.ReadToFollowing("Players"); 

                    List<Player> players = new(); // Players preparation
                    XmlReader subtreeReader = reader.ReadSubtree();

                    subtreeReader.ReadToFollowing("PlayerIdentifier");
                    do
                    {
                        try
                        {
                            players.Add(PlayerRepository.Instance.Retrieve(subtreeReader.ReadElementContentAsInt())); // Player
                        }
                        catch (InvalidOperationException)
                        {
                            subtreeReader.Close();
                            break;
                        }
                    } while (!subtreeReader.EOF);

                    teams.Add(new Team(capacity, players));
                }
                while (reader.ReadToFollowing("Team"));
            }

            Trace.WriteLine("Team loading finished!");
        }
        #endregion

        #region CRUD
        public Team Create(int capacity)
        {
            Team team= new Team(capacity);

            teams.Add(team);

            return team;
        }

        public Team Retrieve(int identifier)
        {
            foreach (Team team in teams)
            {
                if (team.Identifier == identifier)
                    return team;
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public List<Team> RetrieveAll()
        {
            return teams;
        }

        public void UpdatePlayers(int identifier, List<Player> players)
        {
            foreach (Team team in teams)
            {
                if (team.Identifier == identifier)
                {
                    team.Players.Clear();

                    foreach (Player player in players)
                        team.Players.Add(player);

                    return;
                }
            }

            throw new ArgumentException($"No player with identifier {identifier} found.");
        }

        public void Delete(int identifier)
        {
            teams.Remove(Retrieve(identifier));
        }
        #endregion
    }
}
