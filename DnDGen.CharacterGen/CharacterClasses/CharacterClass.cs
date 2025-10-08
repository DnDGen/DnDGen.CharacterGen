using System.Collections.Generic;

namespace DnDGen.CharacterGen.CharacterClasses
{
    public class CharacterClass
    {
        public int Level { get; set; }
        public int LevelAdjustment { get; set; }
        public bool IsNPC { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> SpecialistFields { get; set; }
        public IEnumerable<string> ProhibitedFields { get; set; }

        public double EffectiveLevel => LevelAdjustment + Level;

        public string Summary => $"Level {Level} {Name}";

        public CharacterClass()
        {
            Name = string.Empty;
            SpecialistFields = [];
            ProhibitedFields = [];
        }

        public override string ToString() => Summary;
    }
}