using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.IoC
{
    [TestFixture]
    [IoC]
    public abstract class IoCTests : IntegrationTests
    {
        protected void AssertNotSingleton<T>()
        {
            var first = GetNewInstanceOf<T>();
            var second = GetNewInstanceOf<T>();
            Assert.That(first, Is.Not.SameAs(second));
        }

        protected void AssertNotSingleton<T>(string name)
        {
            var first = GetNewInstanceOf<T>(name);
            var second = GetNewInstanceOf<T>(name);
            Assert.That(first, Is.Not.SameAs(second));
        }

        protected void AssertNamedIsInstanceOf<I, T>(string name)
            where T : I
        {
            AssertNotSingleton<I>(name);

            var instance = GetNewInstanceOf<I>(name);
            Assert.That(instance, Is.InstanceOf<T>());
        }
    }
}