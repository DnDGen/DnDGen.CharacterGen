using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Wizards
{
    [TestFixture]
    public class IllusionSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Schools.Illusion);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0", SpellConstants.GhostSound)]
        [TestCase("1",
            SpellConstants.ColorSpray,
            SpellConstants.DisguiseSelf,
            SpellConstants.NystulsMagicAura,
            SpellConstants.SilentImage,
            SpellConstants.Ventriloquism)]
        [TestCase("2",
            SpellConstants.Blur,
            SpellConstants.HypnoticPattern,
            SpellConstants.Invisibility,
            SpellConstants.MagicMouth,
            SpellConstants.MinorImage,
            SpellConstants.MirrorImage,
            SpellConstants.Misdirection,
            SpellConstants.LeomundsTrap)]
        [TestCase("3",
            SpellConstants.Displacement,
            SpellConstants.IllusoryScript,
            SpellConstants.InvisibilitySphere,
            SpellConstants.MajorImage)]
        [TestCase("4",
            SpellConstants.HallucinatoryTerrain,
            SpellConstants.IllusoryWall,
            SpellConstants.Invisibility_Greater,
            SpellConstants.PhantasmalKiller,
            SpellConstants.RainbowPattern,
            SpellConstants.ShadowConjuration)]
        [TestCase("5",
            SpellConstants.Dream,
            SpellConstants.FalseVision,
            SpellConstants.MirageArcana,
            SpellConstants.Nightmare,
            SpellConstants.PersistentImage,
            SpellConstants.Seeming,
            SpellConstants.ShadowEvocation)]
        [TestCase("6",
            SpellConstants.Mislead,
            SpellConstants.PermanentImage,
            SpellConstants.ProgrammedImage,
            SpellConstants.ShadowWalk,
            SpellConstants.Veil)]
        [TestCase("7",
            SpellConstants.Invisibility_Mass,
            SpellConstants.ProjectImage,
            SpellConstants.ShadowConjuration_Greater,
            SpellConstants.Simulacrum)]
        [TestCase("8",
            SpellConstants.ScintillatingPattern,
            SpellConstants.Screen,
            SpellConstants.ShadowEvocation_Greater)]
        [TestCase("9",
            SpellConstants.Shades,
            SpellConstants.Weird)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
