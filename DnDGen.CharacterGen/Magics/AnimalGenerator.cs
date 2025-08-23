using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Generators.Magics
{
    internal class AnimalGenerator : IAnimalGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;

        public AnimalGenerator(ICollectionSelector collectionsSelector, IAdjustmentsSelector adjustmentsSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
        }

        public string GenerateFrom(Alignment alignment, CharacterClass characterClass, Race race, IEnumerable<Feat> feats)
        {
            var effectiveCharacterClass = GetEffectiveCharacterClass(characterClass);
            if (effectiveCharacterClass.Level < 1)
                return string.Empty;

            var animals = AnimalsForCharacter(effectiveCharacterClass, race, feats);

            if (animals.Any() == false)
                return string.Empty;

            var animal = collectionsSelector.SelectRandomFrom(animals);
            return animal;
        }

        private CharacterClass GetEffectiveCharacterClass(CharacterClass characterClass)
        {
            var minimumLevel = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.LevelAdjustments, characterClass.Name);
            var effectiveCharacterClass = new CharacterClass
            {
                Name = characterClass.Name,
                Level = characterClass.Level
            };

            if (characterClass.Level < minimumLevel)
            {
                effectiveCharacterClass.Level = 0;
            }

            if (characterClass.Name == CharacterClassConstants.Ranger)
            {
                effectiveCharacterClass.Name = CharacterClassConstants.Druid;
                effectiveCharacterClass.Level /= 2;
            }

            return effectiveCharacterClass;
        }

        private IEnumerable<string> AnimalsForCharacter(CharacterClass characterClass, Race race, IEnumerable<Feat> feats)
        {
            var animals = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AnimalGroups, characterClass.Name);
            var animalsForSize = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AnimalGroups, race.Size);
            var filteredAnimals = animals.Intersect(animalsForSize);

            var minimumLevels = adjustmentsSelector.SelectAllFrom(TableNameConstants.Set.Adjustments.LevelAdjustments);
            filteredAnimals = filteredAnimals.Where(a => characterClass.Level > minimumLevels[a]);

            var arcaneSpellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane);

            if (!arcaneSpellcasters.Contains(characterClass.Name))
                return filteredAnimals;

            var improvedFamiliars = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AnimalGroups, FeatConstants.ImprovedFamiliar);
            var characterHasImprovedFamiliarFeat = feats.Any(f => f.Name == FeatConstants.ImprovedFamiliar);

            var animalsForMetarace = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AnimalGroups, race.Metarace);
            filteredAnimals = filteredAnimals.Where(a => characterHasImprovedFamiliarFeat || !improvedFamiliars.Contains(a));

            return filteredAnimals.Intersect(animalsForMetarace);
        }
    }
}
