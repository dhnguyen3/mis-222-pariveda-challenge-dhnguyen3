using System;
using System.IO;

public static class SaveSystem
{
    private static readonly string saveFolder = "Saves";

    public static void SaveGame(Player player, string song, int score, int combo)
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string fileName = $"{player.Email.Replace("@", "_at_").Replace(".", "_")}.txt";
        string filePath = Path.Combine(saveFolder, fileName);

        // If the file doesn't exist, write the header first
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Date|Song|Score|MaxCombo\n");
        }

        // Append the player's session
        string sessionLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{song}|{score}|{combo}";
        File.AppendAllText(filePath, sessionLine + "\n");

        Console.WriteLine($"\nGame saved to {filePath}.");
    }

    public static void LoadGame()
    {
        Console.Clear();
        Console.Write("Enter your email to load saved progress: ");
        string email = Console.ReadLine();
        string fileName = $"{email.Replace("@", "_at_").Replace(".", "_")}.txt";
        string filePath = Path.Combine(saveFolder, fileName);

        if (File.Exists(filePath))
        {
            Console.WriteLine($"\n=== Saved Progress for {email} ===\n");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 4 && parts[0] != "Date")
                {
                    Console.WriteLine($"[{parts[0]}] 🎵 {parts[1]} - Score: {parts[2]}, Combo: {parts[3]}");
                }
                else if (parts[0] == "Date")
                {
                    Console.WriteLine("Date\t\t\tSong\tScore\tCombo");
                    Console.WriteLine("--------------------------------------------------");
                }
            }
        }
        else
        {
            Console.WriteLine("No saved data found for that email.");
        }

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }
}
