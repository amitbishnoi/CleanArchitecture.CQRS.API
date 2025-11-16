using System.Text.Json.Serialization;

namespace Application.Common.Responses
{
    /// <summary>
    /// Metadata for paginated responses.
    /// Included in ApiResponse to provide pagination information to clients.
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Search term used in the query (if any)
        /// </summary>
        [JsonPropertyName("searchTerm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Creates pagination metadata
        /// </summary>
        public static PaginationMetadata Create(int pageNumber, int pageSize, int totalCount, string? searchTerm = null)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var hasNextPage = pageNumber < totalPages;
            var hasPreviousPage = pageNumber > 1;

            return new PaginationMetadata
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
                SearchTerm = searchTerm
            };
        }
    }
}
