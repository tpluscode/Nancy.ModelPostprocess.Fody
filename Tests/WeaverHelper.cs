using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Nancy.ModelPostprocess.Fody;

public class WeaverHelper
{
    public static Assembly WeaveAssembly()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
        var assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssembly, true);

        var moduleDefinition = ModuleDefinition.ReadModule(newAssembly);
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = new DefaultAssemblyResolver(),
                                  LogError = Console.Error.Write,
                                  LogInfo = Console.Out.Write,
                                  LogWarning = Console.Out.Write
                              };

        weavingTask.Execute();
        moduleDefinition.Write(newAssembly);

        return Assembly.LoadFile(newAssembly);
    }
}