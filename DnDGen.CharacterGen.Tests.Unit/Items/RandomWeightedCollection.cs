using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    public class RandomWeightedCollection<T>
    {
        public T[] Common { get; set; } = [];
        public T[] Uncommon { get; set; } = [];
        public T[] Rare { get; set; } = [];
        public T[] VeryRare { get; set; } = [];
        public T Expected { get; set; }

        public bool Any => Common?.Length > 0 || Uncommon?.Length > 0 || Rare?.Length > 0 || VeryRare?.Length > 0;

        public void SetupMock(Mock<ICollectionSelector> mockCollectionSelector)
        {
            if (!Any)
                return;

            mockCollectionSelector
                .Setup(s => s.SelectRandomFrom(
                    EquivalentSet(Common),
                    EquivalentSet(Uncommon),
                    EquivalentSet(Rare),
                    EquivalentSet(VeryRare)))
                .Returns(Expected);
        }

        public static IEnumerable<T> EquivalentSet(params T[] expected)
        {
            if (expected is null)
                return null;

            return It.Is<IEnumerable<T>>(ss => ss.Intersect(expected).Count() == expected.Length && ss.Count() == expected.Length);
        }

        public void VerifyMock(Mock<ICollectionSelector> mockCollectionSelector)
        {
            if (!Any)
                return;

            mockCollectionSelector
                .Verify(s => s.SelectRandomFrom(
                    EquivalentSet(Common),
                    EquivalentSet(Uncommon),
                    EquivalentSet(Rare),
                    EquivalentSet(VeryRare)), Times.Once);
        }
    }
}
