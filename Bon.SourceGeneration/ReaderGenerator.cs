﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Generates source code that can deserialize every type that has a BonObject attribute.
    /// However these deserializers can only handle input with exactly the right schema.
    /// </summary>
    internal sealed class ReaderGenerator
    {
        private readonly CodeGenerator _codeGenerator;

        private readonly HashSet<string> _writtenMethods = new HashSet<string>();

        public ReaderGenerator(CodeGenerator codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        public void Run(IEnumerable<IDefinition> definitions)
        {
            _codeGenerator.StartNewSection();

            foreach (var definition in definitions)
            {
                var id = GetId(definition);

                _codeGenerator.AddStatement(
                    $"bonFacade.AddDeserializer(" +
                    $"{definition.TypeOf}, " +
                    $"{definition.IsNullable.GetName()}, " +
                    $"(Bon.Serializer.Deserialization.Read<{definition.Type}>){GetMethodName(definition.IsNullable)}{id});");
            }
        }

        private int GetId(IDefinition definition)
        {
            var id = _codeGenerator.GetId(definition.TypeNonNullable);

            if (_writtenMethods.Add(definition.Type))
            {
                AddReadMethod(definition, id);
            }

            return id;
        }

        private void AddReadMethod(IDefinition definition, int id)
        {
            switch (definition)
            {
                case EnumDefinition enumDefinition:
                    AddReadMethod(enumDefinition, id);
                    break;

                case RecordDefinition recordDefinition:
                    AddReadMethod(recordDefinition, id);
                    break;

                case UnionDefinition unionDefinition:
                    AddReadMethod(unionDefinition, id);
                    break;

                case ArrayDefinition arrayDefinition:
                    AddReadMethod(arrayDefinition, id);
                    break;

                case DictionaryDefinition dictionaryDefinition:
                    AddReadMethod(dictionaryDefinition, id);
                    break;

                case ITupleDefinition tupleDefinition:
                    AddReadMethod(tupleDefinition, id);
                    break;

                default:
                    throw new ArgumentException($"Cannot handle '{definition}'.", nameof(definition));
            }
        }

        private void AddReadMethod(EnumDefinition definition, int id)
        {
            var method = StartReadMethod(id, definition);
            method.Add($"return ({definition.Type}){Read(definition.UnderlyingDefinition)};");
            AddMethod(method);
        }

        private void AddReadMethod(RecordDefinition definition, int id)
        {
            // See bookmark 831853187 for all places where a record is serialized/deserialized.

            var method = StartReadMethod(id, definition);

            if (definition.IsNullable)
            {
                method.Add($"return {Read(NativeDefinition.Byte)} == NULL ? null : {Read(id, false)};");
                AddMethod(method);

                return;
            }

            foreach (var member in definition.Members)
            {
                method.Add($"var arg{member.ConstructorIndex} = {Read(member.Definition)};");
            }

            var args = string.Join(", ", Enumerable.Range(0, definition.Members.Count).Select(i => $"arg{i}"));

            method.AddEmptyLine();
            method.Add($"return {definition.GetLongConstructorName(_codeGenerator)}({args});");

            AddMethod(method);
        }

        private void AddReadMethod(ArrayDefinition definition, int id)
        {
            var method = StartReadMethod(id, definition);

            if (definition.IsNullable)
            {
                method.Add("if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count) return null;");
            }
            else
            {
                method.Add("var count = (int)WholeNumberSerializer.Read(input.Reader);");
            }

            if (definition.ReadCollectionType == ReadCollectionType.Array && definition.ElementDefinition.Type == "byte")
            {
                method.Add("return input.Reader.ReadBytes(count);");
                AddMethod(method);

                return;
            }

            method.Add($"var values = {definition.GetConstructor("count")};");
            method.Add("for (var i = 0; i < count; i++)");
            method.Add("{");

            if (definition.ReadCollectionType == ReadCollectionType.Array)
            {
                method.Add($"values[i] = {Read(definition.ElementDefinition)};");
            }
            else if (definition.ReadCollectionType == ReadCollectionType.List)
            {
                method.Add($"values.Add({Read(definition.ElementDefinition)});");
            }
            else
            {
                throw new InvalidOperationException();
            }

            method.Add("}");
            method.AddEmptyLine();
            method.Add("return values;");

            AddMethod(method);
        }

        private void AddReadMethod(DictionaryDefinition definition, int id)
        {
            var method = StartReadMethod(id, definition);

            if (definition.IsNullable)
            {
                method.Add("if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count) return null;");
            }
            else
            {
                method.Add("var count = (int)WholeNumberSerializer.Read(input.Reader);");
            }

            method.Add($"var dictionary = {definition.GetConstructor("count")};");
            method.Add("for (var i = 0; i < count; i++)");
            method.Add("{");

            method.Add($"var key = {Read(definition.KeyDefinition)};");
            method.Add($"var value = {Read(definition.ValueDefinition)};");
            method.Add("dictionary[key] = value;");

            method.Add("}");
            method.AddEmptyLine();
            method.Add("return dictionary;");

            AddMethod(method);
        }

        private void AddReadMethod(ITupleDefinition definition, int id)
        {
            var method = StartReadMethod(id, definition);

            if (definition.IsNullable)
            {
                method.Add($"var firstByte = {Read(NativeDefinition.Byte)};");
                method.Add("if (firstByte == NULL) return null;");
            }

            var counter = 0;

            foreach (var itemDefinition in definition.GetInnerDefinitions())
            {
                method.Add($"var item{++counter} = {Read(itemDefinition)};");
            }

            method.AddEmptyLine();

            var items = Enumerable.Range(1, counter).Select(i => $"item{i}");
            method.Add($"return ({string.Join(", ", items)});");

            AddMethod(method);
        }

        private void AddReadMethod(UnionDefinition definition, int id)
        {
            var method = StartReadMethod(id, definition);

            if (definition.IsNullable)
            {
                method.Add($"return ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is int id) ? ReadNow{id}(input, id) : null;");
            }
            else
            {
                AddReadNowMethod(definition, id);
                method.Add($"return ReadNow{id}(input, (int)WholeNumberSerializer.Read(input.Reader));");
            }

            AddMethod(method);
        }

        private void AddReadNowMethod(UnionDefinition definition, int id)
        {
            var method = StartReadNowMethod(id, definition.Type);
            method.Add("return id switch");
            method.Add("{");

            foreach (var member in definition.Members)
            {
                method.Add($"{member.Id} => {Read(member.Definition)},");
            }

            method.Add("_ => throw new InvalidOperationException(),");
            method.Add("};");

            AddMethod(method);
        }

        private static List<string> StartReadMethod(int id, IDefinition definition) =>
            new List<string> { $"private static {definition.Type} {GetMethodName(definition.IsNullable)}{id}(BonInput input)", "{" };

        private static List<string> StartReadNowMethod(int id, string type) =>
            new List<string> { $"private static {type} ReadNow{id}(BonInput input, int id)", "{" };

        private void AddMethod(IReadOnlyList<string> lines)
        {
            _codeGenerator.AddMethod(lines);
            _codeGenerator.AppendClassBody("}");
        }

        private string Read(IDefinition definition)
        {
            switch (definition)
            {
                case NativeDefinition native:
                    return $"NativeSerializer.Read{native.SimpleType}(input.Reader)";

                case WeakDefinition weak:
                    return $"NativeSerializer.Read{weak.SimpleType}(input.Reader)";

                default:
                    return Read(GetId(definition), definition.IsNullable);
            }
        }

        private string Read(int id, bool isNullable) => $"{GetMethodName(isNullable)}{id}(input)";

        private static string GetMethodName(bool isNullable) => isNullable ? "ReadNullable" : "Read";
    }
}
