using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats.Selectors
{
    internal class CharacterClassFeatSelection
    {
        public string Feat { get; set; }
        public string FocusType { get; set; }
        public int MinimumLevel { get; set; }
        public int MaximumLevel { get; set; }
        public int Power { get; set; }
        public Frequency Frequency { get; set; }
        public IEnumerable<RequiredFeatSelection> RequiredFeats { get; set; }
        public string FrequencyQuantityAbility { get; set; }
        public string SizeRequirement { get; set; }
        public bool AllowFocusOfAll { get; set; }

        public CharacterClassFeatSelection()
        {
            Feat = string.Empty;
            FocusType = string.Empty;
            Frequency = new Frequency();
            RequiredFeats = Enumerable.Empty<RequiredFeatSelection>();
            FrequencyQuantityAbility = string.Empty;
            SizeRequirement = string.Empty;
        }

        public bool RequirementsMet(CharacterClass characterClass, Race race, IEnumerable<Feat> feats)
        {
            if (string.IsNullOrEmpty(SizeRequirement) == false && SizeRequirement != race.Size)
                return false;

            if (MaximumLevel > 0 && characterClass.Level > MaximumLevel)
                return false;

            if (MinimumLevel > characterClass.Level)
                return false;

            foreach (var requirement in RequiredFeats)
            {
                var requirementFeats = feats.Where(f => f.Name == requirement.Feat);

                if (requirementFeats.Any() == false)
                    return false;

                if (requirement.Focus != string.Empty && requirementFeats.Any(f => f.Foci.Contains(requirement.Focus)) == false)
                    return false;
            }

            return true;
        }
    }
}