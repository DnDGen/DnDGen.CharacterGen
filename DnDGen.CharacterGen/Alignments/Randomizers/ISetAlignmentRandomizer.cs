using DnDGen.CharacterGen.Alignments;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    public interface ISetAlignmentRandomizer : IAlignmentRandomizer
    {
        Alignment SetAlignment { get; set; }
    }
}