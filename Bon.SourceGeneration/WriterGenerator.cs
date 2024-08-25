using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Generates source code that can serialize every type that has a BonObject attribute.
    /// </summary>
    internal sealed class WriterGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        public WriterGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        private readonly HashSet<string> _writtenMethods = new HashSet<string>();

        public void Run(IEnumerable<IDefinition> definitions)
        {
            _codeGenerator.StartNewSection();

            foreach (var definition in definitions)
            {
                if (definition.IsNullable && definition.IsReferenceType)
                {
                    continue;
                }

                var id = GetId(definition);
                _codeGenerator.AddStatement($"bonFacade.AddWriter<{definition.Type}>({GetMethodName(definition.IsNullable)}{id});");
            }
        }

        private int GetId(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition.TypeNonNullable);

            if (_writtenMethods.Add(definition.Type))
            {
                AddWriteMethod(definition, id);
            }

            return id;
        }

        private void AddWriteMethod(IDefinition definition, int id)
        {
            switch (definition)
            {
                case EnumDefinition enumDefinition:
                    AddWriteMethod(enumDefinition, id);
                    break;

                case RecordDefinition recordDefinition:
                    AddWriteMethod(recordDefinition, id);
                    break;

                case UnionDefinition unionDefinition:
                    AddWriteMethod(unionDefinition, id);
                    break;

                case ArrayDefinition arrayDefinition:
                    AddWriteMethod(arrayDefinition, id);
                    break;

                case DictionaryDefinition dictionaryDefinition:
                    AddWriteMethod(dictionaryDefinition, id);
                    break;

                case ITupleDefinition tupleDefinition:
                    AddWriteMethod(tupleDefinition, id);
                    break;

                default:
                    throw new ArgumentException($"Cannot handle '{definition}'.", nameof(definition));
            }
        }

        private void AddWriteMethod(EnumDefinition definition, int id)
        {
            var method = StartWriteMethod(id, definition, "value");

            Write(method, definition.UnderlyingDefinition, $"({definition.UnderlyingDefinition.Type})value");

            AddMethod(method);
        }

        private void AddWriteMethod(RecordDefinition definition, int id)
        {
            // See bookmark 831853187 for all places where a record is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "value");

            if (definition.IsNullable)
            {
                method.Add($"if (value is not {definition.TypeNonNullable} obj)");
                method.Add("{");
                Write(method, NativeDefinition.Byte, "NULL");
                method.Add("return;");
                method.Add("}");
                Write(method, NativeDefinition.Byte, "NOT_NULL");
                method.Add($"Write{id}(writer, obj);");
                AddMethod(method);

                return;
            }

            foreach (var member in definition.Members)
            {
                Write(method, member.Definition, $"value.{member.Name}");
            }

            AddMethod(method);
        }

        private void AddWriteMethod(UnionDefinition definition, int id)
        {
            // See bookmark 628227999 for all places where a union is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "value");

            if (definition.IsNullable)
            {
                method.Add("if (value is null)");
                method.Add("{");
                method.Add($"WholeNumberSerializer.WriteNull(writer);");
                method.Add("return;");
                method.Add("}");
                method.Add($"Write{id}(writer, value);");
                AddMethod(method);

                return;
            }

            method.Add("switch (value)");
            method.Add("{");

            foreach (var member in definition.Members)
            {
                method.Add($"case {member.Definition.Type} obj:");
                method.Add($"WholeNumberSerializer.Write(writer, {member.Id});");
                Write(method, member.Definition, "obj");
                method.Add("break;");
                method.AddEmptyLine();
            }

            method.Add($"default:");
            method.Add($"throw new InvalidOperationException($\"Cannot serialize '{{value.GetType()}}' as '{definition.Type}' because this type has not been registered via the [BonInclude] attribute.\");");
            method.Add("}");

            AddMethod(method);
        }

        private void AddWriteMethod(ArrayDefinition definition, int id)
        {
            // See bookmark 791351735 for all places where an array is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "values");

            if (definition.IsNullable)
            {
                method.Add("if (values is null)");
                method.Add("{");
                method.Add("WholeNumberSerializer.WriteNull(writer);");
                method.Add("return;");
                method.Add("}");
            }

            method.AddEmptyLine();
            var values = "values";
            var collectionType = definition.CollectionType;

            if (collectionType == CollectionType.IEnumerable)
            {
                method.Add("var array = values.ToArray();");
                values = "array";
                collectionType = CollectionType.Array;
            }

            method.Add($"var count = {values}.{GetCountMemberName(collectionType)};");
            method.Add($"WholeNumberSerializer.Write(writer, count);");

            if (collectionType == CollectionType.Array && definition.ElementDefinition.Type == "byte")
            {
                method.Add($"writer.Write({values});");
            }
            else
            {
                method.Add("for (var i = 0; i < count; i++)");
                method.Add("{");
                Write(method, definition.ElementDefinition, $"{values}[i]");
                method.Add("}");
            }

            AddMethod(method);
        }

        private void AddWriteMethod(DictionaryDefinition definition, int id)
        {
            // See bookmark 662741575 for all places where a dictionary is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "dictionary");

            if (definition.IsNullable)
            {
                method.Add("if (dictionary is null)");
                method.Add("{");
                method.Add("WholeNumberSerializer.WriteNull(writer);");
                method.Add("return;");
                method.Add("}");
            }

            method.AddEmptyLine();
            method.Add("var count = dictionary.Count;");
            method.Add($"WholeNumberSerializer.Write(writer, count);");
            method.Add("var actualCount = 0;");
            method.AddEmptyLine();
            method.Add("foreach (var (key, value) in dictionary)");
            method.Add("{");
            Write(method, definition.KeyDefinition, "key");
            Write(method, definition.ValueDefinition, "value");
            method.Add("actualCount++;");
            method.Add("}");
            method.AddEmptyLine();
            method.Add("if (actualCount != count)");
            method.Add("{");
            method.Add("throw new InvalidOperationException(\"Dictionary was modified.\");");
            method.Add("}");

            AddMethod(method);
        }

        private void AddWriteMethod(ITupleDefinition definition, int id)
        {
            // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "tuple");

            if (definition.IsNullable)
            {
                method.Add("if (tuple is null)");
                method.Add("{");
                Write(method, NativeDefinition.Byte, "NULL");
                method.Add("return;");
                method.Add("}");
                method.AddEmptyLine();

                Write(method, NativeDefinition.Byte, "NOT_NULL");
            }

            var dotValue = definition.IsNullable ? ".Value" : "";
            var counter = 1;

            foreach (var itemDefinition in definition.GetInnerDefinitions())
            {
                Write(method, itemDefinition, $"tuple{dotValue}.Item{counter++}");
            }

            AddMethod(method);
        }

        private static string GetCountMemberName(CollectionType collectionType)
        {
            if (collectionType == CollectionType.Array)
            {
                return "Length";
            }

            if (collectionType == CollectionType.List)
            {
                return "Count";
            }

            throw new ArgumentOutOfRangeException(nameof(collectionType), collectionType, null);
        }

        private static List<string> StartWriteMethod(int id, IDefinition definition, string parameterName) =>
            new List<string> { $"public static void {GetMethodName(definition.IsNullable)}{id}(BinaryWriter writer, {definition.Type} {parameterName})", "{" };

        private void AddMethod(IReadOnlyList<string> lines)
        {
            _codeGenerator.AddMethod(lines);
            _codeGenerator.AppendClassBody("}");
        }

        private void Write(List<string> lines, IDefinition definition, string argument)
        {
            switch (definition)
            {
                case NativeDefinition native:
                    lines.Add($"NativeSerializer.Write{native.SimpleType}(writer, {argument});");
                    break;

                case WeakDefinition weak:
                    lines.Add($"NativeSerializer.Write{weak.SimpleType}(writer, {argument});");
                    break;

                default:
                    lines.Add(Write(GetId(definition), definition.IsNullable, argument));
                    break;
            }
        }

        private string Write(int id, bool isNullable, string argument) => $"{GetMethodName(isNullable)}{id}(writer, {argument});";

        private static string GetMethodName(bool isNullable) => isNullable ? "WriteNullable" : "Write";
    }
}
