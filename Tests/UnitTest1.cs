namespace Tests
{
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Transactions;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest1
    {
        private static string _serverLocaldbProjectsv12TrustedConnectionSspi
            = @"Server=(localdb)\ProjectsV12; Trusted_Connection=SSPI";


        [TestInitialize]
        public void Initialize()
        {
            using (var connection = new SqlConnection(_serverLocaldbProjectsv12TrustedConnectionSspi))
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
                    sqlCommand.CommandText = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Scripts.Database + data.sql")).ReadToEnd();

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }


        [TestMethod]
        public void TransactionScope1Wrong()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_serverLocaldbProjectsv12TrustedConnectionSspi))
                {

                    sqlConnection.Open();


                    using (var transactionScope = new TransactionScope())
                    {

                        Foo(sqlConnection, new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Scripts.Delete from T3 and T1.sql")).ReadToEnd());

                        transactionScope.Complete();
                    }
                }
            }
            catch (SqlException sqlException)
            {
            }
        }

        private static void Foo(SqlConnection sqlConnection, string commandText)
        {
            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = commandText;

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}