using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using GutenbergPresentation.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GutenbergProject.Models;
using static System.Reflection.Metadata.BlobBuilder;
using Newtonsoft.Json;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace GutenbergPresentation.Controllers
{
    public class ReadingController : Controller
    {
        private readonly ILogger<ReadingController> _logger;
        private readonly HttpClient _httpClient;

        public ReadingController(ILogger<ReadingController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://gutendex.com/");
        }

        [HttpGet]
        public async Task<IActionResult> Read(string ID, int onPage)
        {
            
            string encodedSearch = Uri.EscapeDataString(ID);
            string endpoint = $"/books?ids={encodedSearch}";

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                BookResultViewModel bookResult = JsonConvert.DeserializeObject<BookResultViewModel>(apiResponse);

                BookViewModel result = bookResult.Results[0];

                string format = result.Formats.FirstOrDefault(f => f.Key == "text/plain; charset=us-ascii").Value;




                HttpResponseMessage textResponse = await _httpClient.GetAsync(format);

                if (textResponse.StatusCode == HttpStatusCode.Found)
                {
                    string textContent = await textResponse.Content.ReadAsStringAsync();

                    string httpResponse = textContent;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(httpResponse);

                    HtmlNode linkNode = doc.DocumentNode.SelectSingleNode("//a[@href]");
                    if (linkNode != null)
                    {
                        string linkUrl = linkNode.GetAttributeValue("href", "");
                        HttpResponseMessage linkResponse = await _httpClient.GetAsync(linkUrl);

                        if (linkResponse.IsSuccessStatusCode)
                        {
                            string linkContent = await linkResponse.Content.ReadAsStringAsync();
                            List<string[]> textChunks = SplitTextIntoChunks(linkContent, 200);
                            ViewData["onPage"] = onPage;
                            ViewData["ID"] = ID;
                            ViewData["size"] = textChunks.LongCount();
                            return View("Read", textChunks[onPage]);
                        }
                        else
                        {
                            Console.WriteLine("No link found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No link found.");
                    }


                    return View("Read");

                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }
        private List<string[]> SplitTextIntoChunks(string textContent, int chunkSize)
        {
            string[] rows = textContent.Split('\n');

            List<string[]> chunks = new List<string[]>();
            for (int i = 0; i < rows.Length; i += chunkSize)
            {
                int remainingRows = Math.Min(chunkSize, rows.Length - i);
                string[] chunk = new string[remainingRows];
                Array.Copy(rows, i, chunk, 0, remainingRows);
                chunks.Add(chunk);
            }

            return chunks;
        }

        public async Task<IActionResult> Exit(int onPage, int id)
        {
            

            string token = HttpContext.Request.Headers["Authorization"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            var response = _httpClient.PutAsync($"https://localhost:7219/User/update-reading?OnPage={onPage}&bookId={id}", null).Result;

            
            if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListLibrary", "Book");
                }
                else
                {
                    
                    return BadRequest();
                }
            
        }


        public async Task<IActionResult> Pre(int onPage, int id)
        {
          

            if (onPage < 0)
            {
                onPage++;
            }
            return RedirectToAction("Read", new { ID = id, OnPage = onPage });

        }

        public async Task<IActionResult> Next(int onPage, string id, int textChunks)
        {
           
            
            if (onPage >= textChunks)
            {
                onPage--;
            }

            return RedirectToAction("Read", new { ID = id, OnPage = onPage });
        }

    }
}
