using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[] { new { Id = 1, Name = "John Doe" },
             new { Id = 2, Name = "Jane Smith" } , 
             new { Id = 3, Name = "Alice Johnson" },
             new { Id = 4, Name = "Bob Brown" },
             new { Id = 5, Name = "Charlie Davis" },
             new { Id = 6, Name = "Diana Evans" },
             new { Id = 7, Name = "Frank Green" },
             new { Id = 8, Name = "Grace Harris" },
             new { Id = 9, Name = "Henry Jackson" },
             new { Id = 10, Name = "Ivy King" } ,
             new { Id = 11, Name = "Jack Lee" },
             new { Id = 12, Name = "Karen Martinez" },});
        }
    }
}
