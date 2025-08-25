using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Magics
{
    internal class MagicGenerator : IMagicGenerator
    {
        private readonly ISpellsGenerator spellsGenerator;
        private readonly IAnimalGenerator animalGenerator;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;

        public MagicGenerator(
            ISpellsGenerator spellsGenerator,
            IAnimalGenerator animalGenerator,
            ICollectionSelector collectionsSelector,
            IAdjustmentsSelector adjustmentsSelector)
        {
            this.spellsGenerator = spellsGenerator;
            this.animalGenerator = animalGenerator;
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
        }

        public Magic GenerateWith(
            Alignment alignment,
            CharacterClass characterClass,
            Race race,
            Dictionary<string, Ability> abilities,
            IEnumerable<Feat> feats,
            Equipment equipment)
        {
            var magic = new Magic();

            if (characterClass.Name != CharacterClassConstants.Sorcerer || race.BaseRace != RaceConstants.BaseRaces.Rakshasa)
            {
                magic = MakeSpells(magic, characterClass, abilities);
            }

            if (race.BaseRace == RaceConstants.BaseRaces.Rakshasa && characterClass.Name == CharacterClassConstants.Sorcerer)
            {
                var adjustedClass = new CharacterClass
                {
                    Name = characterClass.Name,
                    Level = characterClass.Level + 7,
                    ProhibitedFields = characterClass.ProhibitedFields,
                    SpecialistFields = characterClass.SpecialistFields
                };

                magic = MakeSpells(magic, adjustedClass, abilities);
            }
            else if (race.BaseRace == RaceConstants.BaseRaces.Rakshasa && characterClass.Name != CharacterClassConstants.Sorcerer)
            {
                var adjustedClass = new CharacterClass
                {
                    Name = CharacterClassConstants.Sorcerer,
                    Level = 7
                };

                var rakshasaMagic = new Magic();
                rakshasaMagic = MakeSpells(rakshasaMagic, adjustedClass, abilities);

                magic.SpellsPerDay = magic.SpellsPerDay.Union(rakshasaMagic.SpellsPerDay);
                magic.KnownSpells = CombineSpellLists(magic.KnownSpells, rakshasaMagic.KnownSpells, true);
                magic.PreparedSpells = CombineSpellLists(magic.PreparedSpells, rakshasaMagic.PreparedSpells, false);
            }

            magic.Animal = animalGenerator.GenerateFrom(alignment, characterClass, race, feats);

            if (equipment.Armor == null && equipment.OffHand == null)
                return magic;

            var arcaneSpellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane);
            if (arcaneSpellcasters.Contains(characterClass.Name) == false)
                return magic;

            var lightArmor = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency);

            if (equipment.Armor != null && (characterClass.Name != CharacterClassConstants.Bard || lightArmor.Contains(equipment.Armor.Name) == false))
                magic.ArcaneSpellFailure += GetArcaneSpellFailure(equipment.Armor);

            if (equipment.OffHand != null && equipment.OffHand.ItemType == ItemTypeConstants.Armor && equipment.OffHand.Attributes.Contains(AttributeConstants.Shield))
                magic.ArcaneSpellFailure += GetArcaneSpellFailure(equipment.OffHand);

            return magic;
        }

        private IEnumerable<Spell> CombineSpellLists(IEnumerable<Spell> first, IEnumerable<Spell> second, bool deduplicate)
        {
            var adjustedSecond = second;

            foreach (var spell in first)
            {
                var secondSpell = adjustedSecond.FirstOrDefault(s => s.Name == spell.Name);
                if (secondSpell == null)
                    continue;

                spell.Sources = spell.Sources
                    .Union(secondSpell.Sources)
                    .ToDictionary(s => s.Key, s => s.Value);
                secondSpell.Sources = spell.Sources;

                if (!deduplicate)
                    continue;

                adjustedSecond = adjustedSecond.Where(s => s.Name != spell.Name);
            }

            return first.Concat(adjustedSecond);
        }

        private Magic MakeSpells(Magic magic, CharacterClass characterClass, Dictionary<string, Ability> stats)
        {
            magic.SpellsPerDay = spellsGenerator.GeneratePerDay(characterClass, stats);
            magic.KnownSpells = spellsGenerator.GenerateKnown(characterClass, stats);

            var classesThatPrepareSpells = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.PreparesSpells);
            if (classesThatPrepareSpells.Contains(characterClass.Name))
                magic.PreparedSpells = spellsGenerator.GeneratePrepared(characterClass, magic.KnownSpells, magic.SpellsPerDay);

            return magic;
        }

        private int GetArcaneSpellFailure(Item item)
        {
            var arcaneSpellFailure = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.ArcaneSpellFailures, item.Name);

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Mithral))
                arcaneSpellFailure -= 10;

            return Math.Max(0, arcaneSpellFailure);
        }
    }
}
