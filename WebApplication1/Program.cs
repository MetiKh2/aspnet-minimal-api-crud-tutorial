using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add db context , here you can we are using in memory database
builder.Services.AddDbContext<MyDataContext>(option =>
{
	option.UseInMemoryDatabase("StudentsDb");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
	var forecast = Enumerable.Range(1, 5).Select(index =>
		new WeatherForecast
		(
			DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			Random.Shared.Next(-20, 55),
			summaries[Random.Shared.Next(summaries.Length)]
		))
		.ToArray();
	return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/SaveStudent",async(Student student, MyDataContext db) =>
{
	await db.Students.AddAsync(student);
	await db.SaveChangesAsync();
	return Results.Created($"/save/{student.Id}",student);
});

app.MapGet("/GetAllStudents", async (MyDataContext db) =>
{
	return Results.Ok(await db.Students.ToListAsync());
});

app.MapGet("/GetStudents/{id}", async (int id ,MyDataContext db) =>
{
	var student =await db.Students.FindAsync(id);
	if (student is null) Results.NotFound();

	return Results.Ok(student) ;
});



app.MapPut("/UpdateStudents/{id}", async (int id,Student inputStudent, MyDataContext db) =>
{
	var student = await db.Students.FindAsync(id);
	if (student is null) return Results.NotFound();

	student.Name = inputStudent.Name;
	student.Email = inputStudent.Email;

	await db.SaveChangesAsync();
	return Results.NoContent();
});

app.MapDelete("/DeleteStudents/{id}", async (int id, MyDataContext db) =>
{
	var student = await db.Students.FindAsync(id);
	if (student is null) return Results.NotFound();

	db.Students.Remove(student);
	await db.SaveChangesAsync();

	return Results.Ok(student);
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
