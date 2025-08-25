using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Skills.Selectors
{
    public class RequiredSkillSelection
    {
        public string Skill { get; set; }
        public string Focus { get; set; }
        public int Ranks { get; set; }

        public RequiredSkillSelection()
        {
            Skill = string.Empty;
            Focus = string.Empty;
        }

        public bool RequirementMet(IEnumerable<Skill> otherSkills)
        {
            var requiredSkill = otherSkills.FirstOrDefault(s => s.Name == Skill && s.Focus == Focus);

            if (requiredSkill == null)
                return false;

            return requiredSkill.EffectiveRanks >= Ranks;
        }
    }
}
