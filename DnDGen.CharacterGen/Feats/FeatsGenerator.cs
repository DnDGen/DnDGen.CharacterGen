using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats
{
    internal class FeatsGenerator(
        IRacialFeatsGenerator racialFeatsGenerator,
        IClassFeatsGenerator classFeatsGenerator,
        IAdditionalFeatsGenerator additionalFeatsGenerator) : IFeatsGenerator
    {
        private readonly IRacialFeatsGenerator racialFeatsGenerator = racialFeatsGenerator;
        private readonly IClassFeatsGenerator classFeatsGenerator = classFeatsGenerator;
        private readonly IAdditionalFeatsGenerator additionalFeatsGenerator = additionalFeatsGenerator;

        public FeatCollections GenerateWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills, BaseAttack baseAttack)
        {
            var featCollections = new FeatCollections
            {
                Racial = racialFeatsGenerator.GenerateWith(race, skills, abilities)
            };
            featCollections.Class = classFeatsGenerator.GenerateWith(characterClass, race, abilities, featCollections.Racial, skills);

            var automaticFeats = featCollections.All.ToArray();
            featCollections.Additional = additionalFeatsGenerator.GenerateWith(characterClass, race, abilities, skills, baseAttack, automaticFeats);


            featCollections.Racial = CombineFoci(featCollections.Racial);
            featCollections.Class = CombineFoci(featCollections.Class);
            featCollections.Additional = CombineFoci(featCollections.Additional);

            return featCollections;
        }

        private static IEnumerable<Feat> CombineFoci(IEnumerable<Feat> source)
        {
            var combinedFeats = source.Where(f => !f.Foci.Any()).ToList();
            var fociGroups = source
                .Where(f => f.Foci.Any())
                .GroupBy(f => f.Name + f.Power + f.Frequency.Quantity + f.Frequency.TimePeriod);

            foreach (var group in fociGroups)
            {
                var first = group.First();
                if (group.Count() == 1)
                {
                    combinedFeats.Add(first);
                    continue;
                }

                first.Foci = [.. group.SelectMany(g => g.Foci).Distinct()];

                if (first.Foci.Contains(GroupConstants.All))
                    first.Foci = [GroupConstants.All];

                combinedFeats.Add(first);
            }

            return combinedFeats;
        }
    }
}