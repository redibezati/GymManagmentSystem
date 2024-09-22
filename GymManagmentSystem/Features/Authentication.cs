using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using GymManagmentSystem.Models;

namespace GymManagmentSystem.Features
{
    public class Authentication
    {
        private readonly string _connectionString;

        public Authentication(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to verify user credentials
        public bool VerifyLogin(string username, string password, out User user)
        {
            user = null;
            string hashedPassword = HashPassword(password);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = (int)reader["Id"],
                            Username = reader["Username"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                        return true; 
                    }
                }
            }
            return false; 
        }

        // Method to check if the user has a specific role
        public bool UserHasRole(int userId, string roleName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(*) FROM UserRoles ur " +
                             "JOIN Roles r ON ur.RoleId = r.Id " +
                             "WHERE ur.UserId = @UserId AND r.RoleName = @RoleName AND ur.IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RoleName", roleName);

                conn.Open();
                int roleCount = (int)cmd.ExecuteScalar();
                return roleCount > 0; // Returns true if the user has the role
            }
        }

        // Method to hash the password using SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password string to a byte array
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
