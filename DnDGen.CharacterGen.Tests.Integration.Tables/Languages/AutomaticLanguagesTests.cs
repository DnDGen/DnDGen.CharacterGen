using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Languages;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Languages
{
    [TestFixture]
    public class AutomaticLanguagesTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.AutomaticLanguages; }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var metaraceGroups = GetTable(TableNameConstants.Set.Collection.MetaraceGroups);
            var classes = GetTable(TableNameConstants.Set.Collection.ClassNameGroups);

            var names = classes[GroupConstants.All].Union(baseRaceGroups[GroupConstants.All]).Union(metaraceGroups[GroupConstants.All]);

            AssertCollectionNames(names);
        }

        [TestCase(CharacterClassConstants.Adept)]
        [TestCase(CharacterClassConstants.Aristocrat)]
        [TestCase(CharacterClassConstants.Barbarian)]
        [TestCase(CharacterClassConstants.Bard)]
        [TestCase(CharacterClassConstants.Cleric)]
        [TestCase(CharacterClassConstants.Commoner)]
        [TestCase(CharacterClassConstants.Druid, LanguageConstants.Special.Druidic)]
        [TestCase(CharacterClassConstants.Expert)]
        [TestCase(CharacterClassConstants.Fighter)]
        [TestCase(CharacterClassConstants.Monk)]
        [TestCase(CharacterClassConstants.Paladin)]
        [TestCase(CharacterClassConstants.Ranger)]
        [TestCase(CharacterClassConstants.Rogue)]
        [TestCase(CharacterClassConstants.Sorcerer)]
        [TestCase(CharacterClassConstants.Warrior)]
        [TestCase(CharacterClassConstants.Wizard)]
        [TestCase(RaceConstants.BaseRaces.Aasimar,
            LanguageConstants.Common,
            LanguageConstants.Celestial)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.Azer,
            LanguageConstants.Common,
            LanguageConstants.Ignan)]
        [TestCase(RaceConstants.BaseRaces.BlueSlaad, LanguageConstants.Special.Slaad)]
        [TestCase(RaceConstants.BaseRaces.Bugbear,
            LanguageConstants.Common,
            LanguageConstants.Goblin)]
        [TestCase(RaceConstants.BaseRaces.Centaur,
            LanguageConstants.Sylvan,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.CloudGiant,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad,
            LanguageConstants.Special.Slaad,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf,
            LanguageConstants.Common,
            LanguageConstants.Dwarven)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling,
            LanguageConstants.Common,
            LanguageConstants.Halfling,
            LanguageConstants.Dwarven)]
        [TestCase(RaceConstants.BaseRaces.Derro,
            LanguageConstants.Common,
            LanguageConstants.Dwarven)]
        [TestCase(RaceConstants.BaseRaces.Doppelganger, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Drow,
            LanguageConstants.Common,
            LanguageConstants.Elven,
            LanguageConstants.Undercommon)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf,
            LanguageConstants.Common,
            LanguageConstants.Dwarven,
            LanguageConstants.Undercommon)]
        [TestCase(RaceConstants.BaseRaces.FireGiant,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome,
            LanguageConstants.Common,
            LanguageConstants.Elven,
            LanguageConstants.Gnome,
            LanguageConstants.Sylvan)]
        [TestCase(RaceConstants.BaseRaces.FrostGiant,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.Gargoyle, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, LanguageConstants.Special.Githyanki)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, LanguageConstants.Special.Githzerai)]
        [TestCase(RaceConstants.BaseRaces.Gnoll, LanguageConstants.Gnoll)]
        [TestCase(RaceConstants.BaseRaces.Goblin,
            LanguageConstants.Common,
            LanguageConstants.Goblin)]
        [TestCase(RaceConstants.BaseRaces.GrayElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.GraySlaad,
            LanguageConstants.Special.Slaad,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad,
            LanguageConstants.Special.Slaad,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Grimlock,
            LanguageConstants.Special.Grimlock,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.Metaraces.HalfCelestial, LanguageConstants.Celestial)]
        [TestCase(RaceConstants.Metaraces.HalfDragon, LanguageConstants.Draconic)]
        [TestCase(RaceConstants.BaseRaces.HalfElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.Metaraces.HalfFiend, LanguageConstants.Infernal)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc,
            LanguageConstants.Common,
            LanguageConstants.Orc)]
        [TestCase(RaceConstants.BaseRaces.Harpy, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.HighElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf,
            LanguageConstants.Common,
            LanguageConstants.Dwarven)]
        [TestCase(RaceConstants.BaseRaces.HillGiant, LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin,
            LanguageConstants.Common,
            LanguageConstants.Goblin)]
        [TestCase(RaceConstants.BaseRaces.HoundArchon, LanguageConstants.Celestial)]
        [TestCase(RaceConstants.BaseRaces.Human, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Janni, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Kobold, LanguageConstants.Draconic)]
        [TestCase(RaceConstants.BaseRaces.KuoToa, LanguageConstants.Special.KuoToa)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling,
            LanguageConstants.Common,
            LanguageConstants.Halfling)]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk,
            LanguageConstants.Common,
            LanguageConstants.Draconic)]
        [TestCase(RaceConstants.BaseRaces.Locathah, LanguageConstants.Aquan)]
        [TestCase(RaceConstants.BaseRaces.Merfolk,
            LanguageConstants.Common,
            LanguageConstants.Aquan)]
        [TestCase(RaceConstants.BaseRaces.Merrow,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.MindFlayer,
            LanguageConstants.Common,
            LanguageConstants.Undercommon)]
        [TestCase(RaceConstants.BaseRaces.Minotaur,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf,
            LanguageConstants.Common,
            LanguageConstants.Dwarven)]
        [TestCase(RaceConstants.BaseRaces.Mummy, LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.Ogre,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.OgreMage,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.Orc,
            LanguageConstants.Common,
            LanguageConstants.Orc)]
        [TestCase(RaceConstants.BaseRaces.Pixie,
            LanguageConstants.Common,
            LanguageConstants.Sylvan)]
        [TestCase(RaceConstants.BaseRaces.Rakshasa,
            LanguageConstants.Common,
            LanguageConstants.Infernal)]
        [TestCase(RaceConstants.BaseRaces.RedSlaad, LanguageConstants.Special.Slaad)]
        [TestCase(RaceConstants.BaseRaces.RockGnome,
            LanguageConstants.Common,
            LanguageConstants.Gnome)]
        [TestCase(RaceConstants.BaseRaces.Sahuagin,
            LanguageConstants.Common,
            LanguageConstants.Special.Sahuagin)]
        [TestCase(RaceConstants.BaseRaces.Satyr, LanguageConstants.Sylvan)]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk,
            LanguageConstants.Common,
            LanguageConstants.Terran)]
        [TestCase(RaceConstants.BaseRaces.Scrag, LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.StoneGiant, LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.StormGiant,
            LanguageConstants.Common,
            LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin,
            LanguageConstants.Common,
            LanguageConstants.Gnome,
            LanguageConstants.Undercommon)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling,
            LanguageConstants.Common,
            LanguageConstants.Halfling)]
        [TestCase(RaceConstants.BaseRaces.Tiefling,
            LanguageConstants.Common,
            LanguageConstants.Infernal)]
        [TestCase(RaceConstants.BaseRaces.Troglodyte, LanguageConstants.Draconic)]
        [TestCase(RaceConstants.BaseRaces.Troll, LanguageConstants.Giant)]
        [TestCase(RaceConstants.BaseRaces.WildElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.WoodElf,
            LanguageConstants.Common,
            LanguageConstants.Elven)]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination,
            LanguageConstants.Special.YuanTi,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood,
            LanguageConstants.Special.YuanTi,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood,
            LanguageConstants.Special.YuanTi,
            LanguageConstants.Common)]
        [TestCase(RaceConstants.Metaraces.Ghost)]
        [TestCase(RaceConstants.Metaraces.Lich, LanguageConstants.Common)]
        [TestCase(RaceConstants.Metaraces.None)]
        [TestCase(RaceConstants.Metaraces.Vampire)]
        [TestCase(RaceConstants.Metaraces.Werebear)]
        [TestCase(RaceConstants.Metaraces.Wereboar)]
        [TestCase(RaceConstants.Metaraces.Wereboar_Dire)]
        [TestCase(RaceConstants.Metaraces.Wererat)]
        [TestCase(RaceConstants.Metaraces.Weretiger)]
        [TestCase(RaceConstants.Metaraces.Werewolf)]
        [TestCase(RaceConstants.Metaraces.Werewolf_Dire)]
        public void AutomaticLanguages(string name, params string[] languages)
        {
            base.AssertDistinctCollection(name, languages);
        }
    }
}