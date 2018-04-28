using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace BasicFodyAddin.Fody
{
    public class ModuleWeaver: BaseModuleWeaver
    {
        public override void Execute()
        {
            var methodsToBeCached = ModuleDefinition.GetTypes()
                .SelectMany(x => x.Methods)
                .Where(y => y.CustomAttributes.Any(x => x.AttributeType.FullName == "BasicFodyAddin.CacheAttribute"));

            foreach (var method in methodsToBeCached)
            {
                AddCachingInstructions(method);
            }
        }

        public void AddCachingInstructions(MethodDefinition method)
        {
            method.Body.InitLocals = true;

            if(!MethodReturnsValue(method)) return;

            var processor = method.Body.GetILProcessor();
            var firstInstruction = method.Body.Instructions.First();

            var cacheKey = CreateCacheKeyString(method);

            var getCacheKeyInstruction = processor.Create(OpCodes.Ldstr, cacheKey);
            processor.InsertBefore(firstInstruction, getCacheKeyInstruction);

            var cachingAssemblyName = "BasicFodyAddin.dll";
            var containsMethod = ModuleDefinition.ReadModule(cachingAssemblyName).Types.SelectMany(x => x.Methods).FirstOrDefault(y => y.Name == "Contains");       

            var checkIfResultIsCached = processor.Create(OpCodes.Call, containsMethod);
            processor.InsertBefore(firstInstruction, checkIfResultIsCached);

            var test = cacheKey;
        }


        static bool MethodReturnsValue(MethodDefinition method)
        {
            ICollection<Instruction> returnInstructions =
                method.Body.Instructions.ToList().Where(x => x.OpCode == OpCodes.Ret).ToList();

            return returnInstructions.Count > 0;
        }

        private static string CreateCacheKeyString(MethodDefinition methodDefinition)
        {
            var builder = new StringBuilder();

            builder.Append(methodDefinition.DeclaringType.FullName);
            builder.Append(".");

            if (methodDefinition.IsSpecialName && (methodDefinition.IsSetter || methodDefinition.IsGetter))
            {
                builder.Append(Regex.Replace(methodDefinition.Name, "[gs]et_", string.Empty));
            }
            else
            {
                builder.Append(methodDefinition.Name);

                for (var i = 0; i < methodDefinition.Parameters.Count + methodDefinition.GenericParameters.Count; i++)
                {
                    builder.Append($"_{{{i}}}");
                }
            }

            return builder.ToString();
        }


        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
            //yield return "BasicFodyAddin";
            //yield return "System.Collections.Generic";
            //yield return "System.Boolean";
            //yield return "System.String";
        }


        public override bool ShouldCleanReference => true;
    }
}