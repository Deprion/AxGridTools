﻿using System.Collections.Generic;
using System.Reflection;

namespace AxGrid.Utils
{
    public static class DictionaryHelper
    {
        #region Dictionary
        /// <summary>
        /// Unionise two dictionaries of generic types.
        /// Duplicates take their value from the leftmost dictionary.
        /// </summary>
        /// <typeparam name="T1">Generic key type</typeparam>
        /// <typeparam name="T2">Generic value type</typeparam>
        /// <param name="D1">Dictionary 1</param>
        /// <param name="D2">Dictionary 2</param>
        /// <returns>The combined dictionaries.</returns>
        public static Dictionary<T1, T2> UnionDictionaries<T1, T2>(Dictionary<T1, T2> D2, Dictionary<T1, T2> D1)
        {
            Dictionary<T1, T2> rd = new Dictionary<T1, T2>(D1);
            foreach (var key in D2.Keys)
            {
                if (!rd.ContainsKey(key))
                    rd.Add(key, D2[key]);
                else if(rd[key].GetType().IsGenericType)
                {
                    if (rd[key].GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        var mBase = MethodBase.GetCurrentMethod();
                        MethodInfo info = mBase is MethodInfo ? (MethodInfo)mBase : typeof(DictionaryHelper).GetMethod("UnionDictionaries", BindingFlags.Public | BindingFlags.Static);
                        var genericMethod = info.MakeGenericMethod(rd[key].GetType().GetGenericArguments()[0], rd[key].GetType().GetGenericArguments()[1]);
                        var invocationResult = genericMethod.Invoke(null, new object[] { rd[key], D2[key] });
                        rd[key] = (T2)invocationResult;
                    }
                }
            }
            return rd;
        }
        #endregion
    }
}
