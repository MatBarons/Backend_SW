namespace Academy.Api.Domain.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the original <paramref name="enumerable"/>
        /// or an empty <see cref="IEnumerable{T}"/> if <paramref name="enumerable"/> is null.
        /// <br/>
        /// Why? So that the following example won't throw an error:
        /// <code>
        /// foreach (var item in enumerable.EmptyIfNull()) { ... };
        /// </code>
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Returns true when an <see cref="IEnumerable{T}"/> is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
        {
            if (enumerable == null || !enumerable.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
