using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Skills.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.TreasureGen.Items;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats.Selectors
{
    internal class AdditionalFeatSelection
    {
        public string Feat { get; set; }
        public Frequency Frequency { get; set; }
        public int Power { get; set; }
        public IEnumerable<RequiredFeatSelection> RequiredFeats { get; set; }
        public int RequiredBaseAttack { get; set; }
        public Dictionary<string, int> RequiredAbilities { get; set; }
        public IEnumerable<RequiredSkillSelection> RequiredSkills { get; set; }
        public Dictionary<string, int> RequiredCharacterClasses { get; set; }
        public string FocusType { get; set; }
        public bool CanBeTakenMultipleTimes { get; set; }

        public AdditionalFeatSelection()
        {
            Feat = string.Empty;
            RequiredFeats = Enumerable.Empty<RequiredFeatSelection>();
            RequiredAbilities = new Dictionary<string, int>();
            RequiredSkills = Enumerable.Empty<RequiredSkillSelection>();
            RequiredCharacterClasses = new Dictionary<string, int>();
            FocusType = string.Empty;
            Frequency = new Frequency();
        }

        public bool ImmutableRequirementsMet(BaseAttack baseAttack, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills, CharacterClass characterClass)
        {
            foreach (var stat in RequiredAbilities)
                if (abilities[stat.Key].Value < stat.Value)
                    return false;

            if (RequiredCharacterClasses.Any() && (RequiredCharacterClasses.ContainsKey(characterClass.Name) == false || RequiredCharacterClasses[characterClass.Name] > characterClass.Level))
                return false;

            if (baseAttack.BaseBonus < RequiredBaseAttack)
                return false;

            return !RequiredSkills.Any() || RequiredSkills.Any(s => s.RequirementMet(skills));
        }

        public bool MutableRequirementsMet(IEnumerable<Feat> feats)
        {
            if (FeatSelected(feats) && !CanBeTakenMultipleTimes)
                return false;

            if (!RequiredFeats.Any())
                return true;

            var proficiencyRequirement = RequiredFeats.FirstOrDefault(f => f.Feat == ItemTypeConstants.Weapon + GroupConstants.Proficiency);
            var requirementsWithoutProficiency = RequiredFeats.Except(new[] { proficiencyRequirement });

            return requirementsWithoutProficiency.All(f => f.RequirementMet(feats));
        }

        private bool FeatSelected(IEnumerable<Feat> feats)
        {
            return feats.Any(f => FeatSelected(f));
        }

        private bool FeatSelected(Feat feat)
        {
            return feat.Name == Feat && FocusSelected(feat);
        }

        private bool FocusSelected(Feat feat)
        {
            return string.IsNullOrEmpty(FocusType) || feat.Foci.Contains(FeatConstants.Foci.All);
        }
    }
}