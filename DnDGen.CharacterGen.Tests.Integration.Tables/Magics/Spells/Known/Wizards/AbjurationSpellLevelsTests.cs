using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class AbjurationSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Abjuration);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0", SpellConstants.Resistance)]
        [TestCase("1",
            SpellConstants.Alarm,
            SpellConstants.EndureElements,
            SpellConstants.HoldPortal,
            SpellConstants.ProtectionFromChaos,
            SpellConstants.ProtectionFromEvil,
            SpellConstants.ProtectionFromGood,
            SpellConstants.ProtectionFromLaw,
            SpellConstants.Shield)]
        [TestCase("2",
            SpellConstants.ArcaneLock,
            SpellConstants.ObscureObject,
            SpellConstants.ProtectionFromArrows,
            SpellConstants.ResistEnergy)]
        [TestCase("3",
            SpellConstants.DispelMagic,
            SpellConstants.ExplosiveRunes,
            SpellConstants.MagicCircleAgainstChaos,
            SpellConstants.MagicCircleAgainstEvil,
            SpellConstants.MagicCircleAgainstGood,
            SpellConstants.MagicCircleAgainstLaw,
            SpellConstants.Nondetection,
            SpellConstants.ProtectionFromEnergy)]
        [TestCase("4",
            SpellConstants.DimensionalAnchor,
            SpellConstants.FireTrap,
            SpellConstants.GlobeOfInvulnerability_Lesser,
            SpellConstants.RemoveCurse,
            SpellConstants.Stoneskin)]
        [TestCase("5",
            SpellConstants.BreakEnchantment,
            SpellConstants.Dismissal,
            SpellConstants.MordenkainensPrivateSanctum)]
        [TestCase("6",
            SpellConstants.AntimagicField,
            SpellConstants.DispelMagic_Greater,
            SpellConstants.GlobeOfInvulnerability,
            SpellConstants.GuardsAndWards,
            SpellConstants.Repulsion)]
        [TestCase("7",
            SpellConstants.Banishment,
            SpellConstants.Sequester,
            SpellConstants.SpellTurning)]
        [TestCase("8",
            SpellConstants.DimensionalLock,
            SpellConstants.MindBlank,
            SpellConstants.PrismaticWall,
            SpellConstants.ProtectionFromSpells)]
        [TestCase("9",
            SpellConstants.Freedom,
            SpellConstants.Imprisonment,
            SpellConstants.MordenkainensDisjunction,
            SpellConstants.PrismaticSphere)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
