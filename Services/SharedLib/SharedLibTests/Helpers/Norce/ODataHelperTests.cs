using Moq;
using SharedLib.Helpers.Norce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedLib.Models.Norce.Query;

namespace SharedLibTests.Helpers.Norce
{
    public class ODataHelperTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Guid _testTraceId;

        public ODataHelperTests()
        {
            _loggerMock = new Mock<ILogger>();
            _testTraceId = Guid.NewGuid();
        }

        [Fact]
        public void DeserializeODataResponse_WithValidJson_ReturnsDeserializedList()
        {
            // Arrange
            var testData = new List<ClientCultureResponse>
            {
                new()
                {
                    CultureCode = "en-GB",
                    ClientId = 1006,
                    IsPrimary = false,
                    Created = DateTime.UtcNow,
                    CreatedBy = 1195
                }
            };

            var oDataResponse = new OdataWrapperResponse<ClientCultureResponse>
            {
                Context = "http://test.com/$metadata#ClientCultureResponses",
                Value = testData
            };

            var json = JsonSerializer.Serialize(oDataResponse);

            // Act
            var result = ODataHelper.DeserializeODataResponse<ClientCultureResponse>(json, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("en-GB", result[0].CultureCode);
            Assert.Equal(1006, result[0].ClientId);
        }

        [Fact]
        public void DeserializeODataResponse_WithNullJson_ThrowsArgumentNullException()
        {
            // Arrange
            string? nullJson = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ODataHelper.DeserializeODataResponse<ClientCultureResponse>(nullJson!, _loggerMock.Object, _testTraceId));
        }

        [Fact]
        public void DeserializeODataResponse_WithEmptyJson_ThrowsArgumentNullException()
        {
            // Arrange
            var emptyJson = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ODataHelper.DeserializeODataResponse<ClientCultureResponse>(emptyJson, _loggerMock.Object, _testTraceId));
        }

        [Fact]
        public void DeserializeODataResponse_WithInvalidJson_ThrowsJsonException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            // Act & Assert
            Assert.Throws<JsonException>(() =>
                ODataHelper.DeserializeODataResponse<ClientCultureResponse>(invalidJson, _loggerMock.Object, _testTraceId));

            // Verify error logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void DeserializeODataResponse_WithNullValueProperty_ReturnsNull()
        {
            // Arrange
            var oDataResponse = new OdataWrapperResponse<ClientCultureResponse>
            {
                Context = "http://test.com/$metadata#ClientCultureResponses",
                Value = null
            };

            var json = JsonSerializer.Serialize(oDataResponse);

            // Act
            var result = ODataHelper.DeserializeODataResponse<ClientCultureResponse>(json, _loggerMock.Object, _testTraceId);

            // Assert
            Assert.Null(result);
        }
    }

   
}
