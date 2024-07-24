namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class UnitTest1
{
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
    
    Root root = new Root
    {
        Items = new List<Item>
        {
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
        }
    };
    
    [TestMethod]
    public void GetProperties_PathIsArray_ShouldReturnItemsProperty()
    {
        var objects = PathResolver.GetProperties(root, ["Items"]);
        Assert.AreEqual(1, objects.Count());
        Assert.AreSame(objects.ElementAt(0), root.Items);
    }
    
    [TestMethod]
    public void GetProperties_PathIsArray__ShouldReturnItemsNameProperties()
    {
        var objects = PathResolver.GetProperties(root, ["Items", "Name"]);
        Assert.AreEqual(2, objects.Count());
        Assert.AreEqual(objects.ElementAt(0), "Item1");
        Assert.AreEqual(objects.ElementAt(1), "Item2");
    }
    
    [TestMethod]
    public void GetProperties_PathIsNotArray_ShouldReturnItemsProperty()
    {

        var objects = PathResolver.GetProperties(root, "Items");
        Assert.AreEqual(1, objects.Count());
        Assert.AreSame(objects.ElementAt(0), root.Items);
    }
    
    [TestMethod]
    public void GetProperties_PathIsNotArray_ShouldReturnItemsNameProperties()
    {
        var objects = PathResolver.GetProperties(root.Items, "Name");
        Assert.AreEqual(2, objects.Count());
        Assert.AreEqual(objects.ElementAt(0), "Item1");
        Assert.AreEqual(objects.ElementAt(1), "Item2");
    }
}