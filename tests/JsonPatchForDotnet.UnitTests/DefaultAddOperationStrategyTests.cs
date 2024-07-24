namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class DefaultAddOperationStrategyTests
{
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
}
