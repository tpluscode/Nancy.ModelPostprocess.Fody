using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;

using Newtonsoft.Json;

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
    public void Modified_module_should_execute_postprocessor()
    {
        // given
        var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(this.assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
        var browser = new Browser(bootstrapper);

        // when
        var response = browser.Get("Model", bc=>bc.Accept(MediaRange.FromString("application/json")));

        // then
        dynamic model = JsonConvert.DeserializeObject(response.Body.AsString());
        Assert.That((string)model.someValue, Is.EqualTo("Set in postprocessing"));
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}