using LibraryApp.Data.Models;
using LibraryApp.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;

namespace LibraryApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryController : Controller
    {
        private readonly ILogger<LibraryController> _logger;
        public LibraryController(ILogger<LibraryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string GetBookList()
        {
            using (MongoRepository<BookModel> repository = new MongoRepository<BookModel>())
            {
                var book = repository.GetAll();
                if (book == null)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return null;
                }
                else
                {
                    var data = JsonConvert.SerializeObject(book);
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    return data;
                }
            }
        }

        [HttpPost]
        public string AddBook([FromBody] BookModel book)
        {
            using (MongoRepository<BookModel> repository = new MongoRepository<BookModel>())
                repository.Add(book);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return $"{book.BookName} book added.";
        }

        [HttpPut]
        public string SetProduct([FromBody] BookModel book)
        {
            using (MongoRepository<BookModel> repository = new MongoRepository<BookModel>())
            {
                repository.Update(x => x.Id.Equals(book.Id), book);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonConvert.SerializeObject(book);
            }
        }

        [HttpDelete]
        public string DeleteProduct([FromQuery] string id)
        {
            using (MongoRepository<BookModel> repository = new MongoRepository<BookModel>())
            {
                repository.Delete(x => x.Id.Equals(id));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return $"{id} book deleted.";
            }
        }
    }
}
