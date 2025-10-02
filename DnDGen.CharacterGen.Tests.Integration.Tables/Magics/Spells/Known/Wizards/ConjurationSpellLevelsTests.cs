using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class ConjurationSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Conjuration);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0", SpellConstants.AcidSplash)]
        [TestCase("1",
            SpellConstants.Grease,
            SpellConstants.MageArmor,
            SpellConstants.Mount,
            SpellConstants.ObscuringMist,
            SpellConstants.SummonMonsterI,
            SpellConstants.UnseenServant)]
        [TestCase("2",
            SpellConstants.MelfsAcidArrow,
            SpellConstants.FogCloud,
            SpellConstants.Glitterdust,
            SpellConstants.SummonMonsterII,
            SpellConstants.SummonSwarm,
            SpellConstants.Web)]
        [TestCase("3",
            SpellConstants.PhantomSteed,
            SpellConstants.SepiaSnakeSigil,
            SpellConstants.SleetStorm,
            SpellConstants.StinkingCloud,
            SpellConstants.SummonMonsterIII)]
        [TestCase("4",
            SpellConstants.EvardsBlackTentacles,
            SpellConstants.DimensionDoor,
            SpellConstants.MinorCreation,
            SpellConstants.LeomundsSecureShelter,
            SpellConstants.SolidFog,
            SpellConstants.SummonMonsterIV)]
        [TestCase("5",
            SpellConstants.Cloudkill,
            SpellConstants.MordenkainensFaithfulHound,
            SpellConstants.MajorCreation,
            SpellConstants.PlanarBinding_Lesser,
            SpellConstants.LeomundsSecretChest,
            SpellConstants.SummonMonsterV,
            SpellConstants.Teleport,
            SpellConstants.WallOfStone)]
        [TestCase("6",
            SpellConstants.AcidFog,
            SpellConstants.PlanarBinding,
            SpellConstants.SummonMonsterVI,
            SpellConstants.WallOfIron)]
        [TestCase("7",
            SpellConstants.DrawmijsInstantSummons,
            SpellConstants.MordenkainensMagnificentMansion,
            SpellConstants.PhaseDoor,
            SpellConstants.PlaneShift,
            SpellConstants.SummonMonsterVII,
            SpellConstants.Teleport_Greater,
            SpellConstants.TeleportObject)]
        [TestCase("8",
            SpellConstants.IncendiaryCloud,
            SpellConstants.Maze,
            SpellConstants.PlanarBinding_Greater,
            SpellConstants.SummonMonsterVIII,
            SpellConstants.TrapTheSoul)]
        [TestCase("9",
            SpellConstants.Gate,
            SpellConstants.Refuge,
            SpellConstants.SummonMonsterIX,
            SpellConstants.TeleportationCircle)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
