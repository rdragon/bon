using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal sealed class FactoryMethodGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        private int _counter;

        public FactoryMethodGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<RecordDefinition> definitions)
        {
            foreach (var definition in definitions.Where(definition => !definition.HasValidConstructor))
            {
                AddFactoryMethod(definition);
            }
        }

        private void AddFactoryMethod(RecordDefinition definition)
        {
            var number = ++_counter;
            var parameterText = string.Join(", ", definition.Members.Select(member => $"{member.Definition.Type} {member.Name}"));
            var argumentText = string.Join(", ", definition.Members.Select(member => $"{member.Name} = {member.Name}"));

            _codeGenerator.AddMethod(
                $"private static {definition.Type} Construct{number}({parameterText})",
                "{",
                $"return new {definition.Type} {{ {argumentText} }};",
                "}");

            _codeGenerator.FactoryMethods.Add(definition.Type, $"Construct{number}");
        }
    }
}
