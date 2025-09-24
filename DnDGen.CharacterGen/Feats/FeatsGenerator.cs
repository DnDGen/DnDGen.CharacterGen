using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats
{
    internal class FeatsGenerator : IFeatsGenerator
    {
        private readonly IRacialFeatsGenerator racialFeatsGenerator;
        private readonly IClassFeatsGenerator classFeatsGenerator;
        private readonly IAdditionalFeatsGenerator additionalFeatsGenerator;

        public FeatsGenerator(IRacialFeatsGenerator racialFeatsGenerator, IClassFeatsGenerator classFeatsGenerator, IAdditionalFeatsGenerator additionalFeatsGenerator)
        {
            this.racialFeatsGenerator = racialFeatsGenerator;
            this.classFeatsGenerator = classFeatsGenerator;
            this.additionalFeatsGenerator = additionalFeatsGenerator;
        }

        public FeatCollections GenerateWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills, BaseAttack baseAttack)
        {
            var featCollections = new FeatCollections
            {
                Racial = racialFeatsGenerator.GenerateWith(race, skills, abilities)
            };
            featCollections.Class = classFeatsGenerator.GenerateWith(characterClass, race, abilities, featCollections.Racial, skills);

            var automaticFeats = featCollections.All.ToArray();
            featCollections.Additional = additionalFeatsGenerator.GenerateWith(characterClass, race, abilities, skills, baseAttack, automaticFeats);

            return featCollections;
        }
    }
}