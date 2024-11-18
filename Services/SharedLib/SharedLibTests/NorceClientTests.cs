using FluentAssertions;
using SharedLib.Clients.Norce;

namespace SharedLibTests;

public class NorceClientTests
{

    [Theory]
    [InlineData("http://baseUrl/products", "api/values")]
    [InlineData("http://baseUrl/products/", "api/values")]
    [InlineData("http://baseUrl/products", "/api/values")]
    [InlineData("http://baseUrl/products/", "/api/values")]

    public void NorceConnect_GetEndpoint_ShouldReturnCorrectEndpoint(string baseUrl, string serviceEndpoint)
    {
        // Arrange
        var expectedUrl = "http://baseUrl/products/api/values";

        // Act
        var result = Endpoints.GetEndpoint(baseUrl, serviceEndpoint);

        // Assert
        result.Should().Be(expectedUrl);
    }
}