using Bon.Example;

BonSerializer serializer = await BonSerializer.CreateAsync(new BonSerializerContext(), "schemas");
byte[] bytes = serializer.Serialize(new Person { Age = 42 });
Person person = await serializer.DeserializeAsync<Person>(bytes);
Console.WriteLine(person.Age);
