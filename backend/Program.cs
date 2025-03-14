using backend.Data;
using backend.Middlewares;
using backend.Repositories;
using backend.Services;
using backend.Services.DocumentGeneration;
using backend.Settings;
using backend.Validation.Student;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/app.log",
                  rollingInterval: RollingInterval.Day,
                  shared: true)  // Allow shared file access if needed
    .CreateLogger();
// builder.Host.UseSerilog();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("Content-Disposition");;
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Student Management API", 
        Version = "v1",
        Description = "API for managing students and study programs"
    });

    // Cấu hình JWT Authentication trong Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IStudyProgramRepository, StudyProgramRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddSingleton<ILoggingService, LoggingService>();
builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IFacultyService, FacultyService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<IStudentValidationService, StudentValidationService>();
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
builder.Services.Configure<StudentSettings>(builder.Configuration.GetSection("Rules:Student"));
builder.Services.Configure<StudentStatusTransitions>(builder.Configuration.GetSection("Rules:StudentStatusTransitions"));
builder.Services.AddSingleton(new SynchronizedConverter(new PdfTools()));
    // Register the DocumentGeneratorFactory as a service
builder.Services.AddTransient<IDocumentGeneratorFactory, DocumentGeneratorFactory>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi seeding database.");
    }
}

// Thêm middleware CORS (đặt trước các middleware khác)
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
