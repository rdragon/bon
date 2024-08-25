using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    internal sealed class EnumConversionGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public EnumConversionGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<EnumDefinition> enumDefinitions)
        {
            _codeGenerator.StartNewSection();

            foreach (var definition in enumDefinitions)
            {
                AddEnumData(definition);
            }
        }

        private void AddEnumData(EnumDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);
            var type = definition.Type;
            var underlyingType = definition.UnderlyingDefinition.Type;

            _codeGenerator.AddMethod(
                $"private static Delegate AddEnumCast{id}(Delegate read)",
                "{",
                $"var f = (Read<{underlyingType}>)read;",
                $"return (Read<{type}>)(input => ({type})f(input));",
                "}");

            _codeGenerator.AddStatement($"bonFacade.AddEnumData(" +
                $"{definition.TypeOf}, " +
                $"{definition.UnderlyingDefinition.TypeOf}, " +
                $"(Func<Delegate, Delegate>)AddEnumCast{id});");
        }
    }
}
