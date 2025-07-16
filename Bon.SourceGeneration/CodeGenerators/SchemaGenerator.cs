using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates for each and every definition it can find a schema (except the native definitions, as for those we can
    /// use the NativeSchema class).
    /// It then publishes the schemas of all found <see cref="ICriticalDefinition"/>s.
    /// </summary>
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

            foreach (var definition in definitions.OfType<ICriticalDefinition>())
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
                !(definition is NativeDefinition) &&
                definitions.Add(definition)))
            {
                AddDefinitionsRecursive(definitions, definition.GetInnerDefinitions());
            }
        }

        private void CreateSchema(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);
            var schemaType = $"SchemaType.{definition.SchemaType}";

            _codeGenerator.AppendClassBody(
                $"var schema{id} = {definition.SchemaBaseClass}.Create({schemaType});");
        }

        private void UpdateSchema(IDefinition definition)
        {
            if (definition is ICustomDefinition customDefinition)
            {
                UpdateCustomSchema(customDefinition);

                return;
            }

            var id = _codeGenerator.GetId(definition);
            var arguments = string.Join(", ", definition.GetInnerDefinitions().Select(GetSchemaArgument));

            if (arguments.Length > 0)
            {
                _codeGenerator.AppendClassBody(
                    $"schema{id}.SetInnerSchemas({arguments});");
            }
        }

        private void UpdateCustomSchema(ICustomDefinition definition)
        {
            var id = _codeGenerator.GetId(definition);
            var arguments = string.Join(", ", definition.Members.Select(GetMemberArgument));

            _codeGenerator.AppendClassBody(
                $"schema{id}.SetMembers({arguments});");
        }

        private string GetSchemaArgument(IDefinition definition)
        {
            if (definition is NativeDefinition nativeDefinition)
            {
                return nativeDefinition.SchemaIdentifier;
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
