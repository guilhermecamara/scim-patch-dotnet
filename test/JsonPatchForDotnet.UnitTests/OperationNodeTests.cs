using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class OperationNodeTests
{
    [TestMethod]
    public void FromOperation_ShouldCreateOperationNodes_ForAddOperationOnRootName()
    {
        // Arrange
        var root = Root.MockRoot();
        var name = "Groot";
        
        var operation = new Operation
        {
            OperationType = OperationType.Add,
            Path = "Name",
            Value = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(name))
        };

        // Act
        var operationNodes = OperationNode.FromOperation(operation, root);

        // Assert
        Assert.AreEqual(1, operationNodes.Count);
        var node = operationNodes[0];
        Assert.IsNotNull(node.OperationStrategy);
        Assert.IsInstanceOfType(node.OperationStrategy, typeof(DefaultAddOperationStrategy));
        node.Value.Should().BeEquivalentTo(name);
    }
    
    [TestMethod]
    public void FromOperation_ShouldCreateOperationNodes_ForAddOperationOnRootItems()
    {
        // Arrange
        var root = Root.MockRoot();
        var item = new Item
        {
            Id = 3,
            Name = "Item3",
            SubItems = new List<SubItem>
            {
                new SubItem { Id = 1, Name = "SubItem3-1" },
                new SubItem { Id = 2, Name = "SubItem3-2" }
            }
        };
        
        var operation = new Operation
        {
            OperationType = OperationType.Add,
            Path = "Items",
            Value = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(item))
        };

        // Act
        var operationNodes = OperationNode.FromOperation(operation, root);

        // Assert
        Assert.AreEqual(1, operationNodes.Count);
        var node = operationNodes[0];
        Assert.IsNotNull(node.OperationStrategy);
        Assert.IsInstanceOfType(node.OperationStrategy, typeof(DefaultAddOperationStrategy));
        node.Value.Should().BeEquivalentTo(item);
    }

    [TestMethod]
    public void FromOperation_ShouldThrowExceptionForInvalidPath()
    {
        // Arrange
        var root = Root.MockRoot();
        var operation = new Operation
        {
            OperationType = OperationType.Add,
            Path = "InvalidPath"
        };

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => OperationNode.FromOperation(operation, root));
    }
    
    [TestMethod]
    public void GetTargetObjects_ShouldReturnCorrectObjectsAndPath()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var (objects, lastPath) = OperationNode
            .GetTargetObjects("Items.SubItems.Name", root);

        // Assert
        Assert.AreEqual(2, objects.Count());
        Assert.AreSame(root.Items[0].SubItems, objects[0]);
        Assert.AreSame(root.Items[1].SubItems, objects[1]);

        Assert.AreEqual(lastPath, "Name");
    }
    
    [TestMethod]
    public void GetTargetObjectsWithFilter_ShouldReturnCorrectObjectsAndPath()
    {
        // Arrange
        var root = Root.MockRoot();
        
        // Act
        var (objects, lastPath) = OperationNode
            .GetTargetObjects("Items[Name eq \"Item1\"].SubItems[not(Id eq 1)].Name", root);

        // Assert
        Assert.AreEqual(1, objects.Count());
        root.Items[0].SubItems.Where(si => si.Id != 1).Should().BeEquivalentTo((IEnumerable<object>)objects[0]);

        Assert.AreEqual(lastPath, "Name");
    }
    
    [TestMethod]
    public void GetSourceObject_ShouldReturnCorrectObjectAndPath()
    {
        // Arrange
        var root = Root.MockRoot();

        // Act
        var (sourceObject, lastPath) = OperationNode
            .GetSourceObject("Items", root);

        // Assert
        Assert.AreSame(root, sourceObject);
        Assert.AreEqual("Items", lastPath);
    }

    [TestMethod]
    public void GetSourceObject_ShouldThrowException()
    {
        // Arrange
        var root = Root.MockRoot();

        // Act
        var action = () => OperationNode
            .GetSourceObject("Items.SubItems.Name", root);

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(() => action());
        Assert.AreEqual("Sequence contains more than one element", exception.Message);
    }
}