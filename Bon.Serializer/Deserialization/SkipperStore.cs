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
            ArraySchema arraySchema => GetArraySkipper(arraySchema),
            DictionarySchema dictionarySchema => GetDictionarySkipper(dictionarySchema),
            Tuple2Schema tuple2Schema => GetTuple2Skipper(tuple2Schema),
            Tuple3Schema tuple3Schema => GetTuple3Skipper(tuple3Schema),
            NativeSchema nativeSchema => GetNativeSkipper(nativeSchema),
            RecordSchema recordSchema => GetRecordSkipper(recordSchema),
            UnionSchema unionSchema => GetUnionSkipper(unionSchema),
        };
    }

    private Action<BonInput> GetArraySkipper(ArraySchema schema)
    {
        var skipElement = GetSkipper(schema.InnerSchema);

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

    private Action<BonInput> GetDictionarySkipper(DictionarySchema schema)
    {
        var skipKey = GetSkipper(schema.InnerSchema1);
        var skipValue = GetSkipper(schema.InnerSchema2);

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

    private Action<BonInput> GetTuple2Skipper(Tuple2Schema schema)
    {
        var skipItem1 = GetSkipper(schema.InnerSchema1);
        var skipItem2 = GetSkipper(schema.InnerSchema2);

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

    private Action<BonInput> GetTuple3Skipper(Tuple3Schema schema)
    {
        var skipItem1 = GetSkipper(schema.InnerSchema1);
        var skipItem2 = GetSkipper(schema.InnerSchema2);
        var skipItem3 = GetSkipper(schema.InnerSchema3);

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

    private Action<BonInput> GetNativeSkipper(NativeSchema schema) => deserializerStore.GetNativeSkipper(schema.SchemaType);

    private Action<BonInput> GetRecordSkipper(RecordSchema schema)
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

    private Action<BonInput> GetUnionSkipper(UnionSchema schema)
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
