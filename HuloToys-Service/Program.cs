using Entities.ConfigModels;
using Entities.Models;
using HuloToys_Service.IRepositories;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Configuration;
using Repositories.IRepositories;
using Repositories.Repositories;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        // Configure JWT Authentication
        //var key = "this is my custom Secret key for authentication";
        //builder.Services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //}).AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        //    };
        //});
        var Configuration = builder.Configuration;
        builder.Services.Configure<DataBaseConfig>(Configuration.GetSection("DataBaseConfig"));
        builder.Services.Configure<MailConfig>(Configuration.GetSection("MailConfig"));
        builder.Services.Configure<DomainConfig>(Configuration.GetSection("DomainConfig"));
        // ??ng ký ApplicationDbContext
        var connectionString = builder.Configuration.GetSection("DataBaseConfig:SqlServer:ConnectionString").Value;
        builder.Services.AddDbContext<DataMSContext>(options =>
                options.UseSqlServer(connectionString));

        builder.Services.AddHttpClient();
        // Register services
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IClientRepository, ClientRepository>();
        builder.Services.AddSingleton<IAccountClientRepository, AccountClientRepository>();
        builder.Services.AddSingleton<IProvinceRepository, ProvinceRepository>();
        builder.Services.AddSingleton<IDistrictRepository, DistrictRepository>();
        builder.Services.AddSingleton<IWardRepository, WardRepository>();

        builder.Services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();
        builder.Services.AddSingleton<IGoogleFormsService, GoogleFormsService>();
        builder.Services.AddSingleton<IValidationService, ValidationService>();
        builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

        builder.Services.AddSingleton<RedisConn>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


     
        app.UseCors("AllowAll");
 
  

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}