

namespace GDT.CEC.Repository.Helpers
{
    public static class ExtensionHelpers
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string input)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
        }
        public static bool IsNullOrEmptyAndWhiteSpace(this string input)
        {
            return string.IsNullOrEmpty(input) && string.IsNullOrWhiteSpace(input);
        }

        public static IEnumerable<T> SortAndPage<T>(this IEnumerable<T> source,
                                    Func<T, object> sortExpression,
                                    string sortDirection,
                                    int pageIndex,
                                    int pageSize)
        {

            var sortedData = sortDirection == "asc"
                ? source.OrderBy(sortExpression)
                : source.OrderByDescending(sortExpression);



            var pagedData = sortedData.Skip((pageIndex - 1) * pageSize)
                                       .Take(pageSize);


            return pagedData;
        }
    }
}
