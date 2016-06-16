#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：MIKE-PC 
     * 文件名：  MongoDBHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2014/8/11 16:25:19 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/8/11 16:25:19 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TLZ.MongoDB
{
    /// <summary>
    /// Remoting调用本类文件中定义的方法
    /// </summary>
    partial class MongoDBHelper
    {
        /// <summary>
        /// 显示全部，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> SelectSortR<T>(string sortByJson)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonSortByDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortByDocument);
            return collection.FindAllAs<T>().SetFields(field).SetSortOrder(sortByDocument).ToList<T>();
        }

        /// <summary>
        /// 显示全部，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortBy"></param>
        /// <param name="fields">显示出了ID之外的另外一些列的值</param>
        /// <returns></returns>
        public static List<T> SelectSortR<T>(string sortByJson, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonSortByDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortByDocument);
            return collection.FindAllAs<T>().SetFields(field).SetSortOrder(sortByDocument).ToList<T>();
        }

        /// <summary>
        /// 条件查询，不支持排序(远程调用方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(string queryJson)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).ToList<T>();
        }
        /// <summary>
        /// 条件查询，不支持排序(远程调用方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(string queryJson, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            recordCount = collection.Count(queryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).ToList<T>();
        }
        /// <summary>
        /// (远程调用方法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(string queryJson, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).ToList<T>();
        }

        public static List<T> SelectR<T>(string queryJson, string sortByJson)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).ToList<T>();
        }

        public static List<T> SelectR<T>(string queryJson, string sortByJson, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).ToList<T>();
        }

        /// <summary>
        /// 分页显示，支持条件，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }

        /// <summary>
        /// 分页显示，支持条件，返回纪录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            recordCount = collection.Count(queryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }

        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, out long recordCount, params string[] fields)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            recordCount = collection.Count(queryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }

        /// <summary>
        /// 分页显示，支持条件，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, string sortByJson)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, string sortByJson, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }

        /// <summary>
        /// 分页显示，支持条件，支持排序，返回纪录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, string sortByJson, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            recordCount = collection.Count(queryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>();
        }

        public static List<T> SelectR<T>(int pageSize, int pageIndex, string queryJson, string sortByJson, out long recordCount, params string[] fields)
        {
            recordCount = 0L;
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            BsonDocument bsonSortDocument = BsonSerializer.Deserialize(sortByJson, typeof(BsonDocument)) as BsonDocument;
            SortByDocument sortByDocument = new SortByDocument(bsonSortDocument);
            recordCount = collection.Count(queryDocument);
            return collection.FindAs<T>(queryDocument).SetFields(field).SetSortOrder(sortByDocument).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static T GetR<T>(string queryJson) where T : class
        {
            T obj = null;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields,0,fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonQueryDocument = BsonSerializer.Deserialize(queryJson, typeof(BsonDocument)) as BsonDocument;
            QueryDocument queryDocument = new QueryDocument(bsonQueryDocument);
            if (collection != null)
            {
                //obj = collection.FindOneAs<T>(queryDocument);
                obj = collection.FindAs<T>(queryDocument).SetFields(field).FirstOrDefault<T>();
            }
            return obj;
        }

    }
}
