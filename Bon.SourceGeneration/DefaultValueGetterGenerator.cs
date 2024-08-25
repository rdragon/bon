using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal sealed class DefaultValueGetterGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public DefaultValueGetterGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<RecordDefinition> recordDefinitions, IEnumerable<UnionDefinition> unionDefinitions)
        {
            _codeGenerator.StartNewSection();

            foreach (var definition in recordDefinitions)
            {
                AddDefaultValueGetterMethod(definition);
            }

            _codeGenerator.StartNewSection();

            foreach (var definition in unionDefinitions)
            {
                AddUnionStatement(definition);
            }
        }

        private void AddDefaultValueGetterMethod(RecordDefinition definition)
        {
            Debug.Assert(!definition.IsNullable);
            var type = definition.Type;
            var members = definition.Members.OrderBy(member => member.ConstructorIndex);
            var arguments = members.Select(member => GetDefaultValue(member.Definition));
            var id = _codeGenerator.GetId(definition);

            _codeGenerator.AddMethod(
                $"private static {type} GetDefaultValue{id}(BonInput input)",
                "{",
                $"return {definition.GetShortConstructorName(_codeGenerator)}({string.Join(", ", arguments)});",
                "}");

            _codeGenerator.AddStatement($"bonFacade.AddDefaultValueGetter({definition.TypeOf}, (Read<{type}>)GetDefaultValue{id});");
        }

        private void AddUnionStatement(UnionDefinition definition)
        {
            var recordDefinition = definition.Members[0].Definition;
            var type = recordDefinition.Type;
            var id = _codeGenerator.GetId(recordDefinition);
            _codeGenerator.AddStatement($"bonFacade.AddDefaultValueGetter({definition.TypeOf}, (Read<{type}>)GetDefaultValue{id});");
        }

        private string GetDefaultValue(IDefinition definition)
        {
            if (definition.IsNullable)
            {
                return "null";
            }

            if (definition is RecordDefinition)
            {
                return $"GetDefaultValue{_codeGenerator.GetId(definition)}(input)";
            }

            if (definition is UnionDefinition unionDefinition && unionDefinition.Members.Count > 0)
            {
                return GetDefaultValue(unionDefinition.Members[0].Definition);
            }

            if (definition is ITupleDefinition tupleDefinition)
            {
                var items = tupleDefinition.GetInnerDefinitions().Select(GetDefaultValue);

                return $"({string.Join(", ", items)})";
            }

            if (definition.IsValueType)
            {
                return "default";
            }

            if (definition is ArrayDefinition)
            {
                return "[]";
            }

            if (definition is DictionaryDefinition dictionaryDefinition)
            {
                return dictionaryDefinition.GetDefaultValue();
            }

            if (definition.SchemaType == SchemaType.String)
            {
                return "\"\"";
            }

            throw new ArgumentException($"Cannot handle '{definition}'.", nameof(definition));
        }
    }
}
