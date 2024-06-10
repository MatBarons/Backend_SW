using System.Security.Cryptography;

namespace Academy.Api.Domain.Extensions;

/// <summary>
/// Extensions for lists
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Shuffles the elements in a list, changing their ordering
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            byte[] box;
            do
            {
                box = RandomNumberGenerator.GetBytes(1);
            } while (!(box[0] < n * (Byte.MaxValue / n)));

            int k = (box[0] % n);
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}