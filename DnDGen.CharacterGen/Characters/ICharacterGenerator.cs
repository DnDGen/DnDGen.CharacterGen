using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Races.Randomizers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.CharacterGen.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DnDGen.CharacterGen.Tests.Integration")]
[assembly: InternalsVisibleTo("DnDGen.CharacterGen.Tests.Integration.IoC")]
[assembly: InternalsVisibleTo("DnDGen.CharacterGen.Tests.Integration.Tables")]
namespace DnDGen.CharacterGen.Characters
{
    public interface ICharacterGenerator
    {
        Character GenerateWith(IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer,
            IAbilitiesRandomizer statsRandomizer);

        CharacterPrototype GeneratePrototypeWith(IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer);
    }
}