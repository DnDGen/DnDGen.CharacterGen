using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class DivinationSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Divination);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0",
            SpellConstants.DetectPoison,
            SpellConstants.DetectMagic,
            SpellConstants.ReadMagic)]
        [TestCase("1",
            SpellConstants.ComprehendLanguages,
            SpellConstants.DetectSecretDoors,
            SpellConstants.DetectUndead,
            SpellConstants.Identify,
            SpellConstants.TrueStrike)]
        [TestCase("2",
            SpellConstants.DetectThoughts,
            SpellConstants.LocateObject,
            SpellConstants.SeeInvisibility)]
        [TestCase("3",
            SpellConstants.ArcaneSight,
            SpellConstants.ClairaudienceClairvoyance,
            SpellConstants.Tongues)]
        [TestCase("4",
            SpellConstants.ArcaneEye,
            SpellConstants.DetectScrying,
            SpellConstants.LocateCreature,
            SpellConstants.Scrying)]
        [TestCase("5",
            SpellConstants.ContactOtherPlane,
            SpellConstants.PryingEyes,
            SpellConstants.RarysTelepathicBond)]
        [TestCase("6",
            SpellConstants.AnalyzeDweomer,
            SpellConstants.LegendLore,
            SpellConstants.TrueSeeing)]
        [TestCase("7",
            SpellConstants.ArcaneSight_Greater,
            SpellConstants.Scrying_Greater,
            SpellConstants.Vision)]
        [TestCase("8",
            SpellConstants.DiscernLocation,
            SpellConstants.MomentOfPrescience,
            SpellConstants.PryingEyes_Greater)]
        [TestCase("9", SpellConstants.Foresight)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
