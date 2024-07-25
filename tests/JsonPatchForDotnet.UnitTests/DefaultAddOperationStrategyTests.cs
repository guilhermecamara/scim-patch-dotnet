namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class DefaultAddOperationStrategyTests
{
    #region Default Target
    
    [TestMethod]
    public async Task ApplyAsync_ShouldSetValue()
    {
        // Arrange
        var testInstance = new TestClass() { Value = 100 };
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        var result = await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(42, testInstance.Value);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public async Task ApplyAsync_ShouldNotSetValue()
    {
        // Arrange
        var testInstance = new TestClass();
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: "42"
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        var result = await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(0, testInstance.Value);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRevertValue()
    {
        // Arrange
        var testInstance = new TestClass { Value = 42 };
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 100
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync(); // Apply new value
        var result =await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.AreEqual(42, testInstance.Value);
        Assert.IsTrue(result);
    }
    
    class TestClass
    {
        public int Value { get; set; }
    }
    
    #endregion
    
    #region IList? Target

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToNullNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = null };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.IsNotNull(testInstance.Items);
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(42, testInstance.Items[0]);
    }

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToEmptyNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int>() };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(42, testInstance.Items[0]);
    }

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToNullableListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(2, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(42, testInstance.Items[1]);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveLastValueFromNullableListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync(); // Apply new value
        await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
    }
    
    [TestMethod]
    public async Task RevertAsync_ShouldRemoveLastValueFromNullNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = null };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 1
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync(); // Apply new value
        await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsNull(testInstance.Items);
    }

    public class TestClassWithNullableList
    {
        public IList<int>? Items { get; set; }
    }

    #endregion
    
    #region IList? Target

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToNullList()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = null };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.IsNotNull(testInstance.Items);
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(42, testInstance.Items[0]);
    }

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToEmptyList()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = new List<int>() };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(42, testInstance.Items[0]);
    }

    [TestMethod]
    public async Task ApplyAsync_ShouldAddValueToListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync();

        // Assert
        Assert.AreEqual(2, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(42, testInstance.Items[1]);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveLastValueFromListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        )
        {
            OperationStrategy = new DefaultAddOperationStrategy()
        };

        // Act
        await operationNode.TryApplyAsync(); // Apply new value
        await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
    }

    public class TestClassWithList
    {
        public IList<int>? Items { get; set; }
    }

    #endregion
}
