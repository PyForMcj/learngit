using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TLZ.MongoDB
{
    partial class MongoDBHelper
    {
        #region Convert\ChangeDateTime

        private static readonly string DECIMAL_TYPE_FULLNAME = typeof(Decimal).FullName;
        private static readonly string DATETIME_TYPE_FULLNAME = typeof(DateTime).FullName;
        /// <summary>
        /// 将任意类型的对象转成BsonDocument类型的对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BsonDocument ConvertBsonDocument(object obj)
        {
            if (obj == null)
                return null;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanRead)
                    continue;
                object value = propertyInfo.GetValue(obj, null);
                if (value != null && value != DBNull.Value)
                {
                    if (value.GetType().FullName == DECIMAL_TYPE_FULLNAME)
                    {
                        dict[propertyInfo.Name] = Convert.ToDouble(value);
                    }
                    else if (value.GetType().FullName == DATETIME_TYPE_FULLNAME)
                    {// [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
                        dict[propertyInfo.Name] = BsonUtils.ToLocalTime(Convert.ToDateTime(value));
                    }
                    //else if (value.GetType().BaseType.FullName == typeof(Enum).FullName)
                    //{
                    //    dict[propertyInfo.Name] = (int)value;
                    //}
                    else
                    {
                        dict[propertyInfo.Name] = value;
                    }
                }
            }
            BsonDocument document = new BsonDocument(dict);
            return document;
        }
        /// <summary>
        /// 修改集合中每个对象的所有是时间类型的属性值为Unix时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public static void ChangeDateTimeDecimal<T>(IEnumerable<T> entities)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.FullName == DATETIME_TYPE_FULLNAME)
                {
                    foreach (T entity in entities)
                    {
                        ChangeLocalDateTime<T>(entity, propertyInfo);
                    }
                }
            }
        }
        /// <summary>
        /// 修改对象中的所有是时间类型的属性值为Unix时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static void ChangeDateTimeDecimal<T>(T entity)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.FullName == DATETIME_TYPE_FULLNAME)
                {
                    ChangeLocalDateTime<T>(entity, propertyInfo);
                }
            }
        }
        /// <summary>
        /// 修改对象中指定时间类型的属性值为Unix时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyInfo"></param>
        public static void ChangeLocalDateTime<T>(T entity, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.FullName == DATETIME_TYPE_FULLNAME)
            {
                object value = propertyInfo.GetValue(entity, null);
                propertyInfo.SetValue(entity, ChangeLocalDateTime(Convert.ToDateTime(value)), null);
            }
        }
        /// <summary>
        /// 将当前时间转换为Unix时间
        /// </summary>
        /// <param name="currentDateTime"></param>
        /// <returns></returns>
        public static DateTime ChangeLocalDateTime(DateTime currentDateTime)
        {
            return BsonUtils.ToLocalTime(currentDateTime);
        }
        /// <summary>
        /// 将更新的对象,变成UpdateDocument类型之后,可以直接调用带有IMongoUpdate类型的Update方法了.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static UpdateDocument ConvertUpdateDocument<T>(T entity)
        {
            if (entity == null)
                return null;
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value = propertyInfo.GetValue(entity, null);
                if (value != null)
                {
                    if (value.GetType().FullName == DECIMAL_TYPE_FULLNAME)
                    {
                        dict[propertyInfo.Name] = Convert.ToDouble(value);
                    }
                    else if (value.GetType().FullName == DATETIME_TYPE_FULLNAME)
                    {// [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
                        dict[propertyInfo.Name] = BsonUtils.ToLocalTime(Convert.ToDateTime(value));
                    }
                    //else if (value.GetType().BaseType.FullName == typeof(Enum).FullName)
                    //{
                    //    dict[propertyInfo.Name] = (int)value;
                    //}
                    else
                    {
                        dict[propertyInfo.Name] = value;
                    }
                }
            }
            //UpdateDocument updateDocument = new UpdateDocument(dict);
            QueryDocument queryDocument = new QueryDocument(dict);//wangyunpeng 2015-7-14 修改Mongodb支持批量更新
            UpdateDocument updateDocument = new UpdateDocument { { "$set", queryDocument } };
            return updateDocument;
        }
        /// <summary>
        /// 将更新的对象,变成UpdateDocument类型之后,可以直接调用带有IMongoUpdate类型的Update方法了.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static UpdateDocument ConvertUpdateDocument(object entity)
        {
            if (entity == null)
                return null;
            PropertyInfo[] propertyInfos = entity.GetType().GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value = propertyInfo.GetValue(entity, null);
                if (value != null)
                {
                    if (value.GetType().FullName == DECIMAL_TYPE_FULLNAME)
                    {
                        dict[propertyInfo.Name] = Convert.ToDouble(value);
                    }
                    else if (value.GetType().FullName == DATETIME_TYPE_FULLNAME)
                    {// [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
                        dict[propertyInfo.Name] = BsonUtils.ToLocalTime(Convert.ToDateTime(value));
                    }
                    //else if (value.GetType().BaseType.FullName == typeof(Enum).FullName)
                    //{
                    //    dict[propertyInfo.Name] = (int)value;
                    //}
                    else
                    {
                        dict[propertyInfo.Name] = value;
                    }
                }
            }
            //UpdateDocument updateDocument = new UpdateDocument(dict);
            QueryDocument queryDocument = new QueryDocument(dict);//wangyunpeng 2015-7-14 修改Mongodb支持批量更新
            UpdateDocument updateDocument = new UpdateDocument { { "$set", queryDocument } };
            return updateDocument;
        }
        /// <summary>
        /// 将排序条件转成SortByDocument类型的对象,可以直接调用带有IMongoSortBy类型的Select方法了.
        /// </summary>
        /// <param name="sortDict"></param>
        /// <returns></returns>
        public static SortByDocument ConvertSortByDocument(Dictionary<string, int> sortDict)
        {
            if (sortDict == null)
                return null;
            return new SortByDocument(sortDict);
        }
        #endregion
    }
}
