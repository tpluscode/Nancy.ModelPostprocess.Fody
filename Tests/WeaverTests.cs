using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class WeaverTests
{
    Assembly assembly;

    [TestFixtureSetUp]
    public void Setup()
    {
        assembly = WeaverHelper.WeaveAssembly();
    }

    [Test]
    public void Should_remove_attribute()
    {
        var type = assembly.GetType("AssemblyToProcess.Program", true);

        foreach (var method in type.GetMethods(BindingFlags.NonPublic|BindingFlags.Static))
        {
            Assert.That(method.GetCustomAttributes(true), Is.Empty);
        }
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}