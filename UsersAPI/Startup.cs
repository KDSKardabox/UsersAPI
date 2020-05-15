using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Jaeger;
using Jaeger.Samplers;
using OpenTracing;
using OpenTracing.Util;
using UsersAPI.DatabaseContext;

namespace UsersAPI
{
  public class Startup
  {
    private async Task SeedData(UserDbContext dbContext)
    {
      dbContext.Database.EnsureCreated();

      var testUser = await dbContext.Users.SingleOrDefaultAsync(b => b.Id == 1);
      if (testUser == null)
      {
        dbContext.Users.Add(new User { Name = "Joe Dark", Age = 20, Email = "joe@micro.com" });
        dbContext.SaveChanges();
      }
    }
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<UserDbContext>(opts =>
        opts.UseSqlServer(Configuration["ConnectionStrings:DatabaseConnection"]));
      services.AddScoped<UserDbContext>();
      services.AddSingleton<ITracer>(serviceProvider =>
        {
          string serviceName = Assembly.GetEntryAssembly().GetName().Name;
          ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
          ISampler sampler = new ConstSampler(sample: true);
          ITracer tracer = new Tracer.Builder(serviceName)
                      .WithLoggerFactory(loggerFactory)
                      .WithSampler(sampler)
                      .Build();
          GlobalTracer.Register(tracer);
          return tracer;
        });
      services.AddOpenTracing();
      services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserDbContext dbContext)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      SeedData(dbContext).Wait();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
