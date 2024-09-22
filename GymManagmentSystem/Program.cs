using System;
using GymManagmentSystem.Features;
using GymManagmentSystem.Models;

namespace GymManagmentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=LAPTOP-POTNIV06;Initial Catalog=GymManagmentSystem;Integrated Security=True";

            MembersCRUD membersCRUD = new MembersCRUD(connectionString);
            SubscriptionsCRUD subscriptionsCRUD = new SubscriptionsCRUD(connectionString);
            MemberSubscriptionsCRUD memberSubscriptionsCRUD = new MemberSubscriptionsCRUD(connectionString);
            UsersCRUD usersCRUD = new UsersCRUD(connectionString);
            DiscountsCRUD discountsCRUD = new DiscountsCRUD(connectionString);

            // Login process
            User loggedInUser = Login(usersCRUD);

            if (loggedInUser == null)
            {
                Console.WriteLine("Login failed. Exiting application.");
                return;
            }

            Console.WriteLine($"Welcome, {loggedInUser.FirstName} ({loggedInUser.Role})!");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("1. Manage Members");
                Console.WriteLine("2. Manage Subscriptions");
                Console.WriteLine("3. Manage Member Subscriptions");

                if (loggedInUser.Role == "Admin")
                {
                    Console.WriteLine("4. Manage Users");
                }

                Console.WriteLine("5. Manage Discounts");
                Console.WriteLine("0. Logout");
                ConsoleKeyInfo userInputKey = Console.ReadKey();

                switch (userInputKey.KeyChar)
                {
                    case '1':
                        ManageMembers(membersCRUD);
                        break;
                    case '2':
                        ManageSubscriptions(subscriptionsCRUD);
                        break;
                    case '3':
                        ManageMemberSubscriptions(memberSubscriptionsCRUD);
                        break;
                    case '4':
                        if (loggedInUser.Role == "Admin")
                        {
                            ManageUsers(usersCRUD);
                        }
                        else
                        {
                            Console.WriteLine("\nYou do not have permission to manage users.");
                            Console.ReadKey();
                        }
                        break;
                    case '5':
                        ManageDiscounts(discountsCRUD);
                        break;
                    case '0':
                        Console.WriteLine("\nLogging out...");
                        loggedInUser = Login(usersCRUD);
                        if (loggedInUser == null)
                        {
                            Console.WriteLine("Login failed. Exiting application.");
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static User Login(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Login to Gym Management System");

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = ReadPassword(); 

            return usersCRUD.VerifyLogin(username, password);
        }

        // Securely read password
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    Console.Write("*");
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }

        static void ManageMembers(MembersCRUD membersCRUD)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Member Management");
                Console.WriteLine("1. Create Member");
                Console.WriteLine("2. View All Members");
                Console.WriteLine("3. Search Members");
                Console.WriteLine("4. Update Member");
                Console.WriteLine("5. Delete Member");
                Console.WriteLine("6. Restore Member");
                Console.WriteLine("0. Go Back");
                ConsoleKeyInfo inputKey = Console.ReadKey();

                switch (inputKey.KeyChar)
                {
                    case '1':
                        CreateMember(membersCRUD);
                        break;
                    case '2':
                        ViewAllMembers(membersCRUD);
                        break;
                    case '3':
                        SearchMembers(membersCRUD);
                        break;
                    case '4':
                        UpdateMember(membersCRUD);
                        break;
                    case '5':
                        DeleteMember(membersCRUD);
                        break;
                    case '6':
                        RestoreMember(membersCRUD);
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateMember(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Create New Member");

            Console.WriteLine("First Name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Last Name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Birthday (yyyy-MM-dd):");
            DateTime birthday;
            while (!DateTime.TryParse(Console.ReadLine(), out birthday))
            {
                Console.WriteLine("Invalid date format. Please enter again:");
            }

            Console.WriteLine("ID Card Number:");
            string idCardNumber = Console.ReadLine();

            Console.WriteLine("Email:");
            string email = Console.ReadLine();

            Member newMember = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday,
                IdCardNumber = idCardNumber,
                Email = email,
                RegistrationDate = DateTime.Now
            };

            try
            {
                membersCRUD.CreateMember(newMember);
                Console.WriteLine("Member created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllMembers(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("All Members:");
            var members = membersCRUD.GetAllMembers();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found.");
            }
            else
            {
                foreach (var member in members)
                {
                    Console.WriteLine($"ID: {member.Id}, Name: {member.FirstName} {member.LastName}, Email: {member.Email}, " +
                                      $"Birthday: {member.Birthday:yyyy-MM-dd}, ID Card Number: {member.IdCardNumber}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void SearchMembers(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Search Members");

            Console.WriteLine("First Name (leave blank if any):");
            string firstName = Console.ReadLine();

            Console.WriteLine("Last Name (leave blank if any):");
            string lastName = Console.ReadLine();

            Console.WriteLine("ID Card Number (leave blank if any):");
            string idCardNumber = Console.ReadLine();

            Console.WriteLine("Email (leave blank if any):");
            string email = Console.ReadLine();

            var members = membersCRUD.GetMembersByCriteria(firstName, lastName, idCardNumber, email);
            if (members.Count == 0)
            {
                Console.WriteLine("No members found matching the criteria.");
            }
            else
            {
                foreach (var member in members)
                {
                    Console.WriteLine($"ID: {member.Id}, Name: {member.FirstName} {member.LastName}, Email: {member.Email}, " +
                                      $"Birthday: {member.Birthday:yyyy-MM-dd}, ID Card Number: {member.IdCardNumber}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateMember(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Update Member");

            Console.WriteLine("Enter Member ID to update:");
            if (int.TryParse(Console.ReadLine(), out int memberId))
            {
                var member = membersCRUD.GetMemberById(memberId);
                if (member != null)
                {
                    Console.WriteLine($"Updating Member: {member.FirstName} {member.LastName}");

                    Console.WriteLine("New First Name (leave blank to keep current):");
                    string firstName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(firstName)) member.FirstName = firstName;

                    Console.WriteLine("New Last Name (leave blank to keep current):");
                    string lastName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(lastName)) member.LastName = lastName;

                    Console.WriteLine("New Birthday (leave blank to keep current, format yyyy-MM-dd):");
                    string birthdayInput = Console.ReadLine();
                    if (DateTime.TryParse(birthdayInput, out DateTime birthday)) member.Birthday = birthday;

                    Console.WriteLine("New ID Card Number (leave blank to keep current):");
                    string idCardNumber = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(idCardNumber)) member.IdCardNumber = idCardNumber;

                    Console.WriteLine("New Email (leave blank to keep current):");
                    string email = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(email)) member.Email = email;

                    try
                    {
                        membersCRUD.UpdateMember(member);
                        Console.WriteLine("Member updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Member not found or deleted.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteMember(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Delete Member");

            Console.WriteLine("Enter Member ID to delete:");
            if (int.TryParse(Console.ReadLine(), out int memberId))
            {
                try
                {
                    membersCRUD.DeleteMember(memberId);
                    Console.WriteLine("Member deleted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RestoreMember(MembersCRUD membersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Restore Member");

            Console.WriteLine("Enter Member ID to restore:");
            if (int.TryParse(Console.ReadLine(), out int memberId))
            {
                try
                {
                    membersCRUD.RestoreMember(memberId);
                    Console.WriteLine("Member restored successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ManageSubscriptions(SubscriptionsCRUD subscriptionsCRUD)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Subscription Management");
                Console.WriteLine("1. Create Subscription");
                Console.WriteLine("2. View All Subscriptions");
                Console.WriteLine("3. Update Subscription");
                Console.WriteLine("4. Delete Subscription");
                Console.WriteLine("5. Restore Subscription");
                Console.WriteLine("0. Go Back");
                ConsoleKeyInfo inputKey = Console.ReadKey();

                switch (inputKey.KeyChar)
                {
                    case '1':
                        CreateSubscription(subscriptionsCRUD);
                        break;
                    case '2':
                        ViewAllSubscriptions(subscriptionsCRUD);
                        break;
                    case '3':
                        UpdateSubscription(subscriptionsCRUD);
                        break;
                    case '4':
                        DeleteSubscription(subscriptionsCRUD);
                        break;
                    case '5':
                        RestoreSubscription(subscriptionsCRUD);
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateSubscription(SubscriptionsCRUD subscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Create New Subscription");

            Console.WriteLine("Code:");
            string code = Console.ReadLine();

            Console.WriteLine("Description:");
            string description = Console.ReadLine();

            Console.WriteLine("Number of Months:");
            int numberOfMonths;
            while (!int.TryParse(Console.ReadLine(), out numberOfMonths) || numberOfMonths <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the number of months:");
            }

            Console.WriteLine("Week Frequency (e.g., '2 days/week'):");
            string weekFrequency = Console.ReadLine();

            Console.WriteLine("Total Number of Sessions:");
            int totalSessions;
            while (!int.TryParse(Console.ReadLine(), out totalSessions) || totalSessions <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the total number of sessions:");
            }

            Console.WriteLine("Total Price:");
            decimal totalPrice;
            while (!decimal.TryParse(Console.ReadLine(), out totalPrice) || totalPrice <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the total price:");
            }

            Subscription newSubscription = new Subscription
            {
                Code = code,
                Description = description,
                NumberOfMonths = numberOfMonths,
                WeekFrequency = weekFrequency,
                TotalNumberOfSessions = totalSessions,
                TotalPrice = totalPrice
            };

            try
            {
                subscriptionsCRUD.CreateSubscription(newSubscription);
                Console.WriteLine("Subscription created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllSubscriptions(SubscriptionsCRUD subscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("All Subscriptions:");
            var subscriptions = subscriptionsCRUD.GetAllSubscriptions();
            if (subscriptions.Count == 0)
            {
                Console.WriteLine("No subscriptions found.");
            }
            else
            {
                foreach (var subscription in subscriptions)
                {
                    Console.WriteLine($"ID: {subscription.Id}, Code: {subscription.Code}, Description: {subscription.Description}, " +
                                      $"Number of Months: {subscription.NumberOfMonths}, Week Frequency: {subscription.WeekFrequency}, " +
                                      $"Total Sessions: {subscription.TotalNumberOfSessions}, Total Price: {subscription.TotalPrice}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateSubscription(SubscriptionsCRUD subscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Update Subscription");

            Console.WriteLine("Enter Subscription ID to update:");
            if (int.TryParse(Console.ReadLine(), out int subscriptionId))
            {
                var subscription = subscriptionsCRUD.GetSubscriptionById(subscriptionId);
                if (subscription != null)
                {
                    Console.WriteLine($"Updating Subscription: {subscription.Code}");

                    Console.WriteLine("New Code (leave blank to keep current):");
                    string code = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(code)) subscription.Code = code;

                    Console.WriteLine("New Description (leave blank to keep current):");
                    string description = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(description)) subscription.Description = description;

                    Console.WriteLine("New Number of Months (leave blank to keep current):");
                    string monthsInput = Console.ReadLine();
                    if (int.TryParse(monthsInput, out int numberOfMonths)) subscription.NumberOfMonths = numberOfMonths;

                    Console.WriteLine("New Week Frequency (leave blank to keep current):");
                    string weekFrequency = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(weekFrequency)) subscription.WeekFrequency = weekFrequency;

                    Console.WriteLine("New Total Number of Sessions (leave blank to keep current):");
                    string sessionsInput = Console.ReadLine();
                    if (int.TryParse(sessionsInput, out int totalSessions)) subscription.TotalNumberOfSessions = totalSessions;

                    Console.WriteLine("New Total Price (leave blank to keep current):");
                    string priceInput = Console.ReadLine();
                    if (decimal.TryParse(priceInput, out decimal totalPrice)) subscription.TotalPrice = totalPrice;

                    try
                    {
                        subscriptionsCRUD.UpdateSubscription(subscription);
                        Console.WriteLine("Subscription updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Subscription not found or deleted.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteSubscription(SubscriptionsCRUD subscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Delete Subscription");

            Console.WriteLine("Enter Subscription ID to delete:");
            if (int.TryParse(Console.ReadLine(), out int subscriptionId))
            {
                try
                {
                    subscriptionsCRUD.DeleteSubscription(subscriptionId);
                    Console.WriteLine("Subscription deleted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RestoreSubscription(SubscriptionsCRUD subscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Restore Subscription");

            Console.WriteLine("Enter Subscription ID to restore:");
            if (int.TryParse(Console.ReadLine(), out int subscriptionId))
            {
                try
                {
                    subscriptionsCRUD.RestoreSubscription(subscriptionId);
                    Console.WriteLine("Subscription restored successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ManageMemberSubscriptions(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Member Subscription Management");
                Console.WriteLine("1. Create Member Subscription");
                Console.WriteLine("2. View All Member Subscriptions");
                Console.WriteLine("3. Update Member Subscription");
                Console.WriteLine("4. Delete Member Subscription");
                Console.WriteLine("5. Restore Member Subscription");
                Console.WriteLine("0. Go Back");
                ConsoleKeyInfo inputKey = Console.ReadKey();

                switch (inputKey.KeyChar)
                {
                    case '1':
                        CreateMemberSubscription(memberSubscriptionsCRUD);
                        break;
                    case '2':
                        ViewAllMemberSubscriptions(memberSubscriptionsCRUD);
                        break;
                    case '3':
                        UpdateMemberSubscription(memberSubscriptionsCRUD);
                        break;
                    case '4':
                        DeleteMemberSubscription(memberSubscriptionsCRUD);
                        break;
                    case '5':
                        RestoreMemberSubscription(memberSubscriptionsCRUD);
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateMemberSubscription(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Create New Member Subscription");

            Console.WriteLine("Member ID:");
            int memberId;
            while (!int.TryParse(Console.ReadLine(), out memberId) || memberId <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid Member ID:");
            }

            Console.WriteLine("Subscription ID:");
            int subscriptionId;
            while (!int.TryParse(Console.ReadLine(), out subscriptionId) || subscriptionId <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid Subscription ID:");
            }

            Console.WriteLine("Original Price:");
            decimal originalPrice;
            while (!decimal.TryParse(Console.ReadLine(), out originalPrice) || originalPrice <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the original price:");
            }

            Console.WriteLine("Discount Value:");
            decimal discountValue;
            while (!decimal.TryParse(Console.ReadLine(), out discountValue) || discountValue < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number for the discount value:");
            }

            decimal paidPrice = originalPrice - discountValue;

            Console.WriteLine("Start Date (yyyy-MM-dd):");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.WriteLine("Invalid date format. Please enter the start date in yyyy-MM-dd format:");
            }

            Console.WriteLine("End Date (yyyy-MM-dd):");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate <= startDate)
            {
                Console.WriteLine("Invalid date format. End date must be after start date. Please enter again:");
            }

            Console.WriteLine("Remaining Sessions:");
            int remainingSessions;
            while (!int.TryParse(Console.ReadLine(), out remainingSessions) || remainingSessions <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the remaining sessions:");
            }

            MemberSubscription newMemberSubscription = new MemberSubscription
            {
                MemberId = memberId,
                SubscriptionId = subscriptionId,
                OriginalPrice = originalPrice,
                DiscountValue = discountValue,
                PaidPrice = paidPrice,
                StartDate = startDate,
                EndDate = endDate,
                RemainingSessions = remainingSessions
            };

            try
            {
                memberSubscriptionsCRUD.CreateMemberSubscription(newMemberSubscription);
                Console.WriteLine("Member subscription created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllMemberSubscriptions(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("All Member Subscriptions:");
            var memberSubscriptions = memberSubscriptionsCRUD.GetAllMemberSubscriptions();
            if (memberSubscriptions.Count == 0)
            {
                Console.WriteLine("No member subscriptions found.");
            }
            else
            {
                foreach (var memberSubscription in memberSubscriptions)
                {
                    Console.WriteLine($"ID: {memberSubscription.Id}, Member ID: {memberSubscription.MemberId}, Subscription ID: {memberSubscription.SubscriptionId}, " +
                                      $"Original Price: {memberSubscription.OriginalPrice}, Discount: {memberSubscription.DiscountValue}, Paid Price: {memberSubscription.PaidPrice}, " +
                                      $"Start Date: {memberSubscription.StartDate:yyyy-MM-dd}, End Date: {memberSubscription.EndDate:yyyy-MM-dd}, Remaining Sessions: {memberSubscription.RemainingSessions}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateMemberSubscription(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Update Member Subscription");

            Console.WriteLine("Enter Member Subscription ID to update:");
            if (int.TryParse(Console.ReadLine(), out int memberSubscriptionId))
            {
                var memberSubscription = memberSubscriptionsCRUD.GetMemberSubscriptionById(memberSubscriptionId);
                if (memberSubscription != null)
                {
                    Console.WriteLine($"Updating Member Subscription: {memberSubscription.Id}");

                    Console.WriteLine("New Member ID (leave blank to keep current):");
                    string memberIdInput = Console.ReadLine();
                    if (int.TryParse(memberIdInput, out int memberId)) memberSubscription.MemberId = memberId;

                    Console.WriteLine("New Subscription ID (leave blank to keep current):");
                    string subscriptionIdInput = Console.ReadLine();
                    if (int.TryParse(subscriptionIdInput, out int subscriptionId)) memberSubscription.SubscriptionId = subscriptionId;

                    Console.WriteLine("New Original Price (leave blank to keep current):");
                    string originalPriceInput = Console.ReadLine();
                    if (decimal.TryParse(originalPriceInput, out decimal originalPrice)) memberSubscription.OriginalPrice = originalPrice;

                    Console.WriteLine("New Discount Value (leave blank to keep current):");
                    string discountValueInput = Console.ReadLine();
                    if (decimal.TryParse(discountValueInput, out decimal discountValue)) memberSubscription.DiscountValue = discountValue;

                    memberSubscription.PaidPrice = memberSubscription.OriginalPrice - memberSubscription.DiscountValue;

                    Console.WriteLine("New Start Date (leave blank to keep current, format yyyy-MM-dd):");
                    string startDateInput = Console.ReadLine();
                    if (DateTime.TryParse(startDateInput, out DateTime startDate)) memberSubscription.StartDate = startDate;

                    Console.WriteLine("New End Date (leave blank to keep current, format yyyy-MM-dd):");
                    string endDateInput = Console.ReadLine();
                    if (DateTime.TryParse(endDateInput, out DateTime endDate)) memberSubscription.EndDate = endDate;

                    Console.WriteLine("New Remaining Sessions (leave blank to keep current):");
                    string remainingSessionsInput = Console.ReadLine();
                    if (int.TryParse(remainingSessionsInput, out int remainingSessions)) memberSubscription.RemainingSessions = remainingSessions;

                    try
                    {
                        memberSubscriptionsCRUD.UpdateMemberSubscription(memberSubscription);
                        Console.WriteLine("Member subscription updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Member subscription not found or deleted.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteMemberSubscription(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Delete Member Subscription");

            Console.WriteLine("Enter Member Subscription ID to delete:");
            if (int.TryParse(Console.ReadLine(), out int memberSubscriptionId))
            {
                try
                {
                    memberSubscriptionsCRUD.DeleteMemberSubscription(memberSubscriptionId);
                    Console.WriteLine("Member subscription deleted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RestoreMemberSubscription(MemberSubscriptionsCRUD memberSubscriptionsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Restore Member Subscription");

            Console.WriteLine("Enter Member Subscription ID to restore:");
            if (int.TryParse(Console.ReadLine(), out int memberSubscriptionId))
            {
                try
                {
                    memberSubscriptionsCRUD.RestoreMemberSubscription(memberSubscriptionId);
                    Console.WriteLine("Member subscription restored successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ManageUsers(UsersCRUD usersCRUD)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("User Management");
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. View All Users");
                Console.WriteLine("3. Update User Role");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Restore User");
                Console.WriteLine("0. Go Back");
                ConsoleKeyInfo inputKey = Console.ReadKey();

                switch (inputKey.KeyChar)
                {
                    case '1':
                        CreateUser(usersCRUD);
                        break;
                    case '2':
                        ViewAllUsers(usersCRUD);
                        break;
                    case '3':
                        UpdateUserRole(usersCRUD);
                        break;
                    case '4':
                        DeleteUser(usersCRUD);
                        break;
                    case '5':
                        RestoreUser(usersCRUD);
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateUser(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Create New User");

            Console.WriteLine("Username:");
            string username = Console.ReadLine();

            Console.WriteLine("Password:");
            string password = ReadPassword();
            string passwordHash = UsersCRUD.HashPassword(password);

            Console.WriteLine("First Name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Last Name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Email:");
            string email = Console.ReadLine();

            Console.WriteLine("Role (Admin/Receptionist):");
            string role = Console.ReadLine();

            if (role != "Admin" && role != "Receptionist")
            {
                Console.WriteLine("Invalid role. Only 'Admin' and 'Receptionist' are allowed.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            User newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role
            };

            try
            {
                usersCRUD.CreateUser(newUser);
                Console.WriteLine("User created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllUsers(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("All Users:");
            var users = usersCRUD.GetAllUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("No users found.");
            }
            else
            {
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Name: {user.FirstName} {user.LastName}, Email: {user.Email}, Role: {user.Role}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateUserRole(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Update User Role");

            Console.WriteLine("Enter User ID to update role:");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Enter New Role (Admin/Receptionist):");
                string newRole = Console.ReadLine();

                if (newRole != "Admin" && newRole != "Receptionist")
                {
                    Console.WriteLine("Invalid role. Only 'Admin' and 'Receptionist' are allowed.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }

                try
                {
                    usersCRUD.UpdateUserRole(userId, newRole);
                    Console.WriteLine("User role updated successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteUser(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Delete User");

            Console.WriteLine("Enter User ID to delete:");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                try
                {
                    usersCRUD.DeleteUser(userId);
                    Console.WriteLine("User deleted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RestoreUser(UsersCRUD usersCRUD)
        {
            Console.Clear();
            Console.WriteLine("Restore User");

            Console.WriteLine("Enter User ID to restore:");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                try
                {
                    usersCRUD.RestoreUser(userId);
                    Console.WriteLine("User restored successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ManageDiscounts(DiscountsCRUD discountsCRUD)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Discount Management");
                Console.WriteLine("1. Create Discount");
                Console.WriteLine("2. View All Discounts");
                Console.WriteLine("3. Update Discount");
                Console.WriteLine("4. Delete Discount");
                Console.WriteLine("5. Restore Discount");
                Console.WriteLine("0. Go Back");
                ConsoleKeyInfo inputKey = Console.ReadKey();

                switch (inputKey.KeyChar)
                {
                    case '1':
                        CreateDiscount(discountsCRUD);
                        break;
                    case '2':
                        ViewAllDiscounts(discountsCRUD);
                        break;
                    case '3':
                        UpdateDiscount(discountsCRUD);
                        break;
                    case '4':
                        DeleteDiscount(discountsCRUD);
                        break;
                    case '5':
                        RestoreDiscount(discountsCRUD);
                        break;
                    case '0':
                        return;
                    default:
                        Console.WriteLine("\nInvalid selection. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateDiscount(DiscountsCRUD discountsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Create New Discount");

            Console.WriteLine("Code:");
            string code = Console.ReadLine();

            Console.WriteLine("Value:");
            decimal value;
            while (!decimal.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for the value:");
            }

            Console.WriteLine("Start Date (yyyy-MM-dd):");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.WriteLine("Invalid date format. Please enter the start date in yyyy-MM-dd format:");
            }

            Console.WriteLine("End Date (yyyy-MM-dd):");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate <= startDate)
            {
                Console.WriteLine("Invalid date format. End date must be after start date. Please enter again:");
            }

            Discount newDiscount = new Discount
            {
                Code = code,
                Value = value,
                StartDate = startDate,
                EndDate = endDate
            };

            try
            {
                discountsCRUD.CreateDiscount(newDiscount);
                Console.WriteLine("Discount created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllDiscounts(DiscountsCRUD discountsCRUD)
        {
            Console.Clear();
            Console.WriteLine("All Discounts:");
            var discounts = discountsCRUD.GetAllDiscounts();
            if (discounts.Count == 0)
            {
                Console.WriteLine("No discounts found.");
            }
            else
            {
                foreach (var discount in discounts)
                {
                    Console.WriteLine($"ID: {discount.Id}, Code: {discount.Code}, Value: {discount.Value}, " +
                                      $"Start Date: {discount.StartDate:yyyy-MM-dd}, End Date: {discount.EndDate:yyyy-MM-dd}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateDiscount(DiscountsCRUD discountsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Update Discount");

            Console.WriteLine("Enter Discount ID to update:");
            if (int.TryParse(Console.ReadLine(), out int discountId))
            {
                var discount = discountsCRUD.GetDiscountById(discountId);
                if (discount != null)
                {
                    Console.WriteLine($"Updating Discount: {discount.Code}");

                    Console.WriteLine("New Code (leave blank to keep current):");
                    string code = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(code)) discount.Code = code;

                    Console.WriteLine("New Value (leave blank to keep current):");
                    string valueInput = Console.ReadLine();
                    if (decimal.TryParse(valueInput, out decimal value)) discount.Value = value;

                    Console.WriteLine("New Start Date (leave blank to keep current, format yyyy-MM-dd):");
                    string startDateInput = Console.ReadLine();
                    if (DateTime.TryParse(startDateInput, out DateTime startDate)) discount.StartDate = startDate;

                    Console.WriteLine("New End Date (leave blank to keep current, format yyyy-MM-dd):");
                    string endDateInput = Console.ReadLine();
                    if (DateTime.TryParse(endDateInput, out DateTime endDate)) discount.EndDate = endDate;

                    try
                    {
                        discountsCRUD.UpdateDiscount(discount);
                        Console.WriteLine("Discount updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Discount not found or deleted.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteDiscount(DiscountsCRUD discountsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Delete Discount");

            Console.WriteLine("Enter Discount ID to delete:");
            if (int.TryParse(Console.ReadLine(), out int discountId))
            {
                try
                {
                    discountsCRUD.DeleteDiscount(discountId);
                    Console.WriteLine("Discount deleted successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RestoreDiscount(DiscountsCRUD discountsCRUD)
        {
            Console.Clear();
            Console.WriteLine("Restore Discount");

            Console.WriteLine("Enter Discount ID to restore:");
            if (int.TryParse(Console.ReadLine(), out int discountId))
            {
                try
                {
                    discountsCRUD.RestoreDiscount(discountId);
                    Console.WriteLine("Discount restored successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
