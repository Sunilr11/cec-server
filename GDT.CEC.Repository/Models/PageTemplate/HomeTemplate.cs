using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDT.CEC.Repository.Models.AzureLabs;

namespace GDT.CEC.Repository.Models.PageTemplate
{
    public class HomeTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("introCard")]
        public IntroCard IntroCard { get; set; }

        [BsonElement("title")]
        public string  Title { get; set; }


        [BsonElement("subTitle")]
        public string  SubTitle { get; set; }
        [BsonElement("labs")]
        public List<AzureLab> Labs { get; set; }
    }


    public class IntroCard
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("bgImage")]
        public string bgImage { get; set; }
    }

    public class Labs
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("subTitle")]
        public string SubTitle { get; set; }
        [BsonElement("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("image")]
        public string Image { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("buttonText")]
        public string ButtonText { get; set; }
    }
}
