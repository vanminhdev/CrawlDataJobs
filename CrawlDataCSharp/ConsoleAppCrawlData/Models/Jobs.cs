using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleAppCrawlData.Models
{
    public class Jobs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("link")]
        public string Link { get; set; }

        [BsonElement("jobName")]
        public string JobName { get; set; } = null!;

        [BsonElement("startSalary")]
        public int? StartSalary { get; set; }

        [BsonElement("endSalary")]
        public int? EndSalary { get; set; }

        [BsonElement("skills")]
        public List<string> Skills { get; set; }

        [BsonElement("positions")]
        public List<string> Positions { get; set; }

        [BsonElement("experience")]
        public string Experience { get; set; }

        [BsonElement("postTime")]
        public DateTime? PostTime { get; set; }

        [BsonElement("company")]
        public Company Company { get; set; }

        [BsonElement("workingTime")]
        public string WorkingTime { get; set; }

        [BsonElement("overtime")]
        public string Overtime { get; set; }
    }

    public class Company
    {
        [BsonElement("companyName")]
        public string CompanyName { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("startCompanySize")]
        public int? StartCompanySize { get; set; }

        [BsonElement("endCompanySize")]
        public int? EndCompanySize { get; set; }

        /// <summary>
        /// Loại công ty: product, outsource
        /// </summary>
        [BsonElement("companyType")]
        public string CompanyType { get; set; }

        [BsonElement("nation")]
        public string Nation { get; set; }
    }
}
