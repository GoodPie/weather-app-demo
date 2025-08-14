namespace DAL.Repository.Contracts;

public interface IGeoCodeSearchRepository
{
    /// <summary>
    /// Determines if a search query has previously been searched.
    /// </summary>
    /// <param name="query">The search query to check.</param>
    /// <param name="cancelToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the query has been searched before.</returns>
    public Task<bool> HasBeenSearchedAsync(string query, CancellationToken cancelToken);

    /// <summary>
    /// Adds a search query to the database along with the number of results returned.
    /// </summary>
    /// <param name="query">The search query to be recorded.</param>
    /// <param name="resultCount">The number of results returned for the search query.</param>
    /// <param name="cancelToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddSearchAsync(string query, int resultCount, CancellationToken cancelToken);
}