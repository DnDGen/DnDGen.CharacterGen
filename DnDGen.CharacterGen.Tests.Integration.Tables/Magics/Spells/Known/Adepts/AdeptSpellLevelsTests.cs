using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Adepts
{
    [TestFixture]
    public class AdeptSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Adept);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 6).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0",
            SpellConstants.CreateWater,
            SpellConstants.CureMinorWounds,
            SpellConstants.DetectMagic,
            SpellConstants.GhostSound,
            SpellConstants.Guidance,
            SpellConstants.Light,
            SpellConstants.Mending,
            SpellConstants.PurifyFoodAndDrink,
            SpellConstants.ReadMagic,
            SpellConstants.TouchOfFatigue)]
        [TestCase("1",
            SpellConstants.Bless,
            SpellConstants.BurningHands,
            SpellConstants.CauseFear,
            SpellConstants.Command,
            SpellConstants.ComprehendLanguages,
            SpellConstants.CureLightWounds,
            SpellConstants.DetectChaos,
            SpellConstants.DetectEvil,
            SpellConstants.DetectGood,
            SpellConstants.DetectLaw,
            SpellConstants.EndureElements,
            SpellConstants.ObscuringMist,
            SpellConstants.ProtectionFromChaos,
            SpellConstants.ProtectionFromEvil,
            SpellConstants.ProtectionFromGood,
            SpellConstants.ProtectionFromLaw,
            SpellConstants.Sleep)]
        [TestCase("2",
            SpellConstants.Aid,
            SpellConstants.AnimalTrance,
            SpellConstants.BearsEndurance,
            SpellConstants.BullsStrength,
            SpellConstants.CatsGrace,
            SpellConstants.CureModerateWounds,
            SpellConstants.Darkness,
            SpellConstants.DelayPoison,
            SpellConstants.Invisibility,
            SpellConstants.MirrorImage,
            SpellConstants.ResistEnergy,
            SpellConstants.ScorchingRay,
            SpellConstants.SeeInvisibility,
            SpellConstants.Web)]
        [TestCase("3",
            SpellConstants.AnimateDead,
            SpellConstants.BestowCurse,
            SpellConstants.Contagion,
            SpellConstants.ContinualFlame,
            SpellConstants.CureSeriousWounds,
            SpellConstants.Daylight,
            SpellConstants.DeeperDarkness,
            SpellConstants.LightningBolt,
            SpellConstants.NeutralizePoison,
            SpellConstants.RemoveCurse,
            SpellConstants.RemoveDisease,
            SpellConstants.Tongues)]
        [TestCase("4",
            SpellConstants.CureCriticalWounds,
            SpellConstants.MinorCreation,
            SpellConstants.Polymorph,
            SpellConstants.Restoration,
            SpellConstants.Stoneskin,
            SpellConstants.WallOfFire)]
        [TestCase("5",
            SpellConstants.BalefulPolymorph,
            SpellConstants.BreakEnchantment,
            SpellConstants.Commune,
            SpellConstants.Heal,
            SpellConstants.MajorCreation,
            SpellConstants.RaiseDead,
            SpellConstants.TrueSeeing,
            SpellConstants.WallOfStone)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
