using DnDGen.CharacterGen.Randomizers.Alignments;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Alignments
{
    [TestFixture]
    public class AlignmentRandomizerTypeConstantsTests
    {
        [TestCase(AlignmentRandomizerTypeConstants.Any, "Any")]
        [TestCase(AlignmentRandomizerTypeConstants.Chaotic, "Chaotic")]
        [TestCase(AlignmentRandomizerTypeConstants.Evil, "Evil")]
        [TestCase(AlignmentRandomizerTypeConstants.Good, "Good")]
        [TestCase(AlignmentRandomizerTypeConstants.Lawful, "Lawful")]
        [TestCase(AlignmentRandomizerTypeConstants.Neutral, "Neutral")]
        [TestCase(AlignmentRandomizerTypeConstants.NonChaotic, "Non-chaotic")]
        [TestCase(AlignmentRandomizerTypeConstants.NonEvil, "Non-evil")]
        [TestCase(AlignmentRandomizerTypeConstants.NonGood, "Non-good")]
        [TestCase(AlignmentRandomizerTypeConstants.NonLawful, "Non-lawful")]
        [TestCase(AlignmentRandomizerTypeConstants.NonNeutral, "Non-neutral")]
        public void AlignmentRandomizerTypeConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}
