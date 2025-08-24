using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Selectors.Selections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Skills
{
    internal class SkillsGenerator : ISkillsGenerator
    {
        private readonly ISkillSelector skillSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly IPercentileSelector percentileSelector;

        public SkillsGenerator(
            ISkillSelector skillSelector,
            ICollectionSelector collectionsSelector,
            IAdjustmentsSelector adjustmentsSelector,
            IPercentileSelector percentileSelector)
        {
            this.skillSelector = skillSelector;
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
            this.percentileSelector = percentileSelector;
        }

        public IEnumerable<Skill> GenerateWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities)
        {
            //INFO: Calling ToList so we can add specialist skills later
            var classSkillNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassSkills, characterClass.Name).ToList();
            var crossClassSkillNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillGroups, GroupConstants.Untrained);

            foreach (var specialistField in characterClass.SpecialistFields)
            {
                var specialistSkills = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassSkills, specialistField);
                classSkillNames.AddRange(specialistSkills);
            }

            var classSkillSelections = GetSkillSelections(classSkillNames);
            var crossClassSkillSelections = GetSkillSelections(crossClassSkillNames);

            if (characterClass.Name == CharacterClassConstants.Expert)
                classSkillSelections = GetRandomClassSkillSelections();

            classSkillSelections = AddProfessionSkills(classSkillSelections);

            var skills = InitializeSkills(abilities, classSkillSelections, crossClassSkillSelections, characterClass);

            skills = AddRanks(characterClass, race, abilities, classSkillSelections, crossClassSkillSelections, skills);

            var monsters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters);
            if (monsters.Contains(race.BaseRace))
                skills = AddMonsterSkillRanks(race, abilities, skills);

            skills = ApplySkillSynergies(skills);

            return skills;
        }

        private IEnumerable<SkillSelection> AddProfessionSkills(IEnumerable<SkillSelection> classSkillSelections)
        {
            var professionSkill = classSkillSelections.FirstOrDefault(s => s.SkillName == SkillConstants.Profession);

            if (professionSkill == null)
                return classSkillSelections;

            var profession = professionSkill.Focus;
            var professionSkillNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassSkills, profession);
            var professionSkillSelections = GetSkillSelections(professionSkillNames);

            return classSkillSelections.Union(professionSkillSelections);
        }

        private IEnumerable<SkillSelection> GetRandomClassSkillSelections()
        {
            var allSkills = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillGroups, GroupConstants.All);
            var randomSkillSelections = new List<SkillSelection>();

            while (randomSkillSelections.Count < 10)
            {
                var skill = collectionsSelector.SelectRandomFrom(allSkills);
                var skillSelection = skillSelector.SelectFor(skill);
                var explodedSelections = ExplodeSelectedSkill(skillSelection);
                var newSelections = explodedSelections.Where(ss => !randomSkillSelections.Any(s => s.IsEqualTo(ss)));

                randomSkillSelections.AddRange(newSelections);
            }

            return randomSkillSelections;
        }

        private IEnumerable<Skill> AddMonsterSkillRanks(Race race, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills)
        {
            var monsterHitDice = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace);

            if (monsterHitDice == 0)
                return skills;

            var skillsList = skills.ToList();
            var monsterSkills = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassSkills, race.BaseRace);
            var monsterSkillSelections = GetSkillSelections(monsterSkills);

            foreach (var monsterSkillSelection in monsterSkillSelections)
            {
                if (!skillsList.Any(monsterSkillSelection.IsEqualTo))
                {
                    if (!abilities.ContainsKey(monsterSkillSelection.BaseStatName))
                        continue;

                    var newSkill = new Skill(monsterSkillSelection.SkillName, abilities[monsterSkillSelection.BaseStatName], 3, monsterSkillSelection.Focus);
                    skillsList.Add(newSkill);
                }

                var monsterSkill = skillsList.First(monsterSkillSelection.IsEqualTo);

                monsterSkill.RankCap += monsterHitDice;
                monsterSkill.ClassSkill = true;
            }

            var points = GetTotalSkillPoints(race.BaseRace, monsterHitDice, abilities[AbilityConstants.Intelligence], race);
            var validMonsterSkills = FilterOutInvalidSkills(monsterSkillSelections, skillsList);

            while (points-- > 0 && validMonsterSkills.Any())
            {
                var skillSelection = collectionsSelector.SelectRandomFrom(validMonsterSkills);
                var skill = skillsList.First(skillSelection.IsEqualTo);
                skill.Ranks++;

                validMonsterSkills = FilterOutInvalidSkills(monsterSkillSelections, skillsList);
            }

            return skillsList;
        }

        private IEnumerable<SkillSelection> GetSkillSelections(IEnumerable<string> skillNames)
        {
            var selections = skillNames.Select(skillSelector.SelectFor);
            var explodedSelections = selections.SelectMany(ExplodeSelectedSkill);

            //INFO: Calling immediate execution, since exploding includes potentially random results, and after this method is complete, we want consistent results.
            return explodedSelections.ToList();
        }

        private IEnumerable<SkillSelection> ExplodeSelectedSkill(SkillSelection skillSelection)
        {
            if (skillSelection.RandomFociQuantity == 0)
                return [skillSelection];

            var skillFoci = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillGroups, skillSelection.SkillName).ToList();

            if (skillSelection.RandomFociQuantity >= skillFoci.Count)
            {
                return skillFoci.Select(f => new SkillSelection { BaseStatName = skillSelection.BaseStatName, SkillName = skillSelection.SkillName, Focus = f });
            }

            var selections = new List<SkillSelection>();

            while (skillSelection.RandomFociQuantity > selections.Count)
            {
                var focus = collectionsSelector.SelectRandomFrom(skillFoci);
                var selection = new SkillSelection
                {
                    BaseStatName = skillSelection.BaseStatName,
                    SkillName = skillSelection.SkillName,
                    Focus = focus
                };

                selections.Add(selection);
                skillFoci.Remove(focus);
            }

            return selections;
        }

        private IEnumerable<Skill> InitializeSkills(
            Dictionary<string, Ability> stats,
            IEnumerable<SkillSelection> classSkillSelections,
            IEnumerable<SkillSelection> crossClassSkillSelections,
            CharacterClass characterClass)
        {
            var skills = new List<Skill>();
            var allSkillSelections = classSkillSelections.Union(crossClassSkillSelections);

            foreach (var skillSelection in allSkillSelections)
            {
                if (stats.ContainsKey(skillSelection.BaseStatName) == false)
                    continue;

                var skill = skills.FirstOrDefault(s => skillSelection.IsEqualTo(s));

                if (skill == null)
                {
                    skill = new Skill(skillSelection.SkillName, stats[skillSelection.BaseStatName], characterClass.Level + 3, skillSelection.Focus);

                    var skillsWithArmorCheckPenalties = collectionsSelector.SelectFrom(
                        Config.Name,
                        TableNameConstants.Set.Collection.SkillGroups,
                        GroupConstants.ArmorCheckPenalty);
                    skill.HasArmorCheckPenalty = skillsWithArmorCheckPenalties.Contains(skill.Name);
                    skills.Add(skill);
                }

                skill.ClassSkill = classSkillSelections.Any(s => s.IsEqualTo(skill));
            }

            return skills;
        }

        private IEnumerable<Skill> AddRanks(
            CharacterClass characterClass,
            Race race,
            Dictionary<string, Ability> stats,
            IEnumerable<SkillSelection> classSkillSelections,
            IEnumerable<SkillSelection> crossClassSkillSelections,
            IEnumerable<Skill> skills)
        {
            var points = GetTotalSkillPoints(characterClass.Name, characterClass.Level, stats[AbilityConstants.Intelligence], race);
            var allSkillSelections = classSkillSelections.Union(crossClassSkillSelections);
            var validSkills = FilterOutInvalidSkills(allSkillSelections, skills);

            while (points > 0 && validSkills.Any())
            {
                var skillCollection = GetRandomSkillCollection(skills, classSkillSelections, crossClassSkillSelections);
                var skillSelection = collectionsSelector.SelectRandomFrom(skillCollection);
                var skill = skills.First(skillSelection.IsEqualTo);

                skill.Ranks++;
                points--;

                validSkills = FilterOutInvalidSkills(allSkillSelections, skills);
            }

            return skills;
        }

        private int GetTotalSkillPoints(string source, int levels, Ability intelligence, Race race)
        {
            var points = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.SkillPoints, source);
            var perLevel = points + intelligence.Bonus;
            var multiplier = levels + 3;

            if (race.BaseRace == RaceConstants.BaseRaces.Human)
                perLevel++;

            return Math.Max(perLevel * multiplier, levels);
        }

        private IEnumerable<SkillSelection> GetRandomSkillCollection(
            IEnumerable<Skill> skills,
            IEnumerable<SkillSelection> classSkillSelections,
            IEnumerable<SkillSelection> crossClassSkillSelections)
        {
            var validClassSkills = FilterOutInvalidSkills(classSkillSelections, skills);
            var validCrossClassSkills = FilterOutInvalidSkills(crossClassSkillSelections, skills);

            if (!validClassSkills.Any())
                return validCrossClassSkills;

            if (!validCrossClassSkills.Any())
                return validClassSkills;

            var shouldAddPointToCrossClassSkill = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.AssignPointToCrossClassSkill);
            if (shouldAddPointToCrossClassSkill)
                return validCrossClassSkills;

            return validClassSkills;
        }

        private IEnumerable<SkillSelection> FilterOutInvalidSkills(IEnumerable<SkillSelection> skillSelections, IEnumerable<Skill> skills)
        {
            return skillSelections.Where(ss => skills.Any(s => ss.IsEqualTo(s) && s.RanksMaxedOut == false));
        }

        private IEnumerable<Skill> ApplySkillSynergies(IEnumerable<Skill> skills)
        {
            var skillsWarrantingSynergy = skills.Where(s => s.QualifiesForSkillSynergy);

            foreach (var skill in skillsWarrantingSynergy)
            {
                var name = skill.Focus.Any() ? $"{skill.Name}/{skill.Focus}" : skill.Name;
                var synergySkillNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillSynergy, name);

                foreach (var synergySkillName in synergySkillNames)
                {
                    var synergySkill = skills.FirstOrDefault(s => s.IsEqualTo(synergySkillName));

                    if (synergySkill != null)
                        synergySkill.Bonus += 2;
                }
            }

            return skills;
        }

        public IEnumerable<Skill> UpdateSkillsFromFeats(IEnumerable<Skill> skills, IEnumerable<Feat> feats)
        {
            var allFeatGrantingSkillBonuses = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, FeatConstants.SkillBonus);
            var featGrantingSkillBonuses = feats.Where(f => allFeatGrantingSkillBonuses.Contains(f.Name));
            var allSkillFocusNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci, GroupConstants.Skills);

            foreach (var feat in featGrantingSkillBonuses)
            {
                if (feat.Foci.Any())
                {
                    foreach (var focus in feat.Foci)
                    {
                        if (!allSkillFocusNames.Any(focus.StartsWith))
                            continue;

                        var skillName = allSkillFocusNames.First(focus.StartsWith);
                        var skill = skills.FirstOrDefault(s => s.IsEqualTo(skillName));

                        if (skill == null)
                            continue;

                        var circumstantial = !allSkillFocusNames.Contains(focus);
                        skill.CircumstantialBonus |= circumstantial;

                        if (!circumstantial)
                            skill.Bonus += feat.Power;
                    }
                }
                else
                {
                    var skillsToReceiveBonus = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillGroups, feat.Name);

                    foreach (var skillName in skillsToReceiveBonus)
                    {
                        var skill = skills.FirstOrDefault(s => s.IsEqualTo(skillName));

                        if (skill != null)
                            skill.Bonus += feat.Power;
                    }
                }
            }

            return skills;
        }

        public IEnumerable<Skill> UpdateSkillsFromEquipment(IEnumerable<Skill> skills, Equipment equipment)
        {
            var armorCheckPenaltySkills = skills.Where(s => s.HasArmorCheckPenalty);

            foreach (var skill in armorCheckPenaltySkills)
            {
                skill.ArmorCheckPenalty += ComputeArmorCheckPenalty(equipment.Armor, skill.Name);

                if (equipment.OffHand != null && equipment.OffHand.Attributes.Contains(AttributeConstants.Shield))
                {
                    skill.ArmorCheckPenalty += ComputeArmorCheckPenalty(equipment.OffHand, skill.Name);
                }
            }

            return skills;
        }

        private int ComputeArmorCheckPenalty(Item item, string skillName)
        {
            if (item == null || item is not Armor)
                return 0;

            var armor = item as Armor;

            if (skillName == SkillConstants.Swim && armor.Name == ArmorConstants.PlateArmorOfTheDeep)
                return 0;

            if (skillName == SkillConstants.Swim)
                return armor.TotalArmorCheckPenalty * 2;

            return armor.TotalArmorCheckPenalty;
        }
    }
}