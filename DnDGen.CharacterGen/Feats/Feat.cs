using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats
{
    public class Feat
    {
        public string Name { get; set; }
        public IEnumerable<string> Foci { get; set; }
        public int Power { get; set; }
        public Frequency Frequency { get; set; }
        public bool CanBeTakenMultipleTimes { get; set; }

        public string Summary
        {
            get
            {
                if (!Foci.Any())
                    return Name;

                return $"{Name} ({string.Join(", ", Foci)})";
            }
        }

        public Feat()
        {
            Name = string.Empty;
            Foci = [];
            Frequency = new Frequency();
        }

        public Feat Clone()
        {
            var clone = new Feat
            {
                CanBeTakenMultipleTimes = CanBeTakenMultipleTimes,
                Foci = [.. Foci]
            };
            clone.Frequency.Quantity = Frequency.Quantity;
            clone.Frequency.TimePeriod = Frequency.TimePeriod;
            clone.Name = Name;
            clone.Power = Power;

            return clone;
        }

        public override string ToString() => Summary;
    }
}