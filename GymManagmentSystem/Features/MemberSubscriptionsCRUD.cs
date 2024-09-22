using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GymManagmentSystem.Models;

namespace GymManagmentSystem.Features
{
    public class MemberSubscriptionsCRUD
    {
        private readonly string _connectionString;

        public MemberSubscriptionsCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Create a new member subscription
        public void CreateMemberSubscription(MemberSubscription memberSubscription)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO dbo.MemberSubscriptions (MemberId, SubscriptionId, OriginalPrice, DiscountValue, PaidPrice, StartDate, EndDate, RemainingSessions, IsDeleted) " +
                             "VALUES (@MemberId, @SubscriptionId, @OriginalPrice, @DiscountValue, @PaidPrice, @StartDate, @EndDate, @RemainingSessions, 0)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberId", memberSubscription.MemberId);
                cmd.Parameters.AddWithValue("@SubscriptionId", memberSubscription.SubscriptionId);
                cmd.Parameters.AddWithValue("@OriginalPrice", memberSubscription.OriginalPrice);
                cmd.Parameters.AddWithValue("@DiscountValue", memberSubscription.DiscountValue);
                cmd.Parameters.AddWithValue("@PaidPrice", memberSubscription.PaidPrice);
                cmd.Parameters.AddWithValue("@StartDate", memberSubscription.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", memberSubscription.EndDate);
                cmd.Parameters.AddWithValue("@RemainingSessions", memberSubscription.RemainingSessions);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while creating the member subscription.", ex);
                }
            }
        }

        // Get a specific member subscription by ID
        public MemberSubscription GetMemberSubscriptionById(int memberSubscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.MemberSubscriptions WHERE Id = @MemberSubscriptionId AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberSubscriptionId", memberSubscriptionId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MemberSubscription
                        {
                            Id = (int)reader["Id"],
                            MemberId = (int)reader["MemberId"],
                            SubscriptionId = (int)reader["SubscriptionId"],
                            OriginalPrice = (decimal)reader["OriginalPrice"],
                            DiscountValue = (decimal)reader["DiscountValue"],
                            PaidPrice = (decimal)reader["PaidPrice"],
                            StartDate = (DateTime)reader["StartDate"],
                            EndDate = (DateTime)reader["EndDate"],
                            RemainingSessions = (int)reader["RemainingSessions"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                }
            }
            return null; // Member subscription not found or deleted
        }

        // Get all member subscriptions
        public List<MemberSubscription> GetAllMemberSubscriptions()
        {
            List<MemberSubscription> subscriptions = new List<MemberSubscription>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.MemberSubscriptions WHERE IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(new MemberSubscription
                        {
                            Id = (int)reader["Id"],
                            MemberId = (int)reader["MemberId"],
                            SubscriptionId = (int)reader["SubscriptionId"],
                            OriginalPrice = (decimal)reader["OriginalPrice"],
                            DiscountValue = (decimal)reader["DiscountValue"],
                            PaidPrice = (decimal)reader["PaidPrice"],
                            StartDate = (DateTime)reader["StartDate"],
                            EndDate = (DateTime)reader["EndDate"],
                            RemainingSessions = (int)reader["RemainingSessions"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        });
                    }
                }
            }

            return subscriptions;
        }

        // Update a member subscription
        public void UpdateMemberSubscription(MemberSubscription memberSubscription)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.MemberSubscriptions SET MemberId = @MemberId, SubscriptionId = @SubscriptionId, OriginalPrice = @OriginalPrice, " +
                             "DiscountValue = @DiscountValue, PaidPrice = @PaidPrice, StartDate = @StartDate, EndDate = @EndDate, " +
                             "RemainingSessions = @RemainingSessions WHERE Id = @MemberSubscriptionId AND IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberSubscriptionId", memberSubscription.Id);
                cmd.Parameters.AddWithValue("@MemberId", memberSubscription.MemberId);
                cmd.Parameters.AddWithValue("@SubscriptionId", memberSubscription.SubscriptionId);
                cmd.Parameters.AddWithValue("@OriginalPrice", memberSubscription.OriginalPrice);
                cmd.Parameters.AddWithValue("@DiscountValue", memberSubscription.DiscountValue);
                cmd.Parameters.AddWithValue("@PaidPrice", memberSubscription.PaidPrice);
                cmd.Parameters.AddWithValue("@StartDate", memberSubscription.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", memberSubscription.EndDate);
                cmd.Parameters.AddWithValue("@RemainingSessions", memberSubscription.RemainingSessions);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while updating the member subscription.", ex);
                }
            }
        }

        // Soft delete a member subscription
        public void DeleteMemberSubscription(int memberSubscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.MemberSubscriptions SET IsDeleted = 1 WHERE Id = @MemberSubscriptionId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberSubscriptionId", memberSubscriptionId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while deleting the member subscription.", ex);
                }
            }
        }

        // Restore a soft-deleted member subscription
        public void RestoreMemberSubscription(int memberSubscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.MemberSubscriptions SET IsDeleted = 0 WHERE Id = @MemberSubscriptionId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MemberSubscriptionId", memberSubscriptionId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while restoring the member subscription.", ex);
                }
            }
        }
    }
}
