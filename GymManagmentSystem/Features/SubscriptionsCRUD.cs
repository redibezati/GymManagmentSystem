using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GymManagmentSystem.Models;

namespace GymManagmentSystem.Features
{
    public class SubscriptionsCRUD
    {
        private readonly string _connectionString;

        public SubscriptionsCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Create a new subscription
        public void CreateSubscription(Subscription subscription)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO dbo.Subscriptions (Code, Description, NumberOfMonths, WeekFrequency, TotalNumberOfSessions, TotalPrice, IsDeleted) " +
                             "VALUES (@Code, @Description, @NumberOfMonths, @WeekFrequency, @TotalNumberOfSessions, @TotalPrice, 0)";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Code", subscription.Code);
                cmd.Parameters.AddWithValue("@Description", subscription.Description);
                cmd.Parameters.AddWithValue("@NumberOfMonths", subscription.NumberOfMonths);
                cmd.Parameters.AddWithValue("@WeekFrequency", subscription.WeekFrequency);
                cmd.Parameters.AddWithValue("@TotalNumberOfSessions", subscription.TotalNumberOfSessions);
                cmd.Parameters.AddWithValue("@TotalPrice", subscription.TotalPrice);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while creating the subscription.", ex);
                }
            }
        }

        // Get a specific subscription by ID
        public Subscription GetSubscriptionById(int subscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Subscriptions WHERE Id = @SubscriptionId AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SubscriptionId", subscriptionId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Subscription
                        {
                            Id = (int)reader["Id"],
                            Code = reader["Code"].ToString(),
                            Description = reader["Description"].ToString(),
                            NumberOfMonths = (int)reader["NumberOfMonths"],
                            WeekFrequency = reader["WeekFrequency"].ToString(),
                            TotalNumberOfSessions = (int)reader["TotalNumberOfSessions"],
                            TotalPrice = (decimal)reader["TotalPrice"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                }
            }
            return null; // Subscription not found or deleted
        }

        // Get all active subscriptions
        public List<Subscription> GetAllSubscriptions()
        {
            List<Subscription> subscriptions = new List<Subscription>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Subscriptions WHERE IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(new Subscription
                        {
                            Id = (int)reader["Id"],
                            Code = reader["Code"].ToString(),
                            Description = reader["Description"].ToString(),
                            NumberOfMonths = (int)reader["NumberOfMonths"],
                            WeekFrequency = reader["WeekFrequency"].ToString(),
                            TotalNumberOfSessions = (int)reader["TotalNumberOfSessions"],
                            TotalPrice = (decimal)reader["TotalPrice"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        });
                    }
                }
            }

            return subscriptions;
        }

        // Update a subscription
        public void UpdateSubscription(Subscription subscription)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Subscriptions SET Code = @Code, Description = @Description, NumberOfMonths = @NumberOfMonths, " +
                             "WeekFrequency = @WeekFrequency, TotalNumberOfSessions = @TotalNumberOfSessions, TotalPrice = @TotalPrice " +
                             "WHERE Id = @SubscriptionId AND IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SubscriptionId", subscription.Id);
                cmd.Parameters.AddWithValue("@Code", subscription.Code);
                cmd.Parameters.AddWithValue("@Description", subscription.Description);
                cmd.Parameters.AddWithValue("@NumberOfMonths", subscription.NumberOfMonths);
                cmd.Parameters.AddWithValue("@WeekFrequency", subscription.WeekFrequency);
                cmd.Parameters.AddWithValue("@TotalNumberOfSessions", subscription.TotalNumberOfSessions);
                cmd.Parameters.AddWithValue("@TotalPrice", subscription.TotalPrice);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while updating the subscription.", ex);
                }
            }
        }

        // Soft delete a subscription
        public void DeleteSubscription(int subscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Subscriptions SET IsDeleted = 1 WHERE Id = @SubscriptionId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SubscriptionId", subscriptionId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while deleting the subscription.", ex);
                }
            }
        }

        // Restore a soft-deleted subscription
        public void RestoreSubscription(int subscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE dbo.Subscriptions SET IsDeleted = 0 WHERE Id = @SubscriptionId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SubscriptionId", subscriptionId);

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while restoring the subscription.", ex);
                }
            }
        }
    }
}
