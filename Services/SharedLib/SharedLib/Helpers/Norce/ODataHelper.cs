using SharedLib.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedLib.Models.Norce.Query;

namespace SharedLib.Helpers.Norce
{
    public static class ODataHelper
    {
        /// <summary>
        /// Deserializes an OData JSON response into a list of the specified type
        /// </summary>
        /// <typeparam name="T">The type of objects in the response value array</typeparam>
        /// <param name="json">The JSON string to deserialize</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <returns>The deserialized list of objects</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null or empty</exception>
        /// <exception cref="JsonException">Thrown when deserialization fails</exception>
        public static List<T>? DeserializeODataResponse<T>(string json, ILogger logger, Guid? traceId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                    throw new ArgumentNullException(nameof(json));

                var response = JsonSerializer.Deserialize<OdataWrapperResponse<T>>(json);
                return response?.Value;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage}",
                    traceId,
                    nameof(ODataHelper),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(DeserializeODataResponse),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to deserialize OData response"
                );
                throw;
            }
        }
    }
}
