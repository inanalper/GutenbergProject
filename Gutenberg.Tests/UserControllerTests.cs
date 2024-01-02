using GutenbergProject;
using GutenbergProject.Controllers;
using GutenbergProject.Entities;
using GutenbergProject.Models;
using GutenbergProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gutenberg.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void AddBook_AddsBookAndReturnsOk()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString(); // Unique database name
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new MyContext(options); // MyContext should be your DbContext
            User user = new User("mail@gmail.com", "TestUser", "68B8D8218E730FC2957BCB12119CB204");
            context.Users.Add(user); // Make sure to match the properties
            context.SaveChanges();

            var mockJwtDecoder = new Mock<IJWTDecoder>();
            mockJwtDecoder.Setup(j => j.GetUserIdFromToken(It.IsAny<string>())).Returns("1");


            var controller = new UserController(context, mockJwtDecoder.Object); // BookController should be the name of your controller
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // Mock HttpContext
            };
            controller.HttpContext.Request.Headers["Authorization"] = "Bearer testtoken"; // Set a dummy token

            AddBookModel bookModel = new AddBookModel // Presuming AddBookModel is your model class
            {
                // Initialize properties as necessary
                bookName = "Test Book",
                bookId = "123",
                bookImage = "imageurl"
            };

            // Act
            var result = controller.addBook(bookModel);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var userBook = context.UserBooks.FirstOrDefault();
            Assert.NotNull(userBook);
            Assert.Equal("Test Book", userBook.bookName);
            Assert.Equal("123", userBook.bookId);
            Assert.Equal("imageurl", userBook.bookImage);
            Assert.Equal(0, userBook.onPage);
        }

        [Fact]
        public void DeleteBook_BookExists_DeletesBookAndReturnsOk()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString(); // Unique database name
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new MyContext(options);
            var bookId = "123"; // Test book ID
            var userId = 1; // Test user ID

            // Add a test user and a test book to the in-memory database
            var user = new User("mail@gmail.com", "TestUser", "passwordHash");
            context.Users.Add(user);
            var userBook = new UserBook { bookId = bookId, userId = userId , bookImage = "imageurl", bookName = "Test Book"}; // Adjust as per your UserBook entity
            context.UserBooks.Add(userBook);
            context.SaveChanges();

            var mockJwtDecoder = new Mock<IJWTDecoder>();
            mockJwtDecoder.Setup(j => j.GetUserIdFromToken(It.IsAny<string>())).Returns(userId.ToString());

            var controller = new UserController(context, mockJwtDecoder.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Request.Headers["Authorization"] = "Bearer testtoken";

            // Act
            var result = controller.deleteBook(bookId.ToString());

            // Assert
            Assert.IsType<OkResult>(result);

            // Check if the book was removed from the database
            var deletedBook = context.UserBooks.Find(Convert.ToInt32(bookId));
            Assert.Null(deletedBook);
        }

        [Fact]
        public void GetBooks_ReturnsBookshelf()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new MyContext(options);
            var userId = 1; // Test user ID

            // Add a test user and books to the in-memory database
            var user = new User("mail@gmail.com", "TestUser", "passwordHash");
            context.Users.Add(user);
            context.UserBooks.Add(new UserBook { bookId = "123", bookImage = "imageurl", bookName = "Test Book" });
            // Add more UserBooks as needed
            context.SaveChanges();

            var mockJwtDecoder = new Mock<IJWTDecoder>();
            mockJwtDecoder.Setup(j => j.GetUserIdFromToken(It.IsAny<string>())).Returns(userId.ToString());

            var controller = new UserController(context, mockJwtDecoder.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Request.Headers["Authorization"] = "Bearer testtoken";

            // Act
            var result = controller.getBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bookShelf = Assert.IsAssignableFrom<ICollection<BookModel>>(okResult.Value);

        }

        [Fact]
        public void UpdateLastRead_BookExists_UpdatesPageAndReturnsOk()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new MyContext(options);
            var bookId = "1234"; // Test book ID
            var userId = 1; // Test user ID
            var onPage = 50; // Test page number

            // Add a test user and a test book to the in-memory database
            var user = new User("mail@gmail.com", "TestUser", "passwordHash");
            context.Users.Add(user);
            var userBook = new UserBook { bookId = bookId, userId = userId, onPage = 30, bookImage = "imageurl", bookName = "Test Book" }; // Adjust as per your UserBook entity
            context.UserBooks.Add(userBook);
            context.SaveChanges();

            var mockJwtDecoder = new Mock<IJWTDecoder>();
            mockJwtDecoder.Setup(j => j.GetUserIdFromToken(It.IsAny<string>())).Returns(userId.ToString());

            var controller = new UserController(context, mockJwtDecoder.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Request.Headers["Authorization"] = "Bearer testtoken";

            // Act
            var result = controller.updateLastRead(bookId, onPage);

            // Assert
            Assert.IsType<OkResult>(result);

            // Check if the book's onPage and lastReaded fields were updated
            var updatedUserBook = context.UserBooks.FirstOrDefault(ub => ub.bookId == bookId && ub.userId == userId);
            Assert.NotNull(updatedUserBook);
            Assert.Equal(onPage, updatedUserBook.onPage);

            // Check if lastReaded is recently set and not null
            Assert.True(updatedUserBook.lastReaded.HasValue);
            Assert.True(DateTime.Now.Subtract(updatedUserBook.lastReaded.GetValueOrDefault()).TotalMinutes < 1);

        }

        [Fact]
        public void GetLastReadBook_BookExists_ReturnsLastReadBook()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new MyContext(options);
            var userId = 1; // Test user ID

            var user = new User("mail@gmail.com", "TestUser", "passwordHash");
            context.Users.Add(user);
            context.UserBooks.Add(new UserBook { userId = userId, lastReaded = DateTime.Now.AddDays(-1), bookId = "12", bookImage = "imageurl", bookName = "Test Book" }); // Earlier read book
            var expectedBook = new UserBook { userId = userId, lastReaded = DateTime.Now, bookId = "12", bookImage = "imageurl", bookName = "Test Book" }; // Last read book
            context.UserBooks.Add(expectedBook);
            context.SaveChanges();

            var mockJwtDecoder = new Mock<IJWTDecoder>();
            mockJwtDecoder.Setup(j => j.GetUserIdFromToken(It.IsAny<string>())).Returns(userId.ToString());

            var controller = new UserController(context, mockJwtDecoder.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Request.Headers["Authorization"] = "Bearer testtoken";

            // Act
            var result = controller.GetLastReadBook();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var lastReadBook = Assert.IsType<UserBook>(okResult.Value);
            Assert.Equal(expectedBook, lastReadBook);
        }

    }
}