using System;
using System.Data.SqlClient;

namespace GameToBeNamed.src
{
    internal sealed class GameSqlAccess : IDisposable
    {
        readonly SqlConnection connection = new SqlConnection("Data Source=10.230.233.180;Initial Catalog=Game_Accounts;User ID=gamequeryer;Password=Password123$");
        readonly string login_query = "SELECT UserUUID FROM users WHERE UserName=@username AND UserPassword=@password";

        public GameSqlAccess()
        {
            try
            {
                connection.Open();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch(SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public bool login(string username, string password)
        {
            using (SqlCommand command = new SqlCommand(login_query, connection))
            {
                // Set the parameters for the query
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                return command.ExecuteScalar() != null;
            }
        }

        public void Dispose() => connection.Close();
    }
}
