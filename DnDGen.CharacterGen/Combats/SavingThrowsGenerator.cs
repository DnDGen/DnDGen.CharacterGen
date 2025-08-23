using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Combats
{
    internal class SavingThrowsGenerator : ISavingThrowsGenerator
    {
        private readonly ICollectionSelector collectionsSelector;

        public SavingThrowsGenerator(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public SavingThrows GenerateWith(CharacterClass characterClass, IEnumerable<Feat> feats, Dictionary<string, Ability> abilities)
        {
            var savingThrows = new SavingThrows();

            savingThrows.HasFortitudeSave = abilities.ContainsKey(AbilityConstants.Constitution);

            savingThrows.Reflex = abilities[AbilityConstants.Dexterity].Bonus;
            savingThrows.Will = abilities[AbilityConstants.Wisdom].Bonus;

            savingThrows.Reflex += GetClassSavingThrowBonus(characterClass, SavingThrowConstants.Reflex);
            savingThrows.Will += GetClassSavingThrowBonus(characterClass, SavingThrowConstants.Will);

            savingThrows.Reflex += GetFeatSavingThrowBonus(feats, SavingThrowConstants.Reflex);
            savingThrows.Will += GetFeatSavingThrowBonus(feats, SavingThrowConstants.Will);

            var anySavingThrowFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.SavingThrows);
            var anySavingThrowFeats = feats.Where(f => anySavingThrowFeatNames.Contains(f.Name));

            foreach (var feat in anySavingThrowFeats)
            {
                savingThrows.Reflex += GetBonus(feat, SavingThrowConstants.Reflex);
                savingThrows.Will += GetBonus(feat, SavingThrowConstants.Will);
                savingThrows.CircumstantialBonus |= HasCircumstantialBonus(feat);
            }

            if (savingThrows.HasFortitudeSave)
            {
                savingThrows.Fortitude = abilities[AbilityConstants.Constitution].Bonus;
                savingThrows.Fortitude += GetClassSavingThrowBonus(characterClass, SavingThrowConstants.Fortitude);
                savingThrows.Fortitude += GetFeatSavingThrowBonus(feats, SavingThrowConstants.Fortitude);

                foreach (var feat in anySavingThrowFeats)
                {
                    savingThrows.Fortitude += GetBonus(feat, SavingThrowConstants.Fortitude);
                    savingThrows.CircumstantialBonus |= HasCircumstantialBonus(feat);
                }
            }

            return savingThrows;
        }

        private int GetBonus(Feat feat, string savingThrow)
        {
            if (feat.Foci.Contains(FeatConstants.Foci.All) || feat.Foci.Contains(savingThrow))
                return feat.Power;

            return 0;
        }

        private bool HasCircumstantialBonus(Feat feat)
        {
            var saveFoci = new[]
            {
                FeatConstants.Foci.All,
                SavingThrowConstants.Fortitude,
                SavingThrowConstants.Reflex,
                SavingThrowConstants.Will
            };

            return feat.Foci.Intersect(saveFoci).Any() == false;
        }

        private int GetClassSavingThrowBonus(CharacterClass characterClass, string savingThrow)
        {
            var strong = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, savingThrow);
            if (strong.Contains(characterClass.Name))
                return characterClass.Level / 2 + 2;

            return characterClass.Level / 3;
        }

        private int GetFeatSavingThrowBonus(IEnumerable<Feat> feats, string savingThrow)
        {
            var saveFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, savingThrow);
            var saveFeats = feats.Where(f => saveFeatNames.Contains(f.Name));

            return saveFeats.Sum(f => f.Power);
        }
    }
}