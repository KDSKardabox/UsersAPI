using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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