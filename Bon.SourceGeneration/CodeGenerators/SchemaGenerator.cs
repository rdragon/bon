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
            definitions = GetAllDefinitions(definitions);

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

        private IReadOnlyList<IDefinition> GetAllDefinitions(IEnumerable<IDefinition> definitions)
        {
            var result = new Dictionary<int, IDefinition>();

            AddDefinitionsRecursive(result, definitions);

            return result.Values.ToArray();
        }

        private void AddDefinitionsRecursive(Dictionary<int, IDefinition> definitions, IEnumerable<IDefinition> definitionsToAdd)
        {
            foreach (var definition in definitionsToAdd.Where(definition =>
                !(definition is NativeDefinition) &&
                !definitions.ContainsKey(GetId(definition))))
            {
                definitions.Add(GetId(definition), definition);
                AddDefinitionsRecursive(definitions, definition.GetInnerDefinitions());
            }
        }

        private void CreateSchema(IDefinition definition)
        {
            var id = GetId(definition);
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

            var id = GetId(definition);
            var arguments = string.Join(", ", definition.GetInnerDefinitions().Select(GetSchemaArgument));

            if (arguments.Length > 0)
            {
                _codeGenerator.AppendClassBody(
                    $"schema{id}.SetInnerSchemas({arguments});");
            }
        }

        private void UpdateCustomSchema(ICustomDefinition definition)
        {
            var id = GetId(definition);
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

            var id = GetId(definition);

            return $"schema{id}";
        }

        private string GetMemberArgument(IMember member)
        {
            return $"new SchemaMember({member.Id}, {GetSchemaArgument(member.Definition)})";
        }

        private void PushSchema(IDefinition definition)
        {
            var id = GetId(definition);

            _codeGenerator.AppendClassBody(
                $"bonFacade.AddSchema(typeof({definition.TypeForWriter}), schema{id});");
        }

        private int GetId(IDefinition definition) => _codeGenerator.GetId(definition.TypeForWriter);
    }
}
