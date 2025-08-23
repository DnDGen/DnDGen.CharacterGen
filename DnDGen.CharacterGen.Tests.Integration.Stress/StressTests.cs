using DnDGen.CharacterGen.Characters;
using DnDGen.CharacterGen.Generators.Characters;
using DnDGen.CharacterGen.Randomizers.Abilities;
using DnDGen.CharacterGen.Randomizers.Alignments;
using DnDGen.CharacterGen.Randomizers.CharacterClasses;
using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Verifiers;
using DnDGen.Stress;
using NUnit.Framework;
using System.Reflection;

namespace DnDGen.CharacterGen.Tests.Integration.Stress
{
    [TestFixture]
    [Stress]
    public abstract class StressTests : IntegrationTests
    {
        protected IRandomizerVerifier randomizerVerifier;
        protected IAlignmentRandomizer alignmentRandomizer;
        protected IClassNameRandomizer classNameRandomizer;
        protected ILevelRandomizer levelRandomizer;
        protected RaceRandomizer baseRaceRandomizer;
        protected RaceRandomizer metaraceRandomizer;
        protected IAbilitiesRandomizer abilitiesRandomizer;
        protected ICharacterGenerator characterGenerator;
        protected Stressor stressor;

        [OneTimeSetUp]
        public void StressOneTimeSetup()
        {
            var options = new StressorOptions();
            options.RunningAssembly = Assembly.GetExecutingAssembly();
            options.TimeLimitPercentage = .90;

#if STRESS
            options.IsFullStress = true;
#else
            options.IsFullStress = false;
#endif

            stressor = new Stressor(options);
        }

        [SetUp]
        public void StressSetup()
        {
            characterGenerator = GetNewInstanceOf<ICharacterGenerator>();
            metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta);
            baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AnyBase);
            levelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Any);
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyPlayer);
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.Any);
            abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.BestOfFour);
            randomizerVerifier = GetNewInstanceOf<IRandomizerVerifier>();
        }

        protected CharacterPrototype GetCharacterPrototype()
        {
            var prototype = characterGenerator.GeneratePrototypeWith(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);
            return prototype;
        }
    }
}