using Bon.SourceGeneration.Definitions;
using System.Collections.Generic;

namespace Bon.SourceGeneration.CodeGenerators
{
    internal class CodeGenerator
    {
        public Dictionary<string, string> FactoryMethods { get; } = new Dictionary<string, string>();

        private readonly List<string> _class = new List<string>();
        private readonly List<string> _runMethod = new List<string>();
        private readonly Dictionary<string, int> _definitionIds = new Dictionary<string, int>();
        private bool _newSectionStarting;

        public CodeGenerator(ContextClass contextClass)
        {
            StartClass(contextClass);
        }

        public void StartNewSection() => _newSectionStarting = true;

        public void AddMethod(params string[] lines) => AddMethod((IReadOnlyList<string>)lines);

        public void AddMethod(IReadOnlyList<string> lines)
        {
            _class.AddEmptyLine();
            _class.AddRange(lines);
        }

        public void AppendClassBody(params string[] lines) => _class.AddRange(lines);

        public void AddStatement(params string[] lines)
        {
            if (_newSectionStarting)
            {
                _runMethod.AddEmptyLine();
                _newSectionStarting = false;
            }

            _runMethod.AddRange(lines);
        }

        private void StartClass(ContextClass contextClass)
        {
            AppendClassBody(
                "#nullable enable",
                "",
                "// v3",
                "using System;",
                "using System.Collections.Generic;",
                "using System.IO;",
                "using System.Linq;",
                "using System.Threading;",
                "using System.Threading.Tasks;",
                "using Bon.Serializer;",
                "using Bon.Serializer.Schemas;",
                "using Bon.Serializer.Serialization;",
                "using Bon.Serializer.Deserialization;",
                "",
                $"namespace {contextClass.NamespaceName};",
                "",
                $"sealed partial class {contextClass.ClassName} : IBonSerializerContext",
                "{",
                "public ISourceGenerationContext SourceGenerationContext => MySourceGenerationContext.Default;",
                "",
                "private sealed class MySourceGenerationContext : ISourceGenerationContext",
                "{",
                "private const byte NULL = 255;",
                "private const byte NOT_NULL = 254;",
                "",
                "public static MySourceGenerationContext Default { get; } = new();");
        }

        private void EndClass()
        {
            AddMethod("public void Run(BonFacade bonFacade)", "{");
            _class.AddRange(_runMethod);
            AppendClassBody("}", "}", "}");
        }

        public IReadOnlyList<string> Build()
        {
            EndClass();

            return _class;
        }

        public string TryGetFactoryMethod(IDefinition definition)
        {
            return FactoryMethods.TryGetValue(definition.TypeNonNullable, out var factoryMethod) ? factoryMethod : null;
        }

        public int GetId(IDefinition definition) => GetId(definition.Type);

        public int GetId(string type)
        {
            if (!_definitionIds.TryGetValue(type, out var id))
            {
                id = _definitionIds.Count;
                _definitionIds[type] = id;
            }

            return id;
        }
    }
}
