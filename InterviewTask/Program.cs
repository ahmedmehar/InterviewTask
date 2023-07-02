using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
public static class Extensions
{
    public static T Add<T>(this T arg1, T arg2)
    {
        try
        {
            dynamic a = arg1;
            dynamic b = arg2;
            return a + b;
        }
        catch (Exception)
        {
            throw new ArgumentException("Unsupported Type.");
        }
    }
}

public static class AppConstants
{
    public const string ConnectionString = "Server=DESKTOP-OJR9LJ3;Database=InterviewTask;Integrated Security=True";
}

public class DatabaseService
{
    public void StoreUserArgumentsInDB(string arg1, string arg2)
    {
        var connection = new SqlConnection(AppConstants.ConnectionString);
        connection.Open();

        var query = "INSERT INTO Arguments (Variable1, Variable2) VALUES (@arg1, @arg2)";
        var command = new SqlCommand(query, connection);
        command.Parameters.Add("@arg1", SqlDbType.NVarChar, 100).Value = arg1;
        command.Parameters.Add("@arg2", SqlDbType.NVarChar, 100).Value = arg2;

        var rowsAffected = command.ExecuteNonQuery();
        Console.WriteLine(rowsAffected > 0 ? "Arguments saved successfully!" : "Failed to save arguments.");
    }

    public List<string[]> RetrieveArguments()
    {
        var argumentsList = new List<string[]>();
        var connection = new SqlConnection(AppConstants.ConnectionString);
        connection.Open();

        var query = "SELECT Variable1, Variable2 FROM Arguments";
         var command = new SqlCommand(query, connection);
         var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var argument1 = reader.GetString(0);
            var argument2 = reader.GetString(1);
            argumentsList.Add(new[] { argument1, argument2 });
        }

        return argumentsList;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var (arg1, arg2) = GetUserInput();
        var concatenation = arg1.Add(arg2);
        Console.WriteLine($"String Concatenation: {concatenation}");

        // Initialize DatabaseService
        var databaseService = new DatabaseService();

        try
        {
            databaseService.StoreUserArgumentsInDB(arg1, arg2);
            Console.WriteLine("Press Enter to Retrieve the values from Database:");
            Console.ReadLine();

            var argumentsList = databaseService.RetrieveArguments();
            DisplayArguments(argumentsList);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"An error occurred while interacting with the database: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            Console.ReadLine();
        }
    }

    private static (string, string) GetUserInput()
    {
        Console.WriteLine("Please provide first arguments.");
        var arg1 = Console.ReadLine();

        Console.WriteLine("Please provide second arguments.");
        var arg2 = Console.ReadLine();

        return (arg1, arg2);
    }

    private static void DisplayArguments(List<string[]> argumentsList)
    {
        Console.WriteLine("Retrieved Arguments:\n");

        foreach (var arguments in argumentsList)
        {
            Console.WriteLine($"Variable1: {arguments[0]}");
            Console.WriteLine($"Variable2: {arguments[1]}\n");
        }
    }
}