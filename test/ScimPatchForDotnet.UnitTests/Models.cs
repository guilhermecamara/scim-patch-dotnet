namespace ScimPatchForDotnet.UnitTests;

public class SubItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<SubItem> SubItems { get; set; } = null!;
}

public class Root
{
    public string Name { get; set; } = null!;
    public List<Item> Items { get; set; } = null!;

    internal static Root MockRoot()
    {
        return new Root
        {
            Name = "Root",
            Items =
            [
                new Item
                {
                    Id = 1,
                    Name = "Item1",
                    SubItems = new List<SubItem>
                    {
                        new SubItem { Id = 1, Name = "SubItem1-1" },
                        new SubItem { Id = 2, Name = "SubItem1-2" }
                    }
                },

                new Item
                {
                    Id = 2,
                    Name = "Item2",
                    SubItems = new List<SubItem>
                    {
                        new SubItem { Id = 3, Name = "SubItem2-1" },
                        new SubItem { Id = 4, Name = "SubItem2-2" }
                    }
                }
            ]
        };
    }
}