using FluentAssertions;

namespace ScimPatch.IntegrationTests;

[TestClass]
public class OperationTrackerTests
{
    private string LoadJson(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    [TestMethod]
    public async Task Apply_SingleAddOperation_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.StringProperty = "New Value";
        var json = LoadJson("Mocks/001_SingleAdd.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_SingleRemoveOperation_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.StringList.Remove("Item2");
        var json = LoadJson("Mocks/002_SingleRemove.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_AddAndRemoveOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.IntProperty = 123;
        expectedObject.NullableBoolProperty = null;
        var json = LoadJson("Mocks/003_TwoOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_ThreeOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.NestedObject.Title = "Nested Name";
        expectedObject.NestedList[0].Title = "Title 1";
        expectedObject.StringList.RemoveAt(1);
        var json = LoadJson("Mocks/004_ThreeOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }
        
        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_FourOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.DateProperty = new DateTime(2024, 8, 20, 0, 0, 0,  DateTimeKind.Utc);
        expectedObject.NullableIntList = null;
        expectedObject.LongProperty = 9999999999L;
        expectedObject.NestedList[1].NestedDate = null;
        var json = LoadJson("Mocks/005_FourOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime().Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
    }

    [TestMethod]
    public async Task Apply_FiveOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.NullableIntProperty = 42;
        expectedObject.BoolProperty = false;
        expectedObject.NestedObject.NestedList[0].NestedInt = 7;
        expectedObject.StringList.Insert(2, "Added Value");
        expectedObject.NestedList[0].Title = null;
        var json = LoadJson("Mocks/006_FiveOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_SixOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.StringProperty = "Updated String";
        expectedObject.NullableDateProperty = null; // Already null, no change
        expectedObject.NestedObject.NestedList.Add(new NestedObject { Name = "Nested Item 3" });
        expectedObject.NestedList[1].NestedDate = new DateTime(2024, 8, 20, 12, 0, 0, DateTimeKind.Utc);
        expectedObject.LongProperty = 0; // Assuming default value for long is 0 after removal
        expectedObject.BoolProperty = true;
        var json = LoadJson("Mocks/007_SixOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime().Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
    }

    [TestMethod]
    public async Task Apply_SevenOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.NullableLongProperty = 1234567890L;
        expectedObject.StringList.RemoveAt(2);
        expectedObject.NestedObject.Title = "New Nested Name";
        expectedObject.DateProperty = new DateTime(2024, 8, 21, 0, 0, 0, DateTimeKind.Utc);
        expectedObject.NullableBoolProperty = null; // Already null, no change
        expectedObject.NestedList[1].Title = "Added Title";
        expectedObject.IntProperty = 0; // Assuming default value for int is 0 after removal
        var json = LoadJson("Mocks/008_SevenOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime().Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
    }

    [TestMethod]
    public async Task Apply_EightOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.StringProperty = "Final String";
        expectedObject.NullableIntList = null;
        expectedObject.NestedObject.NestedList[1].NestedInt = 9;
        expectedObject.StringList.Add("New String in List");
        expectedObject.LongProperty = 0; // Assuming default value for long is 0 after removal
        expectedObject.BoolProperty = false;
        expectedObject.NestedList[0].NestedDate = null;
        expectedObject.NestedList[1].Title = "Another Added Title";
        var json = LoadJson("Mocks/009_EightOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject);
    }

    [TestMethod]
    public async Task Apply_NineOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.NullableIntProperty = 50;
        expectedObject.BoolProperty = false;
        expectedObject.NestedObject.NestedList[0].NestedInt = 14;
        expectedObject.DateProperty = new DateTime(2024, 8, 22, 0, 0, 0, DateTimeKind.Utc);
        expectedObject.StringProperty = null; // Assuming removal sets string to null
        expectedObject.NullableDateProperty = new DateTime(2024, 8, 23, 0, 0, 0, DateTimeKind.Utc);
        expectedObject.NullableBoolProperty = null; // Already null, no change
        expectedObject.NestedList[0].Title = "New Nested Title";
        expectedObject.IntProperty = 0; // Assuming default value for int is 0 after removal
        var json = LoadJson("Mocks/010_NineOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime().Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
    }

    [TestMethod]
    public async Task Apply_TenOperations_ExpectedResultShouldBeCorrect()
    {
        // Arrange
        var targetObject = TargetObject.Mock();
        var expectedObject = TargetObject.Mock();
        expectedObject.LongProperty = 88888888L;
        expectedObject.NullableIntList = null;
        expectedObject.StringProperty = "Updated String Again";
        expectedObject.BoolProperty = false;
        expectedObject.NestedObject.NestedList[0].NestedInt = 21;
        expectedObject.StringList.Add("Another String");
        expectedObject.NullableDateProperty = null; // Assuming removal sets DateTime to null
        expectedObject.DateProperty = new DateTime(2024, 8, 24, 0, 0, 0, DateTimeKind.Utc);
        expectedObject.NestedList[0].Title = null;
        expectedObject.NestedList[1].NestedDate = new DateTime(2024, 8, 25, 0, 0, 0, DateTimeKind.Utc);
        var json = LoadJson("Mocks/011_TenOperations.json");

        // Act
        List<bool> results = [];
        var operations = OperationTracker.FromJson(targetObject, json);
        foreach (var operationNode in operations)
        {
            results.Add(await operationNode.TryApplyAsync());
        }

        // Assert
        results.Should().OnlyContain(r => r);
        targetObject.Should().BeEquivalentTo(expectedObject, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime().Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
    }
}