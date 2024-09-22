using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GymManagmentSystem.Models;

namespace GymManagmentSystem.Features
{
    public class DiscountsCRUD
    {
        private readonly string _connectionString;

        public DiscountsCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Create a new discount
        public void CreateDiscount(Discount discount)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO dbo.Discounts (Code, Value, StartDate, EndDate, IsDeleted) " +
                             "VALUES (@Code, @Value, @StartDate, @EndDate, 0)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Code", discount.Code);
                cmd.Parameters.AddWithValue("@Value", discount.Value);
                cmd.Parameters.AddWithValue("@StartDate", discount.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", discount.EndDate);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while creating the discount.", ex);
                }
            }
        }

        // Get a specific discount by ID
        public Discount GetDiscountById(int discountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Discounts WHERE Id = @DiscountId AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DiscountId", discountId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Discount
                        {
                            Id = (int)reader["Id"],
                            Code = reader["Code"].ToString(),
                            Value = (decimal)reader["Value"],
                            StartDate = (DateTime)reader["StartDate"],
                            EndDate = (DateTime)reader["EndDate"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                }
            }
            return null; // Discount not found or deleted
        }

        // Get all active discounts
        public List<Discount> GetAllDiscounts()
        {
            List<Discount> discounts = new List<Discount>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Discounts WHERE IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        discounts.Add(new Discount
                        {
                            Id = (int)reader["Id"],
                            Code = reader["Code"].ToString(),
                            Value = (decimal)reader["Value"],
                            StartDate = (DateTime)reader["StartDate"],
                            EndDate = (DateTime)reader["EndDate"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        });
                    }
                }
            }

            return discounts;
        }

        // Update a discount
        public void UpdateDiscount(Discount discount)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Discounts SET Code = @Code, Value = @Value, StartDate = @StartDate, EndDate = @EndDate " +
                             "WHERE Id = @DiscountId AND IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DiscountId", discount.Id);
                cmd.Parameters.AddWithValue("@Code", discount.Code);
                cmd.Parameters.AddWithValue("@Value", discount.Value);
                cmd.Parameters.AddWithValue("@StartDate", discount.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", discount.EndDate);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while updating the discount.", ex);
                }
            }
        }

        // Soft delete a discount
        public void DeleteDiscount(int discountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Discounts SET IsDeleted = 1 WHERE Id = @DiscountId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DiscountId", discountId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while deleting the discount.", ex);
                }
            }
        }

        // Restore a soft-deleted discount
        public void RestoreDiscount(int discountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Discounts SET IsDeleted = 0 WHERE Id = @DiscountId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DiscountId", discountId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while restoring the discount.", ex);
                }
            }
        }
    }
}
