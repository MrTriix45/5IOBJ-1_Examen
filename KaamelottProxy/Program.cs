var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.WithOrigins("http://localhost:5227")
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");

app.MapGet("/api/kaamelott", async (HttpClient http) =>
{
    var result = await http.GetStringAsync("https://kaamelott.chaudie.re/api/all");
    /*
    foreach (var prop in result.GetType().GetProperties())
    {
        if (prop.citation == "citation")
        {
            // Do something with the citation value if needed
        }
    }
    */
    return Results.Content(result, "application/json");
});

app.Run();
