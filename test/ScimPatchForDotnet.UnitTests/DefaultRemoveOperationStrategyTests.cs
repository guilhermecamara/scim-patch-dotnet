namespace ScimPatchForDotnet.UnitTests;

[TestClass]
public class DefaultRemoveOperationStrategyTests
{
    #region Default Target
    
    [TestMethod]
    public async Task ApplyAsync_ShouldRemoveValue()
    {
        // Arrange
        var testInstance = new TestClass { Value = 42 };
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: null
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.AreEqual(0, testInstance.Value);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRevertValue()
    {
        // Arrange
        var testInstance = new TestClass { Value = 42 };
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: null
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply remove
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreEqual(42, testInstance.Value);
    }
    
    class TestClass
    {
        public int Value { get; set; }
    }
    
    #endregion
    
    #region IList? Target

    [TestMethod]
    public async Task ApplyAsync_ShouldRemoveValueFromNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1, 2, 2, 3 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 2
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.AreEqual(3, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(2, testInstance.Items[1]);
        Assert.AreEqual(3, testInstance.Items[2]);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveValueFromNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1, 2, 2, 3 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 2
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreEqual(4, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(2, testInstance.Items[1]);
        Assert.AreEqual(3, testInstance.Items[2]);
        Assert.AreEqual(2, testInstance.Items[3]);

    }
    
    [TestMethod]
    public async Task ApplyAsync_ShouldSetNullToNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1, 2, 2, 3 }};
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: null
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsNull(testInstance.Items);
    }
    
    [TestMethod]
    public async Task RevertAsync_ShouldSetNullToNullableList()
    {
        // Arrange
        var list = new List<int> { 1, 2, 2, 3 };
        var testInstance = new TestClassWithNullableList { Items = list };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: null
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreSame(list, testInstance.Items);
    }

    [TestMethod]
    public async Task ApplyAsync_ShouldRemoveValueFromNullableListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 1
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.AreEqual(0, testInstance.Items.Count);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveLastValueFromNullableListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 1
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
    }

    class TestClassWithNullableList
    {
        public IList<int>? Items { get; set; }
    }

    #endregion
    
    #region IList Target

    [TestMethod]
    public async Task ApplyAsync_ShouldRemoveValueFromList()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = new List<int> { 1, 2, 2, 3 } };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 2
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.AreEqual(3, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(2, testInstance.Items[1]);
        Assert.AreEqual(3, testInstance.Items[2]);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveValueFromList()
    {
        // Arrange
        var testInstance = new TestClassWithList { Items = new List<int> { 1, 2, 2, 3 } };
        var targetProperty = typeof(TestClassWithList).GetProperty(nameof(TestClassWithList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 2
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreEqual(4, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
        Assert.AreEqual(2, testInstance.Items[1]);
        Assert.AreEqual(3, testInstance.Items[2]);
        Assert.AreEqual(2, testInstance.Items[3]);

    }
    
    [TestMethod]
    public async Task ApplyAsync_ShouldRemoveValueToListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 1
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
        Assert.AreEqual(0, testInstance.Items.Count);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRemoveValueToListWithOneElement()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = new List<int> { 1 } };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultRemoveOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 1
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
        var revertResult = await operationNode.TryRevertAsync(); // Revert to previous value

        // Assert
        Assert.IsTrue(applyResult);
        Assert.IsTrue(revertResult);
        Assert.AreEqual(1, testInstance.Items.Count);
        Assert.AreEqual(1, testInstance.Items[0]);
    }

    class TestClassWithList
    {
        public IList<int> Items { get; set; } = null!;
    }

    #endregion
}
