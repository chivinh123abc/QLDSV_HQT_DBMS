using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AuthRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection(username, password);
                connection.Open();

                // If we reach here, connection is valid
                return true;
            }
            catch (SqlException)
            {
                // Login failed (e.g., wrong credentials for individual DB login)
                return false;
            }
        }
    }
}
