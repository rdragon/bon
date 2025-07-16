using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates a factory method for each record that does not have a valid constructor.
    /// This factory method is used during deserialization.
    /// </summary>
    internal sealed class FactoryMethodGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        private int _counter;

        public FactoryMethodGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        /// <summary>
        /// Generates a factory method for each record that does not have a valid constructor.
        /// This factory method is used during deserialization.
        /// </summary>
        public void Run(IEnumerable<RecordDefinition> definitions)
        {
            foreach (var definition in definitions.OfType<RecordDefinition>())
            {
                if (!definition.IsNullableValueType && !definition.HasValidConstructor)
                {
                    AddFactoryMethod(definition);
                }
            }
        }

        private void AddFactoryMethod(RecordDefinition definition)
        {
            var number = ++_counter;
            var parameterText = string.Join(", ", definition.Members.Select(member => $"{member.Definition.Type} {member.Name}"));
            var argumentText = string.Join(", ", definition.Members.Select(member => $"{member.Name} = {member.Name}"));

            _codeGenerator.AddMethod(
                $"private static {definition.TypeNonNullable} Construct{number}({parameterText})",
                "{",
                $"return new {definition.TypeNonNullable} {{ {argumentText} }};",
                "}");

            _codeGenerator.FactoryMethods.Add(definition.TypeNonNullable, $"Construct{number}");
        }
    }
}
