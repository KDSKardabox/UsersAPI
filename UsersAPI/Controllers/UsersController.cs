using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using UsersAPI.DatabaseContext;


namespace UsersAPI.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UsersController : ControllerBase
  {
    public UsersController(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    [HttpGet("{id}")]
    public Object Get(int id)
    {
      var connectionString = Configuration["ConnectionStrings:DatabaseConnection"];
      if (!String.IsNullOrWhiteSpace(connectionString))
      {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        optionsBuilder
          .UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(60))
          .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        using (var dbContext = new UserDbContext(optionsBuilder.Options))
        {

          dbContext.Database.EnsureCreated();
          var testUser = dbContext.Users.FirstOrDefault(b => b.Id == 1);
          if (testUser == null)
          {
            dbContext.Users.Add(new User { Name = "Joe Dark", Age = 20, Email = "joe@micro.com" });
            dbContext.SaveChanges();
          }
          return dbContext.Users.FirstOrDefault(user => user.Id == id);
        }
      }
      return "Error";
    }
  }
}