using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates for each enum two weak deserializer factories (one for the nullable and one for the non-nullable version). 
    /// </summary>
    internal sealed class EnumDataGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public EnumDataGenerator(CodeGenerator codeGenerator)
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
            var type = definition.Type;
            var underlyingType = definition.UnderlyingDefinition.Type;

            _codeGenerator.AddStatement($"bonFacade.AddWeakDeserializerFactory<{type}, {underlyingType}>(x => ({type})x);");
        }
    }
}
