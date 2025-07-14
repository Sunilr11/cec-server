using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.PageTemplate
{
    public class AboutTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("aboutBlocks")]
        public List<AboutBlocks> AboutBlocks { get; set; }

    }

    public class AboutBlocks
    {
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("compProps")]
        public CompProps CompProps { get; set; }
        
    }

    public class CompProps
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("bgImage")]
        public string bgImage { get; set; }
        [BsonElement("buttonText")]
        public string ButtonText { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("team")]
        public List<TeamItems> Team { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("solutions")]
        public List<Solutions> Solutions { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [BsonElement("imgContainerStyle")]
        public ImgContainerStyle ImgContainerStyle { get; set; }

    }

    public class TeamItems
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("designation")]
        public string  Designation { get; set; }
        [BsonElement("linkedIn")]
        public string LinkedIn { get; set; }
        [BsonElement("profileImage")]
        public string ProfileImage { get; set; }
    }

    public class Solutions
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("image")]
        public string Image { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
    }
    public class ImgContainerStyle
    {
        [BsonElement("alignItems")]
        public string AlignItems { get; set; }
    }
}
