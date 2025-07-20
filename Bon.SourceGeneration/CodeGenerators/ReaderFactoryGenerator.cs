using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates a reader factory for each record (for values types even two).
    /// These reader factories are used by the Bon serializer when deserializing a record that was serialized using an "unexpected" schema.
    /// </summary>
    internal class ReaderFactoryGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public ReaderFactoryGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<RecordDefinition> definitions)
        {
            _codeGenerator.StartNewSection();
            var index = 0;

            foreach (var definition in definitions)
            {
                AddReaderFactory(definition, index);
                index++;
            }
        }

        private void AddReaderFactory(RecordDefinition definition, int index)
        {
            var members = definition.Members;

            var parameters = Enumerable.Range(0, members.Count)
                .SelectMany(i => new[] {
                    $"Action<BonInput> skip{i}",
                    $"Func<Delegate> createReader{i}" })
                .Concat(new[] { "Action<BonInput> skipRest" });

            var parameterText = string.Join(", ", parameters);
            var methodName = $"CreateReader{index}";

            _codeGenerator.AddMethod(
                $"private static Read<{definition.Type}> {methodName}({parameterText})",
                "{");

            for (int i = 0; i < members.Count; i++)
            {
                _codeGenerator.AppendClassBody(
                    $"Read<{members[i].Definition.Type}> read{i} = null!;");
            }

            _codeGenerator.AppendClassBody(
                "",
                "return (BonInput input) =>",
                "{");

            if (members.Count > 0)
            {
                _codeGenerator.AppendClassBody(
                    $"if (read{members.Count - 1} is null)",
                    "{");

                for (int i = 0; i < members.Count; i++)
                {
                    _codeGenerator.AppendClassBody(
                        $"read{i} = (Read<{members[i].Definition.Type}>)createReader{i}();");
                }

                _codeGenerator.AppendClassBody(
                    "}");
            }

            for (int i = 0; i < members.Count; i++)
            {
                _codeGenerator.AppendClassBody(
                    $"skip{i}?.Invoke(input);",
                    $"var arg{i} = read{i}(input);");
            }

            var argText = string.Join(", ", Enumerable.Range(0, members.Count).Select(i => $"arg{i}"));

            _codeGenerator.AppendClassBody(
                "skipRest?.Invoke(input);",
                $"return {definition.GetLongConstructorName(_codeGenerator)}({argText});",
                "};",
                "}");

            _codeGenerator.AddStatement($"bonFacade.AddReaderFactory({definition.TypeOf}, (Delegate){methodName});");
        }
    }
}
