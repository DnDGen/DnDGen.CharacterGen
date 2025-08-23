using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.CharacterClasses;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Generators.Classes
{
    internal class CharacterClassGenerator : ICharacterClassGenerator
    {
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IPercentileSelector percentileSelector;

        public CharacterClassGenerator(IAdjustmentsSelector adjustmentsSelector, ICollectionSelector collectionsSelector, IPercentileSelector percentileSelector)
        {
            this.adjustmentsSelector = adjustmentsSelector;
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public CharacterClassPrototype GeneratePrototype(Alignment alignmentPrototype, IClassNameRandomizer classNameRandomizer, ILevelRandomizer levelRandomizer)
        {
            var prototype = new CharacterClassPrototype
            {
                Name = classNameRandomizer.Randomize(alignmentPrototype),
                Level = levelRandomizer.Randomize()
            };

            var npcs = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs);
            prototype.IsNPC = npcs.Contains(prototype.Name);

            return prototype;
        }

        public CharacterClass GenerateWith(Alignment alignment, CharacterClassPrototype classPrototype, RacePrototype racePrototype)
        {
            var characterClass = new CharacterClass
            {
                Level = classPrototype.Level,
                Name = classPrototype.Name,
                IsNPC = classPrototype.IsNPC
            };

            characterClass.LevelAdjustment += adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.LevelAdjustments, racePrototype.BaseRace);
            characterClass.LevelAdjustment += adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.LevelAdjustments, racePrototype.Metarace);

            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, characterClass.Name);
            var isSpecialist = percentileSelector.SelectFrom<bool>(Config.Name, tableName);

            if (!isSpecialist)
                return characterClass;

            characterClass.SpecialistFields = GenerateSpecialistFields(characterClass, alignment, racePrototype);
            characterClass.ProhibitedFields = GenerateProhibitedFields(characterClass, alignment);

            return characterClass;
        }

        private IEnumerable<string> GenerateSpecialistFields(CharacterClass characterClass, Alignment alignment, RacePrototype racePrototype)
        {
            var allClassSpecialistFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, characterClass.Name);
            var allBaseRaceSpecialistFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace);
            var allMetaraceSpecialistFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace);
            var alignmentFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString());
            var possibleSpecialistFields = allClassSpecialistFields
                .Intersect(allBaseRaceSpecialistFields)
                .Intersect(allMetaraceSpecialistFields)
                .Intersect(alignmentFields);

            var specialistFieldQuantity = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.SpecialistFieldQuantities, characterClass.Name);
            return PopulateFields(specialistFieldQuantity, possibleSpecialistFields);
        }

        private IEnumerable<string> PopulateFields(int quantity, IEnumerable<string> possibleFields)
        {
            var fields = new HashSet<string>();

            if (possibleFields.Count() < quantity)
                return possibleFields;

            possibleFields = possibleFields.Except(fields);

            while (fields.Count < quantity)
            {
                var field = collectionsSelector.SelectRandomFrom(possibleFields);
                fields.Add(field);
            }

            return fields;
        }

        private IEnumerable<string> GenerateProhibitedFields(CharacterClass characterClass, Alignment alignment)
        {
            if (!characterClass.SpecialistFields.Any())
                return [];

            var allProhibitedFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ProhibitedFields, characterClass.Name);
            var possibleProhibitedFields = allProhibitedFields.Except(characterClass.SpecialistFields);
            var prohibitedFieldQuantities = adjustmentsSelector.SelectAllFrom(TableNameConstants.Set.Adjustments.ProhibitedFieldQuantities);

            var prohibitedFieldQuantity = characterClass.SpecialistFields.Sum(f => prohibitedFieldQuantities[f]);
            var prohibitedFields = PopulateFields(prohibitedFieldQuantity, possibleProhibitedFields);

            if (characterClass.Name == CharacterClassConstants.Cleric)
            {
                var nonAlignmentFields = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ProhibitedFields, alignment.ToString());
                prohibitedFields = prohibitedFields.Union(nonAlignmentFields);
            }

            return prohibitedFields;
        }

        public IEnumerable<CharacterClassPrototype> GeneratePrototypes(
            Alignment alignmentPrototype,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer)
        {
            var npcs = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs);
            var classNames = classNameRandomizer.GetAllPossibleResults(alignmentPrototype);
            var levels = levelRandomizer.GetAllPossibleResults();
            var prototypes = new List<CharacterClassPrototype>();

            foreach (var className in classNames)
            {
                foreach (var level in levels)
                {
                    var prototype = new CharacterClassPrototype
                    {
                        Name = className,
                        Level = level,
                        IsNPC = npcs.Contains(className)
                    };

                    prototypes.Add(prototype);
                }
            }

            return prototypes;
        }
    }
}