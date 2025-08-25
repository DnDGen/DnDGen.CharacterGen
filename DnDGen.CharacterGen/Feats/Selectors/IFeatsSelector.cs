using System.Collections.Generic;

namespace DnDGen.CharacterGen.Feats.Selectors
{
    internal interface IFeatsSelector
    {
        IEnumerable<RacialFeatSelection> SelectRacial(string race);
        IEnumerable<AdditionalFeatSelection> SelectAdditional();
        IEnumerable<CharacterClassFeatSelection> SelectClass(string characterClassName);
    }
}