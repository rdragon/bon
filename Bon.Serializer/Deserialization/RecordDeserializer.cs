﻿namespace Bon.Serializer.Deserialization;

// See bookmark 831853187 for all places where a record is serialized/deserialized.
internal sealed class RecordDeserializer(
     DeserializerStore deserializerStore,
     SkipperStore skipperStore,
     DefaultValueGetterFactory defaultValueGetterFactory) : IUseReflection
{
    private const byte NULL = 255;

    /// <summary>
    /// Contains for each record a factory method that creates a deserializer for that record.
    /// 
    /// The deserializer method accepts an array of functions.
    /// Half of the functions are skippers. These skip members of the source schema that are not present in the target schema.
    /// 
    /// The other half are reader factories. These produce readers that generate all members of the target schema.
    /// For every member of the target schema there is one reader factory.
    /// The produced readers either read a member from the input stream and transform it to the target type, or read nothing and
    /// return a default value. The latter is the case when the source schema does not contain a member that corresponds
    /// to the target schema member.
    /// 
    /// The reason for using factories instead of readers directly is that schemas can be recursive.
    /// </summary>
    public Dictionary<Type, Delegate> ReaderFactories { get; } = [];

    public Read<T> CreateDeserializer<T>(RecordSchema sourceSchema, RecordSchema targetSchema)
    {
        var deserialize = CreateDeserializerNow<T>(sourceSchema, targetSchema);

        if (sourceSchema.IsNullable)
        {
            var getDefaultValue = defaultValueGetterFactory.GetDefaultValueGetter<T>(targetSchema.IsNullable);

            return (BonInput input) =>
            {
                return input.Reader.ReadByte() == NULL ? getDefaultValue(input) : deserialize(input);
            };
        }

        return deserialize;
    }

    private Read<T> CreateDeserializerNow<T>(RecordSchema sourceSchema, RecordSchema targetSchema)
    {
        var index = 0;
        var arguments = new object?[targetSchema.Members.Count * 2 + 1];
        var sourceMembers = new MemberCollection(sourceSchema.Members);
        var nonNullableTargetType = typeof(T).UnwrapNullable();

        foreach (var targetMember in targetSchema.Members)
        {
            var targetMemberType = deserializerStore.MemberTypes[(nonNullableTargetType, targetMember.Id)];
            arguments[index++] = GetSkipper(sourceMembers, targetMember.Id - 1); // Bookmark 541895765
            arguments[index++] = GetReaderFactory(sourceMembers, targetMember, targetMemberType);
        }

        arguments[index] = GetSkipper(sourceMembers, int.MaxValue);

        return (Read<T>)ReaderFactories[typeof(T)].Method.Invoke(null, arguments)!;
    }

    private Action<BonInput>? GetSkipper(MemberCollection sourceMembers, int maxId)
    {
        return CombineSkippers(sourceMembers.PopAllUpTo(maxId).Select(GetSkipper).ToArray());
    }

    private Action<BonInput> GetSkipper(SchemaMember sourceMember) => skipperStore.GetSkipper(sourceMember.Schema);

    private static Action<BonInput>? CombineSkippers(IReadOnlyList<Action<BonInput>> skippers)
    {
        if (skippers.Count == 0)
        {
            return null;
        }

        var skip1 = skippers[0];

        if (skippers.Count == 1)
        {
            return skip1;
        }

        var skip2 = skippers[1];

        if (skippers.Count == 2)
        {
            return (BonInput input) =>
            {
                skip1(input);
                skip2(input);
            };
        }

        var skip3 = skippers[2];

        if (skippers.Count == 3)
        {
            return (BonInput input) =>
            {
                skip1(input);
                skip2(input);
                skip3(input);
            };
        }

        // Bookmark 563732229
        return (BonInput input) =>
        {
            for (int i = 0; i < skippers.Count; i++)
            {
                skippers[i](input);
            }
        };
    }

    private Delegate GetReaderFactory(MemberCollection sourceMembers, SchemaMember targetMember, Type targetMemberType)
    {
        if (sourceMembers.TryPopMember(targetMember.Id) is SchemaMember sourceMember)
        {
            return () => deserializerStore.GetDeserializer(sourceMember.Schema, targetMemberType, targetMember.IsNullable);
        }

        return () => defaultValueGetterFactory.GetDefaultValueGetter(targetMemberType, targetMember.IsNullable); // Bookmark 359877524
    }

    public class MemberCollection(IReadOnlyList<SchemaMember> members)
    {
        private int _index;

        public IEnumerable<SchemaMember> PopAllUpTo(int id)
        {
            while (_index < members.Count)
            {
                var member = members[_index];

                if (member.Id > id)
                {
                    yield break;
                }

                yield return member;
                _index++;
            }
        }

        public SchemaMember? TryPopMember(int id)
        {
            if (_index == members.Count)
            {
                return null;
            }

            var member = members[_index];

            if (member.Id == id)
            {
                _index++;
                return member;
            }

            return null;
        }
    }
}
