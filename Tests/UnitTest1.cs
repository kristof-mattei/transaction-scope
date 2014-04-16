namespace Tests
{
    using System.Data.SqlClient;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest1
    {
        private SqlConnection _sqlConnection;

        [TestInitialize()]
        public void Setup()
        {
            this._sqlConnection = new SqlConnection("Server=.; Initial Catalog=TransactionScopeTests; Integrated Security=SSPI");
            this._sqlConnection.Open();

            using (SqlCommand sqlCommand = this._sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "INSERT INTO T1 ([Value]) VALUES (1), (2), (3), (4), (5), (6)"
                                         + "INSERT INTO T2 ([T1Key]) VALUES (1), (2), (3), (4), (5)"
                                         + "INSERT INTO T3 ([T1Key]) VALUES (1), (2), (3), (4), (5)";

                sqlCommand.ExecuteNonQuery();
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (SqlCommand sqlCommand = this._sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "DELETE FROM T2; DELETE FROM T3; DELETE FROM T1;";

                sqlCommand.ExecuteNonQuery();
            }

            _sqlConnection.Close();
        }
    }
}