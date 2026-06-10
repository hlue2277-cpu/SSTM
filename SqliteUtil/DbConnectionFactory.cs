using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SqliteUtil
{
    public static class DbConnectionFactory
    {
        private static string _localDbPath = string.Empty;

        private static string getLocalDbPath()
        {
            string dirPath = string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, "DB");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var filePath = string.Format(@"{0}\{1}", dirPath, "sstm.db3");
            if (!File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);
            }
            return filePath;
        }

        #region Public

        /// <summary>
        /// not forget about conn.Close(), you can put the whole thing in a using, e.g:
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        /// Data Source=c:\mydb.db;Version=3;Pooling=True;Max Pool Size=100;
        public static IDbConnection GetLocalDbConnection()
        {
            if (string.IsNullOrEmpty(_localDbPath))
            {
                _localDbPath = getLocalDbPath();
            }
            OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;
            var sqlLiteConStr = string.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;", _localDbPath);
            var dbFactory = new OrmLiteConnectionFactory(sqlLiteConStr, SqliteDialect.Provider, false);
            var conn = dbFactory.Open();
            return conn;
        }

        #endregion Public
    }
}