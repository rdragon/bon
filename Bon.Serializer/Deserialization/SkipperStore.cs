namespace Bon.Serializer.Deserialization;

internal sealed class SkipperStore(DeserializerStore deserializerStore) : IUseReflection
{
    private const byte NULL = NativeWriter.NULL;

    /// <summary>
    /// Returns a method that reads binary data formatted according to the schema and throws away the result.
    /// </summary>
    public Action<BonInput> GetSkipper(Schema schema)
    {
        return schema switch
        {
            { IsArray: true } => GetArraySkipper(schema),
            { IsDictionary: true } => GetDictionarySkipper(schema),
            { IsTuple2: true } => GetTuple2Skipper(schema),
            { IsTuple3: true } => GetTuple3Skipper(schema),
            { IsNative: true } => GetNativeSkipper(schema),
            { IsRecord: true } => GetRecordSkipper(schema),
            { IsUnion: true } => GetUnionSkipper(schema),
        };
    }

    private Action<BonInput> GetArraySkipper(Schema schema)
    {
        var skipElement = GetSkipper(schema.SchemaArguments[0]);

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int count)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                skipElement(input);
            }
        };
    }

    private Action<BonInput> GetDictionarySkipper(Schema schema)
    {
        var skipKey = GetSkipper(schema.SchemaArguments[0]);
        var skipValue = GetSkipper(schema.SchemaArguments[1]);

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int count)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                skipKey(input);
                skipValue(input);
            }
        };
    }

    private Action<BonInput> GetTuple2Skipper(Schema schema)
    {
        var skipItem1 = GetSkipper(schema.SchemaArguments[0]);
        var skipItem2 = GetSkipper(schema.SchemaArguments[1]);

        if (schema.IsNullable)
        {
            return (BonInput input) =>
            {
                if (input.Reader.ReadByte() == NULL)
                {
                    return;
                }

                skipItem1(input);
                skipItem2(input);
            };
        }

        return (BonInput input) =>
        {
            skipItem1(input);
            skipItem2(input);
        };
    }

    private Action<BonInput> GetTuple3Skipper(Schema schema)
    {
        var skipItem1 = GetSkipper(schema.SchemaArguments[0]);
        var skipItem2 = GetSkipper(schema.SchemaArguments[1]);
        var skipItem3 = GetSkipper(schema.SchemaArguments[2]);

        if (schema.IsNullable)
        {
            return (BonInput input) =>
            {
                if (input.Reader.ReadByte() == NULL)
                {
                    return;
                }

                skipItem1(input);
                skipItem2(input);
                skipItem3(input);
            };
        }

        return (BonInput input) =>
        {
            skipItem1(input);
            skipItem2(input);
            skipItem3(input);
        };
    }

    private Action<BonInput> GetNativeSkipper(Schema schema) => deserializerStore.GetNativeSkipper(schema.SchemaType);

    private Action<BonInput> GetRecordSkipper(Schema schema)
    {
        Action<BonInput>[]? skippers = null;

        if (schema.IsNullable)
        {
            return (BonInput input) =>
            {
                if (input.Reader.ReadByte() == NULL)
                {
                    return;
                }

                skippers ??= getSkippers();

                foreach (var skipper in skippers)
                {
                    skipper(input);
                }
            };
        }

        return (BonInput input) =>
        {
            skippers ??= getSkippers();

            foreach (var skipper in skippers)
            {
                skipper(input);
            }
        };

        Action<BonInput>[] getSkippers() => schema.Members.Select(member => GetSkipper(member.Schema)).ToArray();
    }

    private Action<BonInput> GetUnionSkipper(Schema schema)
    {
        var skippers = schema.Members.ToDictionary(member => member.Id, member => GetSkipper(member.Schema));

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int id)
            {
                return;
            }

            var skip = skippers[id];
            skip(input);
        };
    }
}
