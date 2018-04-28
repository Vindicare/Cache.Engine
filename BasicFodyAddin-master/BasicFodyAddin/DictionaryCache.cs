using System;
using System.Collections.Generic;

namespace BasicFodyAddin
{
    /// <summary>
    /// 
    /// </summary>
    public static class DictionaryCache
    {
        static DictionaryCache()
        {
            Storage = new Dictionary<string, object>();
        }

        private static Dictionary<string, object> Storage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            return Storage.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Retrieve<T>(string key)
        {
            return (T)Storage[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Store(string key, object data)
        {
            Storage[key] = data;
        }        
    }
}
