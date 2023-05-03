using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT認証の設定を追加
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_jwt_signing_key")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// ログインエンドポイントの追加
app.MapPost(
    "/api/login",
    async (HttpContext context) =>
    {
        var jsonString = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var loginData = JsonConvert.DeserializeObject<LoginData>(jsonString);

        string username = loginData.UserName;
        string password = loginData.Password;

        // ユーザー認証のロジックを実装します。ここではダミーの認証処理を行っています。
        if (username == "test" && password == "test")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_jwt_signing_key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Results.Ok(new { jwt });
        }

        return Results.Unauthorized();
    }
);

app.MapGet(
    "/api/data",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // 認証を適用
() =>
    {
        return new List<HogeData>() { 
            new HogeData(1, "Data 1"),
            new HogeData(2, "Data 2"), 
            new HogeData(3, "Data 3") 
        };
    }
)
.WithMetadata(new HttpGetAttribute())
.WithMetadata(new ProducesAttribute(typeof(IEnumerable<HogeData>)))
.WithMetadata(new ProducesResponseTypeAttribute(typeof(IEnumerable<HogeData>), StatusCodes.Status200OK))
.WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));


app.Run();


public record LoginData(string UserName, string Password);
public record HogeData(int Id, string Name);
