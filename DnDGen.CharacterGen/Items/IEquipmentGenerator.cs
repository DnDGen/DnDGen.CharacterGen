using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;

namespace DnDGen.CharacterGen.Items
{
    internal interface IEquipmentGenerator
    {
        Equipment GenerateWith(FeatCollections feats, CharacterClass characterClass, Race race);
    }
}