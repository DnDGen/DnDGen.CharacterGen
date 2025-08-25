using DnDGen.CharacterGen.Abilities;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    internal class SetAbilitiesRandomizer : ISetAbilitiesRandomizer
    {
        public int SetStrength { get; set; }
        public int SetDexterity { get; set; }
        public int SetConstitution { get; set; }
        public int SetIntelligence { get; set; }
        public int SetWisdom { get; set; }
        public int SetCharisma { get; set; }
        public bool AllowAdjustments { get; set; }

        public SetAbilitiesRandomizer()
        {
            SetStrength = 10;
            SetDexterity = 10;
            SetConstitution = 10;
            SetIntelligence = 10;
            SetWisdom = 10;
            SetCharisma = 10;
            AllowAdjustments = true;
        }

        public Dictionary<string, Ability> Randomize()
        {
            var abilities = InitializeAbilities();

            abilities[AbilityConstants.Charisma].Value = SetCharisma;
            abilities[AbilityConstants.Constitution].Value = SetConstitution;
            abilities[AbilityConstants.Dexterity].Value = SetDexterity;
            abilities[AbilityConstants.Intelligence].Value = SetIntelligence;
            abilities[AbilityConstants.Strength].Value = SetStrength;
            abilities[AbilityConstants.Wisdom].Value = SetWisdom;

            return abilities;
        }

        private Dictionary<string, Ability> InitializeAbilities()
        {
            var stats = new Dictionary<string, Ability>();

            stats[AbilityConstants.Charisma] = new Ability(AbilityConstants.Charisma);
            stats[AbilityConstants.Constitution] = new Ability(AbilityConstants.Constitution);
            stats[AbilityConstants.Dexterity] = new Ability(AbilityConstants.Dexterity);
            stats[AbilityConstants.Intelligence] = new Ability(AbilityConstants.Intelligence);
            stats[AbilityConstants.Strength] = new Ability(AbilityConstants.Strength);
            stats[AbilityConstants.Wisdom] = new Ability(AbilityConstants.Wisdom);

            return stats;
        }
    }
}