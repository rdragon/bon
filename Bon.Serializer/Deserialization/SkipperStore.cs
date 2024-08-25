namespace Bon.Serializer.Deserialization;

internal sealed class SkipperStore(DeserializerStore deserializerStore) : IUseReflection
{
    private const byte NULL = 255;

    public Action<BonInput> GetSkipper(Schema sourceSchema)
    {
        return sourceSchema switch
        {
            ArraySchema arraySchema => GetArraySkipper(arraySchema),
            DictionarySchema dictionarySchema => GetDictionarySkipper(dictionarySchema),
            Tuple2Schema tuple2Schema => GetTuple2Skipper(tuple2Schema),
            Tuple3Schema tuple3Schema => GetTuple3Skipper(tuple3Schema),
            NativeSchema nativeSchema => GetNativeSkipper(nativeSchema),
            RecordSchema recordSchema => GetRecordSkipper(recordSchema),
            UnionSchema unionSchema => GetUnionSkipper(unionSchema),
            _ => throw new ArgumentOutOfRangeException(nameof(sourceSchema), sourceSchema, null),
        };
    }

    private Action<BonInput> GetArraySkipper(ArraySchema sourceSchema)
    {
        var skipElement = GetSkipper(sourceSchema.InnerSchema);

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                skipElement(input);
            }
        };
    }

    private Action<BonInput> GetDictionarySkipper(DictionarySchema sourceSchema)
    {
        var skipKey = GetSkipper(sourceSchema.InnerSchema1);
        var skipValue = GetSkipper(sourceSchema.InnerSchema2);

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
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

    private Action<BonInput> GetTuple2Skipper(Tuple2Schema sourceSchema)
    {
        var skipItem1 = GetSkipper(sourceSchema.InnerSchema1);
        var skipItem2 = GetSkipper(sourceSchema.InnerSchema2);

        if (sourceSchema.IsNullable)
        {
            return (BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NULL)
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

    private Action<BonInput> GetTuple3Skipper(Tuple3Schema sourceSchema)
    {
        var skipItem1 = GetSkipper(sourceSchema.InnerSchema1);
        var skipItem2 = GetSkipper(sourceSchema.InnerSchema2);
        var skipItem3 = GetSkipper(sourceSchema.InnerSchema3);

        if (sourceSchema.IsNullable)
        {
            return (BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NULL)
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

    private Action<BonInput> GetNativeSkipper(NativeSchema sourceSchema)
    {
        var type = sourceSchema.AnnotatedSchemaType.ToNativeType();

        return (Action<BonInput>)this.GetPrivateMethod(nameof(CreateNativeSkipperFor))
            .MakeGenericMethod(type)
            .Invoke(this, [sourceSchema])!;
    }

    private Action<BonInput> CreateNativeSkipperFor<T>(Schema sourceSchema)
    {
        var reader = deserializerStore.GetDeserializer<T>(sourceSchema, typeof(T).IsNullable(true));

        return (BonInput input) =>
        {
            reader(input);
        };
    }

    private Action<BonInput> GetRecordSkipper(RecordSchema sourceSchema)
    {
        Action<BonInput>[]? skippers = null;

        if (sourceSchema.IsNullable)
        {
            return (BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NULL)
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

        Action<BonInput>[] getSkippers() => sourceSchema.Members.Select(member => GetSkipper(member.Schema)).ToArray();
    }

    private Action<BonInput> GetUnionSkipper(UnionSchema sourceSchema)
    {
        var skippers = sourceSchema.Members.ToDictionary(member => member.Id, member => GetSkipper(member.Schema));

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int id)
            {
                return;
            }

            var skip = skippers[id];
            skip(input);
        };
    }
}
