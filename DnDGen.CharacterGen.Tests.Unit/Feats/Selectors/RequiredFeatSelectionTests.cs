using DnDGen.CharacterGen.Feats;
using NUnit.Framework;
using System.Collections.Generic;
using DnDGen.CharacterGen.Feats.Selectors;

namespace DnDGen.CharacterGen.Tests.Unit.Feats.Selectors
{
    [TestFixture]
    public class RequiredFeatSelectionTests
    {
        private RequiredFeatSelection requiredFeatSelection;
        private List<Feat> otherFeats;

        [SetUp]
        public void Setup()
        {
            requiredFeatSelection = new RequiredFeatSelection();
            otherFeats = new List<Feat>();
        }

        [Test]
        public void RequiredFeatInitialized()
        {
            Assert.That(requiredFeatSelection.Feat, Is.Empty);
            Assert.That(requiredFeatSelection.Focus, Is.Empty);
        }

        [Test]
        public void RequirementMetIfOtherFeatsContainFeatName()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[1].Name = "feat2";

            requiredFeatSelection.Feat = "feat2";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfOtherFeatDoNotContainFeatName()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[1].Name = "feat2";

            requiredFeatSelection.Feat = "feat3";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementMetIfOtherFeatsContainFeatNAmeAndNoRequiredFocus()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[1].Name = "feat2";
            otherFeats[1].Foci = new[] { "focus" };

            requiredFeatSelection.Feat = "feat2";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementMetIfOtherFeatsContainFeatNameAndRequiredFocusIsOnFeat()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[1].Name = "feat2";
            otherFeats[1].Foci = new[] { "focus" };

            requiredFeatSelection.Feat = "feat2";
            requiredFeatSelection.Focus = "focus";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementMetIfOtherFeatsContainFeatNameAndRequiredFocusIsOnAtLeastOneFeat()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat2";
            otherFeats[0].Foci = new[] { "other focus", "focus" };

            requiredFeatSelection.Feat = "feat2";
            requiredFeatSelection.Focus = "focus";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.True);
        }

        [Test]
        public void RequirementNotMetIfOtherFeatsContainFeatNameAndRequiredFocusIsNotOnMatchingFeat()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[0].Foci = new[] { "focus" };
            otherFeats[1].Name = "feat2";
            otherFeats[1].Foci = new[] { "other focus" };

            requiredFeatSelection.Feat = "feat2";
            requiredFeatSelection.Focus = "focus";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.False);
        }

        [Test]
        public void RequirementNotMetIfOtherFeatsContainFeatNameAndRequiredFocusDoesNotMatch()
        {
            otherFeats.Add(new Feat());
            otherFeats.Add(new Feat());
            otherFeats[0].Name = "feat1";
            otherFeats[1].Name = "feat2";
            otherFeats[1].Foci = new[] { "other focus" };

            requiredFeatSelection.Feat = "feat2";
            requiredFeatSelection.Focus = "focus";

            var met = requiredFeatSelection.RequirementMet(otherFeats);
            Assert.That(met, Is.False);
        }
    }
}