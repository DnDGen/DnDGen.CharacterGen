using DnDGen.CharacterGen.Languages;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Languages
{
    [TestFixture]
    public class LanguageConstantsTests
    {
        [TestCase(LanguageConstants.Abyssal, "Abyssal")]
        [TestCase(LanguageConstants.Aquan, "Aquan")]
        [TestCase(LanguageConstants.Auran, "Auran")]
        [TestCase(LanguageConstants.Celestial, "Celestial")]
        [TestCase(LanguageConstants.Common, "Common")]
        [TestCase(LanguageConstants.Draconic, "Draconic")]
        [TestCase(LanguageConstants.Dwarven, "Dwarven")]
        [TestCase(LanguageConstants.Elven, "Elven")]
        [TestCase(LanguageConstants.Giant, "Giant")]
        [TestCase(LanguageConstants.Gnoll, "Gnoll")]
        [TestCase(LanguageConstants.Gnome, "Gnome")]
        [TestCase(LanguageConstants.Goblin, "Goblin")]
        [TestCase(LanguageConstants.Halfling, "Halfling")]
        [TestCase(LanguageConstants.Ignan, "Ignan")]
        [TestCase(LanguageConstants.Infernal, "Infernal")]
        [TestCase(LanguageConstants.Orc, "Orc")]
        [TestCase(LanguageConstants.Sylvan, "Sylvan")]
        [TestCase(LanguageConstants.Terran, "Terran")]
        [TestCase(LanguageConstants.Undercommon, "Undercommon")]
        [TestCase(LanguageConstants.Special.Druidic, "Druidic")]
        [TestCase(LanguageConstants.Special.Githyanki, "Githyanki")]
        [TestCase(LanguageConstants.Special.Githzerai, "Githzerai")]
        [TestCase(LanguageConstants.Special.Grimlock, "Grimlock")]
        [TestCase(LanguageConstants.Special.KuoToa, "Kuo-toa")]
        [TestCase(LanguageConstants.Special.Sahuagin, "Sahuagin")]
        [TestCase(LanguageConstants.Special.Slaad, "Slaad")]
        [TestCase(LanguageConstants.Special.YuanTi, "Yuan-ti")]
        public void LanguageConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}