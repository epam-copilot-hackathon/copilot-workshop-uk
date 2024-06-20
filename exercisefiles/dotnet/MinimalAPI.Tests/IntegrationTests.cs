namespace IntegrationTests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHelloWorld()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World!", content);
    }

    [Fact]
    public async Task Get_DaysBetweenDates_ReturnsCorrectDays()
    {
        // Arrange
 
        var date1 = new DateTime(2022, 1, 1);
        var date2 = new DateTime(2022, 1, 31);
    
        // Act
        var response = await _client.GetAsync($"/DaysBetweenDates?date1={date1:yyyy-MM-dd}&date2={date2:yyyy-MM-dd}");
    
        // Assert
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.Equal($"Days between {date1.ToShortDateString()} and {date2.ToShortDateString()}: 30", stringResponse);
    }

        [Theory]
        [InlineData("+34666777888", true)]
        [InlineData("+3466677788", false)]
        [InlineData("+44666777888", false)]
        public async Task Get_ValidatePhoneNumber_ReturnsCorrectValidation(string phoneNumber, bool isValid)
        {
            // Arrange
     
            // Act
            var response = await _client.GetAsync($"/validatephonenumber?phoneNumber={phoneNumber}");

            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Equal(isValid.ToString(), stringResponse);
        }

        [Theory]
        [InlineData("12345678Z", "valid")]
        [InlineData("12345678A", "invalid")]
        public async Task Get_ValidateSpanishDNI_ReturnsCorrectValidation(string dni, string validation)
        {
            // Arrange
        

            // Act
            var response = await _client.GetAsync($"/validatespanishdni?dni={dni}");

            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Equal(validation, stringResponse);
        }
    
}
