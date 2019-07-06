using System;
using System.Collections.Generic;
using System.Linq;

namespace Awesome.Net
{
    /// <summary>
    /// A shortcut to use <see cref="Random"/> class.
    /// Also provides some useful methods.
    /// </summary>
    public static class RandomHelper
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
        /// that is, the range of return values includes minValue but not maxValue. 
        /// If minValue equals maxValue, minValue is returned.
        /// </returns>
        public static int GetRandom(int minValue, int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; 
        /// that is, the range of return values ordinarily includes zero but not maxValue. 
        /// However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        public static int GetRandom(int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than <see cref="int.MaxValue"/>.</returns>
        public static int GetRandom()
        {
            lock (Rnd)
            {
                return Rnd.Next();
            }
        }

        /// <summary>
        /// Gets random of given objects.
        /// </summary>
        /// <typeparam name="T">Type of the objects</typeparam>
        /// <param name="objs">List of object to select a random one</param>
        public static T GetRandomOf<T>(params T[] objs)
        {
            if (objs.IsNullOrEmpty()) throw new ArgumentNullException(nameof(objs));

            return objs[GetRandom(0, objs.Length)];
        }

        /// <summary>
        /// Gets random item from the given list.
        /// </summary>
        /// <typeparam name="T">Type of the objects</typeparam>
        /// <param name="list">List of object to select a random one</param>
        public static T GetRandomOfList<T>(IList<T> list)
        {
            if (list.IsNullOrEmpty()) throw new ArgumentNullException(nameof(list));

            return list[GetRandom(0, list.Count)];
        }

        /// <summary>
        /// Generates a randomized list from given enumerable.
        /// </summary>
        /// <typeparam name="T">Type of items in the list</typeparam>
        /// <param name="items">items</param>
        public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var currentList = new List<T>(items);
            var randomList = new List<T>();

            while (currentList.Any())
            {
                var randomIndex = GetRandom(0, currentList.Count);
                randomList.Add(currentList[randomIndex]);
                currentList.RemoveAt(randomIndex);
            }

            return randomList;
        }


        // Some confusing chars are ignored: http://www.crockford.com/wrmg/base32.html
        public static string Generate26UniqueId()
        {
            // Generate a base32 Guid value
            var guid = Guid.NewGuid().ToByteArray();

            var hs = BitConverter.ToInt64(guid, 0);
            var ls = BitConverter.ToInt64(guid, 8);

            const string encode32Chars = "0123456789abcdefghjkmnpqrstvwxyz";
            var charBuffer = new char[26];

            charBuffer[0] = encode32Chars[(int) (hs >> 60) & 31];
            charBuffer[1] = encode32Chars[(int) (hs >> 55) & 31];
            charBuffer[2] = encode32Chars[(int) (hs >> 50) & 31];
            charBuffer[3] = encode32Chars[(int) (hs >> 45) & 31];
            charBuffer[4] = encode32Chars[(int) (hs >> 40) & 31];
            charBuffer[5] = encode32Chars[(int) (hs >> 35) & 31];
            charBuffer[6] = encode32Chars[(int) (hs >> 30) & 31];
            charBuffer[7] = encode32Chars[(int) (hs >> 25) & 31];
            charBuffer[8] = encode32Chars[(int) (hs >> 20) & 31];
            charBuffer[9] = encode32Chars[(int) (hs >> 15) & 31];
            charBuffer[10] = encode32Chars[(int) (hs >> 10) & 31];
            charBuffer[11] = encode32Chars[(int) (hs >> 5) & 31];
            charBuffer[12] = encode32Chars[(int) hs & 31];

            charBuffer[13] = encode32Chars[(int) (ls >> 60) & 31];
            charBuffer[14] = encode32Chars[(int) (ls >> 55) & 31];
            charBuffer[15] = encode32Chars[(int) (ls >> 50) & 31];
            charBuffer[16] = encode32Chars[(int) (ls >> 45) & 31];
            charBuffer[17] = encode32Chars[(int) (ls >> 40) & 31];
            charBuffer[18] = encode32Chars[(int) (ls >> 35) & 31];
            charBuffer[19] = encode32Chars[(int) (ls >> 30) & 31];
            charBuffer[20] = encode32Chars[(int) (ls >> 25) & 31];
            charBuffer[21] = encode32Chars[(int) (ls >> 20) & 31];
            charBuffer[22] = encode32Chars[(int) (ls >> 15) & 31];
            charBuffer[23] = encode32Chars[(int) (ls >> 10) & 31];
            charBuffer[24] = encode32Chars[(int) (ls >> 5) & 31];
            charBuffer[25] = encode32Chars[(int) ls & 31];

            return new string(charBuffer);
        }
    }
}
