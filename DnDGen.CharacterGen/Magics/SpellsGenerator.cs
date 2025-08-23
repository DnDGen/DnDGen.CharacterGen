using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Magics
{
    internal class SpellsGenerator : ISpellsGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly IPercentileSelector percentileSelector;

        public SpellsGenerator(ICollectionSelector collectionsSelector, IAdjustmentsSelector adjustmentsSelector, IPercentileSelector percentileSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
            this.percentileSelector = percentileSelector;
        }

        public IEnumerable<SpellQuantity> GeneratePerDay(CharacterClass characterClass, Dictionary<string, Ability> abilities)
        {
            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);

            if (!spellcasters.Contains(characterClass.Name))
                return [];

            var spellsPerDay = GetSpellsPerDay(characterClass, abilities);
            return spellsPerDay.Where(s => s.Quantity > 0 || s.HasDomainSpell);
        }

        private IEnumerable<SpellQuantity> GetSpellsPerDay(CharacterClass characterClass, Dictionary<string, Ability> abilities)
        {
            var levelForSpells = Math.Min(characterClass.Level, 20);
            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSSpellsPerDay, levelForSpells, characterClass.Name);
            var spellsForClass = adjustmentsSelector.SelectAllFrom(tableName);

            var spellAbility = collectionsSelector
                .SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, characterClass.Name + GroupConstants.Spellcasters)
                .Single();
            var maxSpellLevel = abilities[spellAbility].Value - 10;
            var spellsForCharacter = spellsForClass.Where(kvp => Convert.ToInt32(kvp.Key) <= maxSpellLevel);

            var spellsPerDay = new List<SpellQuantity>();

            foreach (var kvp in spellsForCharacter)
            {
                var spellQuantity = new SpellQuantity
                {
                    Level = Convert.ToInt32(kvp.Key),
                    Quantity = kvp.Value,
                    Source = characterClass.Name
                };
                spellQuantity.HasDomainSpell = characterClass.SpecialistFields.Any() && spellQuantity.Level > 0;

                if (spellQuantity.Level > 0 && spellQuantity.Level <= abilities[spellAbility].Bonus)
                {
                    var bonusSpells = (abilities[spellAbility].Bonus - spellQuantity.Level) / 4 + 1;
                    spellQuantity.Quantity += bonusSpells;
                }

                spellsPerDay.Add(spellQuantity);
            }

            return spellsPerDay;
        }

        public IEnumerable<Spell> GenerateKnown(CharacterClass characterClass, Dictionary<string, Ability> abilities)
        {
            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);
            var spells = new List<Spell>();

            if (!spellcasters.Contains(characterClass.Name))
                return spells;

            var divineCasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Divine);

            if (divineCasters.Contains(characterClass.Name))
                return GetAllKnownSpells(characterClass, abilities);

            var quantities = GetKnownSpellQuantities(characterClass, abilities);

            foreach (var spellQuantity in quantities)
            {
                var levelSpells = GetRandomKnownSpellsForLevel(spellQuantity, characterClass);
                spells.AddRange(levelSpells);
            }

            return spells;
        }

        private IEnumerable<Spell> GetAllKnownSpells(CharacterClass characterClass, Dictionary<string, Ability> abilities)
        {
            var levelForSpells = Math.Min(characterClass.Level, 20);
            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, levelForSpells, characterClass.Name);
            var knownSpellQuantitiesForClass = adjustmentsSelector.SelectAllFrom(tableName);

            var spellAbility = collectionsSelector
                .SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, characterClass.Name + GroupConstants.Spellcasters)
                .Single();
            var maxSpellLevel = abilities[spellAbility].Value - 10;
            var spellLevels = knownSpellQuantitiesForClass.Keys
                .Select(k => Convert.ToInt32(k))
                .Where(l => l <= maxSpellLevel);
            var spells = new List<Spell>();

            foreach (var spellLevel in spellLevels)
            {
                var spellNames = GetSpellNames(characterClass, spellLevel);

                foreach (var spellName in spellNames)
                {
                    var existingSpell = spells.FirstOrDefault(s => s.Name == spellName);
                    if (existingSpell != null)
                    {
                        existingSpell.Sources[characterClass.Name] = spellLevel;
                        continue;
                    }

                    var spell = BuildSpell(spellName, spellLevel, characterClass.Name);
                    spells.Add(spell);
                }

                foreach (var field in characterClass.SpecialistFields)
                {
                    var specialistSpellNames = GetSpellNamesForField(field, spellLevel);

                    foreach (var spellName in specialistSpellNames)
                    {
                        var existingSpell = spells.FirstOrDefault(s => s.Name == spellName);
                        if (existingSpell != null)
                        {
                            existingSpell.Sources[field] = spellLevel;
                            continue;
                        }

                        var spell = BuildSpell(spellName, spellLevel, field);
                        spells.Add(spell);
                    }
                }
            }

            return spells;
        }

        private IEnumerable<string> GetSpellNames(CharacterClass characterClass, int level)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, characterClass.Name);
            var spellsOfLevel = collectionsSelector.SelectFrom(Config.Name, tableName, level.ToString());
            var forbiddenSpellNames = GetSpellNamesForFields(characterClass.ProhibitedFields);

            var spellNames = spellsOfLevel.Except(forbiddenSpellNames);

            return spellNames;
        }

        private Spell BuildSpell(string name, int level, string source)
        {
            var spell = new Spell
            {
                Name = name,
            };
            spell.Sources[source] = level;

            return spell;
        }

        private IEnumerable<SpellQuantity> GetKnownSpellQuantities(CharacterClass characterClass, Dictionary<string, Ability> abilities)
        {
            var levelForSpells = Math.Min(characterClass.Level, 20);
            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, levelForSpells, characterClass.Name);
            var spellsForClass = adjustmentsSelector.SelectAllFrom(tableName);

            var spellAbility = collectionsSelector
                .SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, characterClass.Name + GroupConstants.Spellcasters)
                .Single();
            var maxSpellLevel = abilities[spellAbility].Value - 10;
            var spellQuantitiesForCharacter = spellsForClass.Where(kvp => Convert.ToInt32(kvp.Key) <= maxSpellLevel);

            var spellQuantities = new List<SpellQuantity>();
            var knowsMoreSpellsTableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSKnowsAdditionalSpells, characterClass.Name);

            foreach (var kvp in spellQuantitiesForCharacter)
            {
                var level = Convert.ToInt32(kvp.Key);
                var spellQuantity = new SpellQuantity
                {
                    Source = characterClass.Name,
                    Level = level,
                    Quantity = kvp.Value,
                    HasDomainSpell = characterClass.SpecialistFields.Any() && level > 0,
                };

                while (percentileSelector.SelectFrom<bool>(Config.Name, knowsMoreSpellsTableName))
                    spellQuantity.Quantity++;

                spellQuantities.Add(spellQuantity);
            }

            return spellQuantities;
        }

        private IEnumerable<Spell> GetRandomKnownSpellsForLevel(SpellQuantity spellQuantity, CharacterClass characterClass)
        {
            var spellNamesForLevel = GetSpellNames(characterClass, spellQuantity.Level);
            var knownSpellsForLevel = new HashSet<Spell>();

            if (spellQuantity.Quantity >= spellNamesForLevel.Count())
            {
                foreach (var spellName in spellNamesForLevel)
                {
                    var spell = BuildSpell(spellName, spellQuantity.Level, characterClass.Name);
                    knownSpellsForLevel.Add(spell);
                }

                return knownSpellsForLevel;
            }

            var knownSpellNamesForLevel = knownSpellsForLevel.Select(s => s.Name);
            var unknownSpellNamesForLevel = spellNamesForLevel.Except(knownSpellNamesForLevel);

            while (spellQuantity.Quantity > knownSpellsForLevel.Count)
            {
                var spellName = collectionsSelector.SelectRandomFrom(unknownSpellNamesForLevel);
                var spell = BuildSpell(spellName, spellQuantity.Level, characterClass.Name);
                knownSpellsForLevel.Add(spell);
            }

            foreach (var field in characterClass.SpecialistFields)
            {
                var specialistSpellNamesForLevel = GetSpellNamesForField(field, spellQuantity.Level);
                var knownSpecialistSpellNames = specialistSpellNamesForLevel.Intersect(knownSpellNamesForLevel);

                foreach (var spellName in knownSpecialistSpellNames)
                {
                    var spell = knownSpellsForLevel.First(s => s.Name == spellName);
                    spell.Sources[field] = spellQuantity.Level;
                }

                if (!spellQuantity.HasDomainSpell)
                    continue;

                var unknownSpecialistSpells = specialistSpellNamesForLevel.Except(knownSpellNamesForLevel);
                if (unknownSpecialistSpells.Any())
                {
                    while (spellQuantity.Quantity + 1 > knownSpellsForLevel.Count)
                    {
                        var spellName = collectionsSelector.SelectRandomFrom(unknownSpecialistSpells);
                        var spell = BuildSpell(spellName, spellQuantity.Level, field);
                        knownSpellsForLevel.Add(spell);
                    }

                    continue;
                }

                var unknownNonSpecialistSpells = unknownSpellNamesForLevel.Except(specialistSpellNamesForLevel);

                while (spellQuantity.Quantity + 1 > knownSpellsForLevel.Count)
                {
                    var spellName = collectionsSelector.SelectRandomFrom(unknownNonSpecialistSpells);
                    var spell = BuildSpell(spellName, spellQuantity.Level, characterClass.Name);
                    knownSpellsForLevel.Add(spell);
                }
            }

            return knownSpellsForLevel;
        }

        public IEnumerable<Spell> GeneratePrepared(CharacterClass characterClass, IEnumerable<Spell> knownSpells, IEnumerable<SpellQuantity> spellsPerDay)
        {
            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);
            var spells = new List<Spell>();

            if (!spellcasters.Contains(characterClass.Name))
                return spells;

            foreach (var spellQuantity in spellsPerDay)
            {
                var levelSpells = GetPreparedSpellsForLevel(spellQuantity, knownSpells, characterClass);
                spells.AddRange(levelSpells);
            }

            return spells;
        }

        private IEnumerable<Spell> GetPreparedSpellsForLevel(SpellQuantity spellQuantity, IEnumerable<Spell> knownSpells, CharacterClass characterClass)
        {
            var preparedSpells = new List<Spell>();
            var knownSpellsForLevel = knownSpells.Where(s => s.Sources.ContainsKey(spellQuantity.Source) && s.Sources[spellQuantity.Source] == spellQuantity.Level);

            while (spellQuantity.Quantity > preparedSpells.Count)
            {
                var spell = collectionsSelector.SelectRandomFrom(knownSpellsForLevel);
                preparedSpells.Add(spell);
            }

            if (spellQuantity.HasDomainSpell && spellQuantity.Source == characterClass.Name)
            {
                var specialistSpellsForLevel = GetSpecialistSpellsForLevel(characterClass.SpecialistFields, spellQuantity.Level, knownSpells);
                var spell = collectionsSelector.SelectRandomFrom(specialistSpellsForLevel);

                preparedSpells.Add(spell);
            }

            return preparedSpells;
        }

        private IEnumerable<Spell> GetSpecialistSpellsForLevel(IEnumerable<string> fields, int level, IEnumerable<Spell> knownSpells)
        {
            var specialistSpellsForLevel = Enumerable.Empty<Spell>();
            foreach (var field in fields)
            {
                var fieldSpellsForLevel = knownSpells.Where(s => s.Sources.ContainsKey(field) && s.Sources[field] == level);
                specialistSpellsForLevel = specialistSpellsForLevel.Concat(fieldSpellsForLevel);
            }

            return specialistSpellsForLevel;
        }

        private IEnumerable<string> GetSpellNamesForFields(IEnumerable<string> fields)
        {
            var fieldSpellNames = new List<string>();

            foreach (var field in fields)
            {
                var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, field);
                var fieldSpells = collectionsSelector.SelectAllFrom(Config.Name, tableName).Values.SelectMany(v => v);
                fieldSpellNames.AddRange(fieldSpells);
            }

            return fieldSpellNames;
        }

        private IEnumerable<string> GetSpellNamesForField(string field, int spellLevel)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, field);
            var spellsOfLevel = collectionsSelector.SelectFrom(Config.Name, tableName, spellLevel.ToString());

            return spellsOfLevel;
        }
    }
}
