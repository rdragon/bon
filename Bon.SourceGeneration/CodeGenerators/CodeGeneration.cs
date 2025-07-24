using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.CodeGenerators
{
    internal static class CodeGeneration
    {
        /// <param name="foundDefinitions">All classes, structs, interfaces and enums with a BonObject attribute.</param>
        public static CodeGeneratorOutput GetCode(IEnumerable<IDefinition> foundDefinitions, ContextClass contextClass)
        {
            DebugOutput.PrintDefinitions(foundDefinitions, "foundDefinitions.txt");
            var codeGenerator = new CodeGenerator(contextClass);
            var factoryMethodGenerator = new FactoryMethodGenerator(codeGenerator);
            var schemaGenerator = new SchemaGenerator(codeGenerator);
            var writerGenerator = new WriterGenerator(codeGenerator);
            var readerGenerator = new ReaderGenerator(codeGenerator);
            var memberTypeGenerator = new MemberTypeGenerator(codeGenerator);
            var enumDataGenerator = new EnumDataGenerator(codeGenerator);
            var readerFactoryGenerator = new ReaderFactoryGenerator(codeGenerator);

            var definitions = GetCriticalDefinitions(foundDefinitions, new HashSet<string>()).ToArray();
            DebugOutput.PrintDefinitions(definitions.Cast<IDefinition>(), "criticalDefinitions.txt");
            factoryMethodGenerator.Run(definitions.OfType<RecordDefinition>());
            schemaGenerator.Run(definitions);
            writerGenerator.Run(definitions);
            readerGenerator.Run(definitions);
            memberTypeGenerator.Run(definitions.OfType<ICustomDefinition>());
            enumDataGenerator.Run(definitions.OfType<EnumDefinition>());
            readerFactoryGenerator.Run(definitions.OfType<RecordDefinition>());

            return new CodeGeneratorOutput(Helper.Indent(codeGenerator.Build()), contextClass.ClassName);
        }

        /// <summary>
        /// Returns definitions of all class, struct, interface and enum types with the BonObject attribute.
        /// For value types, both a nullable and a non-nullable version is returned.
        /// For generic types, a definition is returned for every combination of concrete type parameters that is encountered.
        /// Definitions of enum types that do not have the BonObject attribute but that can be found as a property of another
        /// definition are also returned.
        /// </summary>
        private static IEnumerable<ICriticalDefinition> GetCriticalDefinitions(IEnumerable<IDefinition> definitions, HashSet<string> types)
        {
            return definitions.SelectMany(definition => GetCriticalDefinitions(definition, types));
        }

        /// <summary>
        /// Returns <paramref name="definition"/> if its a valid critical definition (plus its nullable/non-nullable counterpart for
        /// value types) and then recursively loops over the inner definitions of the definition.
        /// </summary>
        private static IEnumerable<ICriticalDefinition> GetCriticalDefinitions(IDefinition definition, HashSet<string> types)
        {
            if (definition is ICriticalDefinition criticalDefinition)
            {
                if (!types.Add(criticalDefinition.TypeNonNullable))
                {
                    yield break;
                }

                yield return criticalDefinition;

                if (criticalDefinition is RecordDefinition || criticalDefinition is EnumDefinition)
                {
                    yield return criticalDefinition.SwapNullability();
                }
            }

            foreach (var innerCriticalDefinition in GetCriticalDefinitions(definition.GetInnerDefinitions(), types))
            {
                yield return innerCriticalDefinition;
            }
        }
    }
}
