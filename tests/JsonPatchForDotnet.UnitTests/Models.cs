namespace JsonPatchForDotnet.UnitTests;

public class SubItem
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SubItem> SubItems { get; set; }
}

public class Root
{
    public List<Item> Items { get; set; }
}