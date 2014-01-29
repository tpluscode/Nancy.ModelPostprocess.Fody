using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    // Will log an informational message to MSBuild
    public Action<string> LogWarning { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    public ModuleDefinition ModuleDefinition { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogWarning = m => { if (m != null) Console.WriteLine(m); };
    }

    public void Execute()
    {
        foreach (var typeDefinition in ModuleDefinition.Types)
        {
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                var attributes=methodDefinition.CustomAttributes.Where(a => a.AttributeType.Name == "AOPAttribute").ToList();

                if (attributes.Any())
                {
                    Inject(methodDefinition);
                    RemoveAttributes(methodDefinition,attributes);
                    LogWarning(string.Format("Applied AOP to method {0}", methodDefinition.FullName));
                }
            }
        }

        RemoveReference();
    }

    private void RemoveReference()
    {
        var references = ModuleDefinition.AssemblyReferences.Where(aRef => aRef.Name.StartsWith("BasicFodyAddin"));
        foreach (var reference in references.ToList())
        {
            ModuleDefinition.AssemblyReferences.Remove(reference);
            LogWarning("Removed weaver reference");
        }
    }

    private void RemoveAttributes(MethodDefinition methodDefinition, IEnumerable<CustomAttribute> attributes)
    {
        foreach (var attr in attributes)
        {
            methodDefinition.CustomAttributes.Remove(attr);
        }
    }

    private void Inject(MethodDefinition methodDefinition)
    {
        var worker = methodDefinition.Body.GetILProcessor();

        worker.InsertBefore(methodDefinition.Body.Instructions[0], worker.Create(OpCodes.Ldstr,
                                          string.Format("Wejście: {0} o {{0}}", methodDefinition.Name)));

        var dateTime =
            ModuleDefinition.Import(typeof(DateTime)).Resolve().Properties.First(p => p.Name == "Now").GetMethod;


        var dateTimeNowRef = ModuleDefinition.Import(dateTime);

        var instr = worker.Create(OpCodes.Call, dateTimeNowRef);
        worker.InsertAfter(methodDefinition.Body.Instructions[0], instr);
        var instr2 = worker.Create(OpCodes.Box, ModuleDefinition.Import(typeof(DateTime)));
        worker.InsertAfter(instr, instr2);

        var console = ModuleDefinition.Import(typeof(System.Console));

        var consoleWriteLn = console.Resolve().Methods.Where(
            m => m.Name == "WriteLine" && m.IsPublic);
        var consoleMethod =
            consoleWriteLn.First(m => m.Parameters.Count == 2 && m.Parameters[0].ParameterType.FullName == "System.String");
        var consoleMethodRef = ModuleDefinition.Import(consoleMethod);

        worker.InsertAfter(instr2, worker.Create(OpCodes.Call, consoleMethodRef));

        var last = methodDefinition.Body.Instructions.Last(il => il.OpCode == OpCodes.Ret);
        worker.InsertBefore(last, worker.Create(OpCodes.Ldstr, string.Format("Wyjście: {0} o {{0}}", methodDefinition.Name)));
        worker.InsertBefore(last, worker.Create(OpCodes.Call, dateTimeNowRef));
        worker.InsertBefore(last, worker.Create(OpCodes.Box, ModuleDefinition.Import(typeof(DateTime))));
        worker.InsertBefore(last, worker.Create(OpCodes.Call, consoleMethodRef));
    }
}