using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Languages.Selectors
{
    internal interface ILanguageCollectionsSelector
    {
        IEnumerable<string> SelectAutomaticLanguagesFor(Race race, string className);
        IEnumerable<string> SelectBonusLanguagesFor(string baseRace, string className);
    }
}