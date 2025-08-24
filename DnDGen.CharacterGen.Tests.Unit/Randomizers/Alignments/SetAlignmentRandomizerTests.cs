using DnDGen.CharacterGen.Generators.Randomizers.Alignments;
using DnDGen.CharacterGen.Randomizers.Alignments;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Alignments
{
    [TestFixture]
    public class SetAlignmentRandomizerTests
    {
        private ISetAlignmentRandomizer randomizer;

        [SetUp]
        public void Setup()
        {
            randomizer = new SetAlignmentRandomizer();
        }

        [Test]
        public void SetAlignmentIsInitialized()
        {
            Assert.That(randomizer.SetAlignment, Is.Not.Null);
        }

        [Test]
        public void SetAlignmentRandomizerIsAnAlignmentRandomizer()
        {
            Assert.That(randomizer, Is.InstanceOf<IAlignmentRandomizer>());
        }

        [Test]
        public void ReturnSetAlignment()
        {
            randomizer.SetAlignment.Goodness = "goodness";
            randomizer.SetAlignment.Lawfulness = "lawfulness";

            var alignment = randomizer.Randomize();
            Assert.That(alignment, Is.EqualTo(randomizer.SetAlignment));
            Assert.That(alignment.Goodness, Is.EqualTo("goodness"));
            Assert.That(alignment.Lawfulness, Is.EqualTo("lawfulness"));
        }

        [Test]
        public void ReturnJustSetAlignment()
        {
            randomizer.SetAlignment.Goodness = "goodness";
            randomizer.SetAlignment.Lawfulness = "lawfulness";

            var alignments = randomizer.GetAllPossibleResults();
            Assert.That(alignments, Contains.Item(randomizer.SetAlignment));
            Assert.That(alignments.Count(), Is.EqualTo(1));
        }
    }
}