using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Items
{
    internal class WeaponGenerator : IWeaponGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IPercentileSelector percentileSelector;
        private readonly JustInTimeFactory justInTimeFactory;

        public WeaponGenerator(ICollectionSelector collectionsSelector, IPercentileSelector percentileSelector, JustInTimeFactory justInTimeFactory)
        {
            this.collectionsSelector = collectionsSelector;
            this.percentileSelector = percentileSelector;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Weapon GenerateFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var allWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon);
            var ammo = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ammunition);
            var filteredWeapons = allWeapons.Except(ammo);

            var weapon = GenerateFiltered(feats, characterClass, race, filteredWeapons);
            return weapon;
        }

        private IEnumerable<Feat> GetNonProficiencyFeatsWithWeaponFoci(IEnumerable<Feat> feats)
        {
            var proficiencyFeats = GetProficiencyFeats(feats);
            var allWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon);

            var nonProficiencyFeats = feats.Except(proficiencyFeats).Where(f => f.Name != FeatConstants.WeaponFamiliarity);
            return nonProficiencyFeats.Where(f => allWeapons.Intersect(f.Foci).Any());
        }

        private IEnumerable<Feat> GetProficiencyFeatWithSpecificWeaponFocus(IEnumerable<Feat> feats)
        {
            var proficiencyFeats = GetProficiencyFeats(feats);
            var allWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon);

            return proficiencyFeats.Where(f => allWeapons.Intersect(f.Foci).Any());
        }

        private IEnumerable<Feat> GetProficiencyFeatWithFocusOfAll(IEnumerable<Feat> feats)
        {
            var proficiencyFeats = GetProficiencyFeats(feats);
            return proficiencyFeats.Where(f => f.Foci.Contains(FeatConstants.Foci.All));
        }

        private IEnumerable<Feat> GetProficiencyFeats(IEnumerable<Feat> feats)
        {
            var proficiencyFeatNames = collectionsSelector.SelectFrom(
                Config.Name,
                TableNameConstants.Set.Collection.FeatGroups,
                ItemTypeConstants.Weapon + GroupConstants.Proficiency);
            return feats.Where(f => proficiencyFeatNames.Contains(f.Name));
        }

        public Weapon GenerateAmmunition(CharacterClass characterClass, Race race, string ammunitionType)
        {
            var ammunitions = new[] { ammunitionType };
            var ammunition = GenerateFrom(ammunitions, characterClass, race);

            return ammunition;
        }

        public Weapon GenerateMeleeFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var meleeWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee);
            var weapon = GetWeapon(feats, characterClass, race, meleeWeapons);
            return weapon;
        }

        private Weapon GetWeapon(IEnumerable<Feat> feats, CharacterClass characterClass, Race race, IEnumerable<string> filteredWeapons)
        {
            if (!ValidWeaponIsPossible(feats, filteredWeapons))
                return null;

            var nonProficiencyFeatsWithWeaponFoci = GetNonProficiencyFeatsWithWeaponFoci(feats);
            if (ValidWeaponIsPossible(nonProficiencyFeatsWithWeaponFoci, filteredWeapons))
            {
                var weapon = GenerateFiltered(feats, characterClass, race, filteredWeapons);
                return weapon;
            }

            var featSubset = feats.Except(nonProficiencyFeatsWithWeaponFoci);
            var proficiencyFeatsWithSpecificWeaponFoci = GetProficiencyFeatWithSpecificWeaponFocus(feats);
            if (ValidWeaponIsPossible(proficiencyFeatsWithSpecificWeaponFoci, filteredWeapons))
            {
                var weapon = GenerateFiltered(featSubset, characterClass, race, filteredWeapons);
                return weapon;
            }

            featSubset = featSubset.Except(proficiencyFeatsWithSpecificWeaponFoci);
            if (ValidWeaponIsPossible(featSubset, filteredWeapons))
            {
                var weapon = GenerateFiltered(featSubset, characterClass, race, filteredWeapons);
                return weapon;
            }

            return null;
        }

        private Weapon GenerateFiltered(IEnumerable<Feat> feats, CharacterClass characterClass, Race race, IEnumerable<string> filteredWeapons)
        {
            var allowedWeapons = GetAllowedWeapons(feats).Intersect(filteredWeapons);

            if (allowedWeapons.Any() == false)
                return null;

            var filteredWeapon = GenerateFrom(allowedWeapons, characterClass, race);
            return filteredWeapon;
        }

        private IEnumerable<string> GetAllowedWeapons(IEnumerable<Feat> feats)
        {
            var allowedWeapons = GetWeaponsFromNonProficiencyFeats(feats);

            if (allowedWeapons.Any())
                return allowedWeapons;

            allowedWeapons = GetSpecificWeaponsFromProficiencyFeats(feats);

            if (allowedWeapons.Any())
                return allowedWeapons;

            allowedWeapons = GetWeaponsFromAllProficiencyFeats(feats);

            return allowedWeapons;
        }

        private IEnumerable<string> GetWeaponsFromNonProficiencyFeats(IEnumerable<Feat> feats)
        {
            var nonProficiencyFeatsWithWeaponFoci = GetNonProficiencyFeatsWithWeaponFoci(feats);
            var allowedWeapons = nonProficiencyFeatsWithWeaponFoci.SelectMany(f => f.Foci).Distinct();
            return GetAllowedWeapons(allowedWeapons);
        }

        private IEnumerable<string> GetSpecificWeaponsFromProficiencyFeats(IEnumerable<Feat> feats)
        {
            var proficiencyFeatsWithSpecificWeaponFoci = GetProficiencyFeatWithSpecificWeaponFocus(feats);
            var allowedWeapons = proficiencyFeatsWithSpecificWeaponFoci.SelectMany(f => f.Foci).Distinct();
            return GetAllowedWeapons(allowedWeapons);
        }

        private IEnumerable<string> GetWeaponsFromAllProficiencyFeats(IEnumerable<Feat> feats)
        {
            var proficientWithAllInFeat = GetProficiencyFeatWithFocusOfAll(feats);
            var weapons = new List<string>();

            foreach (var feat in proficientWithAllInFeat)
            {
                var featWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci, feat.Name);
                weapons.AddRange(featWeapons);
            }

            return GetAllowedWeapons(weapons);
        }

        private bool ValidWeaponIsPossible(IEnumerable<Feat> feats, IEnumerable<string> filteredWeapons)
        {
            var nonProficiencyWeapons = GetWeaponsFromNonProficiencyFeats(feats);
            var specificProficiencyWeapons = GetSpecificWeaponsFromProficiencyFeats(feats);
            var allProficiencyWeapons = GetWeaponsFromAllProficiencyFeats(feats);

            var allAllowedWeapons = nonProficiencyWeapons.Union(specificProficiencyWeapons).Union(allProficiencyWeapons);
            return allAllowedWeapons.Intersect(filteredWeapons).Any();
        }

        private IEnumerable<string> GetAllowedWeapons(IEnumerable<string> source)
        {
            var allWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon);
            var allowedWeapons = source.Intersect(allWeapons);

            return allowedWeapons;
        }

        private Weapon GenerateFrom(IEnumerable<string> allowedWeapons, CharacterClass characterClass, Race race)
        {
            var effectiveLevel = (int)Math.Max(1, characterClass.EffectiveLevel);
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            var power = percentileSelector.SelectFrom(Config.Name, tableName);

            var weapon = GenerateWeapon(power, allowedWeapons, race);

            return weapon as Weapon;
        }

        private Item GenerateWeapon(string power, IEnumerable<string> proficientWeapons, Race race)
        {
            var weaponName = collectionsSelector.SelectRandomFrom(proficientWeapons);

            if (power == PowerConstants.Mundane)
            {
                var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
                var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, race.Size);

                return mundaneWeapon;
            }

            var magicalWeaponGenerator = justInTimeFactory.Build<MagicalItemGenerator>(ItemTypeConstants.Weapon);
            var magicalWeapon = magicalWeaponGenerator.Generate(power, weaponName, race.Size);

            return magicalWeapon;
        }

        public Weapon GenerateOneHandedMeleeFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var melee = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee);
            var twoHanded = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.TwoHanded);
            var filteredWeapons = melee.Except(twoHanded);

            var weapon = GetWeapon(feats, characterClass, race, filteredWeapons);
            return weapon;
        }

        public Weapon GenerateRangedFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var rangedWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ranged);
            var ammo = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ammunition);
            var filteredWeapons = rangedWeapons.Except(ammo);

            var weapon = GetWeapon(feats, characterClass, race, filteredWeapons);
            return weapon;
        }
    }
}
