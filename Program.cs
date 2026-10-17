using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


class Program
{
    static void Main(string[] args)
    {
        var PaidNames = new List<string>();
        var MyDebts = LoadDebts();



        string[] inputs = { "add", "pay", "exit", "display" };
        while (true)
        {
            ActionList();
            Console.WriteLine("Please choose an action");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string Input = Console.ReadLine(); Console.ResetColor();

            string Person = "";
            decimal Amount = 0;
            Console.WriteLine();

            //action validation
            if (!inputs.Contains(Input.ToLower()))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Input, try again");
                Console.ResetColor();
                continue;
            }
            else // if action is true
            {
                if (Input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                else if (Input.Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    Addition(ref MyDebts);
                }

                else if (Input.Equals("display", StringComparison.OrdinalIgnoreCase))
                {
                    Display(MyDebts);
                }
                else
                {
                    Pay(MyDebts, PaidNames);
                }
            }
            Console.WriteLine();
        }


    }

    public static void Addition(ref Dictionary<string, decimal> debts)
    {
        while (true)
        {
            string Person = ReadName("Enter a name: ");

            Console.Write("How much you owe them: ");
            decimal Amount = ReadAmount();

            if (debts.ContainsKey(Person))
                debts[Person] += Amount;
            else
                debts.Add(Person, Amount);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Added!");
            Console.ResetColor();
            SaveDebts(debts);

            Console.Write("Do you want to add another debt? (yes/no): ");
            string answer = Console.ReadLine()?.Trim() ?? "";

            if (!answer.Equals("yes", StringComparison.OrdinalIgnoreCase))
                break;
        }
    }


    public static void Pay(Dictionary<string, decimal> debts, List<string> Paid)
    {
        while (true)
        {
            if (debts.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You owe no one");
                Console.ResetColor();
                break;
            }

            string Person = ReadName("Enter a name: ");

            if (!debts.TryGetValue(Person, out decimal Balance))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No such person! Try again");
                Console.ResetColor();
                continue;
            }

            Console.Write($"You owe {Person} {Balance} pounds. How much will you pay? ");
            decimal Amount = ReadAmount();

            if (Amount > Balance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Payment cannot exceed your debt.");
                Console.ResetColor();
                continue;
            }

            decimal Remaining = Balance - Amount;

            if (Remaining == 0)
            {
                Paid.Add(Person);
                debts.Remove(Person);
            }
            else
            {
                debts[Person] = Remaining;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Paid!");
            Console.ResetColor();
            SaveDebts(debts);

            break;
        }

        Console.WriteLine();
    }

    public static void Display(Dictionary<string, decimal> debts)
    {
        if (debts.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You owe no one");
            Console.ResetColor();
            return;
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        decimal total = 0;
        foreach (var debt in debts)
        {
            Console.WriteLine($"{debt.Key}: {debt.Value} pounds");
            total += debt.Value;
        }
        Console.WriteLine();
        Console.WriteLine($"Total debt: {total} pounds");
        Console.ResetColor();
    }

    public static string ReadName(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.Yellow;
            string name = Console.ReadLine()?.Trim()??""; Console.ResetColor();
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("Name cannot be empty! Try again."); Console.ResetColor();
        }
    }

    public static decimal ReadAmount()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string input = Console.ReadLine() ?? "";
            Console.ResetColor();
            if (decimal.TryParse(input, out decimal amount)
                && amount > 0)
            {
                return amount;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Enter a valid positive amount: ");
            Console.ResetColor();
        }
    }

    public static void ActionList()
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Actions:");
        Console.WriteLine(
            "1. \"Add\" to add a debts\n" +
            "2. \"pay\" to payback a debts\n" +
            "3. \"Display\" to show your debts list\n" +
            "4. \"Exit\" to close program\n");
        Console.WriteLine("--------------------------------");
    }

    static string FilePath = "debt.json";

    public static Dictionary<string, decimal> LoadDebts()
    {
        var alt = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(FilePath))
        {
            return alt;
        }

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Dictionary<string, decimal>>(json) ??
                alt;
        }
        catch
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Could not read the debt file. Starting empty.");
            Console.ResetColor();
            return alt;
        }
    }
    public static void SaveDebts( Dictionary<string, decimal> debts)
    {
        string json = JsonSerializer.Serialize(
            debts,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(FilePath, json);
    }

}