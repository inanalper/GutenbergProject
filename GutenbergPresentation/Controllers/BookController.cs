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
            string token = HttpContext.Request.Headers["Authorization"].ToString();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            var response = await client.GetAsync($"https://gutendex.com/books/{bookId}");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                var bookResult = JsonConvert.DeserializeObject<BookViewModel>(apiResponse);


                var imageFormats = string.Join(",", bookResult.Formats
      .Where(kv => kv.Key.StartsWith("image/"))
      .Select(kv => kv.Value));

                var book = new AddBookModel
                {
                    bookId = bookId.ToString(),
                    bookName = bookResult.Title,
                    bookImage = imageFormats
                };


                var addBook = AddBookLib(book);
            }

            return null;
        }


        [HttpPost]
        public async Task<IActionResult> AddBookLib(AddBookModel book)
        {
            var userCredentials = new { bookId = book.bookId, bookName = book.bookName,bookImage=book.bookImage };
            var json = System.Text.Json.JsonSerializer.Serialize(userCredentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string token = HttpContext.Request.Headers["Authorization"].ToString();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

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
        [HttpPost]
        public async Task<IActionResult> DeleteFromLib(int bookId)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString();

             
                string stringBookId = bookId.ToString();

                string apiUrl = $"https://localhost:7219/User/delete-books?bookId={stringBookId}";

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListLibrary");
                }
                else
                {
                    return RedirectToAction("ListLibrary");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }


        public async Task<IActionResult> ListLibrary()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
      
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            var response = await _httpClient.GetAsync("https://localhost:7219/User/get-bookshelf");


            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<BookModel>>(apiResponse);

                return View(books);
            }
            else
            {
                return View(new List<BookModel>());
            }
        }




    }
}
