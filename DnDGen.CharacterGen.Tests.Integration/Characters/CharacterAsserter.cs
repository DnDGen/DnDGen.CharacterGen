using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Characters;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Characters
{
    public class CharacterAsserter
    {
        private readonly IEnumerable<string> skillsWithFoci;

        public CharacterAsserter()
        {
            skillsWithFoci =
            [
                SkillConstants.Craft,
                SkillConstants.Knowledge,
                SkillConstants.Perform,
                SkillConstants.Profession,
            ];
        }

        public void AssertCharacter(Character character)
        {
            AssertSummary(character);
            AssertAlignment(character);
            AssertCharacterClass(character);
            AssertRace(character);
            AssertAbilities(character);
            AssertLanguages(character);
            AssertSkills(character);
            AssertFeats(character);
            AssertEquipment(character);
            AssertMagic(character);
            AssertCombat(character);

            Assert.That(character.InterestingTrait, Is.Not.Empty, character.Summary);
            Assert.That(character.ChallengeRating, Is.Positive, character.Summary);
            Assert.That(character.ChallengeRating, Is.AtLeast(character.Race.ChallengeRating), character.Summary);
        }

        private void AssertSummary(Character character)
        {
            Assert.That(character.Summary, Is.Not.Empty);
            Assert.That(character.Summary, Contains.Substring(character.Alignment.Full));
            Assert.That(character.Summary, Contains.Substring(character.Class.Summary));
            Assert.That(character.Summary, Contains.Substring(character.Race.Summary));
        }

        private void AssertAlignment(Character character)
        {
            Assert.That(character.Alignment.Goodness, Is.EqualTo(AlignmentConstants.Good)
                .Or.EqualTo(AlignmentConstants.Neutral)
                .Or.EqualTo(AlignmentConstants.Evil), character.Summary);
            Assert.That(character.Alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Lawful)
                .Or.EqualTo(AlignmentConstants.Neutral)
                .Or.EqualTo(AlignmentConstants.Chaotic), character.Summary);
        }

        private void AssertCharacterClass(Character character)
        {
            Assert.That(character.Class.Name, Is.Not.Empty, character.Summary);
            Assert.That(character.Class.Level, Is.Positive, character.Summary);
            Assert.That(character.Class.LevelAdjustment, Is.Not.Negative, character.Summary);
            Assert.That(character.Class.EffectiveLevel, Is.Positive, character.Summary);
            Assert.That(character.Class.ProhibitedFields, Is.Not.Null, character.Summary);
            Assert.That(character.Class.SpecialistFields, Is.Not.Null, character.Summary);
            Assert.That(character.Class.Summary, Is.Not.Empty, character.Summary);

            if (character.Class.SpecialistFields.Any())
            {
                var message = $"{character.Summary}; S: {string.Join(", ", character.Class.SpecialistFields)}; P: {string.Join(", ", character.Class.ProhibitedFields)}";
                var intersect = character.Class.SpecialistFields.Intersect(character.Class.ProhibitedFields);
                Assert.That(intersect, Is.Empty, message);

                if (character.Class.SpecialistFields.Any(f => f == CharacterClassConstants.Schools.Divination))
                {
                    Assert.That(character.Class.SpecialistFields.Count(), Is.EqualTo(1), message);
                    Assert.That(character.Class.ProhibitedFields.Count(), Is.EqualTo(1), message);
                }
                else if (character.Class.Name == CharacterClassConstants.Wizard)
                {
                    Assert.That(character.Class.SpecialistFields.Count(), Is.EqualTo(1), message);
                    Assert.That(character.Class.ProhibitedFields.Count(), Is.EqualTo(2), message);
                }
                else if (character.Class.Name == CharacterClassConstants.Cleric)
                {
                    Assert.That(character.Class.SpecialistFields.Count(), Is.EqualTo(2), message);
                }
            }
        }

        private void AssertRace(Character character)
        {
            Assert.That(character.Race.BaseRace, Is.Not.Empty, character.Summary);
            Assert.That(character.Race.Metarace, Is.Not.Null, character.Summary);
            Assert.That(character.Race.MetaraceSpecies, Is.Not.Null, character.Summary);
            Assert.That(character.Race.ChallengeRating, Is.Not.Negative, character.Summary);
            Assert.That(character.Race.Summary, Is.Not.Empty, character.Summary);
            Assert.That(character.Race.Size, Is.EqualTo(RaceConstants.Sizes.Large)
                .Or.EqualTo(RaceConstants.Sizes.Colossal)
                .Or.EqualTo(RaceConstants.Sizes.Gargantuan)
                .Or.EqualTo(RaceConstants.Sizes.Huge)
                .Or.EqualTo(RaceConstants.Sizes.Tiny)
                .Or.EqualTo(RaceConstants.Sizes.Medium)
                .Or.EqualTo(RaceConstants.Sizes.Small), character.Summary);

            AssertLandSpeed(character);
            AssertAerialSpeed(character);
            AssertSwimSpeed(character);
            AssertAge(character);
            AssertMaximumAge(character);
            AssertHeight(character);
            AssertWeight(character);
        }

        private void AssertLandSpeed(Character character)
        {
            Assert.That(character.Race.LandSpeed.Value, Is.Positive, character.Summary);
            Assert.That(character.Race.LandSpeed.Value % 5, Is.EqualTo(0), character.Summary);

            if (character.Race.LandSpeed.Value >= 10)
                Assert.That(character.Race.LandSpeed.Value % 10, Is.EqualTo(0), character.Summary);

            Assert.That(character.Race.LandSpeed.Unit, Is.EqualTo("feet per round"), character.Summary);
            Assert.That(character.Race.LandSpeed.Description, Is.Empty, character.Summary);
        }

        private void AssertAerialSpeed(Character character)
        {
            Assert.That(character.Race.AerialSpeed.Value, Is.Not.Negative, character.Summary);
            Assert.That(character.Race.AerialSpeed.Value % 5, Is.EqualTo(0), character.Summary);

            if (character.Race.AerialSpeed.Value >= 10)
                Assert.That(character.Race.AerialSpeed.Value % 10, Is.EqualTo(0), character.Summary);

            Assert.That(character.Race.AerialSpeed.Unit, Is.EqualTo("feet per round"), character.Summary);

            if (character.Race.AerialSpeed.Value == 0)
                Assert.That(character.Race.AerialSpeed.Description, Is.Empty, character.Summary);
            else
                Assert.That(character.Race.AerialSpeed.Description, Is.Not.Empty, character.Summary);

            if (character.Race.HasWings)
                Assert.That(character.Race.AerialSpeed.Value, Is.Positive, character.Summary);
        }

        private void AssertSwimSpeed(Character character)
        {
            Assert.That(character.Race.SwimSpeed.Value, Is.Not.Negative, character.Summary);
            Assert.That(character.Race.SwimSpeed.Value % 10, Is.EqualTo(0), character.Summary);
            Assert.That(character.Race.SwimSpeed.Unit, Is.EqualTo("feet per round"), character.Summary);
            Assert.That(character.Race.SwimSpeed.Description, Is.Empty, character.Summary);
        }

        private void AssertAge(Character character)
        {
            Assert.That(character.Race.Age.Description, Is.EqualTo(RaceConstants.Ages.Adulthood)
                .Or.EqualTo(RaceConstants.Ages.MiddleAge)
                .Or.EqualTo(RaceConstants.Ages.Old)
                .Or.EqualTo(RaceConstants.Ages.Venerable), character.Summary);
            Assert.That(character.Race.Age.Value, Is.Positive, character.Summary);
            Assert.That(character.Race.Age.Unit, Is.EqualTo("Years"), character.Summary);

            if (character.Race.MaximumAge.Value != RaceConstants.Ages.Ageless)
                Assert.That(character.Race.Age.Value, Is.LessThanOrEqualTo(character.Race.MaximumAge.Value), character.Summary);
        }

        private void AssertMaximumAge(Character character)
        {
            Assert.That(character.Race.MaximumAge.Value, Is.Positive.Or.EqualTo(RaceConstants.Ages.Ageless), character.Summary);
            Assert.That(character.Race.MaximumAge.Unit, Is.EqualTo("Years"), character.Summary);

            if (character.Race.MaximumAge.Value == RaceConstants.Ages.Ageless)
                Assert.That(character.Race.MaximumAge.Description, Is.EqualTo("Immortal"), character.Summary);
            else if (character.Race.BaseRace == RaceConstants.BaseRaces.Pixie)
                Assert.That(character.Race.MaximumAge.Description, Is.EqualTo("Will return to their plane of origin"), character.Summary);
            else
                Assert.That(character.Race.MaximumAge.Description, Is.EqualTo("Will die of natural causes"), character.Summary);
        }

        private void AssertHeight(Character character)
        {
            Assert.That(character.Race.Height.Value, Is.Positive, character.Summary);
            Assert.That(character.Race.Height.Unit, Is.EqualTo("Inches"), character.Summary);
            Assert.That(character.Race.Height.Description, Is.EqualTo("Short").Or.EqualTo("Average").Or.EqualTo("Tall"), character.Summary);
        }

        private void AssertWeight(Character character)
        {
            Assert.That(character.Race.Weight.Value, Is.Positive, character.Summary);
            Assert.That(character.Race.Weight.Unit, Is.EqualTo("Pounds"), character.Summary);
            Assert.That(character.Race.Weight.Description, Is.EqualTo("Light").Or.EqualTo("Average").Or.EqualTo("Heavy"), character.Summary);
        }

        private void AssertLanguages(Character character)
        {
            Assert.That(character.Languages, Is.Not.Empty, character.Summary);
        }

        private void AssertAbilities(Character character)
        {
            Assert.That(character.Abilities.Count, Is.InRange(5, 6), character.Summary);
            Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Charisma), character.Summary);
            Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Dexterity), character.Summary);
            Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Intelligence), character.Summary);
            Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Strength), character.Summary);
            Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Wisdom), character.Summary);

            if (character.Abilities.Count == 6)
                Assert.That(character.Abilities.Keys, Contains.Item(AbilityConstants.Constitution), character.Summary);

            foreach (var statKVP in character.Abilities)
            {
                var stat = statKVP.Value;
                Assert.That(stat.Name, Is.EqualTo(statKVP.Key), character.Summary);
                Assert.That(stat.Value, Is.AtLeast(3), character.Summary);
            }
        }

        private void AssertSkills(Character character)
        {
            Assert.That(character.Skills, Is.Not.Empty, character.Summary);

            foreach (var skill in character.Skills)
            {
                Assert.That(skill.ArmorCheckPenalty, Is.Not.Positive, character.Summary);
                Assert.That(skill.Ranks, Is.AtMost(skill.RankCap), character.Summary);
                Assert.That(skill.RankCap, Is.Positive, character.Summary);
                Assert.That(skill.Bonus, Is.Not.Negative);
                Assert.That(skill.BaseAbility, Is.Not.Null);
                Assert.That(character.Abilities.Values, Contains.Item(skill.BaseAbility));
                Assert.That(skill.Focus, Is.Not.Null);

                if (skillsWithFoci.Contains(skill.Name))
                    Assert.That(skill.Focus, Is.Not.Empty);
                else
                    Assert.That(skill.Focus, Is.Empty);
            }

            var skillNamesAndFoci = character.Skills.Select(s => s.Name + s.Focus);
            Assert.That(skillNamesAndFoci, Is.Unique);
        }

        private void AssertFeats(Character character)
        {
            Assert.That(character.Feats.Class, Is.Not.Empty, character.Summary);
            Assert.That(character.Feats.Racial, Is.Not.Null, character.Summary);
            Assert.That(character.Feats.Additional, Is.Not.Empty, character.Summary);
            Assert.That(character.Feats.All, Is.Not.Empty, character.Summary);

            foreach (var feat in character.Feats.All)
            {
                Assert.That(feat.Name, Is.Not.Empty, character.Summary);
                Assert.That(feat.Foci, Is.Not.Null, feat.Name);
                Assert.That(feat.Power, Is.Not.Negative, feat.Name);
                Assert.That(feat.Frequency.Quantity, Is.Not.Negative, feat.Name);
                Assert.That(feat.Frequency.TimePeriod, Is.EqualTo(FeatConstants.Frequencies.Constant)
                    .Or.EqualTo(FeatConstants.Frequencies.AtWill)
                    .Or.EqualTo(FeatConstants.Frequencies.Hit)
                    .Or.EqualTo(FeatConstants.Frequencies.Round)
                    .Or.EqualTo(FeatConstants.Frequencies.Turn)
                    .Or.EqualTo(FeatConstants.Frequencies.Day)
                    .Or.EqualTo(FeatConstants.Frequencies.Week)
                    .Or.Empty, feat.Name);

                if (feat.Name == FeatConstants.SaveBonus)
                    Assert.That(feat.Foci, Is.Not.Empty, character.Race.BaseRace);
            }

            AssertDistinctFeatsWithfoci(character.Feats.Racial, $"{character.Summary}: Racial");
            AssertDistinctFeatsWithfoci(character.Feats.Class, $"{character.Summary}: Class");
            AssertDistinctFeatsWithfoci(character.Feats.Additional, $"{character.Summary}: Additional");
        }

        private void AssertDistinctFeatsWithfoci(IEnumerable<Feat> feats, string message)
        {
            var featsWithFoci = feats
                .Where(f => f.Foci.Any())
                .GroupBy(f => f.Name + f.Power + f.Frequency.Quantity + f.Frequency.TimePeriod);

            foreach (var group in featsWithFoci)
            {
                Assert.That(group.Count(), Is.EqualTo(1), $"{message}; Group {group.Key}");
            }
        }

        private void AssertEquipment(Character character)
        {
            if (character.Feats.All.SelectMany(f => f.Foci).Contains(FeatConstants.Foci.UnarmedStrike) == false)
            {
                var feats = GetAllFeatsMessage(character.Feats.All);

                Assert.That(character.Equipment.PrimaryHand, Is.Not.Null, feats);
                Assert.That(character.Equipment.PrimaryHand.Name, Is.Not.Empty, feats);
                Assert.That(character.Equipment.PrimaryHand.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.Quantity, Is.Positive, character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.CanBeUsedAsWeaponOrArmor, Is.True, character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.CriticalMultiplier, Is.Not.Empty, character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.DamageRoll, Is.Not.Empty, character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.DamageDescription, Is.Not.Empty, character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.Size, Is.EqualTo(character.Race.Size), character.Equipment.PrimaryHand.Summary);
                Assert.That(character.Equipment.PrimaryHand.ThreatRange, Is.Positive, character.Equipment.PrimaryHand.Summary);

                if (character.Equipment.OffHand != null)
                {
                    Assert.That(character.Equipment.OffHand, Is.InstanceOf<Armor>().Or.InstanceOf<Weapon>(), feats);

                    if (character.Equipment.OffHand is Weapon)
                    {
                        var weapon = character.Equipment.OffHand as Weapon;
                        Assert.That(weapon, Is.Not.Null, feats);
                        Assert.That(weapon.Name, Is.Not.Empty, feats);
                        Assert.That(weapon.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), weapon.Summary);
                        Assert.That(weapon.Quantity, Is.Positive, weapon.Summary);
                        Assert.That(weapon.CanBeUsedAsWeaponOrArmor, Is.True, weapon.Summary);
                        Assert.That(weapon.CriticalMultiplier, Is.Not.Empty, weapon.Summary);
                        Assert.That(weapon.DamageRoll, Is.Not.Empty, weapon.Summary);
                        Assert.That(weapon.DamageDescription, Is.Not.Empty, weapon.Summary);
                        Assert.That(weapon.Size, Is.EqualTo(character.Race.Size), weapon.Summary);
                        Assert.That(weapon.ThreatRange, Is.Positive, weapon.Summary);

                        if (weapon != character.Equipment.PrimaryHand)
                        {
                            //HACK: item attributes are controlled by TreasureGen, and not all one-handed melee weapons contain the One-Handed attribute
                            Assert.That(weapon.Attributes, Contains.Item(AttributeConstants.OneHanded)
                                .Or.Not.Contains(AttributeConstants.TwoHanded), weapon.Summary);
                            Assert.That(weapon.Attributes, Contains.Item(AttributeConstants.Melee), weapon.Summary);
                        }
                        else
                        {
                            Assert.That(weapon.Attributes, Contains.Item(AttributeConstants.TwoHanded), weapon.Summary);
                        }
                    }
                    else if (character.Equipment.OffHand is Armor)
                    {
                        var shield = character.Equipment.OffHand as Armor;
                        Assert.That(shield, Is.Not.Null, feats);
                        Assert.That(shield.Name, Is.Not.Empty, feats);
                        Assert.That(shield.ItemType, Is.EqualTo(ItemTypeConstants.Armor), shield.Summary);
                        Assert.That(shield.Quantity, Is.Positive, shield.Summary);
                        Assert.That(shield.CanBeUsedAsWeaponOrArmor, Is.True, shield.Summary);
                        Assert.That(shield.ArmorBonus, Is.Positive, shield.Summary);
                        Assert.That(shield.ArmorCheckPenalty, Is.Not.Positive, shield.Summary);
                        Assert.That(shield.MaxDexterityBonus, Is.Not.Negative, shield.Summary);
                        Assert.That(shield.Size, Is.EqualTo(character.Race.Size), shield.Summary);
                        Assert.That(shield.Attributes, Contains.Item(AttributeConstants.Shield), shield.Summary);
                    }
                }

                if (character.Equipment.Armor != null)
                {
                    Assert.That(character.Equipment.Armor, Is.Not.Null, feats);
                    Assert.That(character.Equipment.Armor.Name, Is.Not.Empty, feats);
                    Assert.That(character.Equipment.Armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor), character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.Quantity, Is.Positive, character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.CanBeUsedAsWeaponOrArmor, Is.True, character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.ArmorBonus, Is.Positive, character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.ArmorCheckPenalty, Is.Not.Positive, character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.MaxDexterityBonus, Is.Not.Negative, character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.Size, Is.EqualTo(character.Race.Size), character.Equipment.Armor.Summary);
                    Assert.That(character.Equipment.Armor.Attributes, Is.All.Not.EqualTo(AttributeConstants.Shield));
                }
            }

            Assert.That(character.Equipment.Treasure, Is.Not.Null, character.Summary);
            Assert.That(character.Equipment.Treasure.Items, Is.Not.Null, character.Summary);
            Assert.That(character.Equipment.Treasure.Items, Is.All.Not.Null, character.Summary);

            foreach (var item in character.Equipment.Treasure.Items)
            {
                Assert.That(item.Name, Is.Not.Empty, character.Summary);
                Assert.That(item.Quantity, Is.Positive, item.Summary);
            }
        }

        private void AssertMagic(Character character)
        {
            Assert.That(character.Magic.Animal, Is.Not.Null, character.Summary);

            foreach (var spells in character.Magic.SpellsPerDay)
            {
                Assert.That(spells.Level, Is.Not.Negative, spells.Level.ToString());
                Assert.That(spells.Quantity, Is.Not.Negative, spells.Level.ToString());
                Assert.That(spells.Source, Is.Not.Empty, spells.Level.ToString());
            }

            foreach (var spell in character.Magic.KnownSpells)
            {
                Assert.That(spell.Metamagic, Is.Empty, spell.Summary);
                Assert.That(spell.Name, Is.Not.Empty, spell.Summary);
                Assert.That(spell.Sources, Is.Not.Empty, spell.Summary);
                Assert.That(spell.Sources.Keys, Is.All.Not.Empty, spell.Summary);
                Assert.That(spell.Sources.Values, Is.All.Not.Negative, spell.Summary);
            }

            foreach (var spell in character.Magic.PreparedSpells)
            {
                Assert.That(spell.Metamagic, Is.Empty, spell.Summary);
                Assert.That(spell.Name, Is.Not.Empty, spell.Summary);
                Assert.That(spell.Sources, Is.Not.Empty, spell.Summary);
                Assert.That(spell.Sources.Keys, Is.All.Not.Empty, spell.Summary);
                Assert.That(spell.Sources.Values, Is.All.Not.Negative, spell.Summary);
            }

            var knownSpellNames = character.Magic.KnownSpells.Select(s => s.Name);
            var preparedSpellNames = character.Magic.PreparedSpells.Select(s => s.Name);

            Assert.That(knownSpellNames, Is.Unique, character.Summary);
            Assert.That(preparedSpellNames.Distinct(), Is.SubsetOf(knownSpellNames), character.Summary);
        }

        private void AssertCombat(Character character)
        {
            Assert.That(character.Combat.BaseAttack.BaseBonus, Is.Not.Negative, character.Summary);
            Assert.That(character.Combat.BaseAttack.DexterityBonus, Is.EqualTo(character.Abilities[AbilityConstants.Dexterity].Bonus), character.Summary);
            Assert.That(character.Combat.BaseAttack.StrengthBonus, Is.EqualTo(character.Abilities[AbilityConstants.Strength].Bonus), character.Summary);
            Assert.That(character.Combat.BaseAttack.AllMeleeBonuses.Count, Is.InRange(1, 4), character.Summary);
            Assert.That(character.Combat.BaseAttack.AllRangedBonuses.Count, Is.InRange(1, 4), character.Summary);
            Assert.That(character.Combat.BaseAttack.AllRangedBonuses.Count, Is.EqualTo(character.Combat.BaseAttack.AllMeleeBonuses.Count()), character.Summary);
            Assert.That(character.Combat.BaseAttack.AllMeleeBonuses, Is.Unique, character.Summary);
            Assert.That(character.Combat.BaseAttack.AllMeleeBonuses, Is.Ordered.Descending, character.Summary);
            Assert.That(character.Combat.BaseAttack.AllRangedBonuses, Is.Unique, character.Summary);
            Assert.That(character.Combat.BaseAttack.AllRangedBonuses, Is.Ordered.Descending, character.Summary);
            Assert.That(character.Combat.BaseAttack.AllMeleeBonuses.First(), Is.EqualTo(character.Combat.BaseAttack.MeleeBonus));
            Assert.That(character.Combat.BaseAttack.AllRangedBonuses.First(), Is.EqualTo(character.Combat.BaseAttack.RangedBonus));
            Assert.That(character.Combat.BaseAttack.RacialModifier, Is.Not.Negative);

            if (character.Abilities[AbilityConstants.Dexterity].Bonus != character.Abilities[AbilityConstants.Strength].Bonus)
                Assert.That(character.Combat.BaseAttack.AllMeleeBonuses, Is.Not.EquivalentTo(character.Combat.BaseAttack.AllRangedBonuses), character.Summary);

            Assert.That(character.Combat.HitPoints, Is.AtLeast(character.Class.Level), character.Summary);
            Assert.That(character.Combat.ArmorClass.Full, Is.Positive, character.Summary);
            Assert.That(character.Combat.ArmorClass.FlatFooted, Is.Positive, character.Summary);
            Assert.That(character.Combat.ArmorClass.Touch, Is.Positive, character.Summary);
            Assert.That(character.Combat.AdjustedDexterityBonus, Is.AtMost(character.Abilities[AbilityConstants.Dexterity].Bonus), character.Summary);
            Assert.That(character.Combat.InitiativeBonus, Is.AtLeast(character.Combat.AdjustedDexterityBonus));

            Assert.That(character.Combat.SavingThrows.Reflex, Is.AtLeast(character.Abilities[AbilityConstants.Dexterity].Bonus));
            Assert.That(character.Combat.SavingThrows.Will, Is.AtLeast(character.Abilities[AbilityConstants.Wisdom].Bonus));
            Assert.That(character.Combat.SavingThrows.HasFortitudeSave, Is.EqualTo(character.Abilities.ContainsKey(AbilityConstants.Constitution)));

            if (character.Combat.SavingThrows.HasFortitudeSave)
                Assert.That(character.Combat.SavingThrows.Fortitude, Is.AtLeast(character.Abilities[AbilityConstants.Constitution].Bonus));
        }

        private string GetAllFeatsMessage(IEnumerable<Feat> feats)
        {
            var featsWithFoci = feats
                .Where(f => f.Foci.Any())
                .Select(f => $"{f.Name}: {string.Join(", ", f.Foci)}")
                .OrderBy(f => f);
            return string.Join("; ", featsWithFoci);
        }

        public void AssertSpellcaster(Character spellcaster)
        {
            Assert.That(spellcaster.Magic, Is.Not.Null);
            Assert.That(spellcaster.Magic.Animal, Is.Not.Null);
            Assert.That(spellcaster.Magic.ArcaneSpellFailure, Is.InRange(0, 100));

            Assert.That(spellcaster.Magic.SpellsPerDay, Is.Not.Empty, spellcaster.Class.Summary);

            var levelsAndSources = spellcaster.Magic.SpellsPerDay.Select(s => s.Source + s.Level);
            Assert.That(levelsAndSources, Is.Unique);

            var spellsPerDayLevels = spellcaster.Magic.SpellsPerDay.Select(s => s.Level);
            var maxSpellLevel = spellsPerDayLevels.Max();
            var minSpellLevel = spellsPerDayLevels.Min();

            Assert.That(minSpellLevel, Is.InRange(0, 1));
            Assert.That(maxSpellLevel, Is.InRange(0, 9));

            foreach (var spellQuantity in spellcaster.Magic.SpellsPerDay)
            {
                Assert.That(spellQuantity.Level, Is.InRange(minSpellLevel, maxSpellLevel));
                Assert.That(spellQuantity.Quantity, Is.Not.Negative);
                Assert.That(spellQuantity.Source, Is.Not.Empty);

                if (spellQuantity.HasDomainSpell == false)
                    Assert.That(spellQuantity.Quantity, Is.Positive);

                if (spellQuantity.Level > 0 && spellQuantity.Source == spellcaster.Class.Name)
                    Assert.That(spellQuantity.HasDomainSpell, Is.EqualTo(spellcaster.Class.SpecialistFields.Any()));
                else
                    Assert.That(spellQuantity.HasDomainSpell, Is.False);
            }

            Assert.That(spellcaster.Magic.KnownSpells, Is.Not.Empty, spellcaster.Class.Summary);

            //INFO: Adding 1 to max spell, because you might know a spell that you cannot yet cast
            var maxKnownSpellLevel = Math.Min(9, maxSpellLevel + 1);

            foreach (var knownSpell in spellcaster.Magic.KnownSpells)
                AssertSpell(knownSpell, minSpellLevel, maxKnownSpellLevel);

            if (spellcaster.Magic.SpellsPerDay.All(s => s.Source == CharacterClassConstants.Bard || s.Source == CharacterClassConstants.Sorcerer))
            {
                Assert.That(spellcaster.Magic.PreparedSpells, Is.Empty, spellcaster.Class.Summary);
                return;
            }

            Assert.That(spellcaster.Magic.PreparedSpells, Is.Not.Empty, spellcaster.Class.Summary);

            foreach (var preparedSpell in spellcaster.Magic.PreparedSpells)
                AssertSpell(preparedSpell, minSpellLevel, maxSpellLevel);
        }

        private void AssertSpell(Spell spell, int minSpellLevel, int maxSpellLevel)
        {
            Assert.That(spell.Name, Is.Not.Empty, spell.Summary);
            Assert.That(spell.Sources, Is.Not.Empty, spell.Summary);
            Assert.That(spell.Sources.Keys, Is.All.Not.Empty, spell.Summary);
            Assert.That(spell.Sources.Values, Is.All.InRange(minSpellLevel, maxSpellLevel), spell.Summary);
            Assert.That(spell.Metamagic, Is.Empty, spell.Summary);
        }

        public void AssertUndead(Character character)
        {
            Assert.That(character.Race.Metarace, Is.EqualTo(RaceConstants.Metaraces.Ghost)
                .Or.EqualTo(RaceConstants.Metaraces.Lich)
                .Or.EqualTo(RaceConstants.Metaraces.Vampire));
            Assert.That(character.Race.ChallengeRating, Is.Positive, character.Summary);
            Assert.That(character.Abilities.Keys, Is.All.Not.EqualTo(AbilityConstants.Constitution), character.Summary);
            Assert.That(character.Combat.SavingThrows.HasFortitudeSave, Is.False, character.Summary);
        }

        public void AssertGhost(Character character)
        {
            AssertUndead(character);

            Assert.That(character.Race.Metarace, Is.EqualTo(RaceConstants.Metaraces.Ghost));
            Assert.That(character.Race.AerialSpeed.Value, Is.Positive);
            Assert.That(character.Race.AerialSpeed.Description, Is.Not.Empty);

            var ghostSpecialAttacks = new[]
            {
                FeatConstants.CorruptingGaze,
                FeatConstants.CorruptingTouch,
                FeatConstants.DrainingTouch,
                FeatConstants.FrightfulMoan,
                FeatConstants.HorrificAppearance,
                FeatConstants.Malevolence,
                FeatConstants.Telekinesis,
            };

            var featNames = character.Feats.All.Select(f => f.Name);
            var ghostSpecialAttackFeats = featNames.Intersect(ghostSpecialAttacks);
            var ghostSpecialAttackFeat = character.Feats.All.Single(f => f.Name == FeatConstants.GhostSpecialAttack);

            Assert.That(ghostSpecialAttackFeats.Count, Is.EqualTo(ghostSpecialAttackFeat.Foci.Count()));
            Assert.That(ghostSpecialAttackFeats.Count, Is.InRange(1, 3));
        }
    }
}
