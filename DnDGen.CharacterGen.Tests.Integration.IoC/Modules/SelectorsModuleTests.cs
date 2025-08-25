using DnDGen.CharacterGen.Abilities.Selectors;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Languages.Selectors;
using DnDGen.CharacterGen.Leaders.Selectors;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Skills.Selectors;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class SelectorsModuleTests : IoCTests
    {
        [Test]
        public void LanguageSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<ILanguageCollectionsSelector>();
        }

        [Test]
        public void LevelAdjustmentsSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<IAdjustmentsSelector>();
        }

        [Test]
        public void StatAdjustmentsSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<IAbilityAdjustmentsSelector>();
        }

        [Test]
        public void SkillSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<ISkillSelector>();
        }

        [Test]
        public void FeatsSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<IFeatsSelector>();
        }

        [Test]
        public void LeadershipSelectorsAreNotGeneratedAsSingletons()
        {
            AssertNotSingleton<ILeadershipSelector>();
        }
    }
}