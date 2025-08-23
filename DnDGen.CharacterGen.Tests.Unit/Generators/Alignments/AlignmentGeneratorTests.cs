using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Randomizers.Alignments;
using Moq;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Alignments
{
    [TestFixture]
    public class AlignmentGeneratorTests
    {
        private IAlignmentGenerator alignmentGenerator;
        private Mock<IAlignmentRandomizer> mockAlignmentRandomizer;
        private Alignment prototype;

        [SetUp]
        public void Setup()
        {
            alignmentGenerator = new AlignmentGenerator();

            mockAlignmentRandomizer = new Mock<IAlignmentRandomizer>();
            prototype = new Alignment("prototype alignment");
        }

        [Test]
        public void GeneratePrototype()
        {
            mockAlignmentRandomizer.Setup(r => r.Randomize()).Returns(prototype);

            var generatedPrototype = alignmentGenerator.GeneratePrototype(mockAlignmentRandomizer.Object);
            Assert.That(prototype, Is.EqualTo(generatedPrototype));
        }

        [Test]
        public void GenerateAlignment()
        {
            var generatedAlignment = alignmentGenerator.GenerateWith(prototype);
            Assert.That(generatedAlignment, Is.EqualTo(prototype));
            Assert.That(generatedAlignment.Full, Is.EqualTo("prototype alignment"));
        }
    }
}