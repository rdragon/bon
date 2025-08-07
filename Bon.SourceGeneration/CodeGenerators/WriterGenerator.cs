using Bon.SourceGeneration.Definitions;
using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration.CodeGenerators
{
    /// <summary>
    /// Generates source code that can serialize:
    /// 1. every type that has a BonObject attribute,
    /// 2. enums without this attribute that are used inside at least one type with a BonObject attribute.
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
                GetId(definition);
            }
        }

        private int GetId(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition.TypeNonNullable);
            var methodName = GetMethodName(definition, id);

            if (_writtenMethods.Add(methodName))
            {
                AddWriteMethod(definition, id);
                AddMethodToStore(definition, methodName);
            }

            return id;
        }

        private void AddMethodToStore(IDefinition definition, string methodName)
        {
            if (definition.IsReferenceType && !definition.IsNullable)
            {
                return;
            }

            _codeGenerator.AddStatement(
                $"bonFacade.AddWriter(typeof({definition.TypeForWriter}), {methodName});");
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

            if (definition.IsNullable)
            {
                AddWriteMethodForNullableRecord(definition, id);
            }
            else
            {
                AddWriteMethodForRecord(definition, id);
            }
        }

        private void AddWriteMethodForNullableRecord(RecordDefinition definition, int id)
        {
            var method = StartWriteMethod(id, definition, "maybeValue");

            method.Add($"if (maybeValue is not {{ }} value)");
            method.Add("{");
            WriteNull(method);
            method.Add("return;");
            method.Add("}");
            WriteNotNull(method);
            method.AddEmptyLine();

            Write(method, definition.SwapNullability(), "value");

            AddMethod(method);
        }

        private void WriteNull(List<string> method) => WriteByte(method, "NULL");

        private void WriteNotNull(List<string> method) => WriteByte(method, "NOT_NULL");

        private void WriteByte(List<string> method, string value)
        {
            method.Add($"NativeSerializer.WriteByte(writer, {value});");
        }

        private void AddWriteMethodForRecord(RecordDefinition definition, int id)
        {
            var method = StartWriteMethod(id, definition, "value");

            WriteMembers(method, definition);

            AddMethod(method);
        }

        private void WriteMembers(List<string> method, RecordDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                Write(method, member.Definition, member.IsVirtual ? "null" : $"value.{member.Name}");
            }
        }

        private void AddWriteMethod(UnionDefinition definition, int id)
        {
            // See bookmark 628227999 for all places where a union is serialized/deserialized.

            var method = StartWriteMethod(id, definition, "value");

            method.Add("if (value is null)");
            method.Add("{");
            method.Add($"IntSerializer.WriteNull(writer);");
            method.Add("return;");
            method.Add("}");
            method.AddEmptyLine();

            method.Add("switch (value)");
            method.Add("{");

            foreach (var member in definition.Members)
            {
                method.Add($"case {member.Definition.TypeNonNullable} obj:");
                method.Add($"IntSerializer.Write(writer, {member.Id});");
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

            method.Add("if (values is null)");
            method.Add("{");
            method.Add("IntSerializer.WriteNull(writer);");
            method.Add("return;");
            method.Add("}");
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
            method.Add($"IntSerializer.Write(writer, count);");

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

            method.Add("if (dictionary is null)");
            method.Add("{");
            method.Add("IntSerializer.WriteNull(writer);");
            method.Add("return;");
            method.Add("}");
            method.AddEmptyLine();

            method.Add("var count = dictionary.Count;");
            method.Add($"IntSerializer.Write(writer, count);");
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
                WriteNull(method);
                method.Add("return;");
                method.Add("}");
                WriteNotNull(method);
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
            new List<string> { $"public static void {GetMethodName(definition, id)}(BinaryWriter writer, {definition.Type} {parameterName})", "{" };

        /// <param name="lines">The lines of a method, but without the closing brace.</param>
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
                    lines.Add($"{native.GetWriteMethodName()}(writer, {argument});");
                    break;

                default:
                    var id = GetId(definition);
                    lines.Add($"{GetMethodName(definition, id)}(writer, {argument});");
                    break;
            }
        }

        private static string GetMethodName(IDefinition definition, int id)
        {
            var prefix = definition.IsNullable ? "WriteNullable" : "Write";
            return prefix + id;
        }
    }
}
