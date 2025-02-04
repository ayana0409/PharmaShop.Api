<<<<<<< HEAD
﻿using PharmaShop.Application.Setting;
using PharmaShop.Application;
=======
using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure;
using PharmaShop.Infastructure.Data;
>>>>>>> 3883f36214b28efcc294d756b938ed97364d496c

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
<<<<<<< HEAD

builder.Services.AddEndpointsApiExplorer();
=======
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
>>>>>>> 3883f36214b28efcc294d756b938ed97364d496c

builder.Services.AddAuthorization();

builder.Services.RegisterDb(builder.Configuration);

<<<<<<< HEAD
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

app.AutoMigration();

app.SeedData(builder.Configuration).GetAwaiter().GetResult();

=======
var app = builder.Build();

// Configure the HTTP request pipeline.
>>>>>>> 3883f36214b28efcc294d756b938ed97364d496c
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


=======
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

>>>>>>> 3883f36214b28efcc294d756b938ed97364d496c
app.Run();
