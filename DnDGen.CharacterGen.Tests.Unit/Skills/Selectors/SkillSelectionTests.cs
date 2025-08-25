using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Skills.Selectors;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Skills.Selectors
{
    [TestFixture]
    public class SkillSelectionTests
    {
        private SkillSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new SkillSelection();
        }

        [Test]
        public void SkillSelectionInitialized()
        {
            Assert.That(selection.BaseStatName, Is.Empty);
            Assert.That(selection.SkillName, Is.Empty);
            Assert.That(selection.RandomFociQuantity, Is.EqualTo(0));
            Assert.That(selection.Focus, Is.Empty);
        }

        [TestCase("skill", "", "skill", "", true)]
        [TestCase("skill", "focus", "skill", "focus", true)]
        [TestCase("skill", "", "skill", "focus", false)]
        [TestCase("skill", "focus", "skill", "", false)]
        [TestCase("skill", "", "other skill", "", false)]
        [TestCase("skill", "focus", "other skill", "focus", false)]
        [TestCase("skill", "focus", "skill", "other focus", false)]
        public void SkillSelectionEqualsSkill(string selectionName, string selectionFocus, string skillName, string skillFocus, bool shouldEqual)
        {
            selection.Focus = selectionFocus;
            selection.SkillName = selectionName;

            var stat = new Ability("stat name");
            var skill = new Skill(skillName, stat, 0, skillFocus);

            var isEqual = selection.IsEqualTo(skill);
            Assert.That(isEqual, Is.EqualTo(shouldEqual));
        }

        [Test]
        public void IfRandomFociQuantityIsPositive_ThrowExceptionWhenTestingEqualToSkill()
        {
            selection.RandomFociQuantity = 1;
            selection.SkillName = "skill";

            var stat = new Ability("stat name");
            var skill = new Skill("skill", stat, 0);

            Assert.That(() => selection.IsEqualTo(skill), Throws.InvalidOperationException.With.Message.EqualTo("Cannot test equality of a skill selection while random foci quantity is positive"));
        }

        [TestCase("skill", "", "skill", "", true)]
        [TestCase("skill", "focus", "skill", "focus", true)]
        [TestCase("skill", "", "skill", "focus", false)]
        [TestCase("skill", "focus", "skill", "", false)]
        [TestCase("skill", "", "other skill", "", false)]
        [TestCase("skill", "focus", "other skill", "focus", false)]
        [TestCase("skill", "focus", "skill", "other focus", false)]
        public void SkillSelectionEqualsSkillSelection(string selectionName, string selectionFocus, string skillName, string skillFocus, bool shouldEqual)
        {
            selection.Focus = selectionFocus;
            selection.SkillName = selectionName;

            var otherSelection = new SkillSelection();
            otherSelection.SkillName = skillName;
            otherSelection.Focus = skillFocus;

            var isEqual = selection.IsEqualTo(otherSelection);
            Assert.That(isEqual, Is.EqualTo(shouldEqual));
        }

        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        public void IfRandomFociQuantityIsPositive_ThrowExceptionWhenTestingEqualToSkillSelection(int selectionQuantity, int otherSelectionQuantity)
        {
            selection.SkillName = "skill";
            selection.RandomFociQuantity = selectionQuantity;

            var otherSelection = new SkillSelection();
            otherSelection.SkillName = "skill";
            otherSelection.RandomFociQuantity = otherSelectionQuantity;

            Assert.That(() => selection.IsEqualTo(otherSelection), Throws.InvalidOperationException.With.Message.EqualTo("Cannot test equality of a skill selection while random foci quantity is positive"));
        }
    }
}