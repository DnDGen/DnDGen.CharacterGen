using DnDGen.CharacterGen.Combats;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Combats
{
    [TestFixture]
    public class BaseAttackTests
    {
        private BaseAttack baseAttack;

        [SetUp]
        public void Setup()
        {
            baseAttack = new BaseAttack();
        }

        [Test]
        public void BaseAttackInitialized()
        {
            Assert.That(baseAttack.MeleeBonus, Is.EqualTo(0));
            Assert.That(baseAttack.RangedBonus, Is.EqualTo(0));
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(0));
            Assert.That(baseAttack.CircumstantialBonus, Is.False);
            Assert.That(baseAttack.StrengthBonus, Is.EqualTo(0));
            Assert.That(baseAttack.DexterityBonus, Is.EqualTo(0));
            Assert.That(baseAttack.SizeModifier, Is.EqualTo(0));
            Assert.That(baseAttack.RacialModifier, Is.EqualTo(0));
        }

        [Test]
        public void MeleeBonusDoesNotIncludeDexterity()
        {
            baseAttack.BaseBonus = 9266;
            baseAttack.DexterityBonus = 90210;
            baseAttack.SizeModifier = 42;
            baseAttack.StrengthBonus = 600;
            baseAttack.RacialModifier = 1337;

            Assert.That(baseAttack.MeleeBonus, Is.EqualTo(11245));
        }

        [Test]
        public void NegativeMelee()
        {
            baseAttack.BaseBonus = -9266;
            baseAttack.DexterityBonus = 90210;
            baseAttack.SizeModifier = -42;
            baseAttack.StrengthBonus = -600;
            baseAttack.RacialModifier = -1337;

            Assert.That(baseAttack.MeleeBonus, Is.EqualTo(-11245));
        }

        [Test]
        public void RangedBonusDoesNotIncludeStrength()
        {
            baseAttack.BaseBonus = 9266;
            baseAttack.DexterityBonus = 90210;
            baseAttack.SizeModifier = 42;
            baseAttack.StrengthBonus = 600;
            baseAttack.RacialModifier = 1337;

            Assert.That(baseAttack.RangedBonus, Is.EqualTo(100855));
        }

        [Test]
        public void NegativeRanged()
        {
            baseAttack.BaseBonus = -9266;
            baseAttack.DexterityBonus = -90210;
            baseAttack.SizeModifier = -42;
            baseAttack.StrengthBonus = 600;
            baseAttack.RacialModifier = -1337;

            Assert.That(baseAttack.RangedBonus, Is.EqualTo(-100855));
        }

        [TestCase(-5, -5)]
        [TestCase(-4, -4)]
        [TestCase(-3, -3)]
        [TestCase(-2, -2)]
        [TestCase(-1, -1)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6, 1)]
        [TestCase(7, 7, 2)]
        [TestCase(8, 8, 3)]
        [TestCase(9, 9, 4)]
        [TestCase(10, 10, 5)]
        [TestCase(11, 11, 6, 1)]
        [TestCase(12, 12, 7, 2)]
        [TestCase(13, 13, 8, 3)]
        [TestCase(14, 14, 9, 4)]
        [TestCase(15, 15, 10, 5)]
        [TestCase(16, 16, 11, 6, 1)]
        [TestCase(17, 17, 12, 7, 2)]
        [TestCase(18, 18, 13, 8, 3)]
        [TestCase(19, 19, 14, 9, 4)]
        [TestCase(20, 20, 15, 10, 5)]
        [TestCase(21, 21, 16, 11, 6)]
        [TestCase(22, 22, 17, 12, 7)]
        [TestCase(23, 23, 18, 13, 8)]
        [TestCase(24, 24, 19, 14, 9)]
        [TestCase(25, 25, 20, 15, 10)]
        public void RangedBonuses(int baseBonus, params int[] bonuses)
        {
            baseAttack.BaseBonus = baseBonus;
            var attacks = baseAttack.AllRangedBonuses.ToArray();

            for (var i = 0; i < bonuses.Length; i++)
            {
                Assert.That(attacks[i], Is.EqualTo(bonuses[i]));
            }

            var extras = attacks.Except(bonuses);
            Assert.That(extras, Is.Empty);
        }

        [Test]
        public void AllRangedBonuses()
        {
            baseAttack.BaseBonus = 9266;
            baseAttack.DexterityBonus = 90210;
            baseAttack.SizeModifier = 42;
            baseAttack.RacialModifier = 600;

            var attacks = baseAttack.AllRangedBonuses.ToArray();
            Assert.That(attacks[0], Is.EqualTo(100118));
            Assert.That(attacks[1], Is.EqualTo(100113));
            Assert.That(attacks[2], Is.EqualTo(100108));
            Assert.That(attacks[3], Is.EqualTo(100103));
            Assert.That(attacks.Length, Is.EqualTo(4));
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6, 1)]
        [TestCase(7, 7, 2)]
        [TestCase(8, 8, 3)]
        [TestCase(9, 9, 4)]
        [TestCase(10, 10, 5)]
        [TestCase(11, 11, 6, 1)]
        [TestCase(12, 12, 7, 2)]
        [TestCase(13, 13, 8, 3)]
        [TestCase(14, 14, 9, 4)]
        [TestCase(15, 15, 10, 5)]
        [TestCase(16, 16, 11, 6, 1)]
        [TestCase(17, 17, 12, 7, 2)]
        [TestCase(18, 18, 13, 8, 3)]
        [TestCase(19, 19, 14, 9, 4)]
        [TestCase(20, 20, 15, 10, 5)]
        [TestCase(21, 21, 16, 11, 6)]
        [TestCase(22, 22, 17, 12, 7)]
        [TestCase(23, 23, 18, 13, 8)]
        [TestCase(24, 24, 19, 14, 9)]
        [TestCase(25, 25, 20, 15, 10)]
        [TestCase(26, 26, 21, 16, 11)]
        public void MeleeBonuses(int baseBonus, params int[] bonuses)
        {
            baseAttack.BaseBonus = baseBonus;
            var attacks = baseAttack.AllMeleeBonuses.ToArray();

            for (var i = 0; i < bonuses.Length; i++)
            {
                Assert.That(attacks[i], Is.EqualTo(bonuses[i]));
            }

            var extras = attacks.Except(bonuses);
            Assert.That(extras, Is.Empty);
        }

        [Test]
        public void AllMeleeBonuses()
        {
            baseAttack.BaseBonus = 9266;
            baseAttack.StrengthBonus = 90210;
            baseAttack.SizeModifier = 42;
            baseAttack.RacialModifier = 600;

            var attacks = baseAttack.AllMeleeBonuses.ToArray();
            Assert.That(attacks[0], Is.EqualTo(100118));
            Assert.That(attacks[1], Is.EqualTo(100113));
            Assert.That(attacks[2], Is.EqualTo(100108));
            Assert.That(attacks[3], Is.EqualTo(100103));
            Assert.That(attacks.Length, Is.EqualTo(4));
        }

        [Test]
        public void StrengthBonusDoesNotDetermineNumberOfMeleeBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.StrengthBonus = 1;

            var meleeAttacks = baseAttack.AllMeleeBonuses.ToArray();
            Assert.That(meleeAttacks[0], Is.EqualTo(11));
            Assert.That(meleeAttacks[1], Is.EqualTo(6));
            Assert.That(meleeAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void SizeModifierDoesNotDetermineNumberOfMeleeBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.SizeModifier = 1;

            var meleeAttacks = baseAttack.AllMeleeBonuses.ToArray();
            Assert.That(meleeAttacks[0], Is.EqualTo(11));
            Assert.That(meleeAttacks[1], Is.EqualTo(6));
            Assert.That(meleeAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void RacialModifierDoesNotDetermineNumberOfMeleeBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.RacialModifier = 1;

            var meleeAttacks = baseAttack.AllMeleeBonuses.ToArray();
            Assert.That(meleeAttacks[0], Is.EqualTo(11));
            Assert.That(meleeAttacks[1], Is.EqualTo(6));
            Assert.That(meleeAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void DexterityBonusDoesNotDetermineNumberOfRangedBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.DexterityBonus = 1;

            var rangedAttacks = baseAttack.AllRangedBonuses.ToArray();
            Assert.That(rangedAttacks[0], Is.EqualTo(11));
            Assert.That(rangedAttacks[1], Is.EqualTo(6));
            Assert.That(rangedAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void SizeModifierDoesNotDetermineNumberOfRangedBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.SizeModifier = 1;

            var rangedAttacks = baseAttack.AllRangedBonuses.ToArray();
            Assert.That(rangedAttacks[0], Is.EqualTo(11));
            Assert.That(rangedAttacks[1], Is.EqualTo(6));
            Assert.That(rangedAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void RacialModifierDoesNotDetermineNumberOfRangedBonuses()
        {
            baseAttack.BaseBonus = 10;
            baseAttack.RacialModifier = 1;

            var rangedAttacks = baseAttack.AllRangedBonuses.ToArray();
            Assert.That(rangedAttacks[0], Is.EqualTo(11));
            Assert.That(rangedAttacks[1], Is.EqualTo(6));
            Assert.That(rangedAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void CanHaveNegativeBonusInAllMeleeBonuses()
        {
            baseAttack.BaseBonus = 6;
            baseAttack.SizeModifier = -1;
            baseAttack.StrengthBonus = -1;

            var meleeAttacks = baseAttack.AllMeleeBonuses.ToArray();
            Assert.That(meleeAttacks[0], Is.EqualTo(4));
            Assert.That(meleeAttacks[1], Is.EqualTo(-1));
            Assert.That(meleeAttacks.Length, Is.EqualTo(2));
        }

        [Test]
        public void CanHaveNegativeBonusInAllRangedBonuses()
        {
            baseAttack.BaseBonus = 6;
            baseAttack.SizeModifier = -1;
            baseAttack.DexterityBonus = -1;

            var rangedAttacks = baseAttack.AllRangedBonuses.ToArray();
            Assert.That(rangedAttacks[0], Is.EqualTo(4));
            Assert.That(rangedAttacks[1], Is.EqualTo(-1));
            Assert.That(rangedAttacks.Length, Is.EqualTo(2));
        }
    }
}