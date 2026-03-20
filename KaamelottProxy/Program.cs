var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");

app.MapGet("/api/kaamelott", async (HttpClient http) =>
{
    var result = await http.GetStringAsync("https://kaamelott.chaudie.re/api/all");
    return Results.Content(result, "application/json");
});

app.Run();
