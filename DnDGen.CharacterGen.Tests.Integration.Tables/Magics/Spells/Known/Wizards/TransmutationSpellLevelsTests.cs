using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class TransmutationSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Transmutation);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0",
            SpellConstants.MageHand,
            SpellConstants.Mending,
            SpellConstants.Message,
            SpellConstants.OpenClose)]
        [TestCase("1",
            SpellConstants.AnimateRope,
            SpellConstants.EnlargePerson,
            SpellConstants.Erase,
            SpellConstants.ExpeditiousRetreat,
            SpellConstants.FeatherFall,
            SpellConstants.Jump,
            SpellConstants.MagicWeapon,
            SpellConstants.ReducePerson)]
        [TestCase("2",
            SpellConstants.AlterSelf,
            SpellConstants.BearsEndurance,
            SpellConstants.BullsStrength,
            SpellConstants.CatsGrace,
            SpellConstants.Darkvision,
            SpellConstants.EaglesSplendor,
            SpellConstants.FoxsCunning,
            SpellConstants.Knock,
            SpellConstants.Levitate,
            SpellConstants.OwlsWisdom,
            SpellConstants.Pyrotechnics,
            SpellConstants.RopeTrick,
            SpellConstants.SpiderClimb,
            SpellConstants.WhisperingWind)]
        [TestCase("3",
            SpellConstants.Blink,
            SpellConstants.FlameArrow,
            SpellConstants.Fly,
            SpellConstants.GaseousForm,
            SpellConstants.Haste,
            SpellConstants.KeenEdge,
            SpellConstants.MagicWeapon_Greater,
            SpellConstants.SecretPage,
            SpellConstants.ShrinkItem,
            SpellConstants.Slow,
            SpellConstants.WaterBreathing)]
        [TestCase("4",
            SpellConstants.EnlargePerson_Mass,
            SpellConstants.RarysMnemonicEnhancer,
            SpellConstants.Polymorph,
            SpellConstants.ReducePerson_Mass,
            SpellConstants.StoneShape)]
        [TestCase("5",
            SpellConstants.AnimalGrowth,
            SpellConstants.Fabricate,
            SpellConstants.OverlandFlight,
            SpellConstants.Passwall,
            SpellConstants.Telekinesis,
            SpellConstants.TransmuteMudToRock,
            SpellConstants.TransmuteRockToMud)]
        [TestCase("6",
            SpellConstants.BearsEndurance_Mass,
            SpellConstants.BullsStrength_Mass,
            SpellConstants.CatsGrace_Mass,
            SpellConstants.ControlWater,
            SpellConstants.Disintegrate,
            SpellConstants.EaglesSplendor_Mass,
            SpellConstants.FleshToStone,
            SpellConstants.FoxsCunning_Mass,
            SpellConstants.MordenkainensLucubration,
            SpellConstants.MoveEarth,
            SpellConstants.OwlsWisdom_Mass,
            SpellConstants.StoneToFlesh,
            SpellConstants.TensersTransformation)]
        [TestCase("7",
            SpellConstants.ControlWeather,
            SpellConstants.EtherealJaunt,
            SpellConstants.ReverseGravity,
            SpellConstants.Statue)]
        [TestCase("8",
            SpellConstants.IronBody,
            SpellConstants.PolymorphAnyObject,
            SpellConstants.TemporalStasis)]
        [TestCase("9",
            SpellConstants.Etherealness,
            SpellConstants.Shapechange,
            SpellConstants.TimeStop)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
