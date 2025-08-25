using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Races.Randomizers.BaseRaces;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using DnDGen.CharacterGen.Verifiers;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace DnDGen.CharacterGen.Tests.Integration.Generators.Verifiers
{
    [TestFixture]
    public class RandomizerVerifierTests : IntegrationTests
    {
        private IRandomizerVerifier randomizerVerifier;
        private Stopwatch stopwatch;
        private TimeSpan timeLimit;

        [SetUp]
        public void Setup()
        {
            randomizerVerifier = GetNewInstanceOf<IRandomizerVerifier>();
            stopwatch = new Stopwatch();
            timeLimit = TimeSpan.FromSeconds(1);
        }

        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AquaticBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,True", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,True", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.UndeadMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.UndeadMeta + ",,True", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, "Set," + RaceConstants.Metaraces.Wereboar_Dire, true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, "Set," + RaceConstants.Metaraces.Werewolf_Dire, true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AquaticBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Aasimar, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Rakshasa, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Tiefling, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.HillGiant, "Set," + RaceConstants.Metaraces.Wereboar_Dire, true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Human, "Set," + RaceConstants.Metaraces.Werewolf_Dire, true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Aasimar, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Rakshasa, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.Tiefling, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, "Set," + CharacterClassConstants.Adept, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, "Set," + CharacterClassConstants.Cleric, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Any, "Set," + CharacterClassConstants.Sorcerer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Chaotic, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Evil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.Good, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Lawful, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.Neutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonChaotic, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.NonEvil, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Medium, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentRandomizerTypeConstants.NonGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", true)]
        [TestCase(AlignmentRandomizerTypeConstants.NonGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Medium, RaceRandomizerTypeConstants.BaseRace.MonsterBase, "Set," + RaceConstants.Metaraces.Werebear, false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonLawful, ClassNameRandomizerTypeConstants.AnyNPC, LevelRandomizerTypeConstants.Medium, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonLawful, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentRandomizerTypeConstants.NonNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase("Set," + AlignmentConstants.ChaoticEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.ChaoticGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.ChaoticNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.LawfulEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.LawfulGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase("Set," + AlignmentConstants.LawfulNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.NeutralEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.NeutralGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase("Set," + AlignmentConstants.TrueNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        public void RandomizerVerificationIsFast(string alignmentRandomizerName, string classNameRandomizerName, string levelRandomizerName, string baseRaceRandomizerName, string metaraceRandomizerName, bool isValid)
        {
            var alignmentRandomizer = GetAlignmentRandomizer(alignmentRandomizerName);
            var classNameRandomizer = GetClassNameRandomizer(classNameRandomizerName);
            var levelRandomizer = GetLevelRandomizer(levelRandomizerName);
            var baseRaceRandomizer = GetBaseRaceRandomizer(baseRaceRandomizerName);
            var metaraceRandomizer = GetMetaraceRandomizer(metaraceRandomizerName);

            stopwatch.Restart();
            var verified = randomizerVerifier.VerifyCompatibility(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);
            stopwatch.Stop();

            Assert.That(verified, Is.EqualTo(isValid));
            Assert.That(stopwatch.Elapsed, Is.LessThan(timeLimit));
        }

        [TestCase(AlignmentConstants.ChaoticEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", true)]
        [TestCase(AlignmentConstants.ChaoticEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.ChaoticGood, ClassNameRandomizerTypeConstants.AnyPlayer, "Set,1", RaceRandomizerTypeConstants.BaseRace.NonStandardBase, RaceRandomizerTypeConstants.Metarace.UndeadMeta + ",,True", true)]
        [TestCase(AlignmentConstants.ChaoticGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.ChaoticNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", true)]
        [TestCase(AlignmentConstants.LawfulEvil, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Low, RaceRandomizerTypeConstants.BaseRace.AquaticBase, RaceRandomizerTypeConstants.Metarace.NoMeta, true)]
        [TestCase(AlignmentConstants.LawfulEvil, ClassNameRandomizerTypeConstants.Stealth, LevelRandomizerTypeConstants.Medium, "Set," + RaceConstants.BaseRaces.HighElf, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", true)]
        [TestCase(AlignmentConstants.LawfulGood, ClassNameRandomizerTypeConstants.Spellcaster, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AquaticBase, "Set," + RaceConstants.Metaraces.HalfCelestial, false)]
        [TestCase(AlignmentConstants.LawfulGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.LawfulNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralEvil, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.NeutralEvil, "Set," + CharacterClassConstants.Druid, LevelRandomizerTypeConstants.High, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.NoMeta, true)]
        [TestCase(AlignmentConstants.NeutralEvil, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.NeutralGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Low, RaceRandomizerTypeConstants.BaseRace.NonStandardBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.NeutralGood, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Medium, "Set," + RaceConstants.BaseRaces.Bugbear, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentConstants.NeutralGood, ClassNameRandomizerTypeConstants.ArcaneSpellcaster, LevelRandomizerTypeConstants.Low, RaceRandomizerTypeConstants.BaseRace.AquaticBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,True", true)]
        [TestCase(AlignmentConstants.NeutralGood, "Set," + CharacterClassConstants.Adept, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, "Set," + CharacterClassConstants.Cleric, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralGood, "Set," + CharacterClassConstants.Sorcerer, LevelRandomizerTypeConstants.Any, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, ClassNameRandomizerTypeConstants.AnyPlayer, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", true)]
        [TestCase(AlignmentConstants.TrueNeutral, "Set," + CharacterClassConstants.Paladin, LevelRandomizerTypeConstants.Any, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        public void AlignmentVerificationIsFast(string alignmentDescription, string classNameRandomizerName, string levelRandomizerName, string baseRaceRandomizerName, string metaraceRandomizerName, bool isValid)
        {
            var alignment = new Alignment(alignmentDescription);
            var classNameRandomizer = GetClassNameRandomizer(classNameRandomizerName);
            var levelRandomizer = GetLevelRandomizer(levelRandomizerName);
            var baseRaceRandomizer = GetBaseRaceRandomizer(baseRaceRandomizerName);
            var metaraceRandomizer = GetMetaraceRandomizer(metaraceRandomizerName);

            stopwatch.Restart();
            var verified = randomizerVerifier.VerifyAlignmentCompatibility(alignment, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);
            stopwatch.Stop();

            Assert.That(verified, Is.EqualTo(isValid));
            Assert.That(stopwatch.Elapsed, Is.LessThan(timeLimit));
        }

        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Cleric, 13, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, "Set," + RaceConstants.Metaraces.Ghost, true)]
        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Fighter, 3, false, RaceRandomizerTypeConstants.BaseRace.NonStandardBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Expert, 3, true, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,False", true)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Expert, 3, true, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.LycanthropeMeta + ",,True", false)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Fighter, 11, false, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", true)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Fighter, 11, false, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,False", true)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Fighter, 11, false, RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralEvil, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralEvil, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Adept, 1, true, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Cleric, 6, false, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Rogue, 6, false, "Set," + RaceConstants.BaseRaces.Bugbear, RaceRandomizerTypeConstants.Metarace.GeneticMeta + ",,True", false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Sorcerer, 6, false, "Set," + RaceConstants.BaseRaces.StormGiant, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Druid, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Paladin, 1, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", false)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Ranger, 2, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Ranger, 2, false, RaceRandomizerTypeConstants.BaseRace.MonsterBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,True", true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 9, false, RaceRandomizerTypeConstants.BaseRace.AnyBase, RaceRandomizerTypeConstants.Metarace.AnyMeta + ",,False", true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 9, false, RaceRandomizerTypeConstants.BaseRace.AquaticBase, RaceRandomizerTypeConstants.Metarace.UndeadMeta + ",,True", true)]
        public void ClassVerificationIsFast(string alignmentDescription, string className, int level, bool isNPC, string baseRaceRandomizerName, string metaraceRandomizerName, bool isValid)
        {
            var alignment = new Alignment(alignmentDescription);
            var classPrototype = new CharacterClassPrototype();
            classPrototype.Name = className;
            classPrototype.Level = level;
            classPrototype.IsNPC = isNPC;

            var baseRaceRandomizer = GetBaseRaceRandomizer(baseRaceRandomizerName);
            var metaraceRandomizer = GetMetaraceRandomizer(metaraceRandomizerName);

            stopwatch.Restart();
            var verified = randomizerVerifier.VerifyCharacterClassCompatibility(alignment, classPrototype, baseRaceRandomizer, metaraceRandomizer);
            stopwatch.Stop();

            Assert.That(verified, Is.EqualTo(isValid));
            Assert.That(stopwatch.Elapsed, Is.LessThan(timeLimit));
        }

        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Cleric, 13, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Ghost, true)]
        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.ChaoticEvil, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Fighter, 3, false, RaceConstants.BaseRaces.WildElf, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.ChaoticGood, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Expert, 3, true, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.ChaoticNeutral, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Barbarian, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Barbarian, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Fighter, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Fighter, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Monk, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Monk, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Ranger, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Ranger, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Ranger, 2, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Rogue, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Rogue, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Sorcerer, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Sorcerer, 1, false, RaceConstants.BaseRaces.Rakshasa, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 9, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 9, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 15, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 15, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 15, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 16, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 16, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 16, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 17, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 17, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 17, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 18, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 18, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 18, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 19, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 19, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 19, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 20, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 20, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 20, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        [TestCase(AlignmentConstants.LawfulEvil, CharacterClassConstants.Wizard, 15, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Vampire, true)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Fighter, 11, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.HalfCelestial, true)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulGood, CharacterClassConstants.Wizard, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Fighter, 11, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.HalfDragon, false)]
        [TestCase(AlignmentConstants.LawfulNeutral, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.NeutralEvil, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.NeutralEvil, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Adept, 1, true, RaceConstants.BaseRaces.StormGiant, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Cleric, 1, false, RaceConstants.BaseRaces.StormGiant, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Rogue, 6, false, RaceConstants.BaseRaces.Bugbear, RaceConstants.Metaraces.HalfCelestial, false)]
        [TestCase(AlignmentConstants.NeutralGood, CharacterClassConstants.Sorcerer, 1, false, RaceConstants.BaseRaces.StormGiant, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Druid, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Paladin, 1, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Ranger, 2, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Ranger, 2, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.Ghost, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 1, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.None, false)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 9, false, RaceConstants.BaseRaces.Human, RaceConstants.Metaraces.None, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 9, false, RaceConstants.BaseRaces.Merfolk, RaceConstants.Metaraces.Ghost, true)]
        [TestCase(AlignmentConstants.TrueNeutral, CharacterClassConstants.Wizard, 15, false, RaceConstants.BaseRaces.MindFlayer, RaceConstants.Metaraces.Vampire, false)]
        public void RaceVerificationIsFast(string alignmentDescription, string className, int level, bool isNPC, string baseRace, string metarace, bool isValid)
        {
            var alignment = new Alignment(alignmentDescription);

            var classPrototype = new CharacterClassPrototype();
            classPrototype.Name = className;
            classPrototype.Level = level;
            classPrototype.IsNPC = isNPC;

            var racePrototype = new RacePrototype();
            racePrototype.BaseRace = baseRace;
            racePrototype.Metarace = metarace;

            stopwatch.Restart();
            var verified = randomizerVerifier.VerifyRaceCompatibility(alignment, classPrototype, racePrototype);
            stopwatch.Stop();

            Assert.That(verified, Is.EqualTo(isValid));
            Assert.That(stopwatch.Elapsed, Is.LessThan(timeLimit));
        }

        private RaceRandomizer GetMetaraceRandomizer(string name)
        {
            var sections = name.Split(',');

            if (sections.Length == 1)
                return GetNewInstanceOf<RaceRandomizer>(name);

            if (sections.Length == 3)
            {
                var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(sections[0]);
                var forceableMetaraceRandomizer = metaraceRandomizer as IForcableMetaraceRandomizer;
                forceableMetaraceRandomizer.ForceMetarace = Convert.ToBoolean(sections[2]);

                return forceableMetaraceRandomizer;
            }

            var setMetaraceRandomizer = GetNewInstanceOf<ISetMetaraceRandomizer>();
            setMetaraceRandomizer.SetMetarace = sections[1];

            return setMetaraceRandomizer;
        }

        private RaceRandomizer GetBaseRaceRandomizer(string name)
        {
            var sections = name.Split(',');

            if (sections.Length == 1)
                return GetNewInstanceOf<RaceRandomizer>(name);

            var setBaseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            setBaseRaceRandomizer.SetBaseRace = sections[1];

            return setBaseRaceRandomizer;
        }

        private ILevelRandomizer GetLevelRandomizer(string name)
        {
            var sections = name.Split(',');

            if (sections.Length == 1)
                return GetNewInstanceOf<ILevelRandomizer>(name);

            var setLevelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            setLevelRandomizer.SetLevel = Convert.ToInt32(sections[1]);

            return setLevelRandomizer;
        }

        private IClassNameRandomizer GetClassNameRandomizer(string name)
        {
            var sections = name.Split(',');

            if (sections.Length == 1)
                return GetNewInstanceOf<IClassNameRandomizer>(name);

            var setClassNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            setClassNameRandomizer.SetClassName = sections[1];

            return setClassNameRandomizer;
        }

        private IAlignmentRandomizer GetAlignmentRandomizer(string name)
        {
            var sections = name.Split(',');

            if (sections.Length == 1)
                return GetNewInstanceOf<IAlignmentRandomizer>(name);

            var setAlignmentRandomizer = GetNewInstanceOf<ISetAlignmentRandomizer>();
            setAlignmentRandomizer.SetAlignment = new Alignment(sections[1]);

            return setAlignmentRandomizer;
        }
    }
}
