using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DrRecords.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//jwt input: 

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
//jwt slut 




// Add services to the container.

// Repository: Singleton = ét objekt for hele appen, så listen ikke nulstilles ved hvert request
builder.Services.AddSingleton<IMusicRecordsRepositoryList, MusicRecordsRepositoryList>();

// CORS: tillad at vores frontend (en anden "origin") må kalde API'et
const string AllowAllPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAllPolicy, policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(AllowAllPolicy);

app.UseAuthentication(); //hvem er du? (skal komme FØR authorization)

app.UseAuthorization(); //hvad må du?

app.MapControllers();

app.Run();
