using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Items
{
    internal class WeaponGenerator(
        ICollectionSelector collectionsSelector,
        JustInTimeFactory justInTimeFactory,
        ITreasureLevelSelector treasureLevelSelector) : IWeaponGenerator
    {
        private readonly ICollectionSelector collectionsSelector = collectionsSelector;
        private readonly MundaneItemGenerator mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
        private readonly MagicalItemGenerator magicalWeaponGenerator = justInTimeFactory.Build<MagicalItemGenerator>(ItemTypeConstants.Weapon);
        private readonly ITreasureLevelSelector treasureLevelSelector = treasureLevelSelector;

        public Weapon GenerateFrom(FeatCollections feats, CharacterClass characterClass, Race race)
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
            return nonProficiencyFeats.Where(f => GetPossibleWeapons(f).Any());
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
            var ammunition = GenerateFrom(ammunitionType, characterClass, race);
            return ammunition;
        }

        public Weapon GenerateMeleeFrom(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var meleeWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee);
            var weapon = GenerateFiltered(feats, characterClass, race, meleeWeapons);
            return weapon;
        }

        private Weapon GenerateFiltered(FeatCollections feats, CharacterClass characterClass, Race race, IEnumerable<string> filteredWeapons)
        {
            var possibleWeapons = GetPossibleWeapons(feats.All, filteredWeapons);
            if (!possibleWeapons.Any())
                return null;

            var weaponName = GetPreferredWeapon(feats, filteredWeapons);
            var filteredWeapon = GenerateFrom(weaponName, characterClass, race);
            return filteredWeapon;
        }

        private IEnumerable<string> GetPossibleWeapons(IEnumerable<Feat> feats, IEnumerable<string> filteredWeapons)
        {
            var nonProficiencyFeats = GetNonProficiencyFeatsWithWeaponFoci(feats);
            var proficiencyFeats = GetProficiencyFeats(feats);

            var nonProficiencyWeapons = nonProficiencyFeats.SelectMany(GetPossibleWeapons);
            var proficiencyWeapons = proficiencyFeats.SelectMany(GetPossibleWeapons);

            return proficiencyWeapons.Concat(nonProficiencyWeapons).Intersect(filteredWeapons);
        }

        private Weapon GenerateFrom(string weaponName, CharacterClass characterClass, Race race)
        {
            var power = treasureLevelSelector.SelectPowerFrom(characterClass, race);

            var weapon = GenerateWeapon(power, weaponName, race);
            return weapon as Weapon;
        }

        private Item GenerateWeapon(string power, string weaponName, Race race)
        {
            if (power == PowerConstants.Mundane)
            {
                var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, race.Size);
                return mundaneWeapon;
            }

            var magicalWeapon = magicalWeaponGenerator.Generate(power, weaponName, race.Size);
            return magicalWeapon;
        }

        public Weapon GenerateOneHandedMeleeFrom(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var melee = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee);
            var twoHanded = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.TwoHanded);
            var filteredWeapons = melee.Except(twoHanded);

            var weapon = GenerateFiltered(feats, characterClass, race, filteredWeapons);
            return weapon;
        }

        public Weapon GenerateRangedFrom(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var rangedWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ranged);
            var ammo = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ammunition);
            var filteredWeapons = rangedWeapons.Except(ammo);

            var weapon = GenerateFiltered(feats, characterClass, race, filteredWeapons);
            return weapon;
        }

        private string GetPreferredWeapon(FeatCollections feats, IEnumerable<string> filteredWeapons)
        {
            var commonWeapons = GetPreferredWeapons(feats.Additional, filteredWeapons);
            var uncommonWeapons = GetPreferredWeapons(feats.Class, filteredWeapons);
            var rareWeapons = GetPreferredWeapons(feats.Racial, filteredWeapons);

            var preferredWeapon = collectionsSelector.SelectRandomFrom(commonWeapons, uncommonWeapons, rareWeapons);
            return preferredWeapon;
        }

        private IEnumerable<string> GetPreferredWeapons(IEnumerable<Feat> feats, IEnumerable<string> filteredWeapons)
        {
            var proficiencyFeats = GetProficiencyFeats(feats);
            if (!proficiencyFeats.Any())
                return [];

            var preferredFeat = GetPreferredFeat(feats);
            var preferredWeapons = GetPossibleWeapons(preferredFeat);

            return preferredWeapons.Intersect(filteredWeapons);
        }

        private IEnumerable<string> GetPossibleWeapons(Feat feat, IEnumerable<Feat> allFeats)
        {
            var possibleWeapons = feat.Foci;

            if (feat.Foci.Contains(FeatConstants.Foci.All))
            {
                possibleWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci, feat.Name);

                if (feat.Name == FeatConstants.MartialWeaponProficiency)
                {
                    var weaponFamiliarities = allFeats.Where(f => f.Name == FeatConstants.WeaponFamiliarity).SelectMany(f => f.Foci);
                    possibleWeapons = possibleWeapons.Concat(weaponFamiliarities);
                }
            }

            var allWeapons = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon);
            return possibleWeapons.Intersect(allWeapons);
        }

        private Feat GetPreferredFeat(IEnumerable<Feat> feats)
        {
            var nonProficiencyFeats = GetNonProficiencyFeatsWithWeaponFoci(feats);
            var proficiencyFeats = GetProficiencyFeats(feats);

            return collectionsSelector.SelectRandomFrom(
                nonProficiencyFeats,
                proficiencyFeats.Where(f => f.Name == FeatConstants.MartialWeaponProficiency),
                proficiencyFeats.Where(f => f.Name == FeatConstants.SimpleWeaponProficiency),
                proficiencyFeats.Where(f => f.Name == FeatConstants.ExoticWeaponProficiency));
        }
    }
}
