using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Tables
{
    [TestFixture]
    public class TableNameConstantsTests
    {
        [TestCase(TableNameConstants.Set.Adjustments.AerialSpeeds, "AerialSpeeds")]
        [TestCase(TableNameConstants.Set.Adjustments.ArcaneSpellFailures, "ArcaneSpellFailures")]
        [TestCase(TableNameConstants.Set.Adjustments.ChallengeRatings, "ChallengeRatings")]
        [TestCase(TableNameConstants.Set.Adjustments.PCChallengeRatingAdjustments, "PCChallengeRatingAdjustments")]
        [TestCase(TableNameConstants.Set.Adjustments.NPCChallengeRatingAdjustments, "NPCChallengeRatingAdjustments")]
        [TestCase(TableNameConstants.Set.Adjustments.ClassHitDice, "ClassHitDice")]
        [TestCase(TableNameConstants.Set.Adjustments.CohortLevels, "CohortLevels")]
        [TestCase(TableNameConstants.Set.Adjustments.FeatArmorAdjustments, "FeatArmorAdjustments")]
        [TestCase(TableNameConstants.Set.Adjustments.FeatInitiativeBonuses, "FeatInitiativeBonuses")]
        [TestCase(TableNameConstants.Set.Adjustments.FighterFeatLevelRequirements, "FighterFeatLevelRequirements")]
        [TestCase(TableNameConstants.Set.Adjustments.LandSpeeds, "LandSpeeds")]
        [TestCase(TableNameConstants.Set.Adjustments.LeadershipModifiers, "LeadershipModifiers")]
        [TestCase(TableNameConstants.Set.Adjustments.LevelAdjustments, "LevelAdjustments")]
        [TestCase(TableNameConstants.Set.Adjustments.MonsterHitDice, "MonsterHitDice")]
        [TestCase(TableNameConstants.Set.Adjustments.ProhibitedFieldQuantities, "ProhibitedFieldQuantities")]
        [TestCase(TableNameConstants.Set.Adjustments.RacialBaseAttackAdjustments, "RacialBaseAttackAdjustments")]
        [TestCase(TableNameConstants.Set.Adjustments.SizeModifiers, "SizeModifiers")]
        [TestCase(TableNameConstants.Set.Adjustments.SkillPoints, "SkillPoints")]
        [TestCase(TableNameConstants.Set.Adjustments.SpecialistFieldQuantities, "SpecialistFieldQuantities")]
        [TestCase(TableNameConstants.Set.Adjustments.SwimSpeeds, "SwimSpeeds")]
        [TestCase(TableNameConstants.Set.Collection.AbilityGroups, "AbilityGroups")]
        [TestCase(TableNameConstants.Set.Collection.AbilityPriorities, "AbilityPriorities")]
        [TestCase(TableNameConstants.Set.Collection.AdditionalFeatData, "AdditionalFeatData")]
        [TestCase(TableNameConstants.Set.Collection.AerialManeuverability, "AerialManeuverability")]
        [TestCase(TableNameConstants.Set.Collection.AlignmentGroups, "AlignmentGroups")]
        [TestCase(TableNameConstants.Set.Collection.AnimalGroups, "AnimalGroups")]
        [TestCase(TableNameConstants.Set.Collection.ArmorClassModifiers, "ArmorClassModifiers")]
        [TestCase(TableNameConstants.Set.Collection.AutomaticLanguages, "AutomaticLanguages")]
        [TestCase(TableNameConstants.Set.Collection.BaseRaceGroups, "BaseRaceGroups")]
        [TestCase(TableNameConstants.Set.Collection.BonusLanguages, "BonusLanguages")]
        [TestCase(TableNameConstants.Set.Collection.ClassNameGroups, "ClassNameGroups")]
        [TestCase(TableNameConstants.Set.Collection.ClassSkills, "ClassSkills")]
        [TestCase(TableNameConstants.Set.Collection.DragonSpecies, "DragonSpecies")]
        [TestCase(TableNameConstants.Set.Collection.EquivalentFeats, "EquivalentFeats")]
        [TestCase(TableNameConstants.Set.Collection.FeatGroups, "FeatGroups")]
        [TestCase(TableNameConstants.Set.Collection.FeatFoci, "FeatFoci")]
        [TestCase(TableNameConstants.Set.Collection.HeightRolls, "HeightRolls")]
        [TestCase(TableNameConstants.Set.Collection.ItemGroups, "ItemGroups")]
        [TestCase(TableNameConstants.Set.Collection.MaximumAgeRolls, "MaximumAgeRolls")]
        [TestCase(TableNameConstants.Set.Collection.MetaraceGroups, "MetaraceGroups")]
        [TestCase(TableNameConstants.Set.Collection.Names, "Names")]
        [TestCase(TableNameConstants.Set.Collection.ProhibitedFields, "ProhibitedFields")]
        [TestCase(TableNameConstants.Set.Collection.RacialFeatHitDieRequirements, "RacialFeatHitDieRequirements")]
        [TestCase(TableNameConstants.Set.Collection.RequiredFeats, "RequiredFeats")]
        [TestCase(TableNameConstants.Set.Collection.SkillData, "SkillData")]
        [TestCase(TableNameConstants.Set.Collection.SkillGroups, "SkillGroups")]
        [TestCase(TableNameConstants.Set.Collection.SkillSynergy, "SkillSynergy")]
        [TestCase(TableNameConstants.Set.Collection.SpecialistFields, "SpecialistFields")]
        [TestCase(TableNameConstants.Set.Collection.WeightRolls, "WeightRolls")]
        [TestCase(TableNameConstants.Set.Percentile.AlignmentGoodness, "AlignmentGoodness")]
        [TestCase(TableNameConstants.Set.Percentile.AlignmentLawfulness, "AlignmentLawfulness")]
        [TestCase(TableNameConstants.Set.Percentile.LeadershipMovement, "LeadershipMovement")]
        [TestCase(TableNameConstants.Set.Percentile.Reputation, "Reputation")]
        [TestCase(TableNameConstants.Set.Percentile.Traits, "Traits")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.AssignPointToCrossClassSkill, "AssignPointToCrossClassSkill")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.AttractCohortOfDifferentAlignment, "AttractCohortOfDifferentAlignment")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.IncreaseFirstPriorityAbility, "IncreaseFirstPriorityAbility")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.KilledCohort, "KilledCohort")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.KilledFollowers, "KilledFollowers")]
        [TestCase(TableNameConstants.Set.TrueOrFalse.Male, "Male")]
        [TestCase(TableNameConstants.Formattable.Adjustments.CLASSFeatLevelRequirements, "{0}FeatLevelRequirements")]
        [TestCase(TableNameConstants.Formattable.Adjustments.FEATClassRequirements, "{0}ClassRequirements")]
        [TestCase(TableNameConstants.Formattable.Adjustments.FEATSkillRankRequirements, "{0}SkillRankRequirements")]
        [TestCase(TableNameConstants.Formattable.Adjustments.FEATAbilityRequirements, "{0}AbilityRequirements")]
        [TestCase(TableNameConstants.Formattable.Adjustments.GENDERHeights, "{0}Heights")]
        [TestCase(TableNameConstants.Formattable.Adjustments.GENDERWeights, "{0}Weights")]
        [TestCase(TableNameConstants.Formattable.Adjustments.LevelXAnimalTricks, "Level{0}AnimalTricks")]
        [TestCase(TableNameConstants.Formattable.Adjustments.LevelXCLASSSpellsPerDay, "Level{0}{1}SpellsPerDay")]
        [TestCase(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, "Level{0}{1}KnownSpells")]
        [TestCase(TableNameConstants.Formattable.Adjustments.LevelXFollowerQuantities, "Level{0}FollowerQuantities")]
        [TestCase(TableNameConstants.Formattable.Adjustments.RACEAges, "{0}Ages")]
        [TestCase(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, "{0}AgeRolls")]
        [TestCase(TableNameConstants.Formattable.Collection.CLASSSpellLevels, "{0}SpellLevels")]
        [TestCase(TableNameConstants.Formattable.Collection.CLASSFeatData, "{0}FeatData")]
        [TestCase(TableNameConstants.Formattable.Collection.RACEFeatData, "{0}FeatData")]
        [TestCase(TableNameConstants.Formattable.Percentile.GOODNESSCharacterClasses, "{0}CharacterClasses")]
        [TestCase(TableNameConstants.Formattable.Percentile.GOODNESSCLASSBaseRaces, "{0}{1}BaseRaces")]
        [TestCase(TableNameConstants.Formattable.Percentile.GOODNESSCLASSMetaraces, "{0}{1}Metaraces")]
        [TestCase(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, "{0}HasSpecialistFields")]
        [TestCase(TableNameConstants.Formattable.TrueOrFalse.CLASSKnowsAdditionalSpells, "{0}KnowsAdditionalSpells")]
        public void TableNameConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void TableNameConstant_LevelXPower()
        {
            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(9266);
            Assert.That(tableName, Is.EqualTo("Level9266Power"));
        }

        [Test]
        public void RaceFeatDataTable()
        {
            Assert.That(TableNameConstants.Formattable.Collection.RACEFeatData, Is.EqualTo("{0}FeatData"));
        }

        [Test]
        public void ClassFeatDataTable()
        {
            Assert.That(TableNameConstants.Formattable.Collection.CLASSFeatData, Is.EqualTo("{0}FeatData"));
        }

        [Test]
        public void AgeAbilityAdjustmentsTable()
        {
            Assert.That(TableNameConstants.Formattable.Adjustments.AGEAbilityAdjustments, Is.EqualTo("{0}AbilityAdjustments"));
        }

        [Test]
        public void RaceAbilityAdjustmentsTable()
        {
            Assert.That(TableNameConstants.Formattable.Adjustments.RACEAbilityAdjustments, Is.EqualTo("{0}AbilityAdjustments"));
        }
    }
}