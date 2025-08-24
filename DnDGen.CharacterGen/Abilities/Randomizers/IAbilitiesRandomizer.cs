using DnDGen.CharacterGen.Abilities;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    public interface IAbilitiesRandomizer
    {
        Dictionary<string, Ability> Randomize();
    }
}