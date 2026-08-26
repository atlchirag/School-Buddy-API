using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<SchoolBuddy_APIs.Services.HaltsTelemetryService>();
builder.Services.AddScoped<Idatabase_access, database_access>();
builder.Services.AddScoped<IHoliday, HolidayRepository>();

// ✅ Add Session and Distributed Memory Cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add IHttpContextAccessor for accessing session
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Use Session Middleware
app.UseSession();
app.UseAuthorization();

app.MapControllers();
app.Run();


