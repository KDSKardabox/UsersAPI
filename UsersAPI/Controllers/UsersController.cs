using Microsoft.AspNetCore.Mvc;

namespace UsersAPI.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UsersController : ControllerBase
  {
    [HttpGet("{id}")]
    public User Get(int id)
    {
      return new User() { Id = 1, Name = "John Doe", Age = 21, Email = "balle@nodb.com" };
    }
  }
}