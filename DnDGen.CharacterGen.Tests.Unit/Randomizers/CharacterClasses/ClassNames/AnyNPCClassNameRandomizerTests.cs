using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Generators.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.CharacterClasses.ClassNames
{
    [TestFixture]
    public class AnyNPCClassNameRandomizerTests
    {
        private IClassNameRandomizer npcRandomizer;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private List<string> npcs;
        private Alignment alignment;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            npcRandomizer = new AnyNPCClassNameRandomizer(mockCollectionsSelector.Object);
            npcs = new List<string>();
            alignment = new Alignment();

            npcs.Add("npc 1");
            npcs.Add("npc 2");

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs)).Returns(npcs);
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.Last());
        }

        [Test]
        public void ReturnARandomNPCClass()
        {
            var className = npcRandomizer.Randomize(alignment);
            Assert.That(className, Is.EqualTo("npc 2"));
        }

        [Test]
        public void ReturnAllNPCClasses()
        {
            var classNames = npcRandomizer.GetAllPossibleResults(alignment);
            Assert.That(classNames, Is.EqualTo(npcs));
        }
    }
}
