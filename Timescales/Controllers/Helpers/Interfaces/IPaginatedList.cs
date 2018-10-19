namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IPaginatedList
    {
        bool HasNextPage { get; }

        bool HasPreviousPage { get; }

        int PageIndex { get; }

        int TotalPages { get; }
    }
}