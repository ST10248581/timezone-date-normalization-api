using DateInputNormalizer.DateNormalization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("X-Timezone", new OpenApiSecurityScheme
    {
        Name = "X-Timezone",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Time zone header e.g. Africa/Johannesburg"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "X-Timezone",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Timezone"
                },
            },
            new List<string>()
        }
    });
});
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var dateLogic = new DateLogic();
dateLogic.setServerTimeZone();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ZonedDateConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var tzHeader = context.Request.Headers["X-Timezone"].FirstOrDefault();
    var dateLogic = new DateLogic();
    dateLogic.setClientTimeZone(tzHeader);
    await next.Invoke();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
