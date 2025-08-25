using DnDGen.CharacterGen.Abilities.Selectors;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Languages.Selectors;
using DnDGen.CharacterGen.Leaders.Selectors;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Skills.Selectors;
using Ninject.Modules;

namespace DnDGen.CharacterGen.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILanguageCollectionsSelector>().To<LanguageCollectionsSelector>();
            Bind<IAdjustmentsSelector>().To<AdjustmentsSelector>();
            Bind<IAbilityAdjustmentsSelector>().To<AbilityAdjustmentsSelector>();
            Bind<ISkillSelector>().To<SkillSelector>();
            Bind<ILeadershipSelector>().To<LeadershipSelector>();
            Bind<IFeatsSelector>().To<FeatsSelector>();
        }
    }
}