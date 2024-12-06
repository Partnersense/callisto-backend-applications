using SharedLib.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedLib.Options.OptionsTypeConvertersExtension;

namespace SharedLibTests.Options.Extensions
{
    public class OptionsTypeConvertersExtensionTests
    {
        #region ConvertPipeSeparatedStringList Tests

        [Theory]
        [InlineData("value1|value2|value3", new[] { "value1", "value2", "value3" })]
        [InlineData("single", new[] { "single" })]
        [InlineData("value1||value3", new[] { "value1", "value3" })]
        [InlineData("  spaced  |  values  ", new[] { "  spaced  ", "  values  " })]
        public void ConvertPipeSeparatedStringList_WithValidInput_ReturnsExpectedList(string input, string[] expected)
        {
            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedStringList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("|||")]
        public void ConvertPipeSeparatedStringList_WithEmptyOrNullInput_ReturnsEmptyList(string? input)
        {
            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedStringList(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ConvertPipeSeparatedStringList_WithComplexInput_PreservesValues()
        {
            // Arrange
            var input = "value,with,commas|value;with;semicolons|value with spaces";
            var expected = new[] { "value,with,commas", "value;with;semicolons", "value with spaces" };

            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedStringList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region ConvertPipeSeparatedIntList Tests

        [Theory]
        [InlineData("1|2|3", new[] { 1, 2, 3 })]
        [InlineData("42", new[] { 42 })]
        [InlineData("1||3", new[] { 1, 3 })]
        [InlineData("1|0|3", new[] { 1, 0, 3 })]
        [InlineData("0|0|0", new[] { 0, 0, 0 })]
        [InlineData("-1|0|1", new[] { -1, 0, 1 })]
        public void ConvertPipeSeparatedIntList_WithValidInput_ReturnsExpectedList(string input, int[] expected)
        {
            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("|||")]
        public void ConvertPipeSeparatedIntList_WithEmptyOrNullInput_ReturnsEmptyList(string? input)
        {
            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("1|invalid|3", new[] { 1, 3 })]
        [InlineData("1|abc|0|def", new[] { 1, 0 })]
        [InlineData("invalid|text|here", new int[0])]
        [InlineData("1|2.5|3", new[] { 1, 3 })]
        public void ConvertPipeSeparatedIntList_WithInvalidNumbers_FiltersOutInvalidValues(string input, int[] expected)
        {
            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertPipeSeparatedIntList_WithMaxAndMinValues_HandlesThemCorrectly()
        {
            // Arrange
            var input = $"{int.MinValue}|0|{int.MaxValue}";
            var expected = new[] { int.MinValue, 0, int.MaxValue };

            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertPipeSeparatedIntList_WithWhitespaceAroundNumbers_ParsesCorrectly()
        {
            // Arrange
            var input = " 1 | 2 | 3 ";
            var expected = new[] { 1, 2, 3 };

            // Act
            var result = OptionsTypeConvertersExtension.ConvertPipeSeparatedIntList(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
