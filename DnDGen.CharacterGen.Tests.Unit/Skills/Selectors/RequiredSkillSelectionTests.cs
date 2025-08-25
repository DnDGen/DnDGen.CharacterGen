using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Skills.Selectors;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Skills.Selectors
{
    [TestFixture]
    public class RequiredSkillSelectionTests
    {
        private RequiredSkillSelection requiredSkillSelection;
        private List<Skill> otherSkills;
        private Ability baseStat;

        [SetUp]
        public void Setup()
        {
            requiredSkillSelection = new RequiredSkillSelection();
            otherSkills = [];
            baseStat = new Ability("stat name");
        }

        [Test]
        public void RequiredSkillInitialized()
        {
            Assert.That(requiredSkillSelection.Skill, Is.Empty);
            Assert.That(requiredSkillSelection.Focus, Is.Empty);
            Assert.That(requiredSkillSelection.Ranks, Is.EqualTo(0));
        }

        [Test]
        public void RequirementMetIfOtherSkillsContainSkillName()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));

            requiredSkillSelection.Skill = "skill 2";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.True);
        }

        private Skill CreateClassSkill(string skillName, string focus = "")
        {
            var skill = new Skill(skillName, baseStat, int.MaxValue, focus);
            skill.ClassSkill = true;

            return skill;
        }

        [Test]
        public void RequirementNotMetIfOtherSkillDoNotContainSkillName()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));

            requiredSkillSelection.Skill = "skill 3";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementNotMetIfOtherSkillsContainSkillNameAndNoRequiredFocus()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2", "focus"));

            requiredSkillSelection.Skill = "skill 2";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementMetIfOtherSkillsContainSkillNameAndRequiredFocusIsOnSkill()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2", "focus"));

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfOtherSkillsContainSkillNameButNotRequiredFocus()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementNotMetIfOtherSkillsContainSkillNameAndRequiredFocusIsNotOnMatchingSkill()
        {
            otherSkills.Add(CreateClassSkill("skill 1", "focus"));
            otherSkills.Add(CreateClassSkill("skill 2"));

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementNotMetIfOtherSkillsContainSkillNameAndRequiredFocusDoesNotMatch()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2", "other focus"));

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementMetIfSufficientRanks()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));
            otherSkills[1].Ranks = 1;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Ranks = 1;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfInsufficientRanks()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));
            otherSkills[0].Ranks = 2;
            otherSkills[1].Ranks = 1;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Ranks = 2;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementMetIfSufficientRanksWithFocus()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2", "focus"));
            otherSkills[1].Ranks = 1;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";
            requiredSkillSelection.Ranks = 1;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfInsufficientRanksWithFocus()
        {
            otherSkills.Add(CreateClassSkill("skill 1", "focus"));
            otherSkills.Add(CreateClassSkill("skill 2", "focus"));
            otherSkills[0].Ranks = 2;
            otherSkills[1].Ranks = 1;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Focus = "focus";
            requiredSkillSelection.Ranks = 2;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementMetIfSufficientRanksAsCrossClass()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));
            otherSkills[1].Ranks = 2;
            otherSkills[1].ClassSkill = false;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Ranks = 1;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfInsufficientRanksAsCrossClass()
        {
            otherSkills.Add(CreateClassSkill("skill 1"));
            otherSkills.Add(CreateClassSkill("skill 2"));
            otherSkills[0].Ranks = 2;
            otherSkills[1].Ranks = 1;
            otherSkills[1].ClassSkill = false;

            requiredSkillSelection.Skill = "skill 2";
            requiredSkillSelection.Ranks = 1;

            var met = requiredSkillSelection.RequirementMet(otherSkills);
            Assert.That(met, Is.False);
        }
    }
}
