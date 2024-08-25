namespace Bon.FileInspector.Test;

// If you add any classes you'll need to update the test SchemaToText and possibly the test RecursiveClassToJson.
// Keep the classes in alphabetical order.

[BonObject] internal sealed record class EmptyClass();

[BonObject] internal sealed record class RecursiveClass([property: BonMember(1)] int X, [property: BonMember(2)] RecursiveClass? Y);

[BonObject] internal sealed record class WithArray([property: BonMember(1)] EmptyClass[] X);

[BonObject] internal sealed record class WithDictionary([property: BonMember(2)] Dictionary<string, int?> X);

[BonObject] internal sealed record class WithTuple([property: BonMember(3)] (char, DateTime) X);
