using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeVideoDownloader
{
    public class Meta
    {
        public string title { get; set; }
        public string sourceUrl { get; set; }
        public string duration { get; set; }
        public string tags { get; set; }
    }

    public class ResponseSchema
    {
        public string resourceId { get; set; }
        public List<Url> urls { get; set; }
        public Meta meta { get; set; }
        public string pictureUrl { get; set; }
        public List<string> videoQuality { get; set; }
        public string service { get; set; }
    }

    public class Url
    {
        public string url { get; set; }
        public string name { get; set; }
        public string subName { get; set; }
        public string extension { get; set; }
        public string quality { get; set; }
        public int qualityNumber { get; set; }
        public bool audio { get; set; }
        public string itag { get; set; }
        public string videoCodec { get; set; }
        public string audioCodec { get; set; }
        public bool isBundle { get; set; }
        public long filesize { get; set; }
        public long contentLength { get; set; }
    }
}
