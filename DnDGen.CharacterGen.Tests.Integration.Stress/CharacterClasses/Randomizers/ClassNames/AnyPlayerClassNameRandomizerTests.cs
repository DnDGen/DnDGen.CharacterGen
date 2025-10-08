using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.CharacterClasses.Randomizers.ClassNames
{
    [TestFixture]
    public class AnyPlayerClassNameRandomizerTests : ClassNameRandomizerTests
    {
        protected override IEnumerable<string> allowedClassNames
        {
            get
            {
                return new[] {
                    CharacterClassConstants.Barbarian,
                    CharacterClassConstants.Bard,
                    CharacterClassConstants.Cleric,
                    CharacterClassConstants.Druid,
                    CharacterClassConstants.Fighter,
                    CharacterClassConstants.Monk,
                    CharacterClassConstants.Paladin,
                    CharacterClassConstants.Ranger,
                    CharacterClassConstants.Rogue,
                    CharacterClassConstants.Sorcerer,
                    CharacterClassConstants.Wizard
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyPlayer);
        }

        [Test]
        public void StressAnyPlayerClassName()
        {
            stressor.Stress(AssertClassName);
        }
    }
}