using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.ClassNames
{
    [TestFixture]
    public class DivineSpellcasterClassNameRandomizerTests : ClassNameRandomizerTests
    {
        protected override IEnumerable<string> allowedClassNames
        {
            get
            {
                return new[]
                {
                    CharacterClassConstants.Cleric,
                    CharacterClassConstants.Druid,
                    CharacterClassConstants.Paladin,
                    CharacterClassConstants.Ranger,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.DivineSpellcaster);
        }

        [Test]
        public void StressDivineSpellcasterClassName()
        {
            stressor.Stress(AssertClassName);
        }
    }
}