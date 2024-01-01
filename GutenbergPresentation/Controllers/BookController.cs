using GutenbergPresentation.Models;
using GutenbergProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GutenbergPresentation.Controllers
{
    public class BookController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookController(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
                _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBookLib()
        {
            return View();
        }


        [HttpGet]
        public async Task<BookViewModel> BooksGetById(int bookId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://gutendex.com/books/{bookId}");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                var bookResult = JsonConvert.DeserializeObject<BookViewModel>(apiResponse);

             
                var imageFormats = bookResult.Formats
                    .Where(kv => kv.Key.StartsWith("image/"))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                var book = new BookViewModel
                {
                    Id = bookId,
                    Title = bookResult.Title,
                    Formats = imageFormats,
                };

                var addBook = AddBookLib(book);
            }

            return null;
        }


        [HttpPost]
        public async Task<IActionResult> AddBookLib(BookViewModel book)
        {
            var userCredentials = new { Id = book.Id, Title = book.Title ,Formats=book.Formats};
            var json = System.Text.Json.JsonSerializer.Serialize(userCredentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("https://localhost:7219/User/add-books", content).Result;
            if (response.IsSuccessStatusCode)
            {

                return Ok();
            }
            else
            {

                string errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFromLib(int bookId)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7219/User/delete-books?bookId={bookId}");

            if (response.IsSuccessStatusCode)
            {
                return View("Silme işlemi başarılı");
            }
            else
            {
                return View("Silme işlemi başarısız");
            }
        }




        public async Task<IActionResult> ListLibrary()
        {
            var response = await _httpClient.GetAsync("https://localhost:7219/User/get-bookshelf");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<BookViewModel>>(apiResponse);

                return View(books);
            }
            else
            {
                return View(new List<BookViewModel>());
            }
        }

    }
}
