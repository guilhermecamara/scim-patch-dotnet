namespace ScimPatch.IntegrationTests;

public class NestedObject
{
    public string Name { get; set; } = null!;
    public int? NestedInt { get; set; }
}

public class DeepNestedObject
{
    public string? Title { get; set; }
    public DateTime? NestedDate { get; set; }
    public List<NestedObject> NestedList { get; set; } = [];
}

public class TargetObject
{
    public int IntProperty { get; set; }
    public int? NullableIntProperty { get; set; }
    public long LongProperty { get; set; }
    public long? NullableLongProperty { get; set; }
    public DateTime DateProperty { get; set; }
    public DateTime? NullableDateProperty { get; set; }
    public string? StringProperty { get; set; }
    public bool BoolProperty { get; set; }
    public bool? NullableBoolProperty { get; set; }
    public List<string> StringList { get; set; } = [];
    public List<int>? NullableIntList { get; set; }
    public DeepNestedObject NestedObject { get; set; } = new DeepNestedObject();
    public List<DeepNestedObject> NestedList { get; set; } = [];

    internal static TargetObject Mock()
    {
        return new TargetObject
        {
            IntProperty = 10,
            NullableIntProperty = null,
            LongProperty = 1000000L,
            NullableLongProperty = 2000000L,
            DateProperty = new DateTime(2023, 8, 19, 0, 0, 0,  DateTimeKind.Utc),
            NullableDateProperty = null,
            StringProperty = "Initial String",
            BoolProperty = true,
            NullableBoolProperty = null,
            StringList = ["Item1", "Item2", "Item3"],
            NullableIntList = new List<int> { 1, 2, 3 },
            NestedObject = new DeepNestedObject
            {
                Title = "Nested Initial",
                NestedDate = new DateTime(2023, 8, 19, 0, 0, 0,  DateTimeKind.Utc),
                NestedList =
                [
                    new NestedObject { Name = "Nested 1", NestedInt = 10 },
                    new NestedObject { Name = "Nested 2", NestedInt = 20 }
                ]
            },
            NestedList =
            [
                new DeepNestedObject
                {
                    Title = "Deep Nested 1",
                    NestedDate = new DateTime(2023, 8, 19, 0, 0, 0,  DateTimeKind.Utc),
                    NestedList =
                    [
                        new NestedObject { Name = "Deep Nested 1-1", NestedInt = 10 },
                        new NestedObject { Name = "Deep Nested 1-2", NestedInt = 20 }
                    ]
                },

                new DeepNestedObject
                {
                    Title = "Deep Nested 2",
                    NestedDate = null,
                    NestedList = []
                }
            ]
        };
    }
}