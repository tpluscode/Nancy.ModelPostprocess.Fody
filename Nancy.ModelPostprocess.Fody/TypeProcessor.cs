using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Nancy.ModelPostprocess.Fody
{
    public partial class ModuleWeaver
    {
        private void ProcessType(TypeDefinition type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                ProcessConstructor(ctor);
            }
        }

        /// <summary>
        /// Replaces all values set to routes with a call to hydra injection
        /// </summary>
        /// <example>
        /// Replaces
        /// 
        /// Get["aRoute"] = p => DoSomething(p);
        /// 
        /// with
        /// 
        /// Get["aRoute"] = HydraInjectionHelper.Inject(p => DoSomething(p), hydraInjector, this);
        /// </example>
        private void ProcessConstructor(MethodDefinition constructor)
        {
            bool hydraInjected = false;
            var hydraParam = constructor.Parameters.FirstOrDefault(x => x.ParameterType.FullName == InjectorType.FullName);
            Action foundAction;
            if (hydraParam == null)
            {
                hydraParam = new ParameterDefinition("hydraInjector", ParameterAttributes.None, InjectorType);
                foundAction = () => constructor.Parameters.Add(hydraParam);
            }
            else
            {
                foundAction = () => { };
            }

            foreach (var instruction in GetRouteSetters(constructor.Body).ToList())
            {
                var injectMethod = instruction.Item2 ? AsyncInjectMethod : InjectMethod;
                var ilProcessor = constructor.Body.GetILProcessor();
                ilProcessor.InsertBefore(instruction.Item1, Instruction.Create(OpCodes.Ldarg, hydraParam));
                ilProcessor.InsertBefore(instruction.Item1, Instruction.Create(OpCodes.Ldarg_0));
                ilProcessor.InsertBefore(instruction.Item1, Instruction.Create(OpCodes.Call, injectMethod));
                hydraInjected = true;
            }

            if (hydraInjected)
            {
                foundAction();
            }
        }

        private IEnumerable<Tuple<Instruction, bool>> GetRouteSetters(MethodBody body)
        {
            for (int index = 0; index < body.Instructions.Count; index++)
            {
                bool isAsync;
                var instruction = body.Instructions[index];

                if (IsCallToRouteSetter(instruction, out isAsync)
                    && (!IsCallToHydraInjector(body.Instructions[index - 1])))
                {
                    yield return Tuple.Create(instruction, isAsync);
                }
            }
        }

        private bool IsCallToHydraInjector(Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Call;
        }

        private bool IsCallToRouteSetter(Instruction instruction, out bool isAsync)
        {
            var calledMethod = instruction.Operand as MethodReference;

            if (instruction.OpCode != OpCodes.Callvirt || 
                calledMethod == null || 
                calledMethod.Name != "set_Item" || 
                calledMethod.DeclaringType.FullName != RouteBuilderType.FullName)
            {
                isAsync = false;
                return false;
            }

            isAsync = calledMethod.Resolve().Parameters.Any(p => p.ParameterType.Name == "Func`3");
            return true;
        }
    }
}