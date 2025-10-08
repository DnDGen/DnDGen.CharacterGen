using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class NecromancySpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Necromancy);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0",
            SpellConstants.DisruptUndead,
            SpellConstants.TouchOfFatigue)]
        [TestCase("1",
            SpellConstants.CauseFear,
            SpellConstants.ChillTouch,
            SpellConstants.RayOfEnfeeblement)]
        [TestCase("2",
            SpellConstants.BlindnessDeafness,
            SpellConstants.CommandUndead,
            SpellConstants.FalseLife,
            SpellConstants.GhoulTouch,
            SpellConstants.Scare,
            SpellConstants.SpectralHand)]
        [TestCase("3",
            SpellConstants.GentleRepose,
            SpellConstants.HaltUndead,
            SpellConstants.RayOfExhaustion,
            SpellConstants.VampiricTouch)]
        [TestCase("4",
            SpellConstants.AnimateDead,
            SpellConstants.BestowCurse,
            SpellConstants.Contagion,
            SpellConstants.Enervation,
            SpellConstants.Fear)]
        [TestCase("5",
            SpellConstants.Blight,
            SpellConstants.MagicJar,
            SpellConstants.SymbolOfPain,
            SpellConstants.WavesOfFatigue)]
        [TestCase("6",
            SpellConstants.CircleOfDeath,
            SpellConstants.CreateUndead,
            SpellConstants.Eyebite,
            SpellConstants.SymbolOfFear,
            SpellConstants.UndeathToDeath)]
        [TestCase("7",
            SpellConstants.ControlUndead,
            SpellConstants.FingerOfDeath,
            SpellConstants.SymbolOfWeakness,
            SpellConstants.WavesOfExhaustion)]
        [TestCase("8",
            SpellConstants.Clone,
            SpellConstants.CreateGreaterUndead,
            SpellConstants.HorridWilting,
            SpellConstants.SymbolOfDeath)]
        [TestCase("9",
            SpellConstants.AstralProjection,
            SpellConstants.EnergyDrain,
            SpellConstants.SoulBind,
            SpellConstants.WailOfTheBanshee)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
