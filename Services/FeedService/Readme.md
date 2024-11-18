# Norce Product Feed Builder

## Overview

<p>The **FeedService** project is designed to create unified product feeds that combine data from multiple markets. It integrates with the Norce system to retrieve product information, processes that data for different markets, and then generates a combined feed where each product includes market-specific information. This feed is then sent to an Azure Storage Blob to be consumed by external systems. </p>

You will find most of the logic concerning the feed building in the __FeedBuilder.cs__ file

## Key Features

- **Multi-Market Feed Aggregation**  
  The project retrieves product data from Norce for multiple markets, such as Europe, USA, and other regions. Each market's data contains localized values (e.g., titles, descriptions, prices) based on the market's language and currency.

- **Unified Product Feed Generation**  
  The project combines the product data from various markets into a single, unified feed. Each product in the feed includes properties for all markets, with property names suffixed by the market identifier (e.g., `Title_EN`, `Price_US`, etc.). This structure allows external systems to easily access and use market-specific data for each product.

- **Flexible Market Support**  
  The system dynamically supports adding new markets. When new markets are added to the Norce system, the project automatically integrates the new market’s data into the unified feed without requiring significant changes to the codebase.

- **Azure Storage Blob Integration**  
  Once the feed is built, the project uploads the final unified feed to an Azure Storage Blob. This makes the feed available to external systems that rely on this data for e-commerce, marketing, or other business purposes.

## Workflow

1. **Data Fetching**  
   The system fetches a full feed of products from Norce, containing information such as product titles, descriptions, prices, and more, for all markets.

2. **Market Data Mapping**  
   After fetching the data, the project maps each product's properties to their respective market. For example, a product available in both the USA and Europe will have localized titles, descriptions, prices, and other relevant information.

3. **Feed Generation**  
   The project combines the market-specific data into a single product feed. Each product contains properties that are suffixed with the corresponding market identifier (e.g., `Title_US` for United States, `Title_SV` for Swedish market), allowing external systems to handle the product data efficiently.

4. **Feed Deployment**  
   Once the feed is built, it is uploaded to a **public** Azure Storage Blob. This makes it accessible to external systems for further processing, such as integration with e-commerce platforms or digital marketing tools.

## Technologies

- **Norce API**: For retrieving product data from various markets.
- **Azure Storage Blob**: For storing and distributing the final product feed.

## Future Enhancements

- **Improved Validation**  
  Future iterations may include enhanced object validation for products and variants to ensure data consistency and reliability across all markets.

- **Improved Error Handling**  
  Future iterations may include enhanced error handling and reporting to ensure data consistency and reliability across all markets.

- **Improved Logging**  
  Future iterations may include enhanced error handling and reporting to ensure data consistency and reliability across all markets.
