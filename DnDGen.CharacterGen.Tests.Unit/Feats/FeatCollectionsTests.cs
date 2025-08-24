using DnDGen.CharacterGen.Feats;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Feats
{
    [TestFixture]
    public class FeatCollectionsTests
    {
        private FeatCollections featCollections;
        private List<Feat> racialFeats;
        private List<Feat> classFeats;
        private List<Feat> additionalFeats;

        [SetUp]
        public void Setup()
        {
            featCollections = new FeatCollections();
            racialFeats = new List<Feat>();
            classFeats = new List<Feat>();
            additionalFeats = new List<Feat>();

            featCollections.Additional = additionalFeats;
            featCollections.Class = classFeats;
            featCollections.Racial = racialFeats;
        }

        [Test]
        public void FeatCollectionsInitialized()
        {
            featCollections = new FeatCollections();
            Assert.That(featCollections.Additional, Is.Empty);
            Assert.That(featCollections.All, Is.Empty);
            Assert.That(featCollections.Class, Is.Empty);
            Assert.That(featCollections.Racial, Is.Empty);
        }

        [Test]
        public void AllIsUnionOfOtherFeats()
        {
            racialFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });
            classFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });
            additionalFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });

            var names = featCollections.All.Select(f => f.Name);
            Assert.That(names, Is.SupersetOf(racialFeats.Select(f => f.Name)));
            Assert.That(names, Is.SupersetOf(classFeats.Select(f => f.Name)));
            Assert.That(names, Is.SupersetOf(additionalFeats.Select(f => f.Name)));
            Assert.That(names.Count, Is.EqualTo(3));
        }

        [Test]
        public void AllIsUnionOfClonesOfOtherFeats()
        {
            racialFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });
            classFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });
            additionalFeats.Add(new Feat { Name = Guid.NewGuid().ToString() });

            Assert.That(featCollections.All.Count, Is.EqualTo(3));
            Assert.That(featCollections.All.ElementAt(0), Is.Not.SameAs(racialFeats[0]));
            Assert.That(featCollections.All.ElementAt(0).Name, Is.EqualTo(racialFeats[0].Name));
            Assert.That(featCollections.All.ElementAt(1), Is.Not.SameAs(classFeats[0]));
            Assert.That(featCollections.All.ElementAt(1).Name, Is.EqualTo(classFeats[0].Name));
            Assert.That(featCollections.All.ElementAt(2), Is.Not.SameAs(additionalFeats[0]));
            Assert.That(featCollections.All.ElementAt(2).Name, Is.EqualTo(additionalFeats[0].Name));
        }

        [Test]
        public void FeatCollectionsDoNotAlterOriginalFeatsWhenComputingAll()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            classFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 1;
            racialFeats[0].Frequency.TimePeriod = "fortnight";
            classFeats[0].Foci = new[] { "focus" };
            classFeats[0].Name = "feat1";
            classFeats[0].Power = 9266;
            classFeats[0].Frequency.Quantity = 5;
            classFeats[0].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Name = "feat2";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(6));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Name, Is.EqualTo("feat2"));
            Assert.That(featCollections.All.Count(), Is.EqualTo(2));

            Assert.That(featCollections.Racial.Count, Is.EqualTo(2));
            Assert.That(featCollections.Class.Count, Is.EqualTo(1));
            Assert.That(racialFeats[0].Foci.Single(), Is.EqualTo("focus"));
            Assert.That(racialFeats[0].Name, Is.EqualTo("feat1"));
            Assert.That(racialFeats[0].Power, Is.EqualTo(9266));
            Assert.That(racialFeats[0].Frequency.Quantity, Is.EqualTo(1));
            Assert.That(racialFeats[0].Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(classFeats[0].Foci.Single(), Is.EqualTo("focus"));
            Assert.That(classFeats[0].Name, Is.EqualTo("feat1"));
            Assert.That(classFeats[0].Power, Is.EqualTo(9266));
            Assert.That(classFeats[0].Frequency.Quantity, Is.EqualTo(5));
            Assert.That(classFeats[0].Frequency.TimePeriod, Is.EqualTo("fortnight"));
        }

        [Test]
        public void IfNameAndFocusAndPowerAndFrequencyTimePeriodAreEqual_CombineFrequencyQuantities()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 1;
            racialFeats[0].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 5;
            racialFeats[1].Frequency.TimePeriod = "fortnight";
            racialFeats[2].Name = "feat2";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(6));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Name, Is.EqualTo("feat2"));
            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNamesDoNotMatch_DoNotCombineFrequencyQuantities()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 1;
            racialFeats[0].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat2";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 5;
            racialFeats[1].Frequency.TimePeriod = "fortnight";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(1));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(5));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.Name, Is.EqualTo("feat2"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfFociDoNotMatch_DoNotCombineFrequencyQuantities()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 4;
            racialFeats[0].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Foci = new[] { "focus2" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 2;
            racialFeats[1].Frequency.TimePeriod = "fortnight";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(4));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus2"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(2));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfPowersDoNotMatch_DoNotCombineFrequencyQuantities()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 3;
            racialFeats[0].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 42;
            racialFeats[1].Frequency.Quantity = 3;
            racialFeats[1].Frequency.TimePeriod = "fortnight";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(3));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(3));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Power, Is.EqualTo(42));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfFrequencyTimePeriodsDoNotMatch_DoNotCombineFrequencyQuantities()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 1;
            racialFeats[0].Frequency.TimePeriod = "day";
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 5;
            racialFeats[1].Frequency.TimePeriod = "fortnight";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(1));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("day"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(5));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNamesAndFocusAndPowerAreEqual_ConstantWinsOut()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 5;
            racialFeats[0].Frequency.TimePeriod = FeatConstants.Frequencies.Day;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 0;
            racialFeats[1].Frequency.TimePeriod = FeatConstants.Frequencies.Constant;

            var first = featCollections.All.First();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo(FeatConstants.Frequencies.Constant));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IfNamesAndFocusAndPowerAreEqual_AtWillWinsOut()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 5;
            racialFeats[0].Frequency.TimePeriod = FeatConstants.Frequencies.Day;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 0;
            racialFeats[1].Frequency.TimePeriod = FeatConstants.Frequencies.AtWill;

            var first = featCollections.All.First();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo(FeatConstants.Frequencies.AtWill));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ConstantFrequencyBeatsAtWillFrequency()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.Quantity = 0;
            racialFeats[0].Frequency.TimePeriod = FeatConstants.Frequencies.AtWill;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 0;
            racialFeats[1].Frequency.TimePeriod = FeatConstants.Frequencies.Constant;

            var first = featCollections.All.First();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo(FeatConstants.Frequencies.Constant));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IfNoFrequencyAndRestMatchesButUnequalPowers_KeepHigherPower()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 42;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[2].Name = "feat2";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Name, Is.EqualTo("feat2"));
            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfHasFrequencyAndRestMatchesButUnequalPowers_DoNotRemovePowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 42;
            racialFeats[0].Frequency.Quantity = 0;
            racialFeats[0].Frequency.TimePeriod = "time period";
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.Quantity = 0;
            racialFeats[1].Frequency.TimePeriod = "time period";

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("time period"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(42));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("time period"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNoFrequencyButPowersAndNamesDoNotMatch_DoNotRemovePowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 42;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat2";
            racialFeats[1].Power = 9266;

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(42));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Name, Is.EqualTo("feat2"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNoFrequencyButPowerAndFociDoNotMatch_DoNotRemovePowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 42;
            racialFeats[1].Foci = new[] { "focus2" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 9266;

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(42));

            Assert.That(last.Foci.Single(), Is.EqualTo("focus2"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNoFrequencyAndFeatNamesMatchAndFociMatchAndEqualPower_RemoveDuplicatePowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 42;
            racialFeats[1].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Power = 42;

            var first = featCollections.All.First();

            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Power, Is.EqualTo(42));

            Assert.That(featCollections.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void FeatsThatCanBeTakenMultipleTimesAreAllowed()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].CanBeTakenMultipleTimes = true;
            racialFeats[1].Name = "feat1";
            racialFeats[1].CanBeTakenMultipleTimes = true;
            racialFeats[2].Name = "feat2";
            racialFeats[2].CanBeTakenMultipleTimes = true;
            racialFeats[2].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[2].Power = 2;
            racialFeats[3].Name = "feat2";
            racialFeats[3].CanBeTakenMultipleTimes = true;
            racialFeats[3].Foci = new[] { "focus 2", "focus 3" };
            racialFeats[3].Power = 8;

            var featsList = featCollections.All.ToList();

            Assert.That(featsList.Count, Is.EqualTo(4));
            Assert.That(featsList[0].Name, Is.EqualTo("feat1"));
            Assert.That(featsList[1].Name, Is.EqualTo("feat1"));
            Assert.That(featsList[2].Name, Is.EqualTo("feat2"));
            Assert.That(featsList[2].Power, Is.EqualTo(2));
            Assert.That(featsList[2].Foci, Contains.Item("focus 1"));
            Assert.That(featsList[2].Foci, Contains.Item("focus 2"));
            Assert.That(featsList[2].Foci.Count(), Is.EqualTo(2));
            Assert.That(featsList[3].Name, Is.EqualTo("feat2"));
            Assert.That(featsList[3].Power, Is.EqualTo(8));
            Assert.That(featsList[3].Foci, Contains.Item("focus 2"));
            Assert.That(featsList[3].Foci, Contains.Item("focus 3"));
            Assert.That(featsList[3].Foci.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfFeatHasFocusOfAll_RemoveOtherInstancesOfTheFeat()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[2].Name = "feat2";
            racialFeats[3].Name = "feat3";
            racialFeats[3].Foci = new[] { "focus" };

            var feat1 = featCollections.All.First(f => f.Name == "feat1");
            var feat2 = featCollections.All.First(f => f.Name == "feat2");
            var feat3 = featCollections.All.First(f => f.Name == "feat3");

            Assert.That(feat1.Name, Is.EqualTo("feat1"));
            Assert.That(feat1.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
            Assert.That(feat2.Name, Is.EqualTo("feat2"));
            Assert.That(feat2.Foci, Is.Empty);
            Assert.That(feat3.Name, Is.EqualTo("feat3"));
            Assert.That(feat3.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(featCollections.All.Count(), Is.EqualTo(3));
        }

        [Test]
        public void IfFeatWithFocusOfAllCanBeTakenMultipleTimes_DoNotRemoveOtherInstancesOfTheFeat()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].CanBeTakenMultipleTimes = true;
            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[1].Name = "feat1";
            racialFeats[1].CanBeTakenMultipleTimes = true;
            racialFeats[1].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[2].Name = "feat2";
            racialFeats[2].CanBeTakenMultipleTimes = true;
            racialFeats[3].Name = "feat3";
            racialFeats[3].Foci = new[] { "focus" };

            Assert.That(featCollections.All.Count(), Is.EqualTo(4));
        }

        [Test]
        public void IfFeatWithFocusOfAllIsDuplicated_KeepOne()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[2].Name = "feat2";
            racialFeats[3].Name = "feat3";
            racialFeats[3].Foci = new[] { FeatConstants.Foci.All };

            var feat1 = featCollections.All.First(f => f.Name == "feat1");
            var feat2 = featCollections.All.First(f => f.Name == "feat2");
            var feat3 = featCollections.All.First(f => f.Name == "feat3");

            Assert.That(feat1.Name, Is.EqualTo("feat1"));
            Assert.That(feat1.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
            Assert.That(feat2.Name, Is.EqualTo("feat2"));
            Assert.That(feat2.Foci, Is.Empty);
            Assert.That(feat3.Name, Is.EqualTo("feat3"));
            Assert.That(feat3.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
            Assert.That(featCollections.All.Count(), Is.EqualTo(3));
        }

        [Test]
        public void IfMultipleFeatWithFocusOfAllAreDuplicated_KeepOneOfEach()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[2].Name = "feat2";
            racialFeats[2].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[3].Name = "feat2";
            racialFeats[3].Foci = new[] { FeatConstants.Foci.All };

            var feat1 = featCollections.All.First(f => f.Name == "feat1");
            var feat2 = featCollections.All.First(f => f.Name == "feat2");

            Assert.That(feat1.Name, Is.EqualTo("feat1"));
            Assert.That(feat1.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
            Assert.That(feat2.Name, Is.EqualTo("feat2"));
            Assert.That(feat2.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNameAndPowerMatchAndNoFrequency_CombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 9266;

            var onlyFeat = featCollections.All.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo("feat1"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 2"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 3"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 4"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(4));
            Assert.That(onlyFeat.Power, Is.EqualTo(9266));
        }

        [Test]
        public void DoNotCombineFociIfFociIntersect()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 3", "focus 1" };
            racialFeats[1].Power = 9266;

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first, Is.Not.EqualTo(last));

            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Foci, Contains.Item("focus 2"));
            Assert.That(first.Foci, Contains.Item("focus 1"));
            Assert.That(first.Foci.Count(), Is.EqualTo(2));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Foci, Contains.Item("focus 1"));
            Assert.That(last.Foci, Contains.Item("focus 3"));
            Assert.That(last.Foci.Count(), Is.EqualTo(2));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotCombineFociIfOneContainsAll()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].CanBeTakenMultipleTimes = true;
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].CanBeTakenMultipleTimes = true;
            racialFeats[1].Foci = new[] { FeatConstants.Foci.All };
            racialFeats[1].Power = 9266;

            var first = featCollections.All.First();
            var last = featCollections.All.Last();

            Assert.That(first, Is.Not.EqualTo(last));

            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Foci, Contains.Item("focus 2"));
            Assert.That(first.Foci, Contains.Item("focus 1"));
            Assert.That(first.Foci.Count(), Is.EqualTo(2));
            Assert.That(first.Power, Is.EqualTo(9266));

            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Foci, Contains.Item(FeatConstants.Foci.All));
            Assert.That(last.Foci.Count(), Is.EqualTo(1));
            Assert.That(last.Power, Is.EqualTo(9266));

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));
        }

        [Test]
        public void CombineFociIfFeatCanBeTakenMultipleTimes()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].CanBeTakenMultipleTimes = true;
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].CanBeTakenMultipleTimes = true;
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 9266;

            var onlyFeat = featCollections.All.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo("feat1"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 2"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 3"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 4"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(4));
            Assert.That(onlyFeat.Power, Is.EqualTo(9266));
        }

        [Test]
        public void IfNamesDoNotMatch_DoNotCombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat2";
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 9266;

            var firstFeat = featCollections.All.First();
            var lastFeat = featCollections.All.Last();

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 2"));
            Assert.That(firstFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(firstFeat.Power, Is.EqualTo(9266));

            Assert.That(lastFeat.Name, Is.EqualTo("feat2"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 3"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 4"));
            Assert.That(lastFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(lastFeat.Power, Is.EqualTo(9266));
        }

        [Test]
        public void IfPowersDoNotMatch_DoNotCombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 90210;

            var firstFeat = featCollections.All.First();
            var lastFeat = featCollections.All.Last();

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 2"));
            Assert.That(firstFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(firstFeat.Power, Is.EqualTo(9266));

            Assert.That(lastFeat.Name, Is.EqualTo("feat1"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 3"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 4"));
            Assert.That(lastFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(lastFeat.Power, Is.EqualTo(90210));
        }

        [Test]
        public void IfFeatHasFrequency_DoNotCombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.TimePeriod = "time period";
            racialFeats[1].Frequency.Quantity = 42;

            var firstFeat = featCollections.All.First();
            var lastFeat = featCollections.All.Last();

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 2"));
            Assert.That(firstFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(firstFeat.Power, Is.EqualTo(9266));
            Assert.That(firstFeat.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(firstFeat.Frequency.TimePeriod, Is.Empty);

            Assert.That(lastFeat.Name, Is.EqualTo("feat1"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 3"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 4"));
            Assert.That(lastFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(lastFeat.Power, Is.EqualTo(9266));
            Assert.That(lastFeat.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(lastFeat.Frequency.TimePeriod, Is.EqualTo("time period"));
        }

        [Test]
        public void IfBothFeatHaveMatchingFrequencies_DoNotCombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { "focus 1", "focus 2" };
            racialFeats[0].Power = 9266;
            racialFeats[0].Frequency.TimePeriod = "time period";
            racialFeats[0].Frequency.Quantity = 42;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 3", "focus 4" };
            racialFeats[1].Power = 9266;
            racialFeats[1].Frequency.TimePeriod = "time period";
            racialFeats[1].Frequency.Quantity = 42;

            var firstFeat = featCollections.All.First();
            var lastFeat = featCollections.All.Last();

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 2"));
            Assert.That(firstFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(firstFeat.Power, Is.EqualTo(9266));
            Assert.That(firstFeat.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(firstFeat.Frequency.TimePeriod, Is.EqualTo("time period"));

            Assert.That(lastFeat.Name, Is.EqualTo("feat1"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 3"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 4"));
            Assert.That(lastFeat.Foci.Count(), Is.EqualTo(2));
            Assert.That(lastFeat.Power, Is.EqualTo(9266));
            Assert.That(lastFeat.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(lastFeat.Frequency.TimePeriod, Is.EqualTo("time period"));
        }

        [Test]
        public void DoNotCombineFociIfNoFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat1";
            racialFeats[1].Foci = new[] { "focus 1" };
            racialFeats[1].Power = 9266;
            racialFeats[2].Name = "feat2";
            racialFeats[2].Power = 9266;
            racialFeats[2].Foci = new[] { "focus 2" };
            racialFeats[3].Name = "feat2";
            racialFeats[3].Power = 9266;

            var firstFeat = featCollections.All.ElementAt(0);
            var secondFeat = featCollections.All.ElementAt(1);
            var thirdFeat = featCollections.All.ElementAt(2);
            var fourthFeat = featCollections.All.ElementAt(3);

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci, Is.Empty);
            Assert.That(firstFeat.Power, Is.EqualTo(9266));

            Assert.That(secondFeat.Name, Is.EqualTo("feat1"));
            Assert.That(secondFeat.Foci.Single(), Is.EqualTo("focus 1"));
            Assert.That(secondFeat.Power, Is.EqualTo(9266));

            Assert.That(thirdFeat.Name, Is.EqualTo("feat2"));
            Assert.That(thirdFeat.Foci.Single(), Is.EqualTo("focus 2"));
            Assert.That(thirdFeat.Power, Is.EqualTo(9266));

            Assert.That(fourthFeat.Name, Is.EqualTo("feat2"));
            Assert.That(fourthFeat.Foci, Is.Empty);
            Assert.That(fourthFeat.Power, Is.EqualTo(9266));
        }

        [Test]
        public void CanOnlyBeFocusedInAll()
        {
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].Foci = new[] { FeatConstants.Foci.All, "focus 2" };

            var onlyFeat = featCollections.All.Single();
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
        }

        [Test]
        public void CombineFociOfMultipleFeatOfDifferentPowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat1";
            racialFeats[0].CanBeTakenMultipleTimes = true;
            racialFeats[0].Foci = new[] { "focus 1" };
            racialFeats[0].Power = 2;
            racialFeats[1].Name = "feat1";
            racialFeats[1].CanBeTakenMultipleTimes = true;
            racialFeats[1].Foci = new[] { "focus 2" };
            racialFeats[1].Power = 2;
            racialFeats[2].Name = "feat1";
            racialFeats[2].CanBeTakenMultipleTimes = true;
            racialFeats[2].Foci = new[] { "focus 1" };
            racialFeats[2].Power = 8;
            racialFeats[3].Name = "feat1";
            racialFeats[3].CanBeTakenMultipleTimes = true;
            racialFeats[3].Foci = new[] { "focus 2" };
            racialFeats[3].Power = 8;

            var firstFeat = featCollections.All.First();
            var lastFeat = featCollections.All.Last();

            Assert.That(featCollections.All.Count(), Is.EqualTo(2));

            Assert.That(firstFeat.Power, Is.EqualTo(2));
            Assert.That(firstFeat.Foci, Contains.Item("focus 1"));
            Assert.That(firstFeat.Foci, Contains.Item("focus 2"));
            Assert.That(firstFeat.Foci.Count(), Is.EqualTo(2));

            Assert.That(lastFeat.Power, Is.EqualTo(8));
            Assert.That(lastFeat.Foci, Contains.Item("focus 1"));
            Assert.That(lastFeat.Foci, Contains.Item("focus 2"));
            Assert.That(lastFeat.Foci.Count(), Is.EqualTo(2));
        }
    }
}
