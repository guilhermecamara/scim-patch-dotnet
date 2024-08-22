using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScimPatch.UnitTests;

[TestClass]
public class OperationNodeTests
{
    #region Add
    
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
        node.TargetProperty.Should().BeSameAs(root.GetType().GetProperty("Name"));
        node.Instance.Should().BeSameAs(root);
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
        node.TargetProperty.Should().BeSameAs(root.GetType().GetProperty("Items"));
        node.Instance.Should().BeSameAs(root);
    }
    
    [TestMethod]
    public void FromOperation_ShouldThrowExceptionForInvalidPath_ForAddOperation()
    {
        // Arrange
        var root = Root.MockRoot();
        var operation = new Operation
        {
            OperationType = OperationType.Add,
            Path = "InvalidPath"
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => OperationNode.FromOperation(operation, root));
        Assert.AreEqual("Property not found in target object of type ScimPatch.UnitTests.Root (Parameter 'InvalidPath')", ex.Message);
        Assert.AreEqual("InvalidPath", ex.ParamName);
    }
    
    #endregion
    
    #region Remove
    
    [TestMethod]
    public void FromOperation_ShouldCreateOperationNodes_ForRemoveOperationOnRootName()
    {
        // Arrange
        var root = Root.MockRoot();
        
        var operation = new Operation
        {
            OperationType = OperationType.Remove,
            Path = "Name"
        };

        // Act
        var operationNodes = OperationNode.FromOperation(operation, root);

        // Assert
        Assert.AreEqual(1, operationNodes.Count);
        var node = operationNodes[0];
        Assert.IsNotNull(node.OperationStrategy);
        Assert.IsInstanceOfType(node.OperationStrategy, typeof(DefaultRemoveOperationStrategy));
        node.TargetProperty.Should().BeSameAs(root.GetType().GetProperty("Name"));
        node.Instance.Should().BeSameAs(root);
    }
    
    [TestMethod]
    public void FromOperation_ShouldCreateOperationNodes_ForRemoveOperationOnRootItemsItems()
    {
        // Arrange
        var root = Root.MockRoot();
        
        var operation = new Operation
        {
            OperationType = OperationType.Remove,
            Path = "Items[Id eq 1]"
        };

        // Act
        var operationNodes = OperationNode.FromOperation(operation, root);

        // Assert
        Assert.AreEqual(1, operationNodes.Count);
        var node = operationNodes[0];
        Assert.IsNotNull(node.OperationStrategy);
        Assert.IsInstanceOfType(node.OperationStrategy, typeof(DefaultRemoveOperationStrategy));
        node.Value.Should().BeEquivalentTo(root.Items.First(i => i.Id == 1));
        node.TargetProperty.Should().BeSameAs(root.GetType().GetProperty("Items"));
        node.Instance.Should().BeSameAs(root);
    }
    
    [TestMethod]
    public void FromOperation_ShouldCreateOperationNodes_ForRemoveOperationOnRootItems()
    {
        // Arrange
        var root = Root.MockRoot();
        
        var operation = new Operation
        {
            OperationType = OperationType.Remove,
            Path = "Items"
        };

        // Act
        var operationNodes = OperationNode.FromOperation(operation, root);

        // Assert
        Assert.AreEqual(1, operationNodes.Count);
        var node = operationNodes[0];
        Assert.IsNotNull(node.OperationStrategy);
        Assert.IsInstanceOfType(node.OperationStrategy, typeof(DefaultRemoveOperationStrategy));
        node.Value.Should().BeNull();
        node.TargetProperty.Should().BeSameAs(root.GetType().GetProperty("Items"));
        node.Instance.Should().BeSameAs(root);
    }
    
    [TestMethod]
    public void FromOperation_ShouldThrowExceptionForInvalidPath_ForRemoveOperation()
    {
        // Arrange
        var root = Root.MockRoot();
        var operation = new Operation
        {
            OperationType = OperationType.Remove,
            Path = "InvalidPath"
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => OperationNode.FromOperation(operation, root));
        Assert.AreEqual("Property not found in target object of type ScimPatch.UnitTests.Root (Parameter 'InvalidPath')", ex.Message);
        Assert.AreEqual("InvalidPath", ex.ParamName);
    }
    
    #endregion
    
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
        root.Items[0].SubItems.Where(si => si.Id != 1).Should().BeEquivalentTo(objects);

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
    public void GetSourceObjectWithFilter_ShouldReturnCorrectObjectAndPath()
    {
        // Arrange
        var root = Root.MockRoot();

        // Act
        var (sourceObject, lastPath) = OperationNode
            .GetSourceObject("Items[Name eq \"Item1\"].SubItems[not(Id eq 1)].Name", root);

        // Assert
        root.Items[0].SubItems.First(si => si.Id != 1).Should().BeEquivalentTo(sourceObject);

        Assert.AreEqual("Name", lastPath);
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