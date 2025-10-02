using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class EvocationSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Evocation);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0",
            SpellConstants.DancingLights,
            SpellConstants.Flare,
            SpellConstants.Light,
            SpellConstants.RayOfFrost)]
        [TestCase("1",
            SpellConstants.BurningHands,
            SpellConstants.TensersFloatingDisk,
            SpellConstants.MagicMissile,
            SpellConstants.ShockingGrasp)]
        [TestCase("2",
            SpellConstants.ContinualFlame,
            SpellConstants.Darkness,
            SpellConstants.FlamingSphere,
            SpellConstants.GustOfWind,
            SpellConstants.ScorchingRay,
            SpellConstants.Shatter)]
        [TestCase("3",
            SpellConstants.Daylight,
            SpellConstants.Fireball,
            SpellConstants.LightningBolt,
            SpellConstants.LeomundsTinyHut,
            SpellConstants.WindWall)]
        [TestCase("4",
            SpellConstants.FireShield,
            SpellConstants.IceStorm,
            SpellConstants.OtilukesResilientSphere,
            SpellConstants.Shout,
            SpellConstants.WallOfFire,
            SpellConstants.WallOfIce)]
        [TestCase("5",
            SpellConstants.ConeOfCold,
            SpellConstants.BigbysInterposingHand,
            SpellConstants.Sending,
            SpellConstants.WallOfForce)]
        [TestCase("6",
            SpellConstants.ChainLightning,
            SpellConstants.Contingency,
            SpellConstants.BigbysForcefulHand,
            SpellConstants.OtilukesFreezingSphere)]
        [TestCase("7",
            SpellConstants.DelayedBlastFireball,
            SpellConstants.Forcecage,
            SpellConstants.BigbysGraspingHand,
            SpellConstants.MordenkainensSword,
            SpellConstants.PrismaticSpray)]
        [TestCase("8",
            SpellConstants.BigbysClenchedFist,
            SpellConstants.PolarRay,
            SpellConstants.Shout_Greater,
            SpellConstants.Sunburst,
            SpellConstants.OtilukesTelekineticSphere)]
        [TestCase("9",
            SpellConstants.BigbysCrushingHand,
            SpellConstants.MeteorSwarm)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
