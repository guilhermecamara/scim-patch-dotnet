using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ScimPatch.Converters;

namespace ScimPatch.UnitTests.Converters;

[TestClass]
public class ObservableCollectionConverterTests
{
    [TestMethod]
    public void ObservableCollectionConverter_ShouldDeserializeToObservableCollection()
    {
        // Arrange
        var json = "{\"items\":[\"item1\",\"item2\",\"item3\"]}";
            
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ObservableCollectionConverter());

        // Act
        var deserializedModel = JsonConvert.DeserializeObject<SampleModel>(json, settings);

        // Assert
        Assert.IsNotNull(deserializedModel);
        Assert.IsInstanceOfType(deserializedModel.Items, typeof(ObservableCollection<string>));
        Assert.AreEqual(3, deserializedModel.Items.Count);
        Assert.AreEqual("item1", deserializedModel.Items[0]);
        Assert.AreEqual("item2", deserializedModel.Items[1]);
        Assert.AreEqual("item3", deserializedModel.Items[2]);
    }

    [TestMethod]
    public void ObservableCollectionConverter_ShouldSerializeObservableCollection()
    {
        // Arrange
        var model = new SampleModel
        {
            Items = new ObservableCollection<string> { "item1", "item2", "item3" }
        };
            
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ObservableCollectionConverter());

        // Act
        var json = JsonConvert.SerializeObject(model, settings);

        // Assert
        var expectedJson = "{\"items\":[\"item1\",\"item2\",\"item3\"]}";
        Assert.AreEqual(expectedJson, json);
    }
}

public class SampleModel
{
    [JsonProperty("items")]
    [JsonConverter(typeof(ObservableCollectionConverter))]
    public IList<string> Items { get; set; } = null!;
}