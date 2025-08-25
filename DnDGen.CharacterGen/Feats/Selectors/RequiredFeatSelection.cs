using DnDGen.CharacterGen.Feats;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats.Selectors
{
    internal class RequiredFeatSelection
    {
        public string Feat { get; set; }
        public string Focus { get; set; }

        public RequiredFeatSelection()
        {
            Feat = string.Empty;
            Focus = string.Empty;
        }

        public bool RequirementMet(IEnumerable<Feat> otherFeats)
        {
            var requiredFeats = otherFeats.Where(f => f.Name == Feat);

            if (requiredFeats.Any() == false)
                return false;

            if (string.IsNullOrEmpty(Focus))
                return true;

            return requiredFeats.Any(f => f.Foci.Contains(Focus));
        }
    }
}