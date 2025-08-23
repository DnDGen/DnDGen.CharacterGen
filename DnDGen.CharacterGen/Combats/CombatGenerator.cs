using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Combats
{
    internal class CombatGenerator : ICombatGenerator
    {
        private readonly IArmorClassGenerator armorClassGenerator;
        private readonly IHitPointsGenerator hitPointsGenerator;
        private readonly ISavingThrowsGenerator savingThrowsGenerator;
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly ICollectionSelector collectionsSelector;

        public CombatGenerator(
            IArmorClassGenerator armorClassGenerator,
            IHitPointsGenerator hitPointsGenerator,
            ISavingThrowsGenerator savingThrowsGenerator,
            IAdjustmentsSelector adjustmentsSelector,
            ICollectionSelector collectionsSelector)
        {
            this.armorClassGenerator = armorClassGenerator;
            this.hitPointsGenerator = hitPointsGenerator;
            this.savingThrowsGenerator = savingThrowsGenerator;
            this.adjustmentsSelector = adjustmentsSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public BaseAttack GenerateBaseAttackWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> stats)
        {
            var baseAttack = new BaseAttack();
            baseAttack.BaseBonus = GetBaseAttackBonus(characterClass);
            baseAttack.SizeModifier = GetSizeAdjustments(race);
            baseAttack.StrengthBonus = stats[AbilityConstants.Strength].Bonus;
            baseAttack.DexterityBonus = stats[AbilityConstants.Dexterity].Bonus;
            baseAttack.RacialModifier = GetRacialBaseAttackAdjustments(race);

            return baseAttack;
        }

        private int GetBaseAttackBonus(CharacterClass characterClass)
        {
            var baseAttackQuality = collectionsSelector.FindCollectionOf(
                Config.Name,
                TableNameConstants.Set.Collection.ClassNameGroups,
                characterClass.Name,
                GroupConstants.GoodBaseAttack,
                GroupConstants.AverageBaseAttack,
                GroupConstants.PoorBaseAttack);

            switch (baseAttackQuality)
            {
                case GroupConstants.GoodBaseAttack: return GetGoodBaseAttackBonus(characterClass.Level);
                case GroupConstants.AverageBaseAttack: return GetAverageBaseAttackBonus(characterClass.Level);
                case GroupConstants.PoorBaseAttack: return GetPoorBaseAttackBonus(characterClass.Level);
                default: throw new ArgumentException($"{characterClass.Name} has no base attack");
            }
        }

        private int GetGoodBaseAttackBonus(int level) => level;
        private int GetAverageBaseAttackBonus(int level) => level * 3 / 4;
        private int GetPoorBaseAttackBonus(int level) => level / 2;

        private int GetRacialBaseAttackAdjustments(Race race)
        {
            var baseRaceAdjustment = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.RacialBaseAttackAdjustments, race.BaseRace);
            var metaraceAdjustment = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.RacialBaseAttackAdjustments, race.Metarace);

            return baseRaceAdjustment + metaraceAdjustment;
        }

        private int GetSizeAdjustments(Race race)
        {
            var sizeModifier = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.SizeModifiers, race.Size);
            return sizeModifier;
        }

        public Combat GenerateWith(BaseAttack baseAttack, CharacterClass characterClass, Race race, IEnumerable<Feat> feats, Dictionary<string, Ability> stats, Equipment equipment)
        {
            var combat = new Combat();

            combat.BaseAttack = baseAttack;
            combat.BaseAttack.BaseBonus += GetBonusFromFeats(feats);
            combat.BaseAttack.CircumstantialBonus = IsAttackBonusCircumstantial(feats);
            combat.AdjustedDexterityBonus = GetAdjustedDexterityBonus(stats[AbilityConstants.Dexterity], equipment);
            combat.ArmorClass = armorClassGenerator.GenerateWith(equipment, combat.AdjustedDexterityBonus, feats, race);

            if (stats.ContainsKey(AbilityConstants.Constitution))
                combat.HitPoints = hitPointsGenerator.GenerateWith(characterClass, stats[AbilityConstants.Constitution].Bonus, race, feats);
            else
                combat.HitPoints = hitPointsGenerator.GenerateWith(characterClass, 0, race, feats);

            combat.SavingThrows = savingThrowsGenerator.GenerateWith(characterClass, feats, stats);
            combat.InitiativeBonus = GetInitiativeBonus(combat.AdjustedDexterityBonus, feats);

            return combat;
        }

        private int GetBonusFromFeats(IEnumerable<Feat> feats)
        {
            var attackBonusFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, FeatConstants.AttackBonus);
            var attackBonusFeats = feats.Where(f => attackBonusFeatNames.Contains(f.Name) && f.Foci.Any() == false);

            if (attackBonusFeats.Any() == false)
                return 0;

            return attackBonusFeats.Sum(f => f.Power);
        }

        private bool IsAttackBonusCircumstantial(IEnumerable<Feat> feats)
        {
            var attackBonusFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, FeatConstants.AttackBonus);
            var attackBonusFeats = feats.Where(f => attackBonusFeatNames.Contains(f.Name));

            return attackBonusFeats.Any(f => f.Foci.Any());
        }

        private int GetAdjustedDexterityBonus(Ability dexterity, Equipment equipment)
        {
            var maxDexterityBonus = dexterity.Bonus;

            if (equipment.Armor != null)
                maxDexterityBonus = Math.Min(maxDexterityBonus, equipment.Armor.TotalMaxDexterityBonus);

            if (equipment.OffHand is Armor)
            {
                var shield = equipment.OffHand as Armor;
                maxDexterityBonus = Math.Min(maxDexterityBonus, shield.TotalMaxDexterityBonus);
            }

            return maxDexterityBonus;
        }

        private int GetInitiativeBonus(int dexterityBonus, IEnumerable<Feat> feats)
        {
            var initiativeFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.Initiative);
            var initiativeFeats = feats.Where(f => initiativeFeatNames.Contains(f.Name));
            var initiativeFeatBonus = initiativeFeats.Sum(f => f.Power);

            return initiativeFeatBonus + dexterityBonus;
        }
    }
}