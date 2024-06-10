namespace Academy.Api.Domain.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// If <paramref name="dict"/> contains <paramref name="key"/>, returns its corresponding value,
        /// otherwise returns <paramref name="defaultValue"/>
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
