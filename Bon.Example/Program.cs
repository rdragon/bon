using Bon.Example;

BonSerializer serializer = await BonSerializer.CreateAsync(new BonSerializerContext(), "schemas");
byte[] bytes = serializer.Serialize(new Person { Age = 42 });
Person? person = serializer.Deserialize<Person>(bytes);
Console.WriteLine(person?.Age); // 42
