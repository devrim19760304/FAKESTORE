//to get data from body,form 
using Microsoft.AspNetCore.Mvc;
//to get data from external API 
//fake person 
using System.Net.Http;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);

//add services 
//add cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowAll", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyMethod();
    });
});
//add Http client 
builder.Services.AddHttpClient();

var app = builder.Build();
//apply services
app.UseCors("allowAll");
//we use static pages 
app.UseStaticFiles();

//later we will change it to static files 
//this will be our home page
//we need to write a homepage 

//app.MapGet("/", () => "Hello World!");

//instead bind index.htmlpage 
app.MapGet("/", content =>
{
    content.Response.Redirect("/index.html");
    return Task.CompletedTask;

});

//company page get 
app.MapGet("/staff", content =>
{
    content.Response.Redirect("/table.html");
    return Task.CompletedTask;
});





app.MapPost("/staffjson", async (IHttpClientFactory httpClientFactory, [FromBody] PersonRequest personRequest) =>
{
    if (StaffManager.IsFull)
    {
        // Return existing staff wrapped in a "results" key
        return Results.Json(new { results = StaffManager.GetStaff() });
    }

    try
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetStringAsync($"https://randomuser.me/api/?results={personRequest.PersonNumber}");
        var data = JsonDocument.Parse(response).RootElement.GetProperty("results");
        var newStaff = data.EnumerateArray().Select((person, index) => new Person
        {
            Id = index + 1,
            FirstName = person.GetProperty("name").GetProperty("first").GetString() ?? "Unknown",
            LastName = person.GetProperty("name").GetProperty("last").GetString() ?? "Unknown",
            Email = person.GetProperty("email").GetString() ?? "Unknown",
            Phone = person.GetProperty("phone").GetString() ?? "Unknown",
            Location = person.GetProperty("location").GetProperty("city").GetString() ?? "Unknown",
            PhotoUrl = person.GetProperty("picture").GetProperty("large").GetString() ?? "Unknown"
        }).ToList();

        StaffManager.AddStaff(newStaff);

        // Return the new staff wrapped in a "results" key
        return Results.Json(new { results = newStaff });
    }
    catch (Exception exp)
    {
        return Results.BadRequest(new { error = exp.Message });
    }
});





app.Run();


public class PersonRequest
{
    public int PersonNumber { get; set; }
}
