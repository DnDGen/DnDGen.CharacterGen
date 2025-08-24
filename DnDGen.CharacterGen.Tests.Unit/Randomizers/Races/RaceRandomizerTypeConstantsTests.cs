using DnDGen.CharacterGen.Randomizers.Races;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races
{
    [TestFixture]
    public class RaceRandomizerTypeConstantsTests
    {
        [TestCase(RaceRandomizerTypeConstants.BaseRace.AnyBase, "Any Base")]
        [TestCase(RaceRandomizerTypeConstants.BaseRace.AquaticBase, "Aquatic Base")]
        [TestCase(RaceRandomizerTypeConstants.BaseRace.MonsterBase, "Monster Base")]
        [TestCase(RaceRandomizerTypeConstants.BaseRace.NonMonsterBase, "Non-Monster Base")]
        [TestCase(RaceRandomizerTypeConstants.BaseRace.NonStandardBase, "Non-Standard Base")]
        [TestCase(RaceRandomizerTypeConstants.BaseRace.StandardBase, "Standard Base")]
        [TestCase(RaceRandomizerTypeConstants.Metarace.AnyMeta, "Any Meta")]
        [TestCase(RaceRandomizerTypeConstants.Metarace.GeneticMeta, "Genetic Meta")]
        [TestCase(RaceRandomizerTypeConstants.Metarace.LycanthropeMeta, "Lycanthrope Meta")]
        [TestCase(RaceRandomizerTypeConstants.Metarace.NoMeta, "No Meta")]
        [TestCase(RaceRandomizerTypeConstants.Metarace.UndeadMeta, "Undead Meta")]
        public void RaceRandomizerTypeConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}
