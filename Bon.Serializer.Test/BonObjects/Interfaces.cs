namespace Bon.Serializer.Test.BonObjects;

[BonObject]
[BonInclude(BonSerializerTestBase.DogId, typeof(Dog))]
[BonInclude(BonSerializerTestBase.CatId, typeof(Cat))]
public interface IAnimal;

[BonObject]
[BonInclude(BonSerializerTestBase.DogId, typeof(Dog))]
public interface IDog : IAnimal;

[BonObject]
[BonInclude(BonSerializerTestBase.CatId, typeof(Cat))]
public interface ICat : IAnimal;

[BonObject]
[BonInclude(BonSerializerTestBase.DogId, typeof(WithInt))]
[BonInclude(BonSerializerTestBase.CatId, typeof(WithString))]
public interface IAnimalImitation;

[BonObject]
[BonInclude(BonSerializerTestBase.DogId, typeof(WithNullableInt))]
[BonInclude(BonSerializerTestBase.CatId, typeof(WithString))]
public interface IAnimalFailedImitation;
