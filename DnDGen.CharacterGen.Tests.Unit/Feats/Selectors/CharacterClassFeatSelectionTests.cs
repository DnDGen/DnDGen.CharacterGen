using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Races;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Feats.Selectors
{
    [TestFixture]
    public class CharacterClassFeatSelectionTests
    {
        private CharacterClassFeatSelection selection;
        private CharacterClass characterClass;
        private Race race;
        private List<Feat> feats;

        [SetUp]
        public void Setup()
        {
            selection = new CharacterClassFeatSelection();
            characterClass = new CharacterClass();
            race = new Race();
            feats = [];
        }

        [Test]
        public void SelectionIsInitialized()
        {
            Assert.That(selection.Feat, Is.Empty);
            Assert.That(selection.FocusType, Is.Empty);
            Assert.That(selection.Frequency, Is.Not.Null);
            Assert.That(selection.MinimumLevel, Is.EqualTo(0));
            Assert.That(selection.RequiredFeats, Is.Empty);
            Assert.That(selection.Power, Is.EqualTo(0));
            Assert.That(selection.MaximumLevel, Is.EqualTo(0));
            Assert.That(selection.FrequencyQuantityAbility, Is.Empty);
            Assert.That(selection.SizeRequirement, Is.Empty);
            Assert.That(selection.AllowFocusOfAll, Is.False);
        }

        [TestCase(1, false)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        public void CharacterLevelMustBeInBoundsToMeetRequirement(int characterLevel, bool met)
        {
            characterClass.Level = characterLevel;
            selection.MaximumLevel = 4;
            selection.MinimumLevel = 2;

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.EqualTo(met));
        }

        [Test]
        public void RequirementsMetWithNoMaximumLevel()
        {
            characterClass.Level = 5;
            selection.MinimumLevel = 2;

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void MetIfNoSizeRequirement()
        {
            race.Size = "size";

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void MetIfMatchingSizeRequirement()
        {
            race.Size = "size";
            selection.SizeRequirement = "size";

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void NotMetIfSizeRequirementDoesNotMatch()
        {
            race.Size = "size";
            selection.SizeRequirement = "other size";

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void MetIfNoRequiredFeats()
        {
            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void MetWithAllRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };
            feats.Add(new Feat { Name = "feat 1" });
            feats.Add(new Feat { Name = "feat 2" });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void NotMetWithSomeRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };
            feats.Add(new Feat { Name = "feat 1" });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void NotMetWithNoRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void MetWithAllRequiredFeatsAndFoci()
        {
            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1", Focus = "focus 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1", Foci = new[] { "focus 1", "focus 3" } });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 4", "focus 2" } });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void MetWithExtraRequiredFeatsAndFoci()
        {
            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1", Focus = "focus 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1", Foci = new[] { "focus 1", "focus 2" } });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 2", "focus 1", "focus 3" } });
            feats.Add(new Feat { Name = "feat 3" });
            feats.Add(new Feat { Name = "feat 4", Foci = new[] { "focus 4" } });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void NotMetWithSomeRequiredFeatsAndFoci()
        {
            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1", Focus = "focus 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1", Foci = new[] { "focus 2" } });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 2", "focus 1" } });
            feats.Add(new Feat { Name = "feat 3", Foci = new[] { "focus 1" } });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void NotMetWithNoRequiredFeatsAndFoci()
        {
            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1", Focus = "focus 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1", Foci = new[] { "focus 2" } });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 1" } });
            feats.Add(new Feat { Name = "feat 3", Foci = new[] { "focus 1", "focus 2" } });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void AllRequirementsMet()
        {
            characterClass.Level = 3;
            race.Size = "size";
            selection.SizeRequirement = "size";
            selection.MaximumLevel = 4;
            selection.MinimumLevel = 2;

            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1" });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 2" } });

            var requirementsMet = selection.RequirementsMet(characterClass, race, feats);
            Assert.That(requirementsMet, Is.True);
        }
    }
}