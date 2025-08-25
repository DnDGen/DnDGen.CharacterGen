using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Races;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Feats.Selectors
{
    [TestFixture]
    public class RacialFeatSelectionTests
    {
        private RacialFeatSelection selection;
        private Race race;
        private Dictionary<string, Ability> stats;
        private List<Feat> feats;

        [SetUp]
        public void Setup()
        {
            selection = new RacialFeatSelection();
            race = new Race();
            stats = [];
            feats = [];

            stats["stat"] = new Ability("stat")
            {
                Value = 42
            };
        }

        [Test]
        public void RacialFeatSelectionInitialization()
        {
            Assert.That(selection.Feat, Is.Empty);
            Assert.That(selection.Power, Is.EqualTo(0));
            Assert.That(selection.MinimumHitDieRequirement, Is.EqualTo(0));
            Assert.That(selection.SizeRequirement, Is.Empty);
            Assert.That(selection.FocusType, Is.Empty);
            Assert.That(selection.Frequency, Is.Not.Null);
            Assert.That(selection.MaximumHitDieRequirement, Is.EqualTo(0));
            Assert.That(selection.MinimumAbilities, Is.Empty);
            Assert.That(selection.RandomFociQuantity, Is.Empty);
            Assert.That(selection.RequiredFeats, Is.Empty);
        }

        [Test]
        public void RequirementsMetIfNoRequirements()
        {
            var met = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementsNotMetIfWrongSize()
        {
            race.Size = "big";
            selection.SizeRequirement = "small";

            var met = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementsNotMetIfBelowHitDiceRange()
        {
            selection.MinimumHitDieRequirement = 3;

            var met = selection.RequirementsMet(race, 2, stats, feats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementsNotMetIfAboveHitDiceRange()
        {
            selection.MaximumHitDieRequirement = 3;

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementsMetIfAboveMaximumOfZero()
        {
            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementsMetForMinimumHitDice()
        {
            selection.MinimumHitDieRequirement = 4;

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementsMetForMaximumHitDice()
        {
            selection.MaximumHitDieRequirement = 4;

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementsNotMetIfDoesNotHaveMinimumStat()
        {
            selection.MinimumAbilities["stat"] = 9266;

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementsMetIfAnyMinimumStatIsMet()
        {
            selection.MinimumAbilities["stat"] = 9266;
            selection.MinimumAbilities["stat 2"] = 600;

            stats["stat 2"] = new Ability("stat 2");
            stats["stat 2"].Value = 600;

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void MetIfNoRequiredFeats()
        {
            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void MetWithAllRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };
            feats.Add(new Feat { Name = "feat 1" });
            feats.Add(new Feat { Name = "feat 2" });

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(requirementsMet, Is.True);
        }

        [Test]
        public void NotMetWithSomeRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };
            feats.Add(new Feat { Name = "feat 1" });

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void NotMetWithNoRequiredFeats()
        {
            selection.RequiredFeats = new[] { new RequiredFeatSelection { Feat = "feat 1" }, new RequiredFeatSelection { Feat = "feat 2" } };

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
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

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
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

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
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

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
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

            var requirementsMet = selection.RequirementsMet(race, 0, stats, feats);
            Assert.That(requirementsMet, Is.False);
        }

        [Test]
        public void AllRequirementsMet()
        {
            race.Size = "big";

            selection.SizeRequirement = race.Size;
            selection.MinimumHitDieRequirement = 3;
            selection.MaximumHitDieRequirement = 5;
            selection.MinimumAbilities["stat"] = 42;

            selection.RequiredFeats = new[]
            {
                new RequiredFeatSelection { Feat = "feat 1" },
                new RequiredFeatSelection { Feat = "feat 2", Focus = "focus 2" }
            };

            feats.Add(new Feat { Name = "feat 1" });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { "focus 2" } });

            var met = selection.RequirementsMet(race, 4, stats, feats);
            Assert.That(met, Is.True);
        }
    }
}