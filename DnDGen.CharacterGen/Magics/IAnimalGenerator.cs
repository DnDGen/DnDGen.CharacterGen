using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Magics
{
    internal interface IAnimalGenerator
    {
        string GenerateFrom(Alignment alignment, CharacterClass characterClass, Race race, IEnumerable<Feat> feats);
    }
}
