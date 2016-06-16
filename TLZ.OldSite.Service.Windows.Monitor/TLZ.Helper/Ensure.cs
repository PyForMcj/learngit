using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TLZ.Helper
{
    /// <summary>
    /// <para>对象安全检查类</para>
    /// 
    /// <para>使用方法:</para>
    /// <para>   Ensure.NotNull(add,"add");</para>
    /// </summary>
    /// <remarks>注意期望值和实际值的位置</remarks>
    public static class Ensure
    {
        private static object ToNullValue(this object value)
        {
            return value ?? "[NULL]";
        }
        private static string ToBrackets(this string value)
        {
            return "[" + value + "]";
        }

        /// <summary>
        /// 确保不为null
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotNull<T>(T actual, string argumentName) where T : class
        {
            if (actual == null)
                throw new ArgumentNullException(argumentName);
        }
        /// <summary>
        /// 确保不为null或者空
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotNullOrEmpty(string actual, string argumentName)
        {
            if (string.IsNullOrEmpty(actual))
                throw new ArgumentNullException(actual, argumentName);
        }
        ///// <summary>
        ///// 确保不为null或者0
        ///// </summary>
        ///// <param name="actual">实际值</param>
        ///// <param name="argumentName">参数名称</param>
        //public static void NotNullOrZero(decimal? actual, string argumentName)
        //{
        //    if (actual == null && actual == 0)
        //        throw new ArgumentNullException(argumentName);
        //}
        ///// <summary>
        ///// 确保不为null或者0
        ///// </summary>
        ///// <param name="actual">实际值</param>
        ///// <param name="argumentName">参数名称</param>
        //public static void NotNullOrZero(int? actual, string argumentName)
        //{
        //    if (actual == null && actual == 0)
        //        throw new ArgumentNullException(argumentName);
        //}
        ///// <summary>
        ///// 确保不为0
        ///// </summary>
        ///// <param name="actual">实际值</param>
        ///// <param name="argumentName">参数名称</param>
        //public static void NotZero(int actual, string argumentName)
        //{
        //    if (actual == 0)
        //        throw new ArgumentNullException(argumentName);
        //}
        /// <summary>
        /// 确保是一个正数(不包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Positive(int actual, string argumentName)
        {
            if (actual <= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be positive.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个不为null正数(不包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Positive(int? actual, string argumentName)
        {
            if (!actual.HasValue || actual <= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be positive.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }

        /// <summary>
        /// 确保是一个正数(不包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Positive(long actual, string argumentName)
        {
            if (actual <= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be positive.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个不为null正数(不包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Positive(long? actual, string argumentName)
        {
            if (!actual.HasValue || actual <= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be positive.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个自然数(包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Nonnegative(long actual, string argumentName)
        {
            if (actual < 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be nonnegative.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个不为null自然数(包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Nonnegative(long? actual, string argumentName)
        {
            if (!actual.HasValue || actual < 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be nonnegative.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个自然数(包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Nonnegative(int actual, string argumentName)
        {
            if (actual < 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be nonnegative.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }
        /// <summary>
        /// 确保是一个不为null自然数(包含0)
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Nonnegative(int? actual, string argumentName)
        {
            if (!actual.HasValue || actual < 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be nonnegative.{0} actual value:{1}", argumentName.ToBrackets(), actual));
        }

        /// <summary>
        /// 确保Guid值非空
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEmptyGuid(Guid actual, string argumentName)
        {
            if (Guid.Empty == actual)
                throw new ArgumentException(string.Format("{0} should be non-empty GUID.", argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(int expected, int actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual), argumentName);
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(long expected, long actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual), argumentName);
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(bool expected, bool actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual), argumentName);
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(object expected, object actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 相等
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal<T>(T expected, T actual, string argumentName)
        {
            if (!expected.Equals(actual))
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected.ToNullValue(), actual.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(string expected, string actual, string argumentName)
        {
            if (expected != actual)
                throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="ignoreCase">或略大小写</param>
        /// <param name="argumentName">参数名称</param>
        public static void Equal(string expected, string actual, bool ignoreCase, string argumentName)
        {
            if (ignoreCase)
            {
                if (!expected.Equals(actual, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual.ToNullValue()), argumentName);
            }
            else
            {
                if (expected != actual)
                    throw new ArgumentException(string.Format("{0} should be equal {1}. {0} actual value: {2}.", argumentName.ToBrackets(), expected, actual.ToNullValue()), argumentName);
            }
        }
        /// <summary>
        /// 结果为False
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="argumentName">参数名称</param>
        public static void IsFalse(bool condition, string argumentName)
        {
            if (condition != false)
                throw new ArgumentException(string.Format("{0} should be false.", argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 结果不为False
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotFalse(bool condition, string argumentName)
        {
            if (condition == false)
                throw new ArgumentException(string.Format("{0} should not be false.", argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 结果为True
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="argumentName">参数名称</param>
        public static void IsTrue(bool condition, string argumentName)
        {
            if (condition != true)
                throw new ArgumentException(string.Format("{0} should be true.", argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 结果不为True
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotTrue(bool condition, string argumentName)
        {
            if (condition == true)
                throw new ArgumentException(string.Format("{0} should not be true.", argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="notExpected">不期望的值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual(object notExpected, object actual, string argumentName)
        {
            if (notExpected == actual)
                throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 不相等
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="notExpected">不期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual<T>(T notExpected, T actual, string argumentName)
        {
            if (notExpected == null || notExpected.Equals(actual))
                throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected.ToNullValue(), actual.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="notExpected">不期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual(double notExpected, double actual, string argumentName)
        {
            if (notExpected == actual)
                throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="notExpected">不期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual(float notExpected, float actual, string argumentName)
        {
            if (notExpected == actual)
                throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="notExpected">不期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="ignoreCase"></param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual(string notExpected, string actual, string argumentName)
        {
            if (notExpected == actual)
                throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual), argumentName);
        }
        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="notExpected">不期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="ignoreCase"></param>
        /// <param name="argumentName">参数名称</param>
        public static void NotEqual(string notExpected, string actual, bool ignoreCase, string argumentName)
        {
            if (ignoreCase)
            {
                if (notExpected.Equals(actual, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual), argumentName);
            }
            else
            {
                if (notExpected == actual)
                    throw new ArgumentException(string.Format("{0} should be not equal {1}.{0} actual value: {2}.", argumentName.ToBrackets(), notExpected, actual), argumentName);
            }
        }
        /// <summary>
        /// 是指定类型的实例
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        public static void InstanceOfType(object expected, Type actual, string argumentName)
        {
            if (expected == null || !actual.IsInstanceOfType(expected))
                throw new ArgumentException(string.Format("{0} should be instance of {1}.{0} actual type: {2}.", argumentName.ToBrackets(), expected.GetType(), actual), argumentName);
        }
        /// <summary>
        /// 不是指定类型的实例
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        public static void NotInstanceOfType(object expected, Type actual, string argumentName)
        {
            if (actual.IsInstanceOfType(expected))
                throw new ArgumentException(string.Format("{0} should not be instance of {1}.{0} actual type: {2}.", argumentName.ToBrackets(), expected.GetType(), actual), argumentName);
        }
        /// <summary>
        /// 是同一引用对象
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Same(object expected, object actual, string argumentName)
        {
            if (!Object.ReferenceEquals(expected, actual))
                throw new ArgumentException(string.Format("{0} should be reference of {1}.", argumentName.ToBrackets(), expected), argumentName);
        }
        /// <summary>
        /// 不是同一引用对象
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void NotSame(object expected, object actual, string argumentName)
        {
            if (expected == null || Object.ReferenceEquals(expected, actual))
                throw new ArgumentException(string.Format("{0} should not be reference of {1}.", argumentName.ToBrackets(), expected), argumentName);
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void GreaterThan(int expected, int actual, string argumentName)
        {
            if (actual <= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void GreaterThan(long expected, long actual, string argumentName)
        {
            if (actual <= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void GreaterThan(double expected, double actual, string argumentName)
        {
            if (actual <= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void GreaterThan(DateTime expected, DateTime actual, string argumentName)
        {
            if (actual <= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 不为空 并且 大于
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void GreaterThan<T>(T expected, T actual, string argumentName) where T : IComparable
        {
            if (actual == null || actual.CompareTo(expected) <= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected.ToNullValue(), actual.ToNullValue()));
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LessThan(int expected, int actual, string argumentName)
        {
            if (actual >= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be less than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LessThan(long expected, long actual, string argumentName)
        {
            if (actual >= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be less than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LessThan(double expected, double actual, string argumentName)
        {
            if (actual >= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be less than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LessThan(DateTime expected, DateTime actual, string argumentName)
        {
            if (actual >= expected)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be less than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected, actual));
        }
        /// <summary>
        /// 不为空 并且 小于
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="expected">期望值</param>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LessThan<T>(T expected, T actual, string argumentName) where T : IComparable
        {
            if (actual == null || actual.CompareTo(expected) >= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} should be greater than {1}.{0} input value:{2}.", argumentName.ToBrackets(), expected.ToNullValue(), actual.ToNullValue()));
        }
        /// <summary>
        /// 区间（包含左侧和右侧）
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Between(int actual, int start, int end, string argumentName)
        {
            if (actual < start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Between(long actual, long start, long end, string argumentName)
        {
            if (actual < start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Between(double actual, double start, double end, string argumentName)
        {
            if (actual < start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Between(DateTime actual, DateTime start, DateTime end, string argumentName)
        {
            if (actual < start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start.ToString("yyyy-MM-dd HH:mm:ss.ffff"), end.ToString("yyyy-MM-dd HH:mm:ss.ffff"), actual));
        }
        /// <summary>
        /// 区间（包含左侧包含右侧）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Between<T>(T actual, T start, T end, string argumentName) where T : IComparable
        {
            if (actual.CompareTo(start) < 0 || actual.CompareTo(end) > 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧不包含右侧）
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenL(int actual, int start, int end, string argumentName)
        {
            if (actual < start || actual >= end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}).{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧不包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenL(long actual, long start, long end, string argumentName)
        {
            if (actual < start || actual >= end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}).{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧不包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenL(double actual, double start, double end, string argumentName)
        {
            if (actual < start || actual >= end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}).{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（包含左侧不包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenL(DateTime actual, DateTime start, DateTime end, string argumentName)
        {
            if (actual < start || actual >= end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}).{0} input value:{3}.", argumentName.ToBrackets(), start.ToString("yyyy-MM-dd HH:mm:ss.ffff"), end.ToString("yyyy-MM-dd HH:mm:ss.ffff"), actual));
        }
        /// <summary>
        /// 不为空 并且 区间（包含左侧不包含右侧）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenL<T>(T actual, T start, T end, string argumentName) where T : IComparable
        {
            if (actual.CompareTo(start) < 0 || actual.CompareTo(end) >= 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is [{1},{2}).{0} input value:{3}.", argumentName.ToBrackets(), start.ToNullValue(), end.ToNullValue(), actual.ToNullValue()));
        }
        /// <summary>
        /// 区间（不包含左侧包含右侧）
        /// </summary>
        /// <param name="expected">期望值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenR(int actual, int start, int end, string argumentName)
        {
            if (actual <= start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is ({1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（不包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenR(long actual, long start, long end, string argumentName)
        {
            if (actual <= start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is ({1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（不包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenR(double actual, double start, double end, string argumentName)
        {
            if (actual <= start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is ({1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start, end, actual));
        }
        /// <summary>
        /// 区间（不包含左侧包含右侧）
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenR(DateTime actual, DateTime start, DateTime end, string argumentName)
        {
            if (actual <= start || actual > end)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is ({1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start.ToString("yyyy-MM-dd HH:mm:ss.ffff"), end.ToString("yyyy-MM-dd HH:mm:ss.ffff"), actual));
        }
        /// <summary>
        /// 区间（不包含左侧包含右侧）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void BetweenR<T>(T actual, T start, T end, string argumentName) where T : IComparable
        {
            if (actual.CompareTo(start) <= 0 || actual.CompareTo(end) > 0)
                throw new ArgumentOutOfRangeException(argumentName, string.Format("{0} range is ({1},{2}].{0} input value:{3}.", argumentName.ToBrackets(), start.ToNullValue(), end.ToNullValue(), actual.ToNullValue()));
        }
        /// <summary>
        /// 不为null 并且 包含指定值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="item">项 or 值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Contains<T>(IList<T> actual, T item, string argumentName)
        {
            if (actual == null || !actual.Contains(item))
                throw new ArgumentException("{0} should be contains {1}.".Formater(argumentName.ToBrackets(), item.ToNullValue()), argumentName);
        }
        /// <summary>
        /// 不为null 并且 包含指定值
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="item">项 or 值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Contains(IList actual, object item, string argumentName)
        {
            if (actual == null || !actual.Contains(item))
                throw new ArgumentException("{0} should be contains {1}.".Formater(argumentName.ToBrackets(), item), argumentName);
        }
        /// <summary>
        /// 不为null 并且 包含指定值
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="item">项 or 值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Contains(string actual, string item, string argumentName)
        {
            if (actual == null || !actual.Contains(item))
                throw new ArgumentException("{0} should be contains {1}.".Formater(argumentName.ToBrackets(), item), argumentName);
        }
        /// <summary>
        /// 不为null 并且 包含指定值
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="item">项 or 值</param>
        /// <param name="argumentName">参数名称</param>
        public static void Contains(string actual, char item, string argumentName)
        {
            if (actual == null || !actual.Contains(item))
                throw new ArgumentException("{0} should be contains {1}.".Formater(argumentName.ToBrackets(), item), argumentName);
        }
        /// <summary>
        /// 不为null 并且 以指定字符串起始
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="start">开始值</param>
        /// <param name="argumentName">参数名称</param>
        public static void StartsWith(string actual, string start, string argumentName)
        {
            if (start != null && (actual == null || !actual.StartsWith(start)))
                throw new ArgumentException("{0} should be start with {1}.".Formater(argumentName.ToBrackets(), start), argumentName);
        }
        /// <summary>
        /// 不为null 并且 以指定字符串结尾
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="end">结束值</param>
        /// <param name="argumentName">参数名称</param>
        public static void EndsWith(string actual, string end, string argumentName)
        {
            if (end != null && (actual == null || !actual.EndsWith(end)))
                throw new ArgumentException("{0} should be end with {1}.".Formater(argumentName.ToBrackets(), end), argumentName);
        }
        /// <summary>
        /// 不为空 并且 至少有一个元素
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void HasMany(ICollection actual, string argumentName)
        {
            if (actual == null || (actual as IList).Count == 0)
                throw new ArgumentException("{0} lenght/count should be greater than 1.".Formater(argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 至少有一个元素
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void HasMany<T>(ICollection<T> actual, string argumentName)
        {
            if (actual == null || (actual as IList).Count == 0)
                throw new ArgumentException("{0} lenght/count should be greater than 1.".Formater(argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 至少有一个元素
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void HasMany<T>(IEnumerable<T> actual, string argumentName)
        {
            if (actual == null || (actual as IList).Count == 0)
                throw new ArgumentException("{0} lenght/count should be greater than 1.".Formater(argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 仅有一个元素
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void HasOne(ICollection actual, string argumentName)
        {
            if (actual == null || (actual as IList).Count != 1)
                throw new ArgumentException("{0} lenght/count should be equal 1.".Formater(argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 仅有一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="actual">实际值</param>
        /// <param name="argumentName">参数名称</param>
        public static void HasOne<T>(ICollection<T> actual, string argumentName)
        {
            if (actual == null || (actual as IList).Count != 1)
                throw new ArgumentException("{0} lenght/count should be equal 1.".Formater(argumentName.ToBrackets()), argumentName);
        }
        /// <summary>
        /// 不为空 并且 长度固定
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="len">长度</param>
        /// <param name="argumentName">参数名称</param>
        public static void Length(string actual, int len, string argumentName)
        {
            if (actual == null || actual.Length != len)
                throw new ArgumentException("{0} lenght should be equal {1}.".Formater(argumentName.ToBrackets(), len), argumentName);
        }
        /// <summary>
        /// 不为空 并且 长度长度超过指定值
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="min">最小值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LengthMin(string actual, int min, string argumentName)
        {
            if (actual == null || actual.Length < min)
                throw new ArgumentException("{0} lenght min value is {1}.".Formater(argumentName.ToBrackets(), min), argumentName);
        }
        /// <summary>
        /// 不为空 并且 长度不超过指定值
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="max">最大值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LengthMax(string actual, int max, string argumentName)
        {
            if (actual == null || actual.Length < max)
                throw new ArgumentException("{0} lenght max value is {1}.".Formater(argumentName.ToBrackets(), max), argumentName);
        }
        /// <summary>
        /// 不为空 并且 长度在一定范围
        /// </summary>
        /// <param name="actual">实际值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="argumentName">参数名称</param>
        public static void LengthRange(string actual, int min, int max, string argumentName)
        {
            if (actual == null || actual.Length < min || actual.Length > max)
                throw new ArgumentOutOfRangeException(argumentName, "{0} lenght range is [{1},{2}].".Formater(argumentName.ToBrackets(), min, max));
        }

    }
}
