using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Abilities;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Generators.Abilities
{
    internal class AbilitiesGenerator : IAbilitiesGenerator
    {
        private readonly IPercentileSelector percentileSelector;
        private readonly IAbilityAdjustmentsSelector abilityAdjustmentsSelector;
        private readonly ICollectionSelector collectionsSelector;

        public AbilitiesGenerator(IPercentileSelector percentileSelector, IAbilityAdjustmentsSelector abilityAdjustmentsSelector, ICollectionSelector collectionsSelector)
        {
            this.percentileSelector = percentileSelector;
            this.abilityAdjustmentsSelector = abilityAdjustmentsSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public Dictionary<string, Ability> GenerateWith(IAbilitiesRandomizer abilitiesRandomizer, CharacterClass characterClass, Race race)
        {
            var abilities = abilitiesRandomizer.Randomize();

            if (CanAdjustAbilities(abilitiesRandomizer))
            {
                abilities = PrioritizeAbilities(abilities, characterClass);
                abilities = AdjustAbilities(race, abilities);
                abilities = SetMinimumAbilities(abilities);
                abilities = IncreaseAbilities(abilities, characterClass, race);
            }
            else
            {
                abilities = SetMinimumAbilities(abilities);
            }

            var undead = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead);
            if (race.BaseRace == RaceConstants.BaseRaces.Mummy || undead.Contains(race.Metarace))
                abilities.Remove(AbilityConstants.Constitution);

            return abilities;
        }

        private bool CanAdjustAbilities(IAbilitiesRandomizer abilitiesRandomizer)
        {
            if (abilitiesRandomizer is not ISetAbilitiesRandomizer)
                return true;

            var setAbilitiesRandomizer = abilitiesRandomizer as ISetAbilitiesRandomizer;
            return setAbilitiesRandomizer.AllowAdjustments;
        }

        private Dictionary<string, Ability> PrioritizeAbilities(Dictionary<string, Ability> abilities, CharacterClass characterClass)
        {
            var abilityPriorities = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityPriorities, characterClass.Name);
            if (abilityPriorities.Any() == false)
                return abilities;

            var firstPriority = abilityPriorities.First();
            var max = abilities.Values.Max(s => s.Value);
            var maxAbility = abilities.Keys.First(k => abilities[k].Value == max);
            abilities = SwapAbilityValues(abilities, firstPriority, maxAbility);

            var secondPriorities = abilityPriorities.Skip(1);
            var nonPriorityAbilityNames = abilities.Keys.Except(abilityPriorities);

            while (secondPriorities.Any())
            {
                var priority = secondPriorities.First();
                var nonPriorityAbilities = abilities.Where(kvp => nonPriorityAbilityNames.Contains(kvp.Key));

                max = nonPriorityAbilities.Max(kvp => kvp.Value.Value);

                if (max > abilities[priority].Value)
                {
                    maxAbility = nonPriorityAbilityNames.First(s => abilities[s].Value == max);
                    abilities = SwapAbilityValues(abilities, priority, maxAbility);
                }

                secondPriorities = secondPriorities.Skip(1);
            }

            return abilities;
        }

        private Dictionary<string, Ability> SwapAbilityValues(Dictionary<string, Ability> abilities, string priorityAbility, string otherAbility)
        {
            var temp = abilities[otherAbility].Value;
            abilities[otherAbility].Value = abilities[priorityAbility].Value;
            abilities[priorityAbility].Value = temp;

            return abilities;
        }

        private Dictionary<string, Ability> AdjustAbilities(Race race, Dictionary<string, Ability> abilities)
        {
            var abilityAdjustments = abilityAdjustmentsSelector.SelectFor(race);

            foreach (var ability in abilities.Keys)
                abilities[ability].Value += abilityAdjustments[ability];

            return abilities;
        }

        private Dictionary<string, Ability> SetMinimumAbilities(Dictionary<string, Ability> abilities)
        {
            foreach (var ability in abilities.Values)
                ability.Value = Math.Max(ability.Value, 3);

            return abilities;
        }

        private Dictionary<string, Ability> IncreaseAbilities(Dictionary<string, Ability> abilities, CharacterClass characterClass, Race race)
        {
            var count = characterClass.Level / 4;

            while (count-- > 0)
            {
                var abilityToIncrease = GetAbilityToIncrease(abilities, race, characterClass);
                abilities[abilityToIncrease].Value++;
            }

            return abilities;
        }

        private string GetAbilityToIncrease(Dictionary<string, Ability> abilities, Race race, CharacterClass characterClass)
        {
            var abilityPriorities = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityPriorities, characterClass.Name);
            var undead = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead);

            if (race.BaseRace == RaceConstants.BaseRaces.Mummy || undead.Contains(race.Metarace))
                abilityPriorities = abilityPriorities.Except([AbilityConstants.Constitution]);

            if (!abilityPriorities.Any())
            {
                var valid = abilities.Keys.AsEnumerable();

                if (race.BaseRace == RaceConstants.BaseRaces.Mummy || undead.Contains(race.Metarace))
                    valid = valid.Except([AbilityConstants.Constitution]);

                var ability = collectionsSelector.SelectRandomFrom(valid);

                return ability;
            }

            var secondPriorityAbilities = abilityPriorities.Skip(1);
            if (!secondPriorityAbilities.Any())
                return abilityPriorities.First();

            var increaseFirst = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.IncreaseFirstPriorityAbility);

            if (increaseFirst)
                return abilityPriorities.First();

            return collectionsSelector.SelectRandomFrom(secondPriorityAbilities);
        }
    }
}