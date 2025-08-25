using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.ClassNames
{
    [TestFixture]
    public class ArcaneSpellcasterClassNameRandomizerTests : ClassNameRandomizerTests
    {
        protected override IEnumerable<string> allowedClassNames
        {
            get
            {
                return new[]
                {
                    CharacterClassConstants.Bard,
                    CharacterClassConstants.Sorcerer,
                    CharacterClassConstants.Wizard
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.ArcaneSpellcaster);
        }

        [Test]
        public void StressArcaneSpellcasterClassName()
        {
            stressor.Stress(AssertClassName);
        }
    }
}