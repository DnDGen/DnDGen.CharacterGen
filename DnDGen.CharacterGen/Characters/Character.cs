using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Characters
{
    public class Character
    {
        public Alignment Alignment { get; set; }
        public CharacterClass Class { get; set; }
        public Race Race { get; set; }
        public string InterestingTrait { get; set; }
        public Combat Combat { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<string> Languages { get; set; }
        public FeatCollections Feats { get; set; }
        public Dictionary<string, Ability> Abilities { get; set; }
        public Equipment Equipment { get; set; }
        public Magic Magic { get; set; }

        public bool IsLeader
        {
            get
            {
                return Feats.All.Any(f => f.Name == FeatConstants.Leadership);
            }
        }

        public string Summary
        {
            get
            {
                if (Class.Level == 0)
                    return string.Empty;

                var summary = $"{Alignment.Full} {Race.Summary} {Class.Summary}";

                return summary;
            }
        }

        public double ChallengeRating
        {
            get
            {
                var adjustment = Class.IsNPC ? Race.NPCChallengeRatingAdjustment : Race.PCChallengeRatingAdjustment;
                var classChallengeRating = Class.Level + (double)adjustment;

                switch (classChallengeRating)
                {
                    case 0: classChallengeRating = 1 / 2d; break;
                    case -1: classChallengeRating = 1 / 3d; break;
                    case -2: classChallengeRating = 1 / 4d; break;
                    default: break;
                }

                var challengeRating = Race.ChallengeRating + classChallengeRating;

                if (challengeRating > 1)
                    return Math.Floor(challengeRating);

                return challengeRating;
            }
        }

        public Character()
        {
            Alignment = new Alignment();
            Class = new CharacterClass();
            Race = new Race();
            InterestingTrait = string.Empty;
            Combat = new Combat();
            Skills = [];
            Languages = [];
            Feats = new FeatCollections();
            Abilities = [];
            Equipment = new Equipment();
            Magic = new Magic();
        }

        public override string ToString() => Summary;
    }
}