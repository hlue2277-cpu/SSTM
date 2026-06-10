using System;
using System.Threading;

namespace Genesis.Utilities
{
    /// <summary>
    /// 指定时间内重试
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// 默认重试时间
        /// </summary>
        private static readonly TimeSpan _defaultRetryInterval = TimeSpan.FromMilliseconds(200.0);

        /// <summary>
        /// 如果指定了异常内容<paramref name="retryTimeoutErrorMessage" />，则<c>Retry.For</c>超时时触发超时异常。
        /// </summary>
        /// <param name="getMethod">重试内容(如果为真，则返回，否则重试)</param>
        /// <param name="retryFor">超时时间范围</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryTimeoutErrorMessage">超时后显示的异常内容</param>
        public static bool For(Func<bool> getMethod, TimeSpan retryFor, TimeSpan? retryInterval = null, string retryTimeoutErrorMessage = null)
        {
            if (ForCore(getMethod, new Predicate<bool>(g => g), retryFor, retryInterval))
                return true;

            if (!string.IsNullOrEmpty(retryTimeoutErrorMessage))
                throw CreateTimeoutError(retryTimeoutErrorMessage);

            return false;
        }

        /// <summary>
        /// 如果指定了异常内容<paramref name="retryTimeoutErrorMessage" />，则<c>Retry.For</c>超时时触发超时异常。
        /// </summary>
        /// <param name="getMethod">重试内容(如果为符合指定条件，则返回，否则重试)</param>
        /// <param name="shouldRetry">指定条件</param>
        /// <param name="retryFor">超时时间范围</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryTimeoutErrorMessage">超时后显示的异常内容</param>
        public static T For<T>(
            Func<T> getMethod, Predicate<T> shouldRetry, TimeSpan retryFor, TimeSpan? retryInterval = null, string retryTimeoutErrorMessage = null)
        {
            var output = default(T);
            ForCore(() => output = getMethod(), shouldRetry, retryFor, retryInterval);

            if (shouldRetry(output) && !string.IsNullOrEmpty(retryTimeoutErrorMessage))
                throw CreateTimeoutError(retryTimeoutErrorMessage);
            else
                return output;
        }

        /// <summary>
        /// 重试处理
        /// </summary>
        private static T ForCore<T>(
            Func<T> getMethod, Predicate<T> shouldRetry, TimeSpan retryFor, TimeSpan? retryInterval = null)
        {
            var now = DateTime.Now;
            while (DateTime.Now.Subtract(now).TotalMilliseconds < retryFor.TotalMilliseconds)
            {
                var element = getMethod();

                if (!typeof(T).IsValueType && (object)element != null && shouldRetry(element) ||
                    typeof(T) == typeof(bool) && shouldRetry(element) ||
                    typeof(T) != typeof(bool) && !IsReferenceTypeAndIsNull<T>(element) && shouldRetry(element))
                    return element;

                Thread.Sleep(retryInterval ?? _defaultRetryInterval);
            }
            return getMethod();
        }

        private static bool IsReferenceTypeAndIsNull<T>(T element)
        {
            if (!typeof(T).IsValueType)
                return object.ReferenceEquals((object)element, (object)null);
            else
                return false;
        }

        private static TimeoutException CreateTimeoutError(string retryTimeoutErrorMessage)
        {
            throw new TimeoutException(message:
                string.IsNullOrEmpty(retryTimeoutErrorMessage) ? "Timeout Error" : retryTimeoutErrorMessage);
        }
    }
}