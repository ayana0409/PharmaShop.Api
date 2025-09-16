using PharmaShop.Application.Setting;
using PharmaShop.Application;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

builder.Services.RegisterDb(builder.Configuration);
builder.Services.AddDependencyInjection();
builder.Services.AddAppDependencyInjection();

builder.Services.AddSwagger();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://pharmashop-api.onrender.com", "https://pharmashop-k2d0.onrender.com", "http://localhost:3001")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        }
    );
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AutoMigration();

app.SeedData(builder.Configuration).GetAwaiter().GetResult();

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
