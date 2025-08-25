using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.ClassNames
{
    [TestFixture]
    public class SetClassNameRandomizerTests : StressTests
    {
        private ISetClassNameRandomizer setClassNameRandomizer;
        private IClassNameRandomizer anyNPCClassNameRandomizer;

        [SetUp]
        public void Setup()
        {
            anyNPCClassNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
            setClassNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
        }

        [Test]
        public void StressSetClassName()
        {
            stressor.Stress(AssertClassName);
        }

        protected void AssertClassName()
        {
            var prototype = GetCharacterPrototype();
            setClassNameRandomizer.SetClassName = prototype.CharacterClass.Name;

            var className = setClassNameRandomizer.Randomize(prototype.Alignment);
            Assert.That(className, Is.EqualTo(setClassNameRandomizer.SetClassName));
        }

        [Test]
        public void StressSetNPCClassName()
        {
            classNameRandomizer = anyNPCClassNameRandomizer;
            stressor.Stress(AssertClassName);
        }
    }
}