using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats
{
    internal class FeatFocusGenerator : IFeatFocusGenerator
    {
        private readonly ICollectionSelector collectionsSelector;

        public FeatFocusGenerator(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public string GenerateFrom(
            string feat,
            string focusType,
            IEnumerable<Skill> skills,
            IEnumerable<RequiredFeatSelection> requiredFeats,
            IEnumerable<Feat> otherFeats,
            CharacterClass characterClass)
        {
            if (string.IsNullOrEmpty(focusType))
                return string.Empty;

            var allSourceFeatFoci = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci);
            if (focusType != FeatConstants.Foci.All && allSourceFeatFoci.Keys.Contains(focusType) == false)
                return focusType;

            var requiredFeatNames = requiredFeats.Select(f => f.Feat);
            var foci = GetFoci(feat, focusType, allSourceFeatFoci, otherFeats, requiredFeatNames);
            var usedFeats = otherFeats.Where(f => f.Name == feat);
            var usedFoci = usedFeats.SelectMany(f => f.Foci);

            if (usedFoci.Contains(FeatConstants.Foci.All))
                return FeatConstants.Foci.All;

            foci = foci.Except(usedFoci);
            var skillFoci = allSourceFeatFoci[GroupConstants.Skills].Intersect(foci);

            if (skillFoci.Any())
            {
                var potentialSkillFoci = skills.Select(s => s.Focus.Any() ? $"{s.Name}/{s.Focus}" : s.Name);
                foci = skillFoci.Intersect(potentialSkillFoci);
            }

            if (focusType == GroupConstants.SchoolsOfMagic)
                foci = foci.Except(characterClass.ProhibitedFields);

            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);
            if (spellcasters.Contains(characterClass.Name) == false)
                foci = foci.Except(new[] { FeatConstants.Foci.Ray });

            if (foci.Any() == false)
                return FeatConstants.Foci.All;

            return collectionsSelector.SelectRandomFrom(foci);
        }

        private IEnumerable<string> GetFoci(
            string feat,
            string focusType,
            Dictionary<string, IEnumerable<string>> allSourceFeatFoci,
            IEnumerable<Feat> otherFeats,
            IEnumerable<string> requiredFeatNames)
        {
            var proficiencyRequired = requiredFeatNames.Contains(ItemTypeConstants.Weapon + GroupConstants.Proficiency);
            var sourceFeatFoci = GetExplodedFoci(allSourceFeatFoci, feat, focusType, otherFeats);

            if (proficiencyRequired == false && otherFeats.Any(f => RequirementHasFocus(requiredFeatNames, f)) == false)
                return sourceFeatFoci;

            var requiredFeatWithFoci = otherFeats.Where(f => RequirementHasFocus(requiredFeatNames, f));

            if (proficiencyRequired)
            {
                var proficiencyFeatNames = collectionsSelector.SelectFrom(
                    Config.Name,
                    TableNameConstants.Set.Collection.FeatGroups,
                    ItemTypeConstants.Weapon + GroupConstants.Proficiency);
                var proficiencyFeats = otherFeats.Where(f => proficiencyFeatNames.Contains(f.Name));

                requiredFeatWithFoci = requiredFeatWithFoci.Union(proficiencyFeats);
            }

            var requiredFeats = requiredFeatWithFoci.Where(f => f.Foci.Contains(FeatConstants.Foci.All) == false);
            var requiredFoci = requiredFeats.SelectMany(f => f.Foci);

            var featsWithAllFocus = requiredFeatWithFoci.Where(f => f.Foci.Contains(FeatConstants.Foci.All));

            foreach (var featWithAllFocus in featsWithAllFocus)
            {
                var explodedFoci = GetExplodedFoci(allSourceFeatFoci, featWithAllFocus.Name, FeatConstants.Foci.All, otherFeats);
                requiredFoci = requiredFoci.Union(explodedFoci);
            }

            var applicableFoci = requiredFoci.Intersect(sourceFeatFoci);

            if (focusType.StartsWith(FeatConstants.Foci.Weapons) && requiredFeatWithFoci.Any() == false)
            {
                var automaticFoci = allSourceFeatFoci[focusType].Except(allSourceFeatFoci[FeatConstants.Foci.Weapons]);
                applicableFoci = applicableFoci.Union(automaticFoci);
            }

            return applicableFoci;
        }

        private IEnumerable<string> GetExplodedFoci(Dictionary<string, IEnumerable<string>> allSourceFeatFoci, string feat, string focusType, IEnumerable<Feat> otherFeats)
        {
            if (focusType != FeatConstants.Foci.All)
                return allSourceFeatFoci[focusType];

            if (feat != FeatConstants.MartialWeaponProficiency && feat != FeatConstants.ExoticWeaponProficiency)
                return allSourceFeatFoci[feat];

            var weaponFamiliarityFeats = otherFeats.Where(f => f.Name == FeatConstants.WeaponFamiliarity);
            var familiarityFoci = weaponFamiliarityFeats.SelectMany(f => f.Foci);

            if (feat == FeatConstants.MartialWeaponProficiency)
                return allSourceFeatFoci[feat].Union(familiarityFoci);

            return allSourceFeatFoci[feat].Except(familiarityFoci);
        }

        private bool RequirementHasFocus(IEnumerable<string> requiredFeatNames, Feat feat)
        {
            return requiredFeatNames.Contains(feat.Name) && feat.Foci.Any();
        }

        public string GenerateFrom(string feat, string focusType, IEnumerable<Skill> skills)
        {
            if (string.IsNullOrEmpty(focusType))
                return string.Empty;

            var allSourceFeatFoci = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci);
            if (focusType != FeatConstants.Foci.All && allSourceFeatFoci.Keys.Contains(focusType) == false)
                return focusType;

            var foci = GetFoci(feat, focusType, allSourceFeatFoci, Enumerable.Empty<Feat>(), Enumerable.Empty<string>());
            var skillFoci = allSourceFeatFoci[GroupConstants.Skills].Intersect(foci);

            if (skillFoci.Any())
            {
                var potentialSkillFoci = skills.Select(s => s.Focus.Any() ? $"{s.Name}/{s.Focus}" : s.Name);
                foci = skillFoci.Intersect(potentialSkillFoci);
            }

            if (foci.Any() == false)
                return FeatConstants.Foci.All;

            return collectionsSelector.SelectRandomFrom(foci);
        }

        public string GenerateAllowingFocusOfAllFrom(
            string feat,
            string focusType,
            IEnumerable<Skill> skills,
            IEnumerable<RequiredFeatSelection> requiredFeats,
            IEnumerable<Feat> otherFeats,
            CharacterClass characterClass)
        {
            if (focusType == FeatConstants.Foci.All)
                return focusType;

            return GenerateFrom(feat, focusType, skills, requiredFeats, otherFeats, characterClass);
        }

        public string GenerateAllowingFocusOfAllFrom(string feat, string focusType, IEnumerable<Skill> skills)
        {
            if (focusType == FeatConstants.Foci.All)
                return focusType;

            return GenerateFrom(feat, focusType, skills);
        }
    }
}