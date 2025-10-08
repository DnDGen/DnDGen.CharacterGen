using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class EnchantmentSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Enchantment);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0", SpellConstants.Daze)]
        [TestCase("1",
            SpellConstants.CharmPerson,
            SpellConstants.Hypnotism,
            SpellConstants.Sleep)]
        [TestCase("2",
            SpellConstants.DazeMonster,
            SpellConstants.TashasHideousLaughter,
            SpellConstants.TouchOfIdiocy)]
        [TestCase("3",
            SpellConstants.DeepSlumber,
            SpellConstants.Heroism,
            SpellConstants.HoldPerson,
            SpellConstants.Rage,
            SpellConstants.Suggestion)]
        [TestCase("4",
            SpellConstants.CharmMonster,
            SpellConstants.Confusion,
            SpellConstants.CrushingDespair,
            SpellConstants.Geas_Lesser)]
        [TestCase("5",
            SpellConstants.DominatePerson,
            SpellConstants.Feeblemind,
            SpellConstants.HoldMonster,
            SpellConstants.MindFog,
            SpellConstants.SymbolOfSleep)]
        [TestCase("6",
            SpellConstants.GeasQuest,
            SpellConstants.Heroism_Greater,
            SpellConstants.Suggestion_Mass,
            SpellConstants.SymbolOfPersuasion)]
        [TestCase("7",
            SpellConstants.HoldPerson_Mass,
            SpellConstants.Insanity,
            SpellConstants.PowerWordBlind,
            SpellConstants.SymbolOfStunning)]
        [TestCase("8",
            SpellConstants.Antipathy,
            SpellConstants.Binding,
            SpellConstants.CharmMonster_Mass,
            SpellConstants.Demand,
            SpellConstants.OttosIrresistibleDance,
            SpellConstants.PowerWordStun,
            SpellConstants.SymbolOfInsanity,
            SpellConstants.Sympathy)]
        [TestCase("9",
            SpellConstants.DominateMonster,
            SpellConstants.HoldMonster_Mass,
            SpellConstants.PowerWordKill)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
