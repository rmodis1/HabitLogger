using System.Globalization;
using Microsoft.Data.Sqlite;

internal class Program
{
    static string connectionString = @"Data Source=habit-Tracker.db";

    private static void Main(string[] args)
	{
		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();

			tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS coding (
										Id INTEGER PRIMARY KEY AUTOINCREMENT,
										Date TEXT,
										Minutes DECIMAL(4,2)
										)";

			tableCmd.ExecuteNonQuery();

			connection.Close();
		}

        GetUserInput();
    }

	public static void GetUserInput()
	{
		Console.Clear();
		bool closeApp = false;
		while (closeApp == false)
		{
			Console.WriteLine("\nMain Menu" +
				"\nWhat would you like to do?" +
				"\nType 0 to Close Application." +
				"\nType 1 to View All Records." +
				"\nType 2 to Insert Record." +
				"\nType 3 to Delete Record." +
				"\nType 4 to Update Record." +
				"\n_______________________\n");

			string commandInput = Console.ReadLine();

			switch (commandInput)
			{
				case "0":
					Console.WriteLine("\nGoodbye!\n");
					closeApp = true;
					Environment.Exit(0);
					break;
				case "1":
					GetAllRecords();
					break;
				case "2":
					Insert("coding");
                    break;
				case "3":
					Delete();
					break;
				case "4":
					Update();
					break;
				default:
                    Console.WriteLine("\nInvalid Command! Please type a number from 0 to 4!\n");
                    break;
            }
		}
	}

    private static void Update()
    {
		Console.Clear();
		GetAllRecords();

        Console.WriteLine("\nPlease type the Id of the record you want to update\n" +
            "or type 0 to go back to the Main Menu.\n");

        int recordId;
        var validFormat = Int32.TryParse(Console.ReadLine(), out recordId);
        while (!validFormat)
        {
            Console.WriteLine("That entry does not seem quite right. Please try again. Remember to pick an integer.\n");
            validFormat = Int32.TryParse(Console.ReadLine(), out recordId);
        }

        using (var connection = new SqliteConnection(connectionString))
        {
			connection.Open();

			var checkCmd = connection.CreateCommand();
			checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM coding WHERE Id = {recordId})";
			int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

			if (checkQuery == 0)
			{
				Console.WriteLine($"\n\nRecord with Id {recordId} does not exist.\n\n");
				connection.Close();
				Console.WriteLine("Press any key to continue.");
				Console.ReadLine();
				GetUserInput();
			}

			string date = GetDateInput();
			decimal minutes = GetNumberInput("coding");

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE coding set date = '{date}', minutes = {minutes} WHERE Id = {recordId}";
			tableCmd.ExecuteNonQuery();

			connection.Close();
        }
        Console.WriteLine($"\n\nRecord with Id {recordId} has been updated.\n\n");
		Console.WriteLine("Press any key to continue.");
        Console.ReadLine();
        GetUserInput();
    }

    private static void Delete()
    {
		Console.Clear();
		GetAllRecords();

		Console.WriteLine("\n\nPlease type the Id of the record you want to delete\n" +
            "or type 0 to go back to the Main Menu.\n\n");
		int recordId;
		var validFormat = Int32.TryParse(Console.ReadLine(), out recordId);

		while (!validFormat)
		{
			Console.WriteLine("That entry does not seem quite right. Please try again. Remember to pick an integer.\n");
            validFormat = Int32.TryParse(Console.ReadLine(), out recordId);
        }

		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText = $"DELETE FROM coding WHERE Id = '{recordId}'";

			int rowCount = tableCmd.ExecuteNonQuery();
			if (rowCount == 0)
			{
				Console.WriteLine($"\n\nRecord with Id {recordId} does not exist.\n\n");
				Console.WriteLine("Press any key to continue.");
				Console.ReadLine();
				GetUserInput();
			}
		}
		Console.WriteLine($"\n\nRecord with Id {recordId} has been deleted.\n\n");
		Console.WriteLine("Press any key to continue.");
		Console.ReadLine();
		GetUserInput();
    }

    private static void GetAllRecords()
    {
		Console.Clear();
		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$"SELECT * FROM coding ";

			List<CodingSession> tableData = new();

			SqliteDataReader reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
                    tableData.Add(
                    new CodingSession
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.ParseExact(reader.GetString(1),
                        "MM-dd-yy",
                        new CultureInfo("en-US")),
                        Minutes = reader.GetDecimal(2)
                    });
                }
			}
			else
			{
				Console.WriteLine("No rows found");
			}

			connection.Close();

			Console.WriteLine("-----------------------\n");

			foreach (var codingSession in tableData)
			{
				Console.WriteLine($"{codingSession.Id} - {codingSession.Date.ToString("MM-dd-yyyy")} - Minutes: {codingSession.Minutes}");
			}
			Console.WriteLine("-----------------------\n");
        }
    }

    internal static void Insert(string activity)
    {
		string date = GetDateInput();

		decimal minutes = GetNumberInput(activity);

		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$"INSERT INTO coding(date, minutes) VALUES('{date}', {minutes})";

			tableCmd.ExecuteNonQuery();

			connection.Close();
		}
    }

    internal static decimal GetNumberInput(string activity)
    {
		Console.WriteLine($"\nPlease insert number of minutes that you spent on {activity}.\n");

		decimal minutesSpent;

		bool validFormat = Decimal.TryParse(Console.ReadLine(), out minutesSpent);

		while (!validFormat || minutesSpent < 0)
		{
			Console.WriteLine("Something went wrong. Please enter a numerical value!");
            validFormat = Decimal.TryParse(Console.ReadLine(), out minutesSpent);
        }

		return minutesSpent;
    }

    internal static string GetDateInput()
    {
		Console.WriteLine("\nPlease insert the date: (Fromat: mm-dd-yy). Type 0 to return to main menu.\n");

		string dateInput = Console.ReadLine();

		if (dateInput == "0")
		{
			GetUserInput();
		}

		while (!DateTime.TryParseExact(dateInput, "MM-dd-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
		{
			Console.WriteLine("\nThat date format does not look quite right. Please try again. Remember, the format is mm-dd-yy.");
			dateInput = Console.ReadLine();
		}
		return dateInput;
    }
}