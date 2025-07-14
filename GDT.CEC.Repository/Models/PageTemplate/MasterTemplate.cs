using GDT.CEC.Repository.Models.AzureLabs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.PageTemplate
{
    public class MasterTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonElement("header")]
        public string Header { get; set; }

        [BsonElement("tabsData")]
        public List<TabData> TabsData { get; set; }

    }

    public class TabData
    {
        [BsonElement("key")]
        public string Key { get; set; }
        [BsonElement("label")]
        public string Label { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }


        [BsonElement("blogs")]
        public Blog Blogs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("labs")]
        public MasterLabs Labs { get; set; }
    }

    public class Blog
    {
        [BsonElement("content")]
        public List<Content> Content { get; set; }
    }

    public class Content
    {
        [BsonElement("image")]
        public string Image { get; set; }
        [BsonElement("tags")]
        public List<string> Tags { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("buttonText")]
        public string ButtonText { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("date")]
        public string Date { get; set; }
        [BsonElement("redirectionUrl")]
        public string RedirectionUrl { get; set; }
            
    }

    public class MasterLabs
    {
        [BsonElement("items")]
        public List<AzureLab> Items { get; set; }
    }
}
