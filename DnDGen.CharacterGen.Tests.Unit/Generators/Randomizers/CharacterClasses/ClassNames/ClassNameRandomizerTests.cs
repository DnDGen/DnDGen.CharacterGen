using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Randomizers.CharacterClasses.ClassNames
{
    [TestFixture]
    public abstract class ClassNameRandomizerTests
    {
        protected const string ClassName = "class name";
        protected const string AlignmentClassName = "alignment class name";
        protected const string GroupClassName = "group class name";

        protected abstract string classNameGroup { get; }

        internal Mock<ICollectionSelector> mockCollectionsSelector;
        internal Mock<IPercentileSelector> mockPercentileResultSelector;
        protected IClassNameRandomizer randomizer;
        protected Alignment alignment;
        protected List<string> alignmentClasses;
        protected List<string> groupClasses;

        [SetUp]
        public void ClassNameRandomizerTestsSetup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockPercentileResultSelector = new Mock<IPercentileSelector>();
            alignment = new Alignment();
            alignmentClasses = new List<string>();
            groupClasses = new List<string>();

            alignment.Goodness = "goodness";
            alignment.Lawfulness = "lawfulness";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, classNameGroup)).Returns(groupClasses);
            mockPercentileResultSelector.Setup(s => s.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(new[] { ClassName, AlignmentClassName, GroupClassName });
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, alignment.ToString())).Returns(alignmentClasses);
            alignmentClasses.Add(AlignmentClassName);
            groupClasses.Add(GroupClassName);
        }
    }
}