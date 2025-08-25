using DnDGen.CharacterGen.Items;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    [TestFixture]
    public class EquipmentTests
    {
        private Equipment equipment;

        [SetUp]
        public void Setup()
        {
            equipment = new Equipment();
        }

        [Test]
        public void EquipmentInitialized()
        {
            Assert.That(equipment.Armor, Is.Null);
            Assert.That(equipment.OffHand, Is.Null);
            Assert.That(equipment.PrimaryHand, Is.Null);
            Assert.That(equipment.Treasure, Is.Not.Null);
        }
    }
}