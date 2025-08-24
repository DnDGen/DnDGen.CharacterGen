using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Tests.Integration.Generators.Characters;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Characters
{
    [TestFixture]
    public class CharacterGeneratorTests : StressTests
    {
        private IClassNameRandomizer nPCClassNameRandomizer;
        private IClassNameRandomizer spellcasterClassNameRandomizer;
        private RaceRandomizer aquaticBaseRaceRandomizer;
        private RaceRandomizer monsterBaseRaceRandomizer;
        private IAbilitiesRandomizer heroicAbilitiesRandomizer;
        private CharacterAsserter characterAsserter;

        [SetUp]
        public void Setup()
        {
            characterAsserter = new CharacterAsserter();
            heroicAbilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Heroic);
            monsterBaseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.MonsterBase);
            aquaticBaseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AquaticBase);
            spellcasterClassNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.Spellcaster);
            nPCClassNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
        }

        [Test]
        public void StressCharacter()
        {
            stressor.Stress(GenerateAndAssertCharacter);
        }

        private void GenerateAndAssertCharacter()
        {
            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.IsNPC, Is.False, character.Summary);
        }

        [Test]
        public void StressMonster()
        {
            stressor.Stress(GenerateAndAssertMonster);
        }

        private void GenerateAndAssertMonster()
        {
            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                monsterBaseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.IsNPC, Is.False, character.Summary);
        }

        [Test]
        public void StressNPC()
        {
            stressor.Stress(GenerateAndAssertNPC);
        }

        private void GenerateAndAssertNPC()
        {
            var npc = characterGenerator.GenerateWith(
                alignmentRandomizer,
                nPCClassNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(npc);
            Assert.That(npc.Class.IsNPC, Is.True);
        }

        [Test]
        public void StressAquatic()
        {
            stressor.Stress(GenerateAndAssertAquatic);
        }

        private void GenerateAndAssertAquatic()
        {
            var aquaticCharacter = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                aquaticBaseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(aquaticCharacter);
            Assert.That(aquaticCharacter.Class.IsNPC, Is.False, aquaticCharacter.Summary);
            Assert.That(aquaticCharacter.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.AquaticElf)
                .Or.EqualTo(RaceConstants.BaseRaces.Kapoacinth)
                .Or.EqualTo(RaceConstants.BaseRaces.KuoToa)
                .Or.EqualTo(RaceConstants.BaseRaces.Locathah)
                .Or.EqualTo(RaceConstants.BaseRaces.Merfolk)
                .Or.EqualTo(RaceConstants.BaseRaces.Merrow)
                .Or.EqualTo(RaceConstants.BaseRaces.Sahuagin)
                .Or.EqualTo(RaceConstants.BaseRaces.Scrag));
            Assert.That(aquaticCharacter.Race.SwimSpeed.Value, Is.Positive);
        }

        [Test]
        public void StressSpellcaster()
        {
            stressor.Stress(GenerateAndAssertSpellcaster);
        }

        private void GenerateAndAssertSpellcaster()
        {
            //INFO: Need at least level 4, since Rangers and Paladins can't cast spells lower than that
            var spellcaster = stressor.Generate(
                () => characterGenerator.GenerateWith(
                    alignmentRandomizer,
                    spellcasterClassNameRandomizer,
                    levelRandomizer,
                    baseRaceRandomizer,
                    metaraceRandomizer,
                    heroicAbilitiesRandomizer),
                c => c.Class.Level >= 4);

            characterAsserter.AssertCharacter(spellcaster);
            Assert.That(spellcaster.Class.IsNPC, Is.False, spellcaster.Summary);
            characterAsserter.AssertSpellcaster(spellcaster);
        }
    }
}