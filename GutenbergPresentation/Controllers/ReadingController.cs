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
        public async Task<IActionResult> Read()
        {
            string ID = "5833";
            int onPage = 0;
            string encodedSearch = Uri.EscapeDataString(ID);
            string endpoint = $"/books?ids={encodedSearch}";

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                BookResultViewModel bookResult = JsonConvert.DeserializeObject<BookResultViewModel>(apiResponse);

                BookViewModel result = bookResult.Results[0];

                string format = result.Formats.FirstOrDefault(f => f.Key == "text/plain; charset=us-ascii").Value;




                // Make another request to fetch the content of the specified page
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




                    // Pass the chunks to the view


                    // Now you have the content split into chunks, and you can use or process it as needed
                    // For example, you might want to display it in the view

                    return View("Read");

                }
                else
                {
                    // Handle the case where fetching the page content fails
                    return View("Error");
                }
            }
            else
            {
                // Handle the case where fetching book details fails
                return View("Error");
            }
        }
        private List<string[]> SplitTextIntoChunks(string textContent, int chunkSize)
        {
            // Split the text content into chunks based on line breaks
            string[] rows = textContent.Split('\n');

            // Chunk the rows into groups of specified size
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
            // Your logic here...

            using (HttpClient client = new HttpClient())
            {
                // Replace "YourApiEndpoint" with the actual API endpoint URL
                string apiUrl = "https://localhost:7219/";

                // Replace "YourPutAction" with the actual PUT action in your API
                string putAction = "/User/update-reading";

                // Construct the URL for the PUT request
                string putUrl = $"{apiUrl}/{putAction}";

                // Create an object with the data you want to send in the request body
                var requestData = new
                {
                    onPage = onPage,
                    id = id
                };

                // Convert the object to JSON
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                // Make the PUT request
                HttpResponseMessage response = await client.PutAsync(putUrl, jsonContent);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Successful PUT request, you can handle the response if needed
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Handle the case where the PUT request was not successful
                    return BadRequest(); // You might want to handle this differently based on your requirements
                }
            }
        }
    }
}
