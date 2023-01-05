using System.Collections.Generic;
using TeamGenerator.Enums;

namespace TeamGenerator.MVVM.Models
{
    public class Relation
    {
        public RelationType RelationType { get; set; }
        public List<Player> Participants { get; set; }

        public Relation(RelationType relationType)
        {
            RelationType = relationType;

            Participants = new List<Player>();
        }
    }
}
