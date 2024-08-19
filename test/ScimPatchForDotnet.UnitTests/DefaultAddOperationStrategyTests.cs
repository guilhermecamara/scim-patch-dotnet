namespace ScimPatchForDotnet.UnitTests;

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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var result = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(42, testInstance.Value);
    }
    
    [TestMethod]
    public async Task ApplyAsync_ShouldNotSetValue()
    {
        // Arrange
        var testInstance = new TestClass();
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: "42"
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsFalse(applyResult);
        Assert.AreEqual(0, testInstance.Value);
    }

    [TestMethod]
    public async Task RevertAsync_ShouldRevertValue()
    {
        // Arrange
        var testInstance = new TestClass { Value = 42 };
        var targetProperty = typeof(TestClass).GetProperty(nameof(TestClass.Value))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 100
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync(); // Apply new value
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
    public async Task ApplyAsync_ShouldAddValueToNullNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = null };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
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
    
    [TestMethod]
    public async Task RevertAsync_ShouldRemoveLastValueFromNullNullableList()
    {
        // Arrange
        var testInstance = new TestClassWithNullableList { Items = null };
        var targetProperty = typeof(TestClassWithNullableList).GetProperty(nameof(TestClassWithNullableList.Items))!;
        var operationNode = new OperationNode(
            operationStrategy: new DefaultAddOperationStrategy(),
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
        Assert.IsNull(testInstance.Items);
    }

    class TestClassWithNullableList
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
        );

        // Act
        var applyResult = await operationNode.TryApplyAsync();

        // Assert
        Assert.IsTrue(applyResult);
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
            operationStrategy: new DefaultAddOperationStrategy(),
            instance: testInstance,
            targetPropertyInfo: targetProperty,
            sourcePropertyInfo: null,
            value: 42
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
        public IList<int>? Items { get; set; }
    }

    #endregion
}
