using DnDGen.CharacterGen.Abilities;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Randomizers.Abilities
{
    internal abstract class BaseAbilitiesRandomizer : IAbilitiesRandomizer
    {
        public Dictionary<string, Ability> Randomize()
        {
            var stats = RollAbilities();

            return stats;
        }

        private Dictionary<string, Ability> RollAbilities()
        {
            var abilities = InitializeAbilities();

            foreach (var ability in abilities.Values)
                ability.Value = RollAbility();

            return abilities;
        }

        private Dictionary<string, Ability> InitializeAbilities()
        {
            var abilities = new Dictionary<string, Ability>();

            abilities[AbilityConstants.Charisma] = new Ability(AbilityConstants.Charisma);
            abilities[AbilityConstants.Constitution] = new Ability(AbilityConstants.Constitution);
            abilities[AbilityConstants.Dexterity] = new Ability(AbilityConstants.Dexterity);
            abilities[AbilityConstants.Intelligence] = new Ability(AbilityConstants.Intelligence);
            abilities[AbilityConstants.Strength] = new Ability(AbilityConstants.Strength);
            abilities[AbilityConstants.Wisdom] = new Ability(AbilityConstants.Wisdom);

            return abilities;
        }

        protected abstract int RollAbility();
    }
}