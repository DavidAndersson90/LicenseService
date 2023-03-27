using LicenseService.Exceptions;
using LicenseService.Handlers;
using LicenseService.MongoDB;
using LicenseService.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<LicenseDatabaseSettings>(builder.Configuration.GetSection(nameof(LicenseDatabaseSettings)));
builder.Services.AddSingleton<ILicenseDatabaseSettings>(sp => sp.GetRequiredService<IOptions<LicenseDatabaseSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("LicenseDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<ILicenseHandler, LicenseHandler>();
builder.Services.AddScoped<ILicenseRepository, LicenseRepository>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
