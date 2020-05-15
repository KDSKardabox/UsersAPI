using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.DatabaseContext;


namespace UsersAPI.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UsersController : ControllerBase
  {

    private UserDbContext _dbContext;

    public UsersController(UserDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    public User Get(int id)
    {
      return _dbContext.Users.FirstOrDefault(user => user.Id == id);
    }
  }
}