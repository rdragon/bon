using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal static class CodeGeneratorHelper
    {
        /// <param name="foundDefinitions">All classes, structs, interfaces and enums with a BonObject attribute.</param>
        public static CodeGeneratorOutput GetCode(IEnumerable<IDefinition> foundDefinitions, ContextClass contextClass)
        {
            var definitions = GetMajorDefinitions(foundDefinitions, new HashSet<string>()).ToArray();
            var nullableDefinitions = definitions.Select(definition => definition.ToNullable()).ToArray();
            var allDefinitions = definitions.Concat(nullableDefinitions).ToArray();
            var recordDefinitions = definitions.OfType<RecordDefinition>().ToArray();
            var unionDefinitions = definitions.OfType<UnionDefinition>().ToArray();
            var allEnumDefinitions = allDefinitions.OfType<EnumDefinition>().ToArray();

            var codeGenerator = new CodeGenerator(contextClass);
            var factoryMethodGenerator = new FactoryMethodGenerator(codeGenerator);
            var schemaGenerator = new SchemaGenerator(codeGenerator);
            var writerGenerator = new WriterGenerator(codeGenerator);
            var readerGenerator = new ReaderGenerator(codeGenerator);
            var memberTypeGenerator = new MemberTypeGenerator(codeGenerator);
            var enumConversionGenerator = new EnumConversionGenerator(codeGenerator);
            var defaultValueGetterGenerator = new DefaultValueGetterGenerator(codeGenerator);
            var readerFactoryGenerator = new ReaderFactoryGenerator(codeGenerator);

            factoryMethodGenerator.Run(recordDefinitions);
            schemaGenerator.Run(allDefinitions);
            writerGenerator.Run(allDefinitions);
            readerGenerator.Run(allDefinitions);
            memberTypeGenerator.Run(definitions.OfType<ICustomDefinition>());
            enumConversionGenerator.Run(allEnumDefinitions);
            defaultValueGetterGenerator.Run(recordDefinitions, unionDefinitions);
            readerFactoryGenerator.Run(recordDefinitions);

            return new CodeGeneratorOutput(Helper.Indent(codeGenerator.Build()), contextClass.ClassName);
        }

        /// <summary>
        /// Returns all major definitions found in the definitions (either directly in the collection or as inner definition).
        /// Only returns the non-nullable version of each definition.
        /// 
        /// There are two reasons why the inner definitions are also considered:
        /// 1. For generic types, every combination of concrete type arguments given to the generic type will result in
        ///    a separate (generic) type. Moreover, the version of the generic type that still has its type arguments (which
        ///    will be the one directly in the collection) will not be returned from this method.
        /// 2. You might want to use existing enums in a record, but you cannot add a BonObject attribute to an existing enum.
        ///    Therefore, these enums cannot be found directly in the collection, but they can be found as inner definitions.
        /// </summary>
        public static IEnumerable<IMajorDefinition> GetMajorDefinitions(IEnumerable<IDefinition> definitions, HashSet<string> types)
        {
            return definitions.SelectMany(definition => GetMajorDefinitions(definition, types)).OrderBy(definition => definition.Type);
        }

        private static IEnumerable<IMajorDefinition> GetMajorDefinitions(IDefinition definition, HashSet<string> types)
        {
            if ((definition as RecordDefinition)?.IsConcreteType == false ||
                definition is NativeDefinition ||
                definition is WeakDefinition)
            {
                yield break;
            }

            if (definition is IMajorDefinition majorDefinition)
            {
                majorDefinition = majorDefinition.ToNonNullable();

                if (!types.Add(majorDefinition.Type))
                {
                    yield break;
                }

                yield return majorDefinition;
            }

            foreach (var innerMajorDefinition in GetMajorDefinitions(definition.GetInnerDefinitions(), types))
            {
                yield return innerMajorDefinition;
            }
        }
    }
}
