using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GDT.CEC.Repository.Models.PageTemplate
{
    public class Menu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public List<string> menus { get; set; }
    }

    public class MenuDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        [BsonElement("menuId")]
        public string MenuId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } 

        [BsonElement("link")]
        public string Link { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("icon")]
        public string IconPath { get; set; }

        [BsonElement("parentId")]
        public string ParentId { get; set; }

        [BsonElement("roleIds")]
        public List<int> RoleIds { get; set; }

        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime createdAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime updatedAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonIgnore]
        public List<MenuDetails> Children { get; set; } = new List<MenuDetails>();
    }

    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }

        [BsonElement("roleID")]
        public int RoleId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime createdAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime updatedAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }

}
