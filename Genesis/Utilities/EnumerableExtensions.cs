using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genesis
{
    public static class EnumerableExtensions
    {
        #region Inner Classes

        private sealed class SelectionComparer<TSource, TKey> : IEqualityComparer<TSource>
        {
            private Func<TSource, TKey> _keySelector;

            public SelectionComparer(Func<TSource, TKey> keySelector)
            {
                _keySelector = keySelector;
            }

            public bool Equals(TSource x, TSource y)
            {
                var xKey = _keySelector(x);
                var yKey = _keySelector(y);

                if (xKey == null)
                {
                    return yKey == null;
                }

                return xKey.Equals(yKey);
            }

            public int GetHashCode(TSource obj)
            {
                var key = _keySelector(obj);
                return key != null ? key.GetHashCode() : 0;
            }
        }

        #endregion Inner Classes

        public static IEnumerable<TSource> Intersect<TSource, TKey>(
            this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
        {
            return first.Intersect(second, new SelectionComparer<TSource, TKey>(keySelector));
        }

        [DebuggerStepThrough]
        public static int CompareCount<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> target)
        {
            if (target == null)
            {
                return 1;
            }
            else if (source == target)
            {
                return 0;
            }

            using (var sourceEnumerator = source.GetEnumerator())
            using (var targetEnumerator = target.GetEnumerator())
            {
                bool existSource;
                bool existTarget;

                while (true)
                {
                    existSource = sourceEnumerator.MoveNext();
                    existTarget = targetEnumerator.MoveNext();

                    if (existSource == false)
                    {
                        return existTarget ? -1 : 0;
                    }
                    else if (existTarget == false)
                    {
                        return 1;
                    }
                }
            }
        }

        [DebuggerStepThrough]
        public static int CompareCount<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (count < 0)
            {
                return 1;
            }

            var index = 0;
            using (var sourceEnumerator = source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    index++;
                    if (count < index)
                    {
                        return 1;
                    }
                }
            }

            return index == count ? 0 : -1;
        }

        [DebuggerStepThrough]
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.Distinct(new SelectionComparer<TSource, TKey>(keySelector));
        }

        [DebuggerStepThrough]
        public static IEnumerable<TSource> Except<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
        {
            return first.Except(second, new SelectionComparer<TSource, TKey>(keySelector));
        }

        [DebuggerStepThrough]
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending)
        {
            return ascending
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }

        [DebuggerStepThrough]
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool ascending)
        {
            return ascending
                ? source.OrderBy(keySelector, comparer)
                : source.OrderByDescending(keySelector, comparer);
        }

        [DebuggerStepThrough]
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending)
        {
            return ascending
                ? source.ThenBy(keySelector)
                : source.ThenByDescending(keySelector);
        }

        [DebuggerStepThrough]
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool ascending)
        {
            return ascending
                ? source.ThenBy(keySelector, comparer)
                : source.ThenByDescending(keySelector, comparer);
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> toAction)
        {
            foreach (T each in enumerable)
            {
                toAction(each);
            }
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this T[] enumerable, Action<T> toAction)
        {
            foreach (T each in enumerable)
            {
                toAction(each);
            }
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> toAction)
        {
            int i = 0;
            foreach (T each in enumerable)
            {
                toAction(each, i++);
            }
        }

        [DebuggerStepThrough]
        public static void ForEach<T>(this T[] enumerable, Action<T, int> toAction)
        {
            int i = 0;
            foreach (T each in enumerable)
            {
                toAction(each, i++);
            }
        }

        [DebuggerStepThrough]
        public static int IndexOf<T>(this IEnumerable<T> enumerable, Predicate<T> func)
        {
            int i = 0;
            foreach (T each in enumerable)
            {
                if (func(each))
                    return i;
                i++;
            }
            return -1;
        }

        [DebuggerStepThrough]
        public static int IndexOf<T>(this T[] enumerable, Predicate<T> func)
        {
            int i = 0;
            foreach (T each in enumerable)
            {
                if (func(each))
                    return i;
                i++;
            }
            return -1;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> With<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var value in enumerable)
            {
                action(value);
                yield return value;
            }
        }

        [DebuggerStepThrough]
        public static bool IsCount<T>(this IEnumerable<T> source, int count)
        {
            var index = 0;
            using (var sourceEnumerator = source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    index++;
                    if (count < index)
                    {
                        break;
                    }
                }
            }

            return index == count;
        }

        public static bool IsGreaterEqual<T>(this IEnumerable<T> source, int count)
        {
            CheckGreaterEqual(count);

            var index = 0;
            using (var sourceEnumerator = source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    index++;
                    if (count == index)
                        return true;
                }
            }

            return count == 0;
        }

        public static bool IsLesserEqual<T>(this IEnumerable<T> source, int count)
        {
            CheckGreaterEqual(count);

            var index = 0;
            using (var sourceEnumerator = source.GetEnumerator())
            {
                while (sourceEnumerator.MoveNext())
                {
                    index++;
                    if (count < index)
                        return false;
                }
            }

            return true;
        }

        [DebuggerStepThrough]
        public static bool IsOverlapped<T>(this IEnumerable<T> source)
        {
            var set = new HashSet<T>();
            foreach (T each in source)
            {
                if (set.Contains(each))
                {
                    return true;
                }
                else
                {
                    set.Add(each);
                }
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsOverlapped<T>(this IEnumerable<T> source, out T element)
        {
            var set = new HashSet<T>();
            foreach (T each in source)
            {
                if (set.Contains(each))
                {
                    element = each;
                    return true;
                }
                else
                {
                    set.Add(each);
                }
            }

            element = default(T);
            return false;
        }

        [DebuggerStepThrough]
        public static bool IsSingle<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Any() && !enumerable.Skip(1).Any();
        }

        [DebuggerStepThrough]
        public static bool IsSingle<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (T each in enumerable)
            {
                if (predicate(each))
                {
                    count++;
                    if (count == 2)
                    {
                        break;
                    }
                }
            }

            return count == 1;
        }

        [DebuggerStepThrough]
        public static bool IsSingleKind<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext() == false)
                {
                    return false;
                }

                T first = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (first == null)
                    {
                        if (current != null)
                        {
                            return false;
                        }
                    }
                    else if (first.Equals(current) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [DebuggerStepThrough]
        public static T MaxOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue = default(T))
        {
            return enumerable.Any() ? enumerable.Max() : defaultValue;
        }

        [DebuggerStepThrough]
        public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> enumerable,
                                                             Func<TSource, TResult> selector,
                                                             TResult defaultValue = default(TResult))
        {
            return enumerable.Any() ? enumerable.Max(selector) : defaultValue;
        }

        [DebuggerStepThrough]
        public static T MinOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue = default(T))
        {
            return enumerable.Any() ? enumerable.Min() : defaultValue;
        }

        [DebuggerStepThrough]
        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> enumerable,
                                                             Func<TSource, TResult> selector,
                                                             TResult defaultValue = default(TResult))
        {
            return enumerable.Any() ? enumerable.Min(selector) : defaultValue;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Recursive<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> get)
        {
            foreach (T each in enumerable)
            {
                yield return each;

                foreach (T eachRecurse in Recursive(get(each), get))
                {
                    yield return eachRecurse;
                }
            }
        }

        public static IEnumerable<Tuple<int, T>> Recursive<T>(this IEnumerable<T> enumerable
            , Func<T, bool> predicate, Func<T, int, IEnumerable<T>> get, bool isAllResult = false, int outline = 0)
        {
            foreach (T each in enumerable)
            {
                if (predicate(each))
                {
                    yield return Tuple.Create(outline, each);

                    if (isAllResult)
                    {
                        predicate = t => true;
                    }
                    else
                    {
                        continue;
                    }
                }

                int nextOutlineNo = outline + 1;
                foreach (var eachRecurse in Recursive(get(each, nextOutlineNo), predicate, get, isAllResult, nextOutlineNo))
                {
                    yield return eachRecurse;
                }
            }
        }

        public static IEnumerable<TSource> TakeDoWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                yield return item;
                if (predicate(item) == false)
                {
                    break;
                }
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(enumerable, comparer);
        }

        public static HashSet<T2> ToHashSet<T, T2>(this IEnumerable<T> enumerable, Func<T, T2> selectFunction)
        {
            return enumerable.Select(selectFunction).ToHashSet();
        }

        public static IEnumerable<IEnumerable<TSource>> Sprit<TSource>(this IEnumerable<TSource> source, int spritSize)
        {
            var enumerator = source.GetEnumerator();
            var list = new List<TSource>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
                if (list.Count == spritSize)
                {
                    yield return list;
                    list = new List<TSource>();
                }
            }
            if (list.Any())
            {
                yield return list;
            }
        }

        public static Dictionary<TKey, List<TValue>> ToValueListDictionary<TKey, TValue, TSource>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            return source.Aggregate(
                new Dictionary<TKey, List<TValue>>(),
                (map, element) =>
                {
                    var key = keySelector(element);
                    var value = valueSelector(element);

                    List<TValue> innerList;
                    if (map.TryGetValue(key, out innerList))
                    {
                        innerList.Add(value);
                    }
                    else
                    {
                        map[key] = new List<TValue> { value };
                    }
                    return map;
                });
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue, TSource>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            return source.Aggregate(
                new ConcurrentDictionary<TKey, TValue>(),
                (map, element) =>
                {
                    var key = keySelector(element);
                    var value = valueSelector(element);
                    map.TryAdd(key, value);
                    return map;
                });
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue, TSource>
            (this ParallelQuery<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var map = new ConcurrentDictionary<TKey, TValue>();
            source.ForAll(e => map.TryAdd(keySelector(e), valueSelector(e)));
            return map;
        }

        private static void CheckGreaterEqual(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "[count] must be equal or greater than 0.");
        }
    }
}