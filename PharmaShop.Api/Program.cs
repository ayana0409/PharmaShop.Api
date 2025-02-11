using PharmaShop.Application.Setting;
using PharmaShop.Application;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.RegisterDb(builder.Configuration);
builder.Services.AddDependencyInjection();
builder.Services.AddAppDependencyInjection();

builder.Services.AddSwagger();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            //builder.AllowAnyOrigin()
            //       .AllowAnyMethod()
            //       .AllowAnyHeader();
            builder.WithOrigins("http://localhost:3000", "http://26.139.159.129", "http://26.139.159.129:3000", "http://localhost:3001")
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

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
