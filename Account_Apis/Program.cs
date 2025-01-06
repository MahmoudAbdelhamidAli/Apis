using Account_Apis.Data;
using Account_Apis.Interfaces;
using Account_Apis.Models;
using Account_Apis.Repositories;
using Account_Apis.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////mahmoud////////////
////////// Add services to the container.
///

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddIdentity<AppUser, IdentityRole>()
//     .AddEntityFrameworkStores<MyDbContext>()
//     .AddDefaultTokenProviders();

//builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));

var emailConfig = builder.Configuration
    .GetSection("SMTPConfig")
    .Get<SMTPConfigModel>();
builder.Services.AddSingleton(emailConfig);


// for Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

// add time out for token
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromHours(5));


// builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();


// Add services to the container(JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:5033",
        ValidAudience = "http://localhost:5033",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dhgfhgkgywkuef65wfrw4fijfio3dbhs864f8r43fuj43hf65w5f86wkjmkfe5wfeiw6w8e888d"))
    };
});


builder.Services.AddAuthorization();



builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();


// map controllers routes to endpoints
app.MapControllers();

app.Run();


