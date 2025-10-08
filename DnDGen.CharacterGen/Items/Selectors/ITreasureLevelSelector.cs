using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;

namespace DnDGen.CharacterGen.Items.Selectors
{
    internal interface ITreasureLevelSelector
    {
        int SelectLevelFrom(CharacterClass characterClass, Race race);
        string SelectPowerFrom(CharacterClass characterClass, Race race);
    }
}
