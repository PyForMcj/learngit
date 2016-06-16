#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称： 
     * 文件名：  MongoDBHelper
     * 版本号：  V1.0.0.0 
     * 创建人：  王云鹏 
     * 创建时间：2014-07-17
     * 描述    : 封装MongoDB操作.
     * =====================================================================
     * 修改时间：2014-07-17 
     * 修改人  ：王云鹏
     * 版本号  ：V1.0.0.0 
     * 描述    ：封装MongoDB操作.
*/
#endregion
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TLZ.OldSite.DB.MongoDB.Model;
using TLZ.Helper;

namespace TLZ.MongoDB
{
    /// <summary>
    /// 封装MongoDB的操作。
    /// http://www.cnblogs.com/stone_w/archive/2012/12/12/2814056.html
    /// Query.All("name", "a", "b");                                        //通过多个元素来匹配数组
    /// Query.And(Query.EQ("name", "a"),Query.EQ("title", "t"));            //同时满足多个条件 
    /// Query.EQ("name", "a");                                              //等于
    /// Query.Exists("type", true);                                         //判断键值是否存在
    /// Query.GT("value", 2);                                               //大于>
    /// Query.GTE("value", 3);                                              //大于等于>=
    /// Query.In("name", "a", "b");                                         //包括指定的所有值,可以指定不同类型的条件和值 
    /// Query.LT("value", 9);                                               //小于<
    /// Query.LTE("value", 8);                                              //小于等于<=
    /// Query.Mod("value", 3, 1);                                           //将查询值除以第一个给定值,若余数等于第二个给定值则返回该结果
    /// Query.NE("name", "c");                                              //不等于
    /// Query.Nor(Array);                                                   //不包括数组中的值
    /// Query.Not("name");                                                  //元素条件语句
    /// Query.NotIn("name", "a", 2);                                        //返回与数组中所有条件都不匹配的文档
    /// Query.Or(Query.EQ("name", "a"), Query.EQ("title", "t"));            //满足其中一个条件
    /// Query.Size("name", 2);                                              //给定键的长度
    /// Query.Type("_id", BsonType.ObjectId );                              //给定键的类型
    /// Query.Where(BsonJavaScript);                                        //执行JavaScript
    /// Query.Matches("Title",str);                                         //模糊查询 相当于sql中like -- str可包含正则表达式
    /// ------------------------------------------------------
    /// 多个查询条件组合在一起
    /// List<IMongoQuery> queryList = new List<IMongoQuery>();
    /// queryList.Add(Query<Person>.GT(e => e.Id, 100));
    /// queryList.Add(Query<Person>.LT(e => e.Id, 200));
    /// IMongoQuery query = Query.And(queryList.ToArray()); //合并多个查询条件
    /// </summary>
    public partial class MongoDBHelper
    {
        #region 变量
        /// <summary>
        /// MongoDB连接字符串
        /// 例子：&lt;add key="MongoDBConstr" value="mongodb://sa:1234@192.168.7.210:27017/admin" /&gt;
        /// </summary>
        private const string CONFIG_MONGODB_CONNECTION_STRING = "MongoDBConstr";
        /// <summary>
        /// MongoDB数据库名称
        /// 例子：&lt;add key="MongoDBName" value="dressve_website" /&gt;
        /// </summary>
        private const string CONFIG_MONGODB_NAME = "MongoDBName";
        /// <summary>
        /// 连接字符串(读写库)
        /// </summary>
        private static string _ConnectionString = "mongodb://192.168.7.210:27017";
        /// <summary>
        /// 数据库名称
        /// </summary>
        private static string _DbName = string.Empty;

        /// <summary>
        /// 存储切换数据库
        /// </summary>
        private static List<MongoConnModel> _MongoConnModelList = null;

        static object locker = new object();
        #endregion

        #region 事件
        private static event Action _ConfigChangedEvent;

        public static event Action ConfigChangedEvent
        {
            add
            {
                Action handler2;
                Action handler3 = _ConfigChangedEvent;
                do
                {
                    handler2 = handler3;
                    Action handler4 = (Action)Delegate.Combine(handler2, value);
                    handler3 = Interlocked.CompareExchange<Action>(ref _ConfigChangedEvent, handler4, handler2);
                }
                while (handler3 != handler2);
            }
            remove
            {
                Action handler2;
                Action handler3 = _ConfigChangedEvent;
                do
                {
                    handler2 = handler3;
                    Action handler4 = (Action)Delegate.Remove(handler2, value);
                    handler3 = Interlocked.CompareExchange<Action>(ref _ConfigChangedEvent, handler4, handler2);
                }
                while (handler3 != handler2);
            }
        }
        #endregion

        #region 构造函数
        static MongoDBHelper()
        {
            InitConfig();

            if (!System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                string currentConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                string currentPath = Path.GetDirectoryName(currentConfig);
                string currentFile = Path.GetFileName(currentConfig);
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(currentPath, currentFile);
                fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                fileSystemWatcher.EnableRaisingEvents = true;
            }

            string dataName = string.Empty;
            string baseIPName = string.Empty;
            string mongoDBConstr = ConfigurationManager.AppSettings["MongoDBConstr"].Trim('.');
            string mongoDBName = ConfigurationManager.AppSettings["MongoDBName"].Trim('.');
            var dataNameModel = MongoDBHelper.Get<DataBaseServerConfigures>(Query.EQ("DataBaseName", BsonValue.Create("103.235.223.120")));//BsonValue.Create("103.235.223.120")修改为prohelper.GetIPAddress()
            if (dataNameModel != null)
            {
                baseIPName = "mongodb://tidebuy:tidebuy@" + dataNameModel.DataBaseName + ":27017";
                baseIPName = baseIPName.Trim(',');
                dataName = "project" + dataNameModel.DataBaseName.Replace('.', '-');
                dataName = dataName.Trim(',');
            }
            else
            {
                throw new Exception("中心配置库DataBaseServerConfigures表不存在对应的服务器数据！");
            }
            string mongoDBConstrAll =
            "[{\"MongoConnType\":1,\"MongoConnStr\":\"" + mongoDBConstr + "\",\"MongoDatabase\":\"" + mongoDBName + "\"},{\"MongoConnType\":2,\"MongoConnStr\":\"" + baseIPName + "\",\"MongoDatabase\":\"" + dataName + "\"}]";

            _MongoConnModelList = JsonHelper.ConvertStrToJson<List<MongoConnModel>>(mongoDBConstrAll);

        }
        #endregion

        #region InitConfig & GetAppSettingValue
        /// <summary>
        /// 读取appSettings中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetAppSettingValue(string key)
        {
            string value = null;
            foreach (string item in ConfigurationManager.AppSettings)
            {
                if (item.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = ConfigurationManager.AppSettings[key];
                    break;
                }
            }
            return value;
        }
        /// <summary>
        /// 初始化配置文件
        /// </summary>
        private static void InitConfig()
        {
            _ConnectionString = GetAppSettingValue(CONFIG_MONGODB_CONNECTION_STRING);
            _DbName = GetAppSettingValue(CONFIG_MONGODB_NAME);
        }
        #endregion

        #region FileSystemWatcher_Changed
        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            do
            {
                try
                {
                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                    break;
                }
                catch (System.Configuration.ConfigurationErrorsException)
                {
                    Thread.Sleep(10);
                }
            }
            while (true);
            InitConfig();
            if (_ConfigChangedEvent != null)
            {
                _ConfigChangedEvent();
            }
        }
        #endregion

        #region config string
        /// <summary>
        /// 连接字符串(仅在测试时使用，部署时直接修改config文件就可以了)
        /// </summary>
        public static string ConnectionString
        {
            get { return MongoDBHelper._ConnectionString; }
            set { MongoDBHelper._ConnectionString = value; }
        }
        /// <summary>
        /// 数据库名称（仅在测试时使用，部署时直接修改config文件就可以了）
        /// </summary>
        public static string DbName
        {
            get { return MongoDBHelper._DbName; }
            set { MongoDBHelper._DbName = value; }
        }

        public static List<MongoConnModel> MongoConnModelList
        {
            get { return MongoDBHelper._MongoConnModelList; }
            set { MongoDBHelper._MongoConnModelList = value; }
        }
        #endregion



        #region Server
        /// <summary>
        /// 初始化MongoServer，并打开连接，(读写库)
        /// </summary>
        private static MongoServer GetServer()
        {
            lock (locker)
            {
                MongoServer server = null;
                try
                {
                    //mongodb://sa:1234@192.168.7.222:18181,192.168.7.85:18181,192.168.7.82:18181/admin?connect=replicaset&replicaSet=shoespie&readPreference=secondaryPreferred
                    //http://www.cnblogs.com/lykbk/p/uioiuouioiuo789879789.html 连接字符串mongodb://server1,server2:27017,server2:27018/?connect=replicaset&replicaSet=test&readPreference=secondaryPreferred
                    //https://docs.mongodb.org/manual/reference/connection-string/
                    //http://api.mongodb.org/java/current/com/mongodb/MongoClient.html
                    server = new MongoClient(ConnectionString).GetServer();

                    //http://blog.csdn.net/jivenbest/article/details/8160736
                    //MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder();
                    //mongoUrlBuilder.ReadPreference = new ReadPreference() { ReadPreferenceMode = ReadPreferenceMode.SecondaryPreferred };
                    //mongoUrlBuilder.ConnectionMode = ConnectionMode.ReplicaSet;
                    //mongoUrlBuilder.DatabaseName = "admin";
                    //mongoUrlBuilder.Username = "sa";
                    //mongoUrlBuilder.Password = "1234";
                    //mongoUrlBuilder.ReplicaSetName = "shoespie";
                    //mongoUrlBuilder.SafeMode = SafeMode.True;
                    //mongoUrlBuilder.FSync = true;
                    //mongoUrlBuilder.Journal = true;
                    //mongoUrlBuilder.WTimeout = TimeSpan.FromMinutes(2);
                    //mongoUrlBuilder.W = WriteConcern.W2.W;
                    // new MongoServerAddress("192.168.7.210", 27017);
                    //mongoUrlBuilder.Servers = new List<MongoServerAddress>() { MongoServerAddress.Parse("test-01"), MongoServerAddress.Parse("test-02"), MongoServerAddress.Parse("test-03") };
                    //server = new MongoClient(mongoUrlBuilder.ToMongoUrl()).GetServer();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    server = null;
                }
                if (server != null && server.State == MongoServerState.Disconnected)
                {
                    server.Connect(TimeSpan.FromSeconds(30));
                }
                return server;
            }
        }

        private static MongoServer GetServer(MongoConnType mongoConnType = MongoConnType.Center)
        {
            lock (locker)
            {
                MongoServer server = null;
                try
                {
                    //mongodb://sa:1234@192.168.7.222:18181,192.168.7.85:18181,192.168.7.82:18181/admin?connect=replicaset&replicaSet=shoespie&readPreference=secondaryPreferred
                    //http://www.cnblogs.com/lykbk/p/uioiuouioiuo789879789.html 连接字符串mongodb://server1,server2:27017,server2:27018/?connect=replicaset&replicaSet=test&readPreference=secondaryPreferred
                    //https://docs.mongodb.org/manual/reference/connection-string/
                    //http://api.mongodb.org/java/current/com/mongodb/MongoClient.html



                    server = new MongoClient(MongoConnModelList.FirstOrDefault(item => item.MongoConnType == mongoConnType).MongoConnStr).GetServer();
                    //http://blog.csdn.net/jivenbest/article/details/8160736
                    //MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder();
                    //mongoUrlBuilder.ReadPreference = new ReadPreference() { ReadPreferenceMode = ReadPreferenceMode.SecondaryPreferred };
                    //mongoUrlBuilder.ConnectionMode = ConnectionMode.ReplicaSet;
                    //mongoUrlBuilder.DatabaseName = "admin";
                    //mongoUrlBuilder.Username = "sa";
                    //mongoUrlBuilder.Password = "1234";
                    //mongoUrlBuilder.ReplicaSetName = "shoespie";
                    //mongoUrlBuilder.SafeMode = SafeMode.True;
                    //mongoUrlBuilder.FSync = true;
                    //mongoUrlBuilder.Journal = true;
                    //mongoUrlBuilder.WTimeout = TimeSpan.FromMinutes(2);
                    //mongoUrlBuilder.W = WriteConcern.W2.W;
                    // new MongoServerAddress("192.168.7.210", 27017);
                    //mongoUrlBuilder.Servers = new List<MongoServerAddress>() { MongoServerAddress.Parse("test-01"), MongoServerAddress.Parse("test-02"), MongoServerAddress.Parse("test-03") };
                    //server = new MongoClient(mongoUrlBuilder.ToMongoUrl()).GetServer();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    server = null;
                }
                if (server != null && server.State == MongoServerState.Disconnected)
                {
                    server.Connect(TimeSpan.FromSeconds(30));
                }
                return server;
            }
        }
        /// <summary>
        /// 断开MongoServer连接，(读写库)
        /// </summary>
        private static void Disconnect(MongoServer server)
        {
            if (server != null)
            {
                //https://darkiri.wordpress.com/2013/05/24/mongodb-c-driver-review/
                if (server.State == MongoServerState.Connected)
                {
                    server.Disconnect();
                }

                server = null;
            }
        }
        #endregion

        #region DB
        /// <summary>
        /// 获取当前MongoDatabase
        /// </summary>
        private static MongoDatabase GetDatabase(MongoServer server)
        {
            MongoDatabase mongoDatabase = null;
            if (server != null)
            {
                mongoDatabase = server.GetDatabase(DbName);
            }
            return mongoDatabase;
        }
        private static MongoDatabase GetDatabase(MongoServer server, MongoConnType mongoConnType = MongoConnType.Center)
        {
            MongoDatabase mongoDatabase = null;
            if (server != null)
            {
                mongoDatabase = server.GetDatabase(MongoDBHelper.MongoConnModelList.FirstOrDefault(item => item.MongoConnType == mongoConnType).MongoDatabase);
                //mongoDatabase = server.GetDatabase(DbName);
            }
            return mongoDatabase;
        }
        /// <summary>
        /// 获取所有数据库
        /// </summary>
        /// <returns></returns>
        public static List<string> AllDB()
        {
            List<string> list = null;
            // 首先创建一个连接
            MongoServer server = GetServer();
            if (server != null)
            {
                list = server.GetDatabaseNames().ToList();
                Disconnect(server);
            }
            return list;
        }
        public static List<string> AllDB(MongoConnType mongoConnType = MongoConnType.Center)
        {
            List<string> list = null;
            // 首先创建一个连接
            MongoServer server = GetServer(mongoConnType);
            if (server != null)
            {
                list = server.GetDatabaseNames().ToList();
                Disconnect(server);
            }
            return list;
        }
        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static bool DropDB(string dbName)
        {
            bool result = false;
            // 首先创建一个连接
            MongoServer server = GetServer();
            if (server != null)
            {
                CommandResult commandResult = server.DropDatabase(dbName);
                result = commandResult != null && commandResult.Ok;
                Disconnect(server);
            }
            return result;
        }
        #endregion

        #region Collection
        /// <summary>
        /// 获取当前MongoCollection
        /// </summary>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        private static MongoCollection<BsonDocument> MongoCollection(string collectionName)
        {
            MongoCollection<BsonDocument> result = null;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        if (!mongoDatabase.CollectionExists(collectionName))
                        {
                            mongoDatabase.CreateCollection(collectionName);
                        }
                        result = mongoDatabase.GetCollection<BsonDocument>(collectionName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        private static MongoCollection<BsonDocument> MongoCollection(string collectionName, MongoConnType mongoConnType = MongoConnType.Center)
        {
            MongoCollection<BsonDocument> result = null;
            MongoServer server = GetServer(mongoConnType);
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server, mongoConnType);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        if (!mongoDatabase.CollectionExists(collectionName))
                        {
                            mongoDatabase.CreateCollection(collectionName);
                        }
                        result = mongoDatabase.GetCollection<BsonDocument>(collectionName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        public static bool Exsit(string collectionName)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        result = mongoDatabase.CollectionExists(collectionName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="collectionName">表名称</param>
        public static bool Create(string collectionName)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        CommandResult commandResult = mongoDatabase.CreateCollection(collectionName);
                        result = result = commandResult != null && commandResult.Ok;
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public static bool Exist(string collectionName)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        result = mongoDatabase.CollectionExists(collectionName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        public static bool Exist(string collectionName, MongoConnType mongoConnType = MongoConnType.Center)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server, mongoConnType);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        result = mongoDatabase.CollectionExists(collectionName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public static bool Remove(string collectionName)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        CommandResult commandResult = mongoDatabase.DropCollection(collectionName);
                        result = commandResult != null && commandResult.Ok;
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        /// <summary>
        /// 重新命名表的名称
        /// </summary>
        /// <param name="oldCollectionName">源表名称</param>
        /// <param name="newCollectionName">新表名称</param>
        /// <returns></returns>
        public static bool Rename(string oldCollectionName, string newCollectionName)
        {
            bool result = false;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase)) //开始连接数据库。
                    {
                        if (mongoDatabase.CollectionExists(oldCollectionName))
                        {
                            CommandResult commandResult = mongoDatabase.RenameCollection(oldCollectionName, newCollectionName);
                            result = commandResult != null && commandResult.Ok;
                        }
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return result;
        }
        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public static List<string> AllCollection()
        {
            List<string> collectionList = null;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        collectionList = mongoDatabase.GetCollectionNames().ToList();
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return collectionList;
        }
        public static List<string> AllCollection(MongoConnType mongoConnType = MongoConnType.Center)
        {
            List<string> collectionList = null;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server, mongoConnType);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        collectionList = mongoDatabase.GetCollectionNames().ToList();
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return collectionList;
        }
        /// <summary>
        /// 删除所有表
        /// </summary>
        public static void RemoveAll()
        {
            List<string> list = AllCollection();
            foreach (string name in list)
            {
                Remove(name);
            }
        }
        #endregion

        #region User
        /// <summary>
        /// 添加指定用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="password">密码</param>
        public static void AddUser(string userName, string password)
        {
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        MongoUser user = new MongoUser(userName, password, false);
                        mongoDatabase.AddUser(user);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
        }
        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        public static void RemoveUser(string userName)
        {
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        mongoDatabase.RemoveUser(userName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
        }
        /// <summary>
        /// 查找指定用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns></returns>
        public static MongoUser FindUser(string userName)
        {
            MongoUser user = null;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        user = mongoDatabase.FindUser(userName);
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return user;
        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public static MongoUser[] ListUsers()
        {
            MongoUser[] users = null;
            MongoServer server = GetServer();
            if (server != null)
            {
                MongoDatabase mongoDatabase = GetDatabase(server);
                if (mongoDatabase != null)
                {
                    using (server.RequestStart(mongoDatabase))//开始连接数据库。
                    {
                        users = mongoDatabase.FindAllUsers();
                    }
                    mongoDatabase = null;
                }
                Disconnect(server);
            }
            return users;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Insert<T>(T entity)
        {
            string collectionName = typeof(T).Name;
            return Insert<T>(collectionName, entity);
        }
        public static bool Insert<T>(T entity, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string collectionName = typeof(T).Name;
            return Insert<T>(collectionName, entity, mongoConnType);
        }
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Insert<T>(string collectionName, T entity)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName);
            ChangeDateTimeDecimal<T>(entity);
            WriteConcernResult writeConcernResult = collection.Insert<T>(entity);
            result = writeConcernResult != null && writeConcernResult.Ok;
            return result;
        }
        public static bool Insert<T>(string collectionName, T entity, MongoConnType mongoConnType = MongoConnType.Center)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            ChangeDateTimeDecimal<T>(entity);
            WriteConcernResult writeConcernResult = collection.Insert<T>(entity);
            result = writeConcernResult != null && writeConcernResult.Ok;
            return result;
        }
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Insert(string collectionName, object entity)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName);
            BsonDocument bsonDocument = ConvertBsonDocument(entity);
            WriteConcernResult writeConcernResult = collection.Insert(bsonDocument);
            result = writeConcernResult != null && writeConcernResult.Ok;
            return result;
        }
        /// <summary>
        /// 批量插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        public static void InsertAll<T>(IEnumerable<T> entitys)
        {
            string collectionName = typeof(T).Name;
            InsertAll<T>(collectionName, entitys);
        }
        /// <summary>
        /// 批量插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="entitys"></param>
        public static void InsertAll<T>(string collectionName, IEnumerable<T> entitys)
        {
            MongoCollection collection = MongoCollection(collectionName);
            ChangeDateTimeDecimal<T>(entitys);
            collection.InsertBatch<T>(entitys);
        }
        /// <summary>
        /// 批量插入对象
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="entitys"></param>
        public static void InsertAll(string collectionName, IEnumerable<object> entitys)
        {
            MongoCollection collection = MongoCollection(collectionName);
            List<BsonDocument> insertDocList = new List<BsonDocument>();
            BsonDocument insertDoc = null;
            foreach (object entity in entitys)
            {
                insertDoc = ConvertBsonDocument(entity);
                if (insertDoc != null)
                {
                    insertDocList.Add(insertDoc);
                }
            }
            insertDoc = null;
            collection.InsertBatch(insertDocList);
            insertDocList.Clear();
            insertDocList = null;
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除对象信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="query">删除条件</param>
        /// <returns></returns>
        public static bool Delete<T>(IMongoQuery query)
        {
            string collectionName = typeof(T).Name;
            return Delete(collectionName, query);
        }
        /// <summary>
        /// 删除对象信息
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool Delete(string collectionName, IMongoQuery query)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName);
            WriteConcernResult writeConernResult = collection.Remove(query);
            result = writeConernResult != null && writeConernResult.Ok;
            return result;
        }
        /// <summary>
        /// 删除表中所有纪录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DeleteAll<T>()
        {
            string collectionName = typeof(T).Name;
            return DeleteAll(collectionName);
        }
        /// <summary>
        /// 删除表中所有纪录
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static bool DeleteAll(string collectionName)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName);
            WriteConcernResult writeConernResult = collection.RemoveAll();
            result = writeConernResult != null && writeConernResult.Ok;
            return result;
        }
        #endregion

        #region Update
        /// <summary>
        /// (UpdateFlags.Multi) http://bbs.csdn.net/topics/390199138
        /// 
        /// 修改对象内容
        /// -----------------------------------------
        /// 修改一列
        /// var query = Query<Person>.EQ(item => item.Id, 3);
        /// var update = Update.Set("Name", "Wangyp");
        /// MongoDBHelper.Update<Person>(query, update);
        /// -----------------------------------------
        /// 修改多列，第一种方式
        /// var query = Query<Person>.EQ(item => item.Id, 3);
        /// var update1 = Update.Set("Name", "Wangyp");
        /// var update2 = Update.Set("Age", 5);
        /// UpdateBuilder builder = new UpdateBuilder();
        /// builder.Combine(update1);
        /// builder.Combine(update2);
        /// MongoDBHelper.Update<Person>(query, builder);
        /// -----------------------------------------
        /// 修改多列，第二种方式
        /// var query = Query<Person>.EQ(item => item.Id, 3);
        /// UpdateBuilder builder = new UpdateBuilder();
        /// builder.Set("Name", "abc");
        /// builder.Set("Age", 32);
        /// MongoDBHelper.Update<Person>(query, builder);
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="query">修改条件</param>
        /// <param name="update">要修改对象的哪些属性和值</param>
        /// <returns></returns>
        public static bool Update<T>(IMongoQuery query, IMongoUpdate update)
        {
            string collectionName = typeof(T).Name;
            return Update(collectionName, query, update);
        }
        /// <summary>
        /// 修改对象内容
        /// </summary>
        /// <param name="collectionName">表名</param>
        /// <param name="query">修改条件</param>
        /// <param name="update">要修改对象的哪些属性和值</param>
        /// <returns></returns>
        public static bool Update(string collectionName, IMongoQuery query, IMongoUpdate update)
        {
            bool result = false;
            MongoCollection collection = MongoCollection(collectionName);
            WriteConcernResult writeConcernResult = collection.Update(query, update, UpdateFlags.Multi);//wangyunpeng 2015-7-14 修改Mongodb支持批量更新
            result = writeConcernResult != null && writeConcernResult.Ok;
            return result;
        }
        /// <summary>
        /// 修改对象内容
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="query">修改条件</param>
        /// <param name="entity">要更新之后的对象内容</param>
        /// <returns></returns>
        public static bool Update<T>(IMongoQuery query, T entity)
        {
            string collectionName = typeof(T).Name;
            UpdateDocument updateDocument = ConvertUpdateDocument<T>(entity);
            return Update(collectionName, query, updateDocument);
        }
        /// <summary>
        /// 修改对象内容
        /// </summary>
        /// <param name="collectionName">表名</param>
        /// <param name="query">修改条件</param>
        /// <param name="entity">要更新之后的对象内容</param>
        /// <returns></returns>
        public static bool Update(string collectionName, IMongoQuery query, object entity)
        {
            UpdateDocument updateDocument = ConvertUpdateDocument(entity);
            return Update(collectionName, query, updateDocument);
        }
        #endregion

        #region Count
        /// <summary>
        /// 只取查询记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        public static long Count<T>(IMongoQuery query)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.Count(query);
        }
        #endregion

        #region Select
        /// <summary>
        /// 显示全部，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Select<T>()
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).ToList();
        }
        public static List<T> Select<T>(MongoConnType mongoConnType = MongoConnType.Center)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAllAs<T>().SetFields(field).ToList();
        }
        /// <summary>
        /// 显示全部纪录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static void Select<T>(out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count();
        }
        public static void Select<T>(out long recordCount, MongoConnType mongoConnType = MongoConnType.Center)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            recordCount = collection.Count();
        }
        public static void Select<T>(IMongoQuery query, out long recordCount, MongoConnType mongoConnType = MongoConnType.Center)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            recordCount = collection.FindAs<T>(query).Count();
        }
        public static void Select<T>(string collectionName, IMongoQuery query, out long recordCount, MongoConnType mongoConnType = MongoConnType.Center)
        {
            recordCount = 0;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            recordCount = collection.FindAs<T>(query).Count();
        }
        /// <summary>
        /// 显示全部，不支持排序
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static List<BsonDocument> Select(string collectionName)
        {
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<BsonDocument>().ToList();
        }
        /// <summary>
        /// 显示全部纪录数
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static void Select(string collectionName, out long recordCount)
        {
            recordCount = 0;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count();
        }
        public static void Select(string collectionName, out long recordCount, MongoConnType mongoConnType = MongoConnType.Center)
        {
            recordCount = 0;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            recordCount = collection.Count();
        }
        /// <summary>
        /// 显示全部，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields">显示出了ID之外的另外一些列的值</param>
        /// <returns></returns>
        public static List<T> Select<T>(params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).ToList();
        }
        /// <summary>
        /// 显示全部，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<T> Select<T>(IMongoSortBy sortBy)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).SetSortOrder(sortBy).ToList();
        }
        /// <summary>
        /// 显示全部，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortBy"></param>
        /// <param name="fields">显示出了ID之外的另外一些列的值</param>
        /// <returns></returns>
        public static List<T> Select<T>(IMongoSortBy sortBy, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).SetSortOrder(sortBy).ToList<T>();
        }
        /// <summary>
        /// 条件查询，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> Select<T>(IMongoQuery query)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).ToList<T>();
        }
        public static List<T> Select<T>(IMongoQuery query, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).ToList<T>();
        }
        public static List<T> Select<T>(string collectionName,IMongoQuery query, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).ToList<T>();
        }
        /// <summary>
        /// 条件查询，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<T> Select<T>(IMongoQuery query, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count(query);
            return collection.FindAs<T>(query).SetFields(field).ToList<T>();
        }

        public static List<T> Select<T>(IMongoQuery query, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).ToList<T>();
        }
        /// <summary>
        /// 条件查询，支持排序
        /// ----------------------------------------
        /// 第一种Query方式：
        /// var query = Query<Person>.GTE(item => item.Id, 2340);
        /// 第二种Query方式：
        /// List<IMongoQuery> queryList = new List<IMongoQuery>();
        /// queryList.Add(Query<Person>.GT(e => e.Id, 100));
        /// queryList.Add(Query<Person>.LT(e => e.Id, 200));
        /// IMongoQuery query = Query.And(queryList.ToArray());
        /// -----------------------------------
        /// 第一种SortBy方式：
        /// var sortBy = SortBy<Person>.Descending(item => item.Id)
        /// 第二种SortBy方式：
        /// SortByDocument sortBy = new SortByDocument();
        /// sortBy.Add("Age", 1);//1 ASC
        /// sortBy.Add("Name", -1);//-1 DESC
        /// List<Person> list = MongoDBHelper.Select<Person>(10, 1, query, sortBy);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<T> Select<T>(IMongoQuery query, IMongoSortBy sortBy)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).ToList<T>();
        }
        public static List<T> Select<T>(IMongoQuery query, IMongoSortBy sortBy, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName,mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).ToList<T>();
        }
        public static List<T> Select<T>(string collectionName, IMongoQuery query, IMongoSortBy sortBy, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).ToList<T>();
        }
        public static List<T> Select<T>(IMongoQuery query, IMongoSortBy sortBy, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).ToList<T>();
        }
        /// <summary>
        /// 分页显示，不支持条件，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static List<T> Select<T>(int pageSize, int pageIndex)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count();
            return collection.FindAllAs<T>().SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAllAs<T>().SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        /// <summary>
        /// 分页显示，支持条件，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
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
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count(query);
            return collection.FindAs<T>(query).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, out long recordCount, params string[] fields)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count(query);
            return collection.FindAs<T>(query).SetFields(field).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        /// <summary>
        /// TOP显示，支持条件，不支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> Select<T>(int topCount, IMongoQuery query)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetLimit(topCount).ToList<T>(); ;
        }        
        /// <summary>
        /// TOP显示，支持条件，支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topCount"></param>
        /// <param name="query"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static List<T> Select<T>(int topCount, IMongoQuery query, IMongoSortBy sortBy)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetLimit(topCount).ToList<T>(); ;
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
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(string collectionName, int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy, MongoConnType mongoConnType = MongoConnType.Center)
        {
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName, mongoConnType);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy, params string[] fields)
        {
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
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
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy, out long recordCount)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count(query);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        public static List<T> Select<T>(int pageSize, int pageIndex, IMongoQuery query, IMongoSortBy sortBy, out long recordCount, params string[] fields)
        {
            recordCount = 0;
            string collectionName = typeof(T).Name;
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            recordCount = collection.Count(query);
            return collection.FindAs<T>(query).SetFields(field).SetSortOrder(sortBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList<T>(); ;
        }
        #endregion

        #region Group
        /// <summary>
        /// 分组聚合
        /// http://www.cnblogs.com/shanyou/archive/2012/08/05/2624073.html
        /// http://www.linuxidc.com/Linux/2014-03/98217.htm
        /// http://runfriends.iteye.com/blog/1814968
        /// http://www.cnblogs.com/zhwl/archive/2013/12/19/3482140.html
        /// 例子：
        /// db.MongoDBSpuCustomerRatingModel.group({
        ///  key : {SPUID:true}, 
        ///  initial : {"RatingCount": 0}, 
        ///  reduce : function Reduce(doc, out) {
        ///     /*
        ///     // At start of Reduce, 'out' = intial value
        ///     out.count += doc.count; 
        ///     */
        ///     out.RatingCount+=doc.RealCustomerRatingCount+doc.ManualCustomerRatingCount;
        ///  }, 
        ///  finalize : function Finalize(out) {
        ///     /*  
        ///     // Make final updates or calculations
        ///     out.countSquare = out.count * out.count
        ///     */
        ///     return out;
        ///  }
        ///  });
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <typeparam name="K">初始化键类型，通常为string</typeparam>
        /// <typeparam name="V">初始化值类型，通常为int、double</typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="groupFields">分组字段名称的数组类型</param>
        /// <param name="initialDict">初始化键和值</param>
        /// <param name="reduceJavaScriptFunction">聚合函数（包括两个参数：第一个参数doc就是正在遍历的文档，第二个参数就是遍历到这个文档之前的聚合结果，这个函数将根据这两个参数形成新的聚合结果。）</param>
        /// <param name="finalizeJavaScriptFunction">聚合完成的结果（包括一个函数：第一个参数用来表示接收聚合完成的结果作最终的计算）</param>
        /// <returns></returns>
        public static List<BsonDocument> Group<T, K, V>(IMongoQuery query, string[] groupFields, Dictionary<K, V> initialDict, string reduceJavaScriptFunction, string finalizeJavaScriptFunction)
        {
            string collectionName = typeof(T).Name;

            GroupByBuilder groupbyBuilder = new GroupByBuilder(groupFields);
            MongoCollection collection = MongoCollection(collectionName);

            return collection.Group(query
                            , groupbyBuilder
                            , BsonDocument.Create(initialDict)
                            , BsonJavaScriptWithScope.Create(reduceJavaScriptFunction)
                            , BsonJavaScriptWithScope.Create(finalizeJavaScriptFunction)).ToList();
        }
        public static List<BsonDocument> Group<T, K, V>(IMongoQuery query, string keyFunction,
            Dictionary<K, V> initialDict, string reduce, string finalize)
        {
            string collectionName = typeof(T).Name;

            MongoCollection collection = MongoCollection(collectionName);

            return collection.Group(query,
                BsonJavaScriptWithScope.Create(keyFunction),
                BsonDocument.Create(initialDict),
                BsonJavaScriptWithScope.Create(reduce),
                BsonJavaScriptWithScope.Create(finalize)).ToList();
        }
        #endregion

        #region Aggregate
        /// <summary>
        /// 统计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="match">过滤</param>
        /// <param name="group">分组</param>
        /// <param name="project">显示</param>
        /// <returns></returns>
        public static List<BsonDocument> Aggregate<T>(BsonDocument match, BsonDocument group, BsonDocument project)
        {
            AggregateArgs aggregateArgs = new AggregateArgs();
            aggregateArgs.Pipeline = new List<BsonDocument> { group, match, project };
            string collectionName = typeof(T).Name;
            MongoCollection collection = MongoCollection(collectionName);
            return collection.Aggregate(aggregateArgs).ToList();
        }
        #endregion

        #region Get
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static T Get<T>(IMongoQuery query) where T : class
        {
            T obj = null;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            if (collection != null)
            {
                //obj = collection.FindOneAs<T>(query);
                obj = collection.FindAs<T>(query).SetFields(field).FirstOrDefault<T>();
            }
            return obj;
        }
        public static T Get<T>(IMongoQuery query, MongoConnType mongoConnType = MongoConnType.Center) where T : class
        {
            T obj = null;
            string collectionName = typeof(T).Name;
            string[] fields = GetFields<T>();
            IMongoFields field = Fields.Include(fields);
            Array.Clear(fields, 0, fields.Length);
            fields = null;
            MongoCollection collection = MongoCollection(collectionName);
            if (collection != null)
            {
                //obj = collection.FindOneAs<T>(query);
                obj = collection.FindAs<T>(query).SetFields(field).FirstOrDefault<T>();
            }
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static BsonValue GetId(BsonDocument document)
        {
            return document.Elements.FirstOrDefault().Value;
        }
        /// <summary>
        /// 获取MongoDB里面的唯一ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static BsonValue GetId(object entity)
        {
            BsonValue id = null;
            if (entity == null)
                return id;
            PropertyInfo propertyInfo = entity.GetType().GetProperty("_id");
            if (propertyInfo == null)
            {
                BsonDocument document = ConvertBsonDocument(entity);
                id = GetId(document);
            }
            else
            {
                object value = propertyInfo.GetValue(entity, null);
                if (value != null)
                {
                    id = BsonValue.Create(value);
                }
            }
            return id;
        }
        /// <summary>
        /// 得到某一个类型的所有属性名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetFields<T>()
        {
            string[] fields = null;
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            int length = propertyInfos.Length;
            fields = new string[length];
            for (int i = 0; i < length; i++)
            {
                fields[i] = propertyInfos[i].Name;
            }
            Array.Clear(propertyInfos, 0, length);
            propertyInfos = null;
            return fields;
        }
        #endregion
    }

    public enum MongoConnType
    {
        Center = 1,
        Project = 2,
        Other = 3
    }
    public class MongoConnModel
    {
        public MongoConnType MongoConnType { get; set; }

        public string MongoConnStr { get; set; }

        public string MongoDatabase { get; set; }
    }
}
