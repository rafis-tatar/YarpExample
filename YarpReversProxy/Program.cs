using Microsoft.AspNetCore.Authentication.JwtBearer;
using YarpReversProxy;
using Auth.Shared;
using Microsoft.AspNetCore.Authorization;

HttpClient http = new HttpClient() { BaseAddress =new Uri( "http://authservice/" )};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRsa();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(op =>
{    
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(async options =>
{    
    var rsa = builder.Configuration.Get<Auth.Shared.JwtRsa>();
    var openKey = await http.GetStringAsync("auth/open-key");

    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = rsa.GetTokenValidationParameters(openKey);
 });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtValidate", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();        
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();
//app.AddRoutes();
app.Run();
