using Nest;

namespace HuloToys_Service.Models.Article
{
    public class ArticleESModel
    {
        [PropertyName("Id")]
<<<<<<< Updated upstream

        public long Id { get; set; }
        [PropertyName("Title")]

        public string Title { get; set; }
        [PropertyName("Lead")]

        public string Lead { get; set; }
        [PropertyName("Body")]

        public string Body { get; set; }
        [PropertyName("Status")]

        public int Status { get; set; }
        [PropertyName("ArticleType")]

        public int ArticleType { get; set; }
        [PropertyName("PageView")]

        public int? PageView { get; set; }
        [PropertyName("PublishDate")]

        public DateTime? PublishDate { get; set; }
        [PropertyName("AuthorId")]

        public int? AuthorId { get; set; }
        [PropertyName("Image169")]

        public string Image169 { get; set; }
        [PropertyName("Image43")]

        public string Image43 { get; set; }
        [PropertyName("Image11")]

        public string Image11 { get; set; }
        [PropertyName("CreatedOn")]

        public DateTime? CreatedOn { get; set; }
        [PropertyName("ModifiedOn")]

        public DateTime? ModifiedOn { get; set; }
        [PropertyName("DownTime")]

        public DateTime? DownTime { get; set; }
        [PropertyName("UpTime")]

        public DateTime? UpTime { get; set; }
        [PropertyName("Position")]

        public short? Position { get; set; }
=======
        public long id { get; set; }

        [PropertyName("Title")]
        public string title { get; set; } = null!;

        [PropertyName("Lead")]
        public string lead { get; set; } = null!;

        [PropertyName("Body")]
        public string body { get; set; } = null!;

        [PropertyName("Status")]
        public int status { get; set; }

        [PropertyName("ArticleType")]
        public int articletype { get; set; }

        [PropertyName("PageView")]
        public int? pageview { get; set; }

        [PropertyName("PublishDate")]
        public DateTime? publishdate { get; set; }

        [PropertyName("AuthorId")]
        public int? authorid { get; set; }

        [PropertyName("Image169")]
        public string image169 { get; set; } = null!;

        [PropertyName("Image43")]
        public string? image43 { get; set; }

        [PropertyName("Image11")]
        public string? image11 { get; set; }

        [PropertyName("CreatedOn")]
        public DateTime? createdon { get; set; }

        [PropertyName("ModifiedOn")]
        public DateTime? modifiedon { get; set; }

        [PropertyName("DownTime")]
        public DateTime? downtime { get; set; }

        [PropertyName("UpTime")]
        public DateTime? uptime { get; set; }

        [PropertyName("Position")]
        public short? position { get; set; }
>>>>>>> Stashed changes
    }
}
