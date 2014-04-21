namespace Tests
{
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Transactions;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransactionTests
    {
        /// <summary>
        ///     Default connection string for all connnections, change 'ProjectsV12' to 'V11.0' for SQL Server Express LocalDb 2012
        /// </summary>
        private const string ConnectionString = @"Server=(localdb)\ProjectsV12; Trusted_Connection=SSPI; Application Name=TransactionScopeTests";

        private static class Resources
        {
            public static readonly string CreateTables = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Scripts.Tables + data.sql")).ReadToEnd();
            public static readonly string Deletion = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Scripts.Delete from T3 and T1.sql")).ReadToEnd();
            public static readonly string CountRows = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Scripts.Count results.sql")).ReadToEnd();
        }

        /// <summary>
        ///     Defines the default transaction options
        /// </summary>
        private readonly TransactionOptions _defaultTransactionOptions = new TransactionOptions()
                                                                         {
                                                                             IsolationLevel = IsolationLevel.ReadCommitted,
                                                                             Timeout = TransactionManager.DefaultTimeout
                                                                         };

        [TestInitialize]
        public void Initialize()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var server = new Server(new ServerConnection(connection));

                // get the database
                const string databaseName = "TransactionScopeTests";
                Database transactionScopeTests = server.Databases[databaseName];

                if (null != transactionScopeTests)
                {
                    server.KillDatabase(databaseName);
                }

                transactionScopeTests = new Database(server, databaseName);

                transactionScopeTests.Create(false);

                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    sqlCommand.CommandText = Resources.CreateTables;

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }


        [TestMethod]
        [Description("Opens the SqlConnection outside of the TransactionScope, thus causing the command not to use a transaction.")]
        [ExpectedException(typeof (AssertFailedException), "This one is supposed to fail.")]
        public void SqlConnectionOpenTransactionScope()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, this._defaultTransactionOptions))
                    {
                        ExecuteNonQuery(sqlConnection, Resources.Deletion);

                        transactionScope.Complete();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                Debug.Write(sqlException);
                ValidateDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof (AssertFailedException), "This one is supposed to fail.")]
        [Description("Opens the SqlConnection without transaction, complete erroneous behavior.")]
        public void SqlConnectionWithoutTransaction()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    ExecuteNonQuery(sqlConnection, Resources.Deletion);
                }
            }
            catch (SqlException sqlException)
            {
                Debug.Write(sqlException);
                ValidateDatabase();
            }
        }

        [Description("Construct and opens a Sql Connection inside a TransactionScope, thus causing the command beeing part of a transaction.")]
        [TestMethod]
        public void TransactionScopeSqlConnectionOpen()
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, this._defaultTransactionOptions))
                {
                    using (var sqlConnection = new SqlConnection(ConnectionString))
                    {
                        sqlConnection.Open();

                        ExecuteNonQuery(sqlConnection, Resources.Deletion);

                        transactionScope.Complete();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                Debug.Write(sqlException);
                ValidateDatabase();
            }
        }

        [TestMethod]
        [Description("Creates a Sql Connection, then a TransactionScope and then opens the Sql connection. It still works.")]
        public void SqlConnectionTransactionScopeOpen()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, this._defaultTransactionOptions))
                    {
                        sqlConnection.Open();

                        ExecuteNonQuery(sqlConnection, Resources.Deletion);

                        transactionScope.Complete();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                Debug.Write(sqlException);
                ValidateDatabase();
            }
        }

        /// <summary>
        ///     Validate the count of the rows in the database
        /// </summary>
        private static void ValidateDatabase()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = Resources.CountRows;

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        sqlDataReader.Read();

                        int t1Count = sqlDataReader.GetInt32(0);
                        int t2Count = sqlDataReader.GetInt32(1);
                        int t3Count = sqlDataReader.GetInt32(2);

                        Assert.AreEqual(6, t1Count);
                        Assert.AreEqual(5, t2Count);
                        Assert.AreEqual(5, t3Count, "Transaction should've been rolled back, this should be 5");
                    }
                }
            }
        }

        /// <summary>
        ///     Executes a query throwing away the result
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="commandText"></param>
        private static void ExecuteNonQuery(SqlConnection sqlConnection, string commandText)
        {
            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = commandText;

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}