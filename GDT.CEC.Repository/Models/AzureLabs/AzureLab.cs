using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.AzureLabs
{
    public class AzureLab
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string ID { get; set; }

        [BsonElement("azurelabid")]

        public string AzureLabID { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("hostPoolName")]
        public string HostPoolName { get; set; }
        [BsonElement("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public string Description { get; set; }

        [BsonElement("buttonText")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ButtonText { get; set; }

        [BsonElement("buttonText2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ButtonText2 { get; set; }

        [BsonElement("image")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Image { get; set; }

        [BsonElement("launchLink")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LaunchLink { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("tabsData")]
        public List<TabDataLab> TabsData { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("categories")]
        public List<CategoryLab> Categories { get; set; }

    }

    public class CategoryLab
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("key")]
        public string Key { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("label")]
        public string Label { get; set; }
    }

    public class TabDataLab
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("key")]
        public string Key { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("label")]
        public string Label { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("title")]
        public string Title { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("description")]
        public string Description { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("content")]
        public string Content { get; set; }
    }


}