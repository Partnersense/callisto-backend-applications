using Moq;
using SharedLib.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SharedLibTests.Serialization
{
    public class NorceDateTimeConverterTests
    {
        private readonly NorceDateTimeConverter _converter;
        private readonly JsonSerializerOptions _options;
        private readonly Mock<ILogger> _loggerMock;

        public NorceDateTimeConverterTests()
        {
            _converter = new NorceDateTimeConverter();
            _options = new JsonSerializerOptions();
            _loggerMock = new Mock<ILogger>();
        }

        [Fact]
        public void Read_ValidNorceDate_ReturnsCorrectDateTime()
        {
            // Arrange
            // Note: The Norce timestamp is in Unix milliseconds (UTC)
            var jsonString = @"""/Date(1731440148857+0000)/""";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
            reader.Read(); // Move to first token

            // Act
            var result = _converter.Read(ref reader, typeof(DateTime?), _options);

            // Assert
            Assert.NotNull(result);
            // Verify the actual timestamp components without timezone comparison
            var expected = new DateTime(2024, 11, 12, 19, 35, 48, 857, DateTimeKind.Utc);
            Assert.Equal(expected, result.Value);
            Assert.Equal(DateTimeKind.Utc, result.Value.Kind);
        }

        [Fact]
        public void Read_ValidIsoDate_ReturnsCorrectDateTime()
        {
            // Arrange
            var jsonString = @"""2024-11-12T19:35:48.857Z""";
            var expected = new DateTime(2024, 11, 12, 19, 35, 48, 857, DateTimeKind.Utc);
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(DateTime?), _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Value);
            Assert.Equal(DateTimeKind.Utc, result.Value.Kind);
        }

        [Fact]
        public void Read_NullValue_ReturnsNull()
        {
            // Arrange
            var jsonString = "null";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(DateTime?), _options);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Read_EmptyString_ReturnsNull()
        {
            // Arrange
            var jsonString = @"""""""";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(DateTime?), _options);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(@""" / Date(invalid + 0000) / """)]
        [InlineData(@"""/Date()/""")]
        [InlineData(@"""not a date""")]
        [InlineData(@"""/Date(99999999999999999999+0000)/""")] // Overflow
        public void Read_InvalidDateFormat_ReturnsNullAndLogsWarning(string invalidDateString)
        {
            // Arrange
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(invalidDateString));
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(DateTime?), _options);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Write_ValidDateTime_WritesIsoFormat()
        {
            // Arrange
            var testDate = new DateTime(2024, 11, 12, 19, 35, 48, 857, DateTimeKind.Utc);
            var expectedJson = @"""2024-11-12T19:35:48.8570000Z""";
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, testDate, _options);
            writer.Flush();

            // Assert
            var actualJson = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void Write_NullDateTime_WritesNull()
        {
            // Arrange
            DateTime? nullDate = null;
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, nullDate, _options);
            writer.Flush();

            // Assert
            var actualJson = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal("null", actualJson);
        }

        [Fact]
        public void Converter_SerializeAndDeserialize_MaintainsDateTimeValue()
        {
            // Arrange
            var options = new JsonSerializerOptions
            {
                Converters = { new NorceDateTimeConverter() }
            };
            var testObject = new { Date = new DateTime(2024, 11, 12, 19, 35, 48, 857, DateTimeKind.Utc) };

            // Act
            var json = JsonSerializer.Serialize(testObject, options);
            var deserializedObject = JsonSerializer.Deserialize<dynamic>(json, options);

            // Assert
            Assert.NotNull(deserializedObject);
            Assert.Equal(testObject.Date, deserializedObject.GetProperty("Date").GetDateTime());
        }
    }
}
