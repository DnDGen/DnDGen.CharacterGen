using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System;
using System.Linq;

namespace DnDGen.CharacterGen.Skills.Selectors
{
    internal class SkillSelector : ISkillSelector
    {
        private readonly ICollectionSelector innerSelector;

        public SkillSelector(ICollectionSelector innerSelector)
        {
            this.innerSelector = innerSelector;
        }

        public SkillSelection SelectFor(string skill)
        {
            var data = innerSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SkillData, skill).ToArray();

            var selection = new SkillSelection();
            selection.BaseStatName = data[DataIndexConstants.SkillSelectionData.BaseStatName];
            selection.SkillName = data[DataIndexConstants.SkillSelectionData.SkillName];
            selection.RandomFociQuantity = Convert.ToInt32(data[DataIndexConstants.SkillSelectionData.RandomFociQuantity]);
            selection.Focus = data[DataIndexConstants.SkillSelectionData.Focus];

            return selection;
        }
    }
}