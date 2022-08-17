using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IENumeratorExtensions 
{
    public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    public static IEnumerable<K> Select<K, T>(this IEnumerator<T> e,
                                         Func<T, K> selector)
    {
        while (e.MoveNext())
        {
            yield return selector(e.Current);
        }
    }
}
