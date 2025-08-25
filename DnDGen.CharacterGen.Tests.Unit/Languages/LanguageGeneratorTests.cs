using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Languages;
using DnDGen.CharacterGen.Languages.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Languages
{
    [TestFixture]
    public class LanguageGeneratorTests
    {
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ILanguageCollectionsSelector> mockLanguageSelector;
        private ILanguageGenerator languageGenerator;
        private Race race;
        private CharacterClass characterClass;
        private List<Skill> skills;
        private List<string> automaticLanguages;
        private List<string> bonusLanguages;
        private Dictionary<string, Ability> abilities;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockLanguageSelector = new Mock<ILanguageCollectionsSelector>();
            languageGenerator = new LanguageGenerator(mockLanguageSelector.Object, mockCollectionsSelector.Object);

            race = new Race();
            characterClass = new CharacterClass();
            skills = new List<Skill>();
            automaticLanguages = new List<string>();
            bonusLanguages = new List<string>();
            abilities = new Dictionary<string, Ability>();

            race.BaseRace = "baserace";
            race.Metarace = "metarace";
            characterClass.Name = "class name";
            AddSkill("skill 1");
            AddSkill("skill 2");
            automaticLanguages.Add("lang 1");
            automaticLanguages.Add("lang 2");
            bonusLanguages.Add("lang 1");
            bonusLanguages.Add("lang 2");
            bonusLanguages.Add("lang 3");

            abilities[AbilityConstants.Intelligence] = new Ability(AbilityConstants.Intelligence);

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> ss) => ss.ElementAt(index++ % ss.Count()));
            mockLanguageSelector.Setup(s => s.SelectAutomaticLanguagesFor(race, characterClass.Name)).Returns(automaticLanguages);
            mockLanguageSelector.Setup(s => s.SelectBonusLanguagesFor(race.BaseRace, characterClass.Name)).Returns(bonusLanguages);
        }

        private void AddSkill(string skillName, string focus = "")
        {
            var stat = new Ability("base state");
            var skill = new Skill(skillName, stat, 0, focus);

            skills.Add(skill);
        }

        [Test]
        public void GetAutomaticLanguagesFromSelector()
        {
            bonusLanguages.Clear();

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages, Contains.Item("lang 2"));
            Assert.That(languages.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetNumberOfBonusLanguagesEqualToIntelligenceModifier()
        {
            automaticLanguages.Clear();
            abilities[AbilityConstants.Intelligence].Value = 14;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages, Contains.Item("lang 3"));
            Assert.That(languages.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllBonusLanguagesIfIntelligenceBonusIsHigher()
        {
            automaticLanguages.Clear();
            abilities[AbilityConstants.Intelligence].Value = 9266;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages, Contains.Item("lang 2"));
            Assert.That(languages, Contains.Item("lang 3"));
            Assert.That(languages.Count(), Is.EqualTo(3));
        }

        [Test]
        public void LanguagesContainAutomaticLanguagesAndBonusLanguages()
        {
            automaticLanguages.Clear();
            bonusLanguages.Clear();

            automaticLanguages.Add("automatic language");
            bonusLanguages.Add("bonus language");

            abilities[AbilityConstants.Intelligence].Value = 12;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("automatic language"));
            Assert.That(languages, Contains.Item("bonus language"));
            Assert.That(languages.Count(), Is.EqualTo(2));
        }

        [Test]
        public void LanguagesAreNotDuplicated()
        {
            abilities[AbilityConstants.Intelligence].Value = 14;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages, Contains.Item("lang 2"));
            Assert.That(languages, Contains.Item("lang 3"));
            Assert.That(languages.Count(), Is.EqualTo(3));
        }

        [Test]
        public void InterpreterGainsExtraLanguage()
        {
            automaticLanguages.Clear();

            AddSkill(SkillConstants.Profession, SkillConstants.Foci.Profession.Interpreter);

            abilities[AbilityConstants.Intelligence].Value = 12;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages, Contains.Item("lang 3"));
            Assert.That(languages.Count(), Is.EqualTo(2));
        }

        [Test]
        public void InterpreterGainsExtraLanguageEvenIfIntelligenceBonusIs0()
        {
            automaticLanguages.Clear();

            AddSkill(SkillConstants.Profession, SkillConstants.Foci.Profession.Interpreter);

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages.Count(), Is.EqualTo(1));
        }

        [Test]
        public void InterpreterGainsExtraLanguageEvenIfIntelligenceBonusIsNegative()
        {
            automaticLanguages.Clear();

            AddSkill(SkillConstants.Profession, SkillConstants.Foci.Profession.Interpreter);

            abilities[AbilityConstants.Intelligence].Value = 9;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages.Count(), Is.EqualTo(1));
        }

        [Test]
        public void NonInterpreterGainsNoExtraLanguage()
        {
            automaticLanguages.Clear();

            AddSkill(SkillConstants.Profession, SkillConstants.Foci.Profession.Adviser);

            abilities[AbilityConstants.Intelligence].Value = 12;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages.Count(), Is.EqualTo(1));
        }

        [Test]
        public void NoProfessionGainsNoExtraLanguage()
        {
            automaticLanguages.Clear();

            AddSkill("other skill", SkillConstants.Foci.Profession.Adviser);

            abilities[AbilityConstants.Intelligence].Value = 12;

            var languages = languageGenerator.GenerateWith(race, characterClass, abilities, skills);
            Assert.That(languages, Contains.Item("lang 1"));
            Assert.That(languages.Count(), Is.EqualTo(1));
        }
    }
}