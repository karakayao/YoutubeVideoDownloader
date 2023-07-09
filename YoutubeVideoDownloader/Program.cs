using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeVideoDownloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Hosgeldiniz. Video Url: ");
            var media = GetMediaDetailsAsync(Console.ReadLine().Trim()).Result;
            Console.WriteLine(media.meta.title);

            for (int i = 0; i < media.urls.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] => {media.urls[i].quality} - {media.urls[i].extension} - {(media.urls[i].filesize / 1000000.0).ToString("0.00")}MB");
            }

            Console.Write("Seciniz: ");
            var selection = Convert.ToInt16(Console.ReadLine().Trim());

            var toDownload = media.urls[selection - 1];

            DownloadFile(toDownload).Wait();

            Console.Read();
        }

        static async Task<ResponseSchema> GetMediaDetailsAsync(string url)
        {
            try
            {
                var requestBody = new { url = url };
                var client = new HttpClient();

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://youtube86.p.rapidapi.com/api/youtube/links"),
                    Headers =
                    {
                        { "X-Forwarded-For", "70.41.3.18" },
                        { "X-RapidAPI-Key", "3a9475c9acmsh856eeb2ee115449p1446a1jsn9bdbdd3114d5" },
                        { "X-RapidAPI-Host", "youtube86.p.rapidapi.com" },
                    },
                    Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ResponseSchema>>(body)[0];
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Medya detayi cekilirken hata olustu: {ex.Message}");
            }
        }

        static async Task DownloadFile(Url url)
        {
            try
            {
                string outputFile = $"downloaded_{DateTime.Now.ToString("yyyymmdd_HHmmss")}_{url.quality}_{url.extension}";
                var client = new HttpClient();

                using (var response = await client.GetAsync(url.url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        long? totalBytes = response.Content.Headers.ContentLength;

                        using (var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[8192];
                            long downloadedBytes = 0;
                            int bytesRead = 0;
                            double percentage = 0;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0) 
                            { 
                                fileStream.Write(buffer, 0, bytesRead);
                                downloadedBytes += bytesRead;

                                percentage = (double)downloadedBytes / totalBytes.Value * 100;

                                await Console.Out.WriteAsync($"\rIndiriliyor... %{percentage.ToString("0.00")}");
                                await Console.Out.FlushAsync();
                            }

                            await Console.Out.WriteLineAsync("Indirme tamamlandi.");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya indirilirken hata olustu: {ex.Message}");
            }
        }

    }
}
