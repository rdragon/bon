using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates for each and every definition it can find a schema (except the native definitions).
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
                "public void LoadSchemas(Action<Type, Schema> onSchemaLoaded)",
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
            var layoutId = GetLayoutId(definition);

            _codeGenerator.AppendClassBody(
                $"var schema{id} = Schema.Create(SchemaType.{definition.SchemaType}, layoutId: {layoutId});");
        }

        private int GetLayoutId(IDefinition definition)
        {
            // Bookmark 458282233
            // At this moment we can't yet give the schema a "real" layout ID, as we have no way of obtaining one.
            // However, at bookmark 557955753 we need to detect a recursive schema.
            // This is done via the layout ID field.
            // Therefore we generate here a temporary layout ID that can be used to detect recursion.
            // We generate negative values to make it clear those are not yet the real layout IDs.
            // We base the IDs on the non-nullable version of the definition, as for detecting recursion nullability shouldn't matter.
            return definition is ICustomDefinition ? -_codeGenerator.GetId(definition.TypeNonNullable) : 0;
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
                    $"schema{id}.SchemaArguments = [{arguments}];");
            }
        }

        private void UpdateCustomSchema(ICustomDefinition definition)
        {
            var id = GetId(definition);

            var arguments = string.Join(", ", definition.Members.Select(GetMemberArgument));

            _codeGenerator.AppendClassBody(
                $"schema{id}.Members = [{arguments}];");
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
                $"onSchemaLoaded(typeof({definition.TypeForWriter}), schema{id});");
        }

        private int GetId(IDefinition definition) => _codeGenerator.GetId(definition.TypeForWriter);
    }
}
