using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Paladins
{
    [TestFixture]
    public class PaladinSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Paladin);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(1, 4).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("1",
            SpellConstants.Bless,
            SpellConstants.BlessWater,
            SpellConstants.BlessWeapon,
            SpellConstants.CreateWater,
            SpellConstants.CureLightWounds,
            SpellConstants.DetectPoison,
            SpellConstants.DetectUndead,
            SpellConstants.DivineFavor,
            SpellConstants.EndureElements,
            SpellConstants.MagicWeapon,
            SpellConstants.ProtectionFromChaos,
            SpellConstants.ProtectionFromEvil,
            SpellConstants.ReadMagic,
            SpellConstants.Resistance,
            SpellConstants.Restoration_Lesser,
            SpellConstants.Virtue)]
        [TestCase("2",
            SpellConstants.BullsStrength,
            SpellConstants.DelayPoison,
            SpellConstants.EaglesSplendor,
            SpellConstants.OwlsWisdom,
            SpellConstants.RemoveParalysis,
            SpellConstants.ResistEnergy,
            SpellConstants.ShieldOther,
            SpellConstants.UndetectableAlignment,
            SpellConstants.ZoneOfTruth)]
        [TestCase("3",
            SpellConstants.CureModerateWounds,
            SpellConstants.Daylight,
            SpellConstants.DiscernLies,
            SpellConstants.DispelMagic,
            SpellConstants.HealMount,
            SpellConstants.MagicCircleAgainstChaos,
            SpellConstants.MagicCircleAgainstEvil,
            SpellConstants.MagicWeapon_Greater,
            SpellConstants.Prayer,
            SpellConstants.RemoveBlindnessDeafness,
            SpellConstants.RemoveCurse)]
        [TestCase("4",
            SpellConstants.BreakEnchantment,
            SpellConstants.CureSeriousWounds,
            SpellConstants.DeathWard,
            SpellConstants.DispelChaos,
            SpellConstants.DispelEvil,
            SpellConstants.HolySword,
            SpellConstants.MarkOfJustice,
            SpellConstants.NeutralizePoison,
            SpellConstants.Restoration)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
