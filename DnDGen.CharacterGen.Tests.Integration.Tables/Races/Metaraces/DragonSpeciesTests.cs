using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Races;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races.Metaraces
{
    [TestFixture]
    public class DragonSpeciesTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.DragonSpecies; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                AlignmentConstants.LawfulGood,
                AlignmentConstants.NeutralGood,
                AlignmentConstants.ChaoticGood,
                AlignmentConstants.LawfulEvil,
                AlignmentConstants.NeutralEvil,
                AlignmentConstants.ChaoticEvil
            };

            AssertCollectionNames(names);
        }

        [TestCase(AlignmentConstants.LawfulGood,
            RaceConstants.Metaraces.Species.Bronze,
            RaceConstants.Metaraces.Species.Gold,
            RaceConstants.Metaraces.Species.Silver)]
        [TestCase(AlignmentConstants.NeutralGood,
            RaceConstants.Metaraces.Species.Bronze,
            RaceConstants.Metaraces.Species.Gold,
            RaceConstants.Metaraces.Species.Silver,
            RaceConstants.Metaraces.Species.Brass,
            RaceConstants.Metaraces.Species.Copper)]
        [TestCase(AlignmentConstants.ChaoticGood,
            RaceConstants.Metaraces.Species.Brass,
            RaceConstants.Metaraces.Species.Copper)]
        [TestCase(AlignmentConstants.LawfulEvil,
            RaceConstants.Metaraces.Species.Blue,
            RaceConstants.Metaraces.Species.Green)]
        [TestCase(AlignmentConstants.NeutralEvil,
            RaceConstants.Metaraces.Species.Blue,
            RaceConstants.Metaraces.Species.Green,
            RaceConstants.Metaraces.Species.Black,
            RaceConstants.Metaraces.Species.Red,
            RaceConstants.Metaraces.Species.White)]
        [TestCase(AlignmentConstants.ChaoticEvil,
            RaceConstants.Metaraces.Species.Black,
            RaceConstants.Metaraces.Species.Red,
            RaceConstants.Metaraces.Species.White)]
        public override void AssertDistinctCollection(string name, params string[] collection)
        {
            base.AssertDistinctCollection(name, collection);
        }
    }
}