using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Rangers
{
    [TestFixture]
    public class RangerSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Ranger);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(1, 4).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("1",
            SpellConstants.Alarm,
            SpellConstants.AnimalMessenger,
            SpellConstants.CalmAnimals,
            SpellConstants.CharmAnimal,
            SpellConstants.DelayPoison,
            SpellConstants.DetectAnimalsOrPlants,
            SpellConstants.DetectPoison,
            SpellConstants.DetectSnaresAndPits,
            SpellConstants.EndureElements,
            SpellConstants.Entangle,
            SpellConstants.HideFromAnimals,
            SpellConstants.Jump,
            SpellConstants.Longstrider,
            SpellConstants.MagicFang,
            SpellConstants.PassWithoutTrace,
            SpellConstants.ReadMagic,
            SpellConstants.ResistEnergy,
            SpellConstants.SpeakWithAnimals,
            SpellConstants.SummonNaturesAllyI)]
        [TestCase("2",
            SpellConstants.Barkskin,
            SpellConstants.BearsEndurance,
            SpellConstants.CatsGrace,
            SpellConstants.CureLightWounds,
            SpellConstants.HoldAnimal,
            SpellConstants.OwlsWisdom,
            SpellConstants.ProtectionFromEnergy,
            SpellConstants.Snare,
            SpellConstants.SpeakWithPlants,
            SpellConstants.SpikeGrowth,
            SpellConstants.SummonNaturesAllyII,
            SpellConstants.WindWall)]
        [TestCase("3",
            SpellConstants.CommandPlants,
            SpellConstants.CureModerateWounds,
            SpellConstants.Darkvision,
            SpellConstants.DiminishPlants,
            SpellConstants.MagicFang_Greater,
            SpellConstants.NeutralizePoison,
            SpellConstants.PlantGrowth,
            SpellConstants.ReduceAnimal,
            SpellConstants.RemoveDisease,
            SpellConstants.RepelVermin,
            SpellConstants.SummonNaturesAllyIII,
            SpellConstants.TreeShape,
            SpellConstants.WaterWalk)]
        [TestCase("4",
            SpellConstants.AnimalGrowth,
            SpellConstants.CommuneWithNature,
            SpellConstants.CureSeriousWounds,
            SpellConstants.FreedomOfMovement,
            SpellConstants.Nondetection,
            SpellConstants.SummonNaturesAllyIV,
            SpellConstants.TreeStride)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
