using GutenbergProject.Entities;
using GutenbergProject.JwtConfig;
using GutenbergProject.Models;
using GutenbergProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GutenbergProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyContext _context;

        public UserController(MyContext context)
        {
            _context = context;
        }

   
        [HttpPost("add-books", Name = "Add to Bookshelf")]
        public IActionResult addBook(AddBookModel book) {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                string userId = JWTDecoder.GetUserIdFromToken(token);
                User user = _context.Users.Find(Convert.ToInt32(userId));
                UserBook userBook = new UserBook();
                userBook.User = user;
                userBook.userId = user.id;
                userBook.bookName = book.bookName;
                userBook.bookId = book.bookId;
                userBook.bookImage = book.bookImage;
                userBook.onPage = 0;
                _context.UserBooks.Add(userBook);
                _context.SaveChanges();
                return Ok();
            } catch (Exception ex)
            {
            Console.WriteLine(ex.Message);
                throw ex;
            }

        }

     
        [HttpDelete("delete-books", Name = "Delete from Bookshelf")]
        public IActionResult deleteBook(string bookId)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                string userId = JWTDecoder.GetUserIdFromToken(token);
                UserBook userBook = _context.UserBooks.FirstOrDefault(book => bookId.Equals(book.bookId) && book.userId == Convert.ToInt32(userId));
                if (userBook == null)
                {
                    return NotFound();
                }
                _context.UserBooks.Remove(userBook);
                _context.SaveChanges();
                return Ok();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

      
        [HttpGet("get-bookshelf", Name = "Get Bookshelf of a user")]
        public IActionResult getBooks() {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                string userId = JWTDecoder.GetUserIdFromToken(token);
                User user = _context.Users
                     .Include(u => u.UserBooks)
                     .FirstOrDefault(u => u.id == Convert.ToInt32(userId));

                ICollection<BookModel> bookShelf = new HashSet<BookModel>();
                foreach (var book in user.UserBooks)
                {
                    BookModel bookModel = new BookModel();
                    bookModel.bookId = book.bookId;
                    bookModel.bookImage = book.bookImage;
                    bookModel.bookName = book.bookName;
                    bookModel.onPage = book.onPage;
                    bookShelf.Add(bookModel);
                }
                return Ok(bookShelf);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        [Authorize]
        [HttpPut("update-reading", Name = "Update last readed page of a book of user")]
        public IActionResult updateLastRead(string bookId, int onPage)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                string userId = JWTDecoder.GetUserIdFromToken(token);
                UserBook userBook = _context.UserBooks.FirstOrDefault(book => bookId.Equals(book.bookId) && book.userId == Convert.ToInt32(userId));
                if (userBook == null)
                {
                    return NotFound();
                }
                userBook.onPage = onPage;
                userBook.lastReaded = DateTime.Now;
                _context.SaveChanges();
                return Ok();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

        }

        [Authorize]
        [HttpGet("last-read-book", Name = "Get Last Read Book")]
        public IActionResult GetLastReadBook()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                string userId = JWTDecoder.GetUserIdFromToken(token);
                UserBook lastReadBook = _context.UserBooks
                                           .Where(ub => ub.userId == Convert.ToInt32(userId))
                                           .OrderByDescending(ub => ub.lastReaded)
                                           .FirstOrDefault();

                if (lastReadBook == null)
                {
                    return NotFound("No books found for this user.");
                }

                return Ok(lastReadBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while retrieving the last read book.");
            }
        }

    }
}
