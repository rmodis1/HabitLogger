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
										Quantity DECIMAL(4,2)
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
			Console.WriteLine("\n\nMain Menu" +
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
					break;
     //           case "1":
					//GetAllRecords();
     //               break;
                case "2":
					Insert("coding");
                    break;
     //           case "3":
					//Delete();
     //               break;
     //           case "4":
					//Update();
     //               break;
				default:
                    Console.WriteLine("\nInvalid Command! Please type a number from 0 to 4!\n");
                    break;
            }
		}
	}

    internal static void Insert(string activity)
    {
		string date = GetDateInput();

		decimal quantity = GetNumberInput(activity);

		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$"INSERT INTO coding(date, quantity) VALUES('{date}', {quantity})";

			tableCmd.ExecuteNonQuery();

			connection.Close();
		}
    }

    internal static decimal GetNumberInput(string activity)
    {
		Console.WriteLine($"\nPlease insert number of hours that you spent on {activity}.\n");

		decimal hoursSpent;

		bool validFormat = Decimal.TryParse(Console.ReadLine(), out hoursSpent);

		while (!validFormat)
		{
			Console.WriteLine("Something went wrong. Please enter a numerical value!");
            validFormat = Decimal.TryParse(Console.ReadLine(), out hoursSpent);
        }

		return hoursSpent;
    }

    internal static string GetDateInput()
    {
		Console.WriteLine("\n Please insert the date: (Fromat: dd-mm-yy). Type 0 to return to main menu.\n");

		string dateInput = Console.ReadLine();

		if (dateInput == "0")
		{
			GetUserInput();
		}
		return dateInput;
    }
}