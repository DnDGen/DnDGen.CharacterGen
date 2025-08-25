using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Combats
{
    internal class HitPointsGenerator : IHitPointsGenerator
    {
        private readonly Dice dice;
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly ICollectionSelector collectionsSelector;

        public HitPointsGenerator(Dice dice, IAdjustmentsSelector adjustmentsSelector, ICollectionSelector collectionsSelector)
        {
            this.dice = dice;
            this.adjustmentsSelector = adjustmentsSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public int GenerateWith(CharacterClass characterClass, int constitutionBonus, Race race, IEnumerable<Feat> feats)
        {
            var hitPoints = GetClassHitPoints(characterClass, constitutionBonus, race);

            var toughness = feats.Where(f => f.Name == FeatConstants.Toughness);
            hitPoints += toughness.Sum(f => f.Power);

            var monsters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters);
            if (monsters.Contains(race.BaseRace) == false)
                return hitPoints;

            var monsterHitPoints = GetAdditionalMonsterHitDice(race, constitutionBonus);
            return hitPoints + monsterHitPoints;
        }

        private int GetClassHitPoints(CharacterClass characterClass, int constitutionBonus, Race race)
        {
            var undead = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead);
            if (undead.Contains(race.Metarace))
                return RollHitPoints(characterClass.Level, 12, constitutionBonus);

            var hitDice = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.ClassHitDice, characterClass.Name);
            return RollHitPoints(characterClass.Level, hitDice, constitutionBonus);
        }

        private int GetAdditionalMonsterHitDice(Race race, int constitutionBonus)
        {
            var hitDice = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace);
            if (hitDice == 0)
                return 0;

            if (race.Metarace == RaceConstants.Metaraces.HalfDragon)
                return RollHitPoints(hitDice, 10, constitutionBonus);

            var undead = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead);
            if (race.BaseRace == RaceConstants.BaseRaces.Mummy || undead.Contains(race.Metarace))
                return RollHitPoints(hitDice, 12, constitutionBonus);

            return RollHitPoints(hitDice, 8, constitutionBonus);
        }

        private int RollHitPoints(int quantity, int die, int constitutionBonus)
        {
            if (constitutionBonus >= 0)
                return dice.Roll(quantity).d(die).Plus(quantity * constitutionBonus).AsSum();

            var rolls = dice.Roll(quantity).d(die).AsIndividualRolls();
            rolls = rolls.Select(r => Math.Max(r + constitutionBonus, 1));

            return rolls.Sum();
        }
    }
}