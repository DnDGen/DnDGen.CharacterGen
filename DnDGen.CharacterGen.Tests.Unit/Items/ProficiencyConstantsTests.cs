using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    [TestFixture]
    public class ProficiencyConstantsTests
    {
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}