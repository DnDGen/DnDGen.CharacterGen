using DnDGen.CharacterGen.Skills.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Skills.Selectors
{
    [TestFixture]
    public class SkillSelectorTests
    {
        private ISkillSelector skillSelector;
        private Mock<ICollectionSelector> mockInnerSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionSelector>();
            skillSelector = new SkillSelector(mockInnerSelector.Object);
        }

        [Test]
        public void GetCollectionFromInnerCollectionSelector()
        {
            var skillData = new string[4];
            skillData[DataIndexConstants.SkillSelectionData.BaseStatName] = "base stat";
            skillData[DataIndexConstants.SkillSelectionData.Focus] = "focus";
            skillData[DataIndexConstants.SkillSelectionData.RandomFociQuantity] = "9266";
            skillData[DataIndexConstants.SkillSelectionData.SkillName] = "skill name";

            mockInnerSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillData, "skill")).Returns(skillData);

            var selection = skillSelector.SelectFor("skill");
            Assert.That(selection.BaseStatName, Is.EqualTo("base stat"));
            Assert.That(selection.SkillName, Is.EqualTo("skill name"));
            Assert.That(selection.Focus, Is.EqualTo("focus"));
            Assert.That(selection.RandomFociQuantity, Is.EqualTo(9266));
        }
    }
}