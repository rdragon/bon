using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal sealed class SchemaGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public SchemaGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<IDefinition> definitions)
        {
            definitions = GetMoreDefinitions(definitions);

            _codeGenerator.AddMethod(
                "public void UpdateSchemaStore(BonFacade bonFacade)",
                "{");

            foreach (var definition in definitions)
            {
                CreateSchema(definition);
            }

            foreach (var definition in definitions)
            {
                UpdateSchema(definition);
            }

            foreach (var definition in definitions.OfType<IMajorDefinition>())
            {
                PushSchema(definition);
            }

            _codeGenerator.AppendClassBody("}");
        }

        private IReadOnlyList<IDefinition> GetMoreDefinitions(IEnumerable<IDefinition> definitions)
        {
            var result = new HashSet<IDefinition>(TypeComparer.Instance);

            AddDefinitionsRecursive(result, definitions);

            return result.ToArray();
        }

        private void AddDefinitionsRecursive(HashSet<IDefinition> definitions, IEnumerable<IDefinition> definitionsToAdd)
        {
            foreach (var definition in definitionsToAdd.Where(definition =>
                !(definition is NativeDefinition || definition is WeakDefinition) &&
                definitions.Add(definition)))
            {
                AddDefinitionsRecursive(definitions, definition.GetInnerDefinitions());
            }
        }

        private void CreateSchema(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);
            var schemaType = $"SchemaType.{definition.SchemaType}";
            var isNullable = definition.IsNullable.ToStringLower();

            _codeGenerator.AppendClassBody(
                $"var schema{id} = {definition.SchemaBaseClass}.Create({schemaType}, {isNullable});");
        }

        private void UpdateSchema(IDefinition definition)
        {
            if (definition is ICustomDefinition customDefinition)
            {
                UpdateCustomSchema(customDefinition);

                return;
            }

            var id = _codeGenerator.GetId(definition);
            var argument = string.Join(", ", definition.GetInnerDefinitions().Select(GetSchemaArgument));

            if (argument.Length > 0)
            {
                _codeGenerator.AppendClassBody(
                    $"schema{id}.SetInnerSchemas({argument});");
            }
        }

        private void UpdateCustomSchema(ICustomDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);
            var argument = string.Join(", ", definition.Members.Select(GetMemberArgument));

            _codeGenerator.AppendClassBody(
                $"schema{id}.SetMembers({argument});");
        }

        private string GetSchemaArgument(IDefinition definition)
        {
            if (definition is NativeDefinition)
            {
                return $"NativeSchema.{definition.SchemaType}";
            }

            if (definition is WeakDefinition)
            {
                return $"NativeSchema.{definition.SchemaType}";
            }

            var id = _codeGenerator.GetId(definition);

            return $"schema{id}";
        }

        private string GetMemberArgument(IMember member)
        {
            return $"new SchemaMember({member.Id}, {GetSchemaArgument(member.Definition)})";
        }

        private void PushSchema(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);

            _codeGenerator.AppendClassBody(
                $"bonFacade.AddSchema({definition.TypeOf}, schema{id});");
        }
    }
}
