using Microsoft.AspNetCore.Mvc; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ADD NEW ENDPOINTS HERE
app.MapGet("/", () => "Hello World!");


app.MapGet("/DaysBetweenDates", (DateTime date1, DateTime date2) =>
{
    TimeSpan duration = date2 - date1;
    int days = duration.Days;
    return $"Days between {date1.ToShortDateString()} and {date2.ToShortDateString()}: {days}";
});

/*
/validatephonenumber:

receive by querystring a parameter called phoneNumber
validate phoneNumber with Spanish format, for example +34666777888
if phoneNumber is valid return true
*/
app.MapGet("/validatephonenumber", (string phoneNumber) =>
{
    return phoneNumber.StartsWith("+34") && phoneNumber.Length == 12;
});

/*
/validatespanishdni:

receive by querystring a parameter called dni
calculate DNI letter
if DNI is valid return "valid"
if DNI is not valid return "invalid"
*/
app.MapGet("/validatespanishdni", (string dni) =>
{
    if (dni.Length != 9)
    {
        return "invalid";
    }

    string dniNumbers = dni.Substring(0, 8);
    string dniLetter = dni.Substring(8, 1);
    string validLetters = "TRWAGMYFPDXBNJZSQVHLCKE";
    int dniNumber = int.Parse(dniNumbers);
    int rest = dniNumber % 23;
    char calculatedLetter = validLetters[rest];
    return dniLetter == calculatedLetter.ToString() ? "valid" : "invalid";
});

app.MapGet("/returncolorcode", async (HttpContext context) => 
{
    var color = context.Request.Query["color"].ToString();

    var json = await File.ReadAllTextAsync("colors.json");
    var colors = JsonSerializer.Deserialize<List<Color>>(json);

    foreach (var colorInfo in colors)
    {
        if (colorInfo.Name == color)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(colorInfo.Code.HEX));
            return;
        }
    }

    context.Response.StatusCode = StatusCodes.Status404NotFound;
});



app.MapGet("/tellmeajoke", async () =>
{
    using (var client = new HttpClient())
    {
        try
        {
            var joke = await client.GetStringAsync("https://api.example.com/jokes/random");
            return new ContentResult { Content = joke, ContentType = "text/plain" };
        }
        catch (HttpRequestException)
        {
            return new ContentResult { Content = "Failed to retrieve a joke.", ContentType = "text/plain" };
        }
    }
});

app.MapGet("/moviesbydirector", async (string director) =>
{
    // Make a call to the movie API to get the list of movies by the director
    var movies = await GetMoviesByDirectorAsync(director);

    // Return the full list of movies
    return movies;
});

async Task<List<Movie>> GetMoviesByDirectorAsync(string director)
{
    // Make the API call to get the movies by the director
    var apiUrl = $"http://www.omdbapi.com/?apikey=22c21365&type=movie&s={director}";
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OmdbApiResponse>(json);
            if (result.Response == "True")
            {
                return result.Search;
            }
            else
            {
                // Handle the case when the API call returns an error
                throw new Exception(result.Error);
            }
        }
        else
        {
            // Handle the case when the API call fails
            throw new Exception("Failed to retrieve movies from the API.");
        }
    }
}

app.MapGet("/parseurl", (string someurl) =>
{
    var uri = new Uri(someurl);
    var protocol = uri.Scheme;
    var host = uri.Host;
    var port = uri.Port;
    var path = uri.AbsolutePath;
    var query = uri.Query;
    var hash = uri.Fragment;

    return host;
});

app.MapGet("/listfiles", () =>
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var files = Directory.GetFiles(currentDirectory);
    return files;
});

app.MapGet("/calculatememoryconsumption", () =>
{
    var process = Process.GetCurrentProcess();
    var memoryUsage = process.WorkingSet64 / (1024.0 * 1024 * 1024);
    var roundedMemoryUsage = Math.Round(memoryUsage, 2);
    return $"{roundedMemoryUsage} GB";
});



app.MapGet("/randomeuropeancountry", () =>
{
    var europeanCountries = new Dictionary<string, string>
    {
        { "Albania", "AL" },
        { "Andorra", "AD" },
        { "Austria", "AT" },
        { "Belarus", "BY" },
        { "Belgium", "BE" },
        { "Bosnia and Herzegovina", "BA" },
        { "Bulgaria", "BG" },
        { "Croatia", "HR" },
        { "Cyprus", "CY" },
        { "Czech Republic", "CZ" },
        { "Denmark", "DK" },
        { "Estonia", "EE" },
        { "Finland", "FI" },
        { "France", "FR" },
        { "Germany", "DE" },
        { "Greece", "GR" },
        { "Hungary", "HU" },
        { "Iceland", "IS" },
        { "Ireland", "IE" },
        { "Italy", "IT" },
        { "Kosovo", "XK" },
        { "Latvia", "LV" },
        { "Liechtenstein", "LI" },
        { "Lithuania", "LT" },
        { "Luxembourg", "LU" },
        { "Malta", "MT" },
        { "Moldova", "MD" },
        { "Monaco", "MC" },
        { "Montenegro", "ME" },
        { "Netherlands", "NL" },
        { "North Macedonia", "MK" },
        { "Norway", "NO" },
        { "Poland", "PL" },
        { "Portugal", "PT" },
        { "Romania", "RO" },
        { "Russia", "RU" },
        { "San Marino", "SM" },
        { "Serbia", "RS" },
        { "Slovakia", "SK" },
        { "Slovenia", "SI" },
        { "Spain", "ES" },
        { "Sweden", "SE" },
        { "Switzerland", "CH" },
        { "Ukraine", "UA" },
        { "United Kingdom", "GB" },
        { "Vatican City", "VA" }
    };

    var random = new Random();
    var randomIndex = random.Next(0, europeanCountries.Count);
    var randomCountry = europeanCountries.ElementAt(randomIndex);
    return $"{randomCountry.Key} ({randomCountry.Value})";
}); 

app.Run();

// Needed to be able to access this type from the MinimalAPI.Tests project.
public partial class Program
{ }

public class OmdbApiResponse
{
    public string Response { get; set; }
    public string Error { get; set; }
    public List<Movie> Search { get; set; }
}

public class Movie
{
    public string Title { get; set; }
    public string Year { get; set; }
    public string Director { get; set; }
    // Add other properties of the movie if needed
}