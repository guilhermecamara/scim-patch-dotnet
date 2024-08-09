namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class PathResolverTests
{
    [TestMethod]
    public void GetProperties_PathIsArray_ShouldReturnItemsProperty()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var objects = PathResolver.GetProperties(root, ["Items"]);
        
        // Assert
        Assert.AreEqual(1, objects.Count());
        Assert.AreSame(objects.ElementAt(0), root.Items);
    }
    
    [TestMethod]
    public void GetProperties_PathIsArray__ShouldReturnItemsNameProperties()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var objects = PathResolver.GetProperties(root, ["Items", "Name"]);
        
        // Assert
        Assert.AreEqual(2, objects.Count());
        Assert.AreEqual(objects.ElementAt(0), "Item1");
        Assert.AreEqual(objects.ElementAt(1), "Item2");
    }
    
    [TestMethod]
    public void GetProperties_PathIsArray__ShouldReturnItemsSubItemsNameProperties()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var objects = PathResolver.GetProperties(root, ["Items", "SubItems", "Name"]);
        
        // Assert
        Assert.AreEqual(4, objects.Count());
        Assert.AreEqual(objects.ElementAt(0), "SubItem1-1");
        Assert.AreEqual(objects.ElementAt(1), "SubItem1-2");
        Assert.AreEqual(objects.ElementAt(2), "SubItem2-1");
        Assert.AreEqual(objects.ElementAt(3), "SubItem2-2");
    }
    
    [TestMethod]
    public void GetProperties_PathIsArray__ShouldReturnItemsSubItemsElementsProperties()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var objects = PathResolver.GetProperties(root, ["Items", "SubItems"]);
        
        // Assert
        Assert.AreEqual(2, objects.Count());
        Assert.AreSame(objects.ElementAt(0), root.Items[0].SubItems);
        Assert.AreSame(objects.ElementAt(1), root.Items[1].SubItems);
    }
    
    [TestMethod]
    public void GetProperties_PathIsNotArray_ShouldReturnItemsProperty()
    {
        // Arrange
        var root = Root.MockRoot();

        // Act
        var objects = PathResolver.GetProperties(root, "Items");
        
        // Assert
        Assert.AreEqual(1, objects.Count());
        Assert.AreSame(objects.ElementAt(0), root.Items);
    }
    
    [TestMethod]
    public void GetProperties_PathIsNotArray_ShouldReturnItemsNameProperties()
    {
        // Arrange
        var root = Root.MockRoot();

        // Act
        var objects = PathResolver.GetProperties(root.Items, "Name");
        
        // Assert
        Assert.AreEqual(2, objects.Count());
        Assert.AreEqual(objects.ElementAt(0), "Item1");
        Assert.AreEqual(objects.ElementAt(1), "Item2");
    }
}