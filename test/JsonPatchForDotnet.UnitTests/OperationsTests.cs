using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace JsonPatchForDotnet.UnitTests;

[TestClass]
public class OperationsTests
{
    [TestMethod]
    public void OperationsFromJson_OperationIsAdd_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
              {
                ""op"": ""add"",
                ""path"": ""/path1"",
                ""value"": { ""key1"": ""value1"" }
              },
              {
                ""op"": ""add"",
                ""path"": ""/path2"",
                ""value"": [ ""value2"", ""value2b"" ]
              }
            ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Add,
                Path = "/path1",
                Value = JToken.Parse(@"{ ""key1"": ""value1"" }"),
            },
            new Operation
            {
                OperationType = OperationType.Add,
                Path = "/path2",
                Value = JToken.Parse(@"[ ""value2"", ""value2b"" ]"),
            }
        };
        
        // Act
        var operations = Operations.FromJson(json);
        
        // Assert
        operations.Should().BeEquivalentTo(expected);
    }
    
    [TestMethod]
    public void OperationsFromJson_OperationIsRemove_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
            {
                ""op"": ""remove"",
                ""path"": ""/path1""
            },
            {
                ""op"": ""remove"",
                ""path"": ""/path2""
            }
        ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Remove,
                Path = "/path1"
            },
            new Operation
            {
                OperationType = OperationType.Remove,
                Path = "/path2"
            }
        };

        // Act
        var operations = Operations.FromJson(json);

        // Assert
        operations.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void OperationsFromJson_OperationIsReplace_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
            {
                ""op"": ""replace"",
                ""path"": ""/path1"",
                ""value"": { ""new_value1"": ""updated"" }
            },
            {
                ""op"": ""replace"",
                ""path"": ""/path2"",
                ""value"": [ ""new_value2"", ""updated"" ]
            }
        ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Replace,
                Path = "/path1",
                Value = JToken.Parse(@"{ ""new_value1"": ""updated"" }"),
            },
            new Operation
            {
                OperationType = OperationType.Replace,
                Path = "/path2",
                Value = JToken.Parse(@"[ ""new_value2"", ""updated"" ]"),
            }
        };

        // Act
        var operations = Operations.FromJson(json);

        // Assert
        operations.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void OperationsFromJson_OperationIsMove_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
            {
                ""op"": ""move"",
                ""from"": ""/source1"",
                ""path"": ""/destination1"",
                ""value"": { ""data1"": ""moved"" }
            },
            {
                ""op"": ""move"",
                ""from"": ""/source2"",
                ""path"": ""/destination2"",
                ""value"": [ ""data2"", ""moved"" ]
            }
        ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Move,
                From = "/source1",
                Path = "/destination1",
                Value = JToken.Parse(@"{ ""data1"": ""moved"" }"),
            },
            new Operation
            {
                OperationType = OperationType.Move,
                From = "/source2",
                Path = "/destination2",
                Value = JToken.Parse(@"[ ""data2"", ""moved"" ]"),
            }};

        // Act
        var operations = Operations.FromJson(json);

        // Assert
        operations.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void OperationsFromJson_OperationIsCopy_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
            {
                ""op"": ""copy"",
                ""from"": ""/source1"",
                ""path"": ""/destination1"",
                ""value"": { ""copied_data1"": ""copied"" }
            },
            {
                ""op"": ""copy"",
                ""from"": ""/source2"",
                ""path"": ""/destination2"",
                ""value"": [ ""copied_data2"", ""copied"" ]
            }
        ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Copy,
                From = "/source1",
                Path = "/destination1",
                Value = JToken.Parse(@"{ ""copied_data1"": ""copied"" }"),
            },
            new Operation
            {
                OperationType = OperationType.Copy,
                From = "/source2",
                Path = "/destination2",
                Value = JToken.Parse(@"[ ""copied_data2"", ""copied"" ]"),
            }
        };

        // Act
        var operations = Operations.FromJson(json);

        // Assert
        operations.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void OperationsFromJson_OperationIsTest_ShouldDeserialize()
    {
        // Arrange
        var json = @"[
            {
                ""op"": ""test"",
                ""path"": ""/path1"",
                ""value"": { ""expected_value1"": ""test"" }
            },
            {
                ""op"": ""test"",
                ""path"": ""/path2"",
                ""value"": [ ""expected_value2"", ""test"" ]
            }
        ]";

        var expected = new List<Operation>
        {
            new Operation
            {
                OperationType = OperationType.Test,
                Path = "/path1",
                Value = JToken.Parse(@"{ ""expected_value1"": ""test"" }"),
            },
            new Operation
            {
                OperationType = OperationType.Test,
                Path = "/path2",
                Value = JToken.Parse(@"[ ""expected_value2"", ""test"" ]"),
            }
        };

        // Act
        var operations = Operations.FromJson(json);

        // Assert
        operations.Should().BeEquivalentTo(expected);
    }
}