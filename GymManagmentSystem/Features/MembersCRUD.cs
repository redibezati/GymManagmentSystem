using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GymManagmentSystem.Models;

namespace GymManagmentSystem.Features
{
    public class MembersCRUD
    {
        private readonly string _connectionString;

        public MembersCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Create a new member
        public void CreateMember(Member member)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO dbo.Members (FirstName, LastName, Birthday, IdCardNumber, Email, RegistrationDate, IsDeleted) " +
                             "VALUES (@FirstName, @LastName, @Birthday, @IdCardNumber, @Email, @RegistrationDate, 0)";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
                cmd.Parameters.AddWithValue("@LastName", member.LastName);
                cmd.Parameters.AddWithValue("@Birthday", member.Birthday);
                cmd.Parameters.AddWithValue("@IdCardNumber", member.IdCardNumber);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while creating the member.", ex);
                }
            }
        }
        
        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Members WHERE IsDeleted = 0", conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new Member
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Birthday = (DateTime)reader["Birthday"],
                            IdCardNumber = reader["IdCardNumber"].ToString(),
                            Email = reader["Email"].ToString(),
                            RegistrationDate = (DateTime)reader["RegistrationDate"]
                        });
                    }
                }
            }

            return members;
        }

        // Get a specific member by ID
        public Member GetMemberById(int memberId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Members WHERE Id = @MemberId AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Member
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Birthday = (DateTime)reader["Birthday"],
                            IdCardNumber = reader["IdCardNumber"].ToString(),
                            Email = reader["Email"].ToString(),
                            RegistrationDate = (DateTime)reader["RegistrationDate"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                }
            }
            return null; // Member not found or deleted
        }

        // Get members by criteria
        public List<Member> GetMembersByCriteria(string firstName = null, string lastName = null, string idCardNumber = null, string email = null)
        {
            List<Member> members = new List<Member>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Members WHERE IsDeleted = 0";
                if (!string.IsNullOrEmpty(firstName))
                    sql += " AND FirstName LIKE @FirstName";
                if (!string.IsNullOrEmpty(lastName))
                    sql += " AND LastName LIKE @LastName";
                if (!string.IsNullOrEmpty(idCardNumber))
                    sql += " AND IdCardNumber = @IdCardNumber";
                if (!string.IsNullOrEmpty(email))
                    sql += " AND Email = @Email";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(firstName))
                    cmd.Parameters.AddWithValue("@FirstName", "%" + firstName + "%");
                if (!string.IsNullOrEmpty(lastName))
                    cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%");
                if (!string.IsNullOrEmpty(idCardNumber))
                    cmd.Parameters.AddWithValue("@IdCardNumber", idCardNumber);
                if (!string.IsNullOrEmpty(email))
                    cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new Member
                        {
                            Id = (int)reader["Id"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Birthday = (DateTime)reader["Birthday"],
                            IdCardNumber = reader["IdCardNumber"].ToString(),
                            Email = reader["Email"].ToString(),
                            RegistrationDate = (DateTime)reader["RegistrationDate"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        });
                    }
                }
            }

            return members;
        }

        // Update a member
        public void UpdateMember(Member member)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Members SET FirstName = @FirstName, LastName = @LastName, Birthday = @Birthday, " +
                             "IdCardNumber = @IdCardNumber, Email = @Email WHERE Id = @MemberId AND IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberId", member.Id);
                cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
                cmd.Parameters.AddWithValue("@LastName", member.LastName);
                cmd.Parameters.AddWithValue("@Birthday", member.Birthday);
                cmd.Parameters.AddWithValue("@IdCardNumber", member.IdCardNumber);
                cmd.Parameters.AddWithValue("@Email", member.Email);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while updating the member.", ex);
                }
            }
        }

        // Soft delete a member
        public void DeleteMember(int memberId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Members SET IsDeleted = 1 WHERE Id = @MemberId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while deleting the member.", ex);
                }
            }
        }

        // Restore a soft-deleted member
        public void RestoreMember(int memberId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Members SET IsDeleted = 0 WHERE Id = @MemberId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while restoring the member.", ex);
                }
            }
        }
    }
}
