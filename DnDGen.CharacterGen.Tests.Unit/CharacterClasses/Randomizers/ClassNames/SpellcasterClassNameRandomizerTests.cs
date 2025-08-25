using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses.Randomizers.ClassNames
{
    [TestFixture]
    public class SpellcasterClassNameRandomizerTests : ClassNameRandomizerTests
    {
        protected override string classNameGroup
        {
            get { return GroupConstants.Spellcasters; }
        }

        [SetUp]
        public void Setup()
        {
            randomizer = new SpellcasterClassNameRandomizer(mockPercentileResultSelector.Object, mockCollectionsSelector.Object);
        }

        [Test]
        public void ClassIsAllowed()
        {
            alignmentClasses.Add(ClassName);
            groupClasses.Add(ClassName);
            var classNames = randomizer.GetAllPossibleResults(alignment);
            Assert.That(classNames, Contains.Item(ClassName));
        }

        [Test]
        public void ClassIsNotInAlignment()
        {
            groupClasses.Add(ClassName);
            var classNames = randomizer.GetAllPossibleResults(alignment);
            Assert.That(classNames, Is.All.Not.EqualTo(ClassName));
        }

        [Test]
        public void ClassIsNotInGroup()
        {
            alignmentClasses.Add(ClassName);
            var classNames = randomizer.GetAllPossibleResults(alignment);
            Assert.That(classNames, Is.All.Not.EqualTo(ClassName));
        }
    }
}