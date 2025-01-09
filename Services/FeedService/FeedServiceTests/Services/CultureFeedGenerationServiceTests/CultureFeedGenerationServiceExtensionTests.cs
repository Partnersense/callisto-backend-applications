using FeedService.Domain.DTOs.External.DataFeedWatch;
using FeedService.Domain.Models;
using FeedService.Domain.Norce;
using FeedService.Services.CultureFeedGenerationServices;
using Microsoft.Extensions.Options;
using Moq;
using SharedLib.Options.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using File = FeedService.Domain.Norce.File;

namespace FeedServiceTests.Services.CultureFeedGenerationServiceTests
{
    public class CultureFeedGenerationServiceExtensionTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IOptionsMonitor<NorceBaseModuleOptions>> _norceOptionsMock;
        private readonly Mock<IOptionsMonitor<BaseModuleOptions>> _baseOptionsMock;
        private readonly Guid _testTraceId;
        private readonly BaseModuleOptions _baseOptions;

        public CultureFeedGenerationServiceExtensionTests()
        {
            _loggerMock = new Mock<ILogger>();
            _norceOptionsMock = new Mock<IOptionsMonitor<NorceBaseModuleOptions>>();
            _baseOptionsMock = new Mock<IOptionsMonitor<BaseModuleOptions>>();
            _testTraceId = Guid.NewGuid();

            _baseOptions = new BaseModuleOptions
            {
                SiteUrl = "http://test.com",
                CultureProductUrlList = new Dictionary<string, string>
                {
                    { "en-US", "http://test.com/us/product/uniqueUrlName" },
                    { "sv-SE", "http://test.com/se/product/uniqueUrlName" },
                    { "en-GB", "http://test.com/gb/product/uniqueUrlName" }
            }
            };
            _baseOptionsMock.Setup(x => x.CurrentValue).Returns(_baseOptions);
        }

        #region MapProductForCulture Tests

        [Fact]
        public void MapProductForCulture_WithValidData_ReturnsProcessedProducts()
        {
            // Arrange
            var product = CreateSampleNorceProduct();
            var culture = new CultureConfiguration
            {
                MarketCode = "GB",
                CultureCode = "en-GB"
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.MapProductForCulture(
                product,
                culture,
                _norceOptionsMock.Object,
                _baseOptionsMock.Object,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.Single(result);

            var mappedProduct = result[0] as DataFeedWatchDto;
            Assert.NotNull(mappedProduct);
            Assert.Equal("100035", mappedProduct.Id);
            Assert.Equal("3D Floral Sleeve Short Dress Black", mappedProduct.Title);
            Assert.Equal("http://test.com/gb/product/3d-floral-sleeve-short-dress-black-238591", mappedProduct.ProductLink);
        }

        [Fact]
        public void MapProductForCulture_WithNullProduct_ThrowsArgumentNullException()
        {
            // Arrange
            NorceFeedProductDto? nullProduct = null;
            var culture = new CultureConfiguration
            {
                MarketCode = "US",
                CultureCode = "en-US"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                CultureFeedGenerationServiceExtension.MapProductForCulture(
                    nullProduct!,
                    culture,
                    _norceOptionsMock.Object,
                    _baseOptionsMock.Object,
                    _loggerMock.Object,
                    _testTraceId));

            Assert.Equal("product", exception.ParamName);

            // Verify error logging
            VerifyErrorLogging(Times.Once());
        }

        [Fact]
        public void MapProductForCulture_WithNullVariants_ReturnsEmptyList()
        {
            // Arrange
            var product = CreateSampleNorceProduct();
            product.Variants = null;
            var culture = new CultureConfiguration
            {
                MarketCode = "US",
                CultureCode = "en-US"
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.MapProductForCulture(
                product,
                culture,
                _norceOptionsMock.Object,
                _baseOptionsMock.Object,
                _loggerMock.Object,
                _testTraceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Product Validation Tests

        [Fact]
        public void ProductAndVariantIsValid_WithValidData_ReturnsTrue()
        {
            // Arrange
            var product = CreateSampleNorceProduct();
            var variant = product.Variants![0];
            var cultureCode = "en-GB";

            // Act & Assert
            bool result = CultureFeedGenerationServiceExtension.ProductAndVariantIsValid(
                product,
                variant,
                cultureCode,
                _loggerMock.Object,
                _testTraceId);

            Assert.True(result);
        }

        [Fact]
        public void ProductAndVariantIsValid_WithInvalidCulture_ReturnsFalse()
        {
            // Arrange
            var product = CreateSampleNorceProduct();
            var variant = product.Variants![0];
            var invalidCultureCode = "invalid-culture";

            // Act & Assert
            bool result = CultureFeedGenerationServiceExtension.ProductAndVariantIsValid(
                product,
                variant,
                invalidCultureCode,
                _loggerMock.Object,
                _testTraceId);

            Assert.False(result);

            // Verify validation failure logging
            VerifyLogging(
                LogLevel.Information,
                "Product validation failed",
                Times.Once());
        }

        #endregion

        #region MappingMethods Tests

        [Fact]
        public void GetCultureSpecificParametricValue_WithValidParametric_ReturnsCorrectValue()
        {
            // Arrange
            var parametric = new Parametric
            {
                Code = "color",
                Type = "list",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Name = "Color",
                        ListValue = new ListValue { Code = "#000000", Name = "Black" }
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        Name = "Color",
                        ListValue = new ListValue { Code = "#000000", Name = "Svart" }
                    }
                }
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.GetCultureSpecificParametricValue(parametric, "en-GB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Black", result);
        }

        [Fact]
        public void GetCultureSpecificParametricValue_WithNullParametric_ReturnsNull()
        {
            // Arrange
            Parametric? nullParametric = null;

            // Act
            var result = CultureFeedGenerationServiceExtension.GetCultureSpecificParametricValue(nullParametric, "en-GB");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCultureSpecificParametricValue_WithNonMatchingCulture_ReturnsNull()
        {
            // Arrange
            var parametric = new Parametric
            {
                Code = "color",
                Type = "list",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Name = "Color",
                        ListValue = new ListValue { Code = "#000000", Name = "Black" }
                    }
                }
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.GetCultureSpecificParametricValue(parametric, "sv-SE");

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("http://test.com/product/uniqueUrlName", "test-product", "http://test.com/product/test-product")]
        [InlineData("http://test.com/product/uniqueUrlName", "", "")]
        [InlineData("", "test-product", "")]
        public void GetProductLink_WithVariousInputs_ReturnsExpectedResults(string url, string uniqueUrlName, string expected)
        {
            // Act
            var result = CultureFeedGenerationServiceExtension.GetProductLink(url, uniqueUrlName);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetProductLink_WithNullUrlName_ReturnsEmptyString()
        {
            // Arrange
            string url = "http://test.com/product/uniqueUrlName";
            string? uniqueUrlName = null;

            // Act
            var result = CultureFeedGenerationServiceExtension.GetProductLink(url, uniqueUrlName);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ExtractAdditionalImages_WithValidFiles_ReturnsCorrectUrls()
        {
            // Arrange
            var files = new List<File>
            {
                new() { Type = "asset", Url = "http://test.com/image1.jpg" },
                new() { Type = "asset", Url = "http://test.com/image2.jpg" },
                new() { Type = "external", Url = "http://test.com/image3.jpg" }
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.ExtractAdditionalImages(files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("http://test.com/image1.jpg", result);
            Assert.Contains("http://test.com/image2.jpg", result);
        }

        [Fact]
        public void ExtractFlags_WithValidFlags_ReturnsCorrectDictionary()
        {
            // Arrange
            var flags = new List<Flag>
            {
                new()
                {
                    Code = "test-flag",
                    GroupCode = "test-group",
                    Cultures = new List<FlagCulture>
                    {
                        new() { CultureCode = "en-US", Name = "Test Flag" },
                        new() { CultureCode = "sv-SE", Name = "Test Flagga" }
                    }
                }
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.ExtractFlags(flags, "en-US");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Flag", result["test-group"]);
        }

        [Fact]
        public void ExtractParametrics_WithValidData_ReturnsCorrectDictionary()
        {
            // Arrange
            var product = new NorceFeedProductDto
            {
                Parametrics = new List<Parametric>
        {
            new()
            {
                Code = "material",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Value = "Cotton"
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        Value = "Bomull"
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        Value = "Bomull"
                    }
                }
            }
        }
            };

            var variant = new NorceFeedVariant
            {
                VariantDefiningParametrics = new List<Parametric>
        {
            new()
            {
                Code = "size",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        ListValue = new ListValue { Name = "Large" }
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        ListValue = new ListValue { Name = "Stor" }
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        ListValue = new ListValue { Name = "Stor" }
                    }
                }
            },
            new()
            {
                Code = "color",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        ListValue = new ListValue { Code = "#000000", Name = "Black" }
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        ListValue = new ListValue { Code = "#000000", Name = "Svart" }
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        ListValue = new ListValue { Code = "#000000", Name = "Svart" }
                    }
                }
            }
        },
                AdditionalParametrics = new List<Parametric>
        {
            new()
            {
                Code = "tags",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        MultipleValues = new List<ListValue>
                        {
                            new() { Name = "New" },
                            new() { Name = "Featured" }
                        }
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        MultipleValues = new List<ListValue>
                        {
                            new() { Name = "Ny" },
                            new() { Name = "Utvald" }
                        }
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        MultipleValues = new List<ListValue>
                        {
                            new() { Name = "Ny" },
                            new() { Name = "Utvalgt" }
                        }
                    }
                }
            },
            new()
            {
                Code = "style",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Value = "Casual"
                    },
                    new()
                    {
                        CultureCode = "sv-SE",
                        Value = "Vardaglig"
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        Value = "Hverdagslig"
                    }
                }
            }
        }
            };

            // Test for English (GB)
            var resultGB = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "en-GB");
            Assert.NotNull(resultGB);
            Assert.Equal(5, resultGB.Count);
            Assert.Equal("Cotton", resultGB["material"]);
            Assert.Equal("Large", resultGB["size"]);
            Assert.Equal("Black", resultGB["color"]);
            Assert.Equal("New,Featured", resultGB["tags"]);
            Assert.Equal("Casual", resultGB["style"]);

            // Test for Swedish
            var resultSE = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "sv-SE");
            Assert.NotNull(resultSE);
            Assert.Equal(5, resultSE.Count);
            Assert.Equal("Bomull", resultSE["material"]);
            Assert.Equal("Stor", resultSE["size"]);
            Assert.Equal("Svart", resultSE["color"]);
            Assert.Equal("Ny,Utvald", resultSE["tags"]);
            Assert.Equal("Vardaglig", resultSE["style"]);

            // Test for Norwegian
            var resultNO = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "nb-NO");
            Assert.NotNull(resultNO);
            Assert.Equal(5, resultNO.Count);
            Assert.Equal("Bomull", resultNO["material"]);
            Assert.Equal("Stor", resultNO["size"]);
            Assert.Equal("Svart", resultNO["color"]);
            Assert.Equal("Ny,Utvalgt", resultNO["tags"]);
            Assert.Equal("Hverdagslig", resultNO["style"]);
        }

        [Fact]
        public void ExtractParametrics_WithEmptyData_ReturnsEmptyDictionary()
        {
            // Arrange
            var product = new NorceFeedProductDto
            {
                Parametrics = new List<Parametric>()
            };

            var variant = new NorceFeedVariant
            {
                VariantDefiningParametrics = new List<Parametric>(),
                AdditionalParametrics = new List<Parametric>()
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "en-GB");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ExtractParametrics_WithNonMatchingCulture_ReturnsEmptyDictionary()
        {
            // Arrange
            var product = new NorceFeedProductDto
            {
                Parametrics = new List<Parametric>
        {
            new()
            {
                Code = "material",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Value = "Cotton"
                    }
                }
            }
        }
            };

            var variant = new NorceFeedVariant
            {
                VariantDefiningParametrics = new List<Parametric>(),
                AdditionalParametrics = new List<Parametric>()
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "sv-SE");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ExtractParametrics_WithMixedValueTypes_ReturnsCorrectDictionary()
        {
            // Arrange
            var product = new NorceFeedProductDto
            {
                Parametrics = new List<Parametric>
        {
            new()
            {
                Code = "weight",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        Value = "100g"
                    }
                }
            },
            new()
            {
                Code = "color",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        ListValue = new ListValue { Name = "Red" }
                    }
                }
            },
            new()
            {
                Code = "categories",
                Cultures = new List<Culture>
                {
                    new()
                    {
                        CultureCode = "en-GB",
                        MultipleValues = new List<ListValue>
                        {
                            new() { Name = "Summer" },
                            new() { Name = "Casual" }
                        }
                    }
                }
            }
        }
            };

            var variant = new NorceFeedVariant
            {
                VariantDefiningParametrics = new List<Parametric>(),
                AdditionalParametrics = new List<Parametric>()
            };

            // Act
            var result = CultureFeedGenerationServiceExtension.ExtractParametrics(product, variant, "en-GB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("100g", result["weight"]);
            Assert.Equal("Red", result["color"]);
            Assert.Equal("Summer,Casual", result["categories"]);
        }
        #endregion

        #region Test Helpers

        private NorceFeedProductDto CreateSampleNorceProduct()
        {
            return new NorceFeedProductDto
            {
                Code = "V40548",
                Manufacturer = new Manufacturer
                {
                    Code = "Happy Holly",
                    Name = "Happy Holly"
                },
                Names = new List<Names>
                {
                    new()
                    {
                        CultureCode = "sv-SE",
                        Name = "3D Floral Sleeve Short Dress Svart",
                        UniqueUrlName = "3d-floral-sleeve-short-dress-svart"
                    },
                    new()
                    {
                        CultureCode = "en-GB",
                        Name = "3D Floral Sleeve Short Dress Black",
                        UniqueUrlName = "3d-floral-sleeve-short-dress-black"
                    },
                    new()
                    {
                        CultureCode = "nb-NO",
                        Name = "3D Floral Sleeve Short Dress Svart",
                        UniqueUrlName = "3d-floral-sleeve-short-dress-svart"
                    }
                },
                PrimaryCategory = new Category
                {
                    Code = "Dresses",
                    Cultures = new List<Culture>
                    {
                        new()
                        {
                            CultureCode = "sv-SE",
                            Name = "Klänningar",
                            FullName = "Fashion - Kvinna - Klänningar",
                            Synonyms = ""
                        },
                        new()
                        {
                            CultureCode = "en-GB",
                            Name = "Dresses",
                            FullName = "Fashion - Woman - Dresses",
                            Synonyms = ""
                        },
                        new()
                        {
                            CultureCode = "nb-NO",
                            Name = "Klänningar",
                            FullName = "Fashion - Kvinna - Klänningar",
                            Synonyms = ""
                        }
                    }
                },
                Variants = new List<NorceFeedVariant>
                {
                    new()
                    {
                        PartNo = "100035",
                        ManufacturerPartNo = "3D Floral Sleeve Short Dress",
                        Names = new List<Names>
                        {
                            new()
                            {
                                CultureCode = "sv-SE",
                                Name = "3D Floral Sleeve Short Dress Svart",
                                UniqueUrlName = "3d-floral-sleeve-short-dress-svart-238591"
                            },
                            new()
                            {
                                CultureCode = "en-GB",
                                Name = "3D Floral Sleeve Short Dress Black",
                                UniqueUrlName = "3d-floral-sleeve-short-dress-black-238591"
                            },
                            new()
                            {
                                CultureCode = "nb-NO",
                                Name = "3D Floral Sleeve Short Dress Svart",
                                UniqueUrlName = "3d-floral-sleeve-short-dress-svart-238591"
                            }
                        },
                        Status = "active",
                        Prices = new List<Price>
                        {
                            new()
                            {
                                SalesArea = "Sweden",
                                PriceListCode = "kampanj",
                                Currency = "SEK",
                                Value = 499.0,
                                IsDiscountable = true,
                                Original = 499.0,
                                VatRate = 1.25,
                                IsActive = true,
                                ValueIncVat = new decimal(623.75),
                                AvailableOnWarehouses = new List<AvailableOnWarehouse>
                                {
                                    new()
                                    {
                                        Code = "STD",
                                        LocationCode = "standard"
                                    }
                                }
                            }
                        },
                        OnHands = new List<NorceFeedVariant.OnHandRecord>
                        {
                            new()
                            {
                                Warehouse = new NorceFeedVariant.Warehouse
                                {
                                    Code = "STD",
                                    LocationCode = "standard"
                                },
                                WarehouseType = "warehouse",
                                Value = 99.0,
                                LeadTimeDayCount = 2,
                                AvailableOnStores = Array.Empty<string>(),
                                AvailableOnPriceLists = new[] { "kampanj" }
                            }
                        },
                        PrimaryImage = new PrimaryImage
                        {
                            Type = "asset",
                            FileCode = "PrimaryImage",
                            Key = "0ff9e168-26c3-44cd-8b17-a72b2df9242f",
                            MimeType = "image/jpeg",
                            Url = "https://media.test.cdn-norce.tech/1006/0ff9e168-26c3-44cd-8b17-a72b2df9242f"
                        },
                        Files = new List<File>
                        {
                            new()
                            {
                                Type = "asset",
                                Code = "F_08AC7899-2C66",
                                FileCode = "i_detail_png",
                                Key = "ad46cf18-c1ac-4ec2-95cb-620a289e62b2",
                                MimeType = "image/png",
                                Url = "https://media.test.cdn-norce.tech/1006/ad46cf18-c1ac-4ec2-95cb-620a289e62b2"
                            }
                        },
                        VariantDefiningParametrics = new List<Parametric>
                        {
                            new()
                            {
                                Code = "color",
                                Type = "list",
                                Cultures = new List<Culture>
                                {
                                    new()
                                    {
                                        CultureCode = "sv-SE",
                                        Name = "Färg",
                                        ListValue = new ListValue { Code = "#000000", Name = "Svart" }
                                    },
                                    new()
                                    {
                                        CultureCode = "en-GB",
                                        Name = "Color",
                                        ListValue = new ListValue { Code = "#000000", Name = "Black"}
                                    }
                                }
                            },
                            new()
                            {
                                Code = "storlek",
                                Type = "list",
                                Cultures = new List<Culture>
                                {
                                    new()
                                    {
                                        CultureCode = "sv-SE",
                                        Name = "Storlek",
                                        ListValue = new ListValue { Code = "32/34", Name = "32/34" }
                                    },
                                    new()
                                    {
                                        CultureCode = "en-GB",
                                        Name = "Size",
                                        ListValue = new ListValue { Code = "32/34", Name = "32/34"}
                                    }
                                }
                            }
                        },
                        Logistics = new Logistics
                        {
                            Width = 42.5,
                            Height = 10,
                            Depth = 30,
                            Weight = 0.5
                        }
                    }
                },
                Flags = new List<Flag>
                {
                    new()
                    {
                        Code = "news",
                        Cultures = new List<FlagCulture>
                        {
                            new()
                            {
                                CultureCode = "sv-SE",
                                Name = "Nyhet",
                                GroupName = "Promotion flags"
                            },
                            new()
                            {
                                CultureCode = "en-GB",
                                Name = "News",
                                GroupName = "Promotion flags"
                            }
                        },
                        Id = 4,
                        GroupCode = "promotionFlags"
                    }
                },
                PrimaryImage = new PrimaryImage
                {
                    Type = "asset",
                    FileCode = "PrimaryImage",
                    Key = "e6ad86de-6787-40ce-b351-d62e004d9c48",
                    MimeType = "image/jpeg",
                    Url = "https://media.test.cdn-norce.tech/1006/e6ad86de-6787-40ce-b351-d62e004d9c48"
                },
                Files = new List<File>
                {
                    new()
                    {
                        Type = "asset",
                        Code = "https://media.test.cdn-norce.tech/1006/ad46cf18-c1ac-4ec2-95cb-620a289e62b2",
                        FileCode = "i_detail_png",
                        Key = "ad46cf18-c1ac-4ec2-95cb-620a289e62b2",
                        MimeType = "image/png",
                        Url = "https://media.test.cdn-norce.tech/1006/ad46cf18-c1ac-4ec2-95cb-620a289e62b2"
                    }
                },
                Parametrics = new List<Parametric>
                {
                    new()
                    {
                        Code = "bredd",
                        Type = "text",
                        Cultures = new List<Culture>
                        {
                            new()
                            {
                                CultureCode = "sv-SE",
                                Name = "Bredd"
                            },
                            new()
                            {
                                CultureCode = "en-GB",
                                Name = "Width"
                            }
                        },
                    }
                },
                Texts = new List<Text>
                {
                    new()
                    {
                        CultureCode = "sv-SE",
                        Description = "Jerseyklänning från Happy Holly.<br />- Stretchig, mjuk kvalitet.<br />- Figurnära passform.<br />- Rundad halsringning.<br />- Avskärning i midjan.<br />- Detalj.<br />- Spets på ärmarna.<br />- Längd från axel: 92 cm i storlek 36/38."
                    },
                    new()
                    {
                        CultureCode = "en-GB",
                        Description = "<p>Jersey dress from Happy Holly.<br />- Stretchy, soft fabric.<br />- Figure-hugging fit.<br />- Rounded neckline.<br />- Waist seam.<br />- Detail.<br />- Lace on the sleeves.<br />- Length from shoulder: 92 cm in size 36/38.</p>"
                    }
                }
            };
        }

        private void VerifyLogging(LogLevel logLevel, string messageContains, Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messageContains)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        private void VerifyErrorLogging(Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        #endregion
    }
}
