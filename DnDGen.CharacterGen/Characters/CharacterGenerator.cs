using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Languages;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.Characters
{
    internal class CharacterGenerator : ICharacterGenerator
    {
        private readonly IAlignmentGenerator alignmentGenerator;
        private readonly ICharacterClassGenerator characterClassGenerator;
        private readonly IRaceGenerator raceGenerator;
        private readonly IRandomizerVerifier randomizerVerifier;
        private readonly IPercentileSelector percentileSelector;
        private readonly ICombatGenerator combatGenerator;
        private readonly IEquipmentGenerator equipmentGenerator;
        private readonly IMagicGenerator magicGenerator;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAbilitiesGenerator abilitiesGenerator;
        private readonly ILanguageGenerator languageGenerator;
        private readonly ISkillsGenerator skillsGenerator;
        private readonly IFeatsGenerator featsGenerator;

        public CharacterGenerator(IAlignmentGenerator alignmentGenerator,
            ICharacterClassGenerator characterClassGenerator,
            IRaceGenerator raceGenerator,
            IRandomizerVerifier randomizerVerifier,
            IPercentileSelector percentileSelector,
            ICombatGenerator combatGenerator,
            IEquipmentGenerator equipmentGenerator,
            IMagicGenerator magicGenerator,
            ICollectionSelector collectionsSelector,
            IAbilitiesGenerator abilitiesGenerator,
            ILanguageGenerator languageGenerator,
            ISkillsGenerator skillsGenerator,
            IFeatsGenerator featsGenerator)
        {
            this.alignmentGenerator = alignmentGenerator;
            this.characterClassGenerator = characterClassGenerator;
            this.raceGenerator = raceGenerator;
            this.abilitiesGenerator = abilitiesGenerator;
            this.combatGenerator = combatGenerator;
            this.equipmentGenerator = equipmentGenerator;
            this.languageGenerator = languageGenerator;
            this.skillsGenerator = skillsGenerator;
            this.featsGenerator = featsGenerator;
            this.magicGenerator = magicGenerator;

            this.randomizerVerifier = randomizerVerifier;
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public Character GenerateWith(IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer,
            IAbilitiesRandomizer statsRandomizer)
        {
            VerifyRandomizers(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);

            var character = GenerateCharacter(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer, statsRandomizer);

            return character;
        }

        private Character GenerateCharacter(IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer,
            IAbilitiesRandomizer statsRandomizer)
        {
            var character = new Character();
            var prototype = GeneratePrototypeWith(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);

            character.Alignment = alignmentGenerator.GenerateWith(prototype.Alignment);
            character.Class = characterClassGenerator.GenerateWith(character.Alignment, prototype.CharacterClass, prototype.Race);
            character.Race = raceGenerator.GenerateWith(character.Alignment, character.Class, prototype.Race);
            character.Abilities = abilitiesGenerator.GenerateWith(statsRandomizer, character.Class, character.Race);
            character.Skills = skillsGenerator.GenerateWith(character.Class, character.Race, character.Abilities);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(character.Class, character.Race, character.Abilities);
            character.Feats = featsGenerator.GenerateWith(character.Class, character.Race, character.Abilities, character.Skills, baseAttack);
            character.Equipment = equipmentGenerator.GenerateWith(character.Feats, character.Class, character.Race);
            character.Skills = skillsGenerator.UpdateSkillsFromFeats(character.Skills, character.Feats.All);
            character.Skills = skillsGenerator.UpdateSkillsFromEquipment(character.Skills, character.Equipment);

            character.Languages = languageGenerator.GenerateWith(character.Race, character.Class, character.Abilities, character.Skills);
            character.Combat = combatGenerator.GenerateWith(baseAttack, character.Class, character.Race, character.Feats.All, character.Abilities, character.Equipment);
            character.InterestingTrait = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.Traits);
            character.Magic = magicGenerator.GenerateWith(character.Alignment, character.Class, character.Race, character.Abilities, character.Feats.All, character.Equipment);

            return character;
        }

        private void VerifyRandomizers(IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer)
        {
            var verified = randomizerVerifier.VerifyCompatibility(alignmentRandomizer, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);

            if (!verified)
                throw new IncompatibleRandomizersException();
        }

        public CharacterPrototype GeneratePrototypeWith(
            IAlignmentRandomizer alignmentRandomizer,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer)
        {
            var prototype = new CharacterPrototype();

            var alignments = alignmentGenerator.GeneratePrototypes(alignmentRandomizer);
            var validAlignments = randomizerVerifier.FilterAlignments(alignments, classNameRandomizer, levelRandomizer, baseRaceRandomizer, metaraceRandomizer);

            if (!validAlignments.Any())
                throw new IncompatibleRandomizersException();

            prototype.Alignment = alignmentGenerator.GeneratePrototype(alignmentRandomizer);

            if (!validAlignments.Contains(prototype.Alignment))
                prototype.Alignment = collectionsSelector.SelectRandomFrom(validAlignments);

            var characterClasses = characterClassGenerator.GeneratePrototypes(prototype.Alignment, classNameRandomizer, levelRandomizer);
            var validCharacterClasses = randomizerVerifier.FilterCharacterClasses(characterClasses, prototype.Alignment, baseRaceRandomizer, metaraceRandomizer);

            if (!validCharacterClasses.Any())
                throw new IncompatibleRandomizersException();

            prototype.CharacterClass = characterClassGenerator.GeneratePrototype(prototype.Alignment, classNameRandomizer, levelRandomizer);

            if (!validCharacterClasses.Contains(prototype.CharacterClass))
                prototype.CharacterClass = collectionsSelector.SelectRandomFrom(validCharacterClasses);

            var races = raceGenerator.GeneratePrototypes(prototype.Alignment, prototype.CharacterClass, baseRaceRandomizer, metaraceRandomizer);
            var validRaces = randomizerVerifier.FilterRaces(races, prototype.Alignment, prototype.CharacterClass);

            if (!validRaces.Any())
                throw new IncompatibleRandomizersException();

            prototype.Race = raceGenerator.GeneratePrototype(prototype.Alignment, prototype.CharacterClass, baseRaceRandomizer, metaraceRandomizer);

            if (!validRaces.Contains(prototype.Race))
            {
                //INFO: Filter out metaraces, only allow "None"
                //If there are no races with No Metarace, then just let it be
                //Otherwise, metaraces appear way too often, for how rare they are supposed to be
                if (validRaces.Any(r => r.Metarace == RaceConstants.Metaraces.None))
                    validRaces = validRaces.Where(r => r.Metarace == RaceConstants.Metaraces.None);

                prototype.Race = collectionsSelector.SelectRandomFrom(validRaces);
            }

            return prototype;
        }
    }
}