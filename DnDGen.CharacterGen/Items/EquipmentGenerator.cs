using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators;
using DnDGen.TreasureGen.Items;
using System;
using System.Linq;

namespace DnDGen.CharacterGen.Items
{
    internal class EquipmentGenerator(
        ICollectionSelector collectionsSelector,
        IWeaponGenerator weaponGenerator,
        ITreasureGenerator treasureGenerator,
        IArmorGenerator armorGenerator,
        ITreasureLevelSelector treasureLevelSelector) : IEquipmentGenerator
    {
        private readonly ICollectionSelector collectionsSelector = collectionsSelector;
        private readonly IArmorGenerator armorGenerator = armorGenerator;
        private readonly IWeaponGenerator weaponGenerator = weaponGenerator;
        private readonly ITreasureGenerator treasureGenerator = treasureGenerator;
        private readonly ITreasureLevelSelector treasureLevelSelector = treasureLevelSelector;

        public Equipment GenerateWith(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var equipment = new Equipment();
            var effectiveLevel = treasureLevelSelector.SelectLevelFrom(characterClass, race);

            equipment.Treasure = treasureGenerator.GenerateAtLevel(effectiveLevel);
            equipment.Armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);

            var twoWeaponFeats = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded);
            var hasTwoWeaponFeats = feats.All.Any(f => twoWeaponFeats.Contains(f.Name));

            if (hasTwoWeaponFeats)
            {
                equipment.PrimaryHand = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
                equipment.OffHand = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            }

            equipment.PrimaryHand ??= weaponGenerator.GenerateFrom(feats, characterClass, race);

            if (equipment.PrimaryHand != null)
            {
                if (!string.IsNullOrEmpty(equipment.PrimaryHand.Ammunition))
                {
                    var ammunition = weaponGenerator.GenerateAmmunition(characterClass, race, equipment.PrimaryHand.Ammunition);
                    equipment.Treasure.Items = equipment.Treasure.Items.Union([ammunition]);
                }

                if (equipment.PrimaryHand.Attributes.Contains(AttributeConstants.TwoHanded))
                    equipment.OffHand = equipment.PrimaryHand;

                if (equipment.PrimaryHand.Attributes.Contains(AttributeConstants.Melee) == false)
                {
                    var meleeWeapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
                    if (meleeWeapon != null)
                    {
                        equipment.Treasure.Items = equipment.Treasure.Items.Union([meleeWeapon]);

                        if (meleeWeapon.Attributes.Contains(AttributeConstants.TwoHanded) == false)
                        {
                            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
                            if (shield != null)
                                equipment.Treasure.Items = equipment.Treasure.Items.Union([shield]);
                        }
                    }
                }
                else
                {
                    var rangedWeapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
                    if (rangedWeapon != null)
                    {
                        equipment.Treasure.Items = equipment.Treasure.Items.Union([rangedWeapon]);

                        if (!string.IsNullOrEmpty(rangedWeapon.Ammunition))
                        {
                            var ammunition = weaponGenerator.GenerateAmmunition(characterClass, race, rangedWeapon.Ammunition);
                            equipment.Treasure.Items = equipment.Treasure.Items.Union([ammunition]);
                        }
                    }
                }
            }

            if (equipment.OffHand == null && (equipment.PrimaryHand == null || equipment.PrimaryHand.Attributes.Contains(AttributeConstants.Melee)))
                equipment.OffHand = armorGenerator.GenerateShieldFrom(feats, characterClass, race);

            return equipment;
        }
    }
}