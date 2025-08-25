using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Abilities.Selectors
{
    internal interface IAbilityAdjustmentsSelector
    {
        Dictionary<string, int> SelectFor(Race race);
    }
}