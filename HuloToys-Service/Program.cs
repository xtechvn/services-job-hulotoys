using Entities.ConfigModels;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Repro.IRepository;
using HuloToys_Service.Repro.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using REPOSITORIES.IRepositories;
using REPOSITORIES.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
var key = "this is my custom Secret key for authentication";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
var Configuration = builder.Configuration;
builder.Services.Configure<DataBaseConfig>(Configuration.GetSection("DataBaseConfig"));
builder.Services.Configure<MailConfig>(Configuration.GetSection("MailConfig"));
builder.Services.Configure<DomainConfig>(Configuration.GetSection("DomainConfig"));


// Register services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton<ITagRepository, TagRepository>();
builder.Services.AddSingleton<IGroupProductRepository, GroupProductRepository>();
builder.Services.AddSingleton<IGroupProductRepository, GroupProductRepository>();
builder.Services.AddSingleton<IIdentifierServiceRepository, IdentifierServiceRepository>();

builder.Services.AddSingleton<RedisConn>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
