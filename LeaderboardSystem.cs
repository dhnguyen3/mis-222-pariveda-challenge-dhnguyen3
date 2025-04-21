// LeaderboardSystem.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class LeaderboardSystem
{
    public static void ShowLeaderboard()
    {
        Console.Clear();
        Console.WriteLine("=== Global Leaderboard ===\n");
        Console.WriteLine("1. Top 10 Scores Overall");
        Console.WriteLine("2. Top Scores by Song");
        Console.WriteLine("3. Reset/Delete Save File");
        Console.WriteLine("4. Return to Menu");
        Console.Write("Select an option: ");

        string input = Console.ReadLine();
        if (input == "1") ShowTopOverall();
        else if (input == "2") ShowTopBySong();
        else if (input == "3") ResetSaveFile();
        else return;

        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
        ShowLeaderboard();
    }

    private static void ShowTopOverall()
    {
        Console.Clear();
        Console.WriteLine("=== Top 10 Scores (All Songs) ===\n");
        string saveFolder = "Saves";
        var scores = new List<(string Email, string Song, int Score)>();

        if (Directory.Exists(saveFolder))
        {
            foreach (string file in Directory.GetFiles(saveFolder, "*.txt"))
            {
                string email = Path.GetFileNameWithoutExtension(file).Replace("_at_", "@").Replace("_", ".");
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines.Skip(1))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4 && int.TryParse(parts[2], out int score))
                    {
                        scores.Add((email, parts[1], score));
                    }
                }
            }

            foreach (var (email, song, score) in scores.OrderByDescending(s => s.Score).Take(10))
            {
                Console.WriteLine($"{email} - {song} - {score} pts");
            }
        }
        else
        {
            Console.WriteLine("No save data found.");
        }
    }

    private static void ShowTopBySong()
    {
        Console.Clear();
        Console.WriteLine("=== Top Score Per Song ===\n");
        string saveFolder = "Saves";
        var topScores = new Dictionary<string, (string Email, int Score)>();

        if (Directory.Exists(saveFolder))
        {
            foreach (string file in Directory.GetFiles(saveFolder, "*.txt"))
            {
                string email = Path.GetFileNameWithoutExtension(file).Replace("_at_", "@").Replace("_", ".");
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines.Skip(1))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4 && int.TryParse(parts[2], out int score))
                    {
                        string song = parts[1];
                        if (!topScores.ContainsKey(song) || score > topScores[song].Score)
                        {
                            topScores[song] = (email, score);
                        }
                    }
                }
            }

            foreach (var entry in topScores.OrderByDescending(e => e.Value.Score))
            {
                Console.WriteLine($"{entry.Key}: {entry.Value.Email} - {entry.Value.Score} pts");
            }
        }
        else
        {
            Console.WriteLine("No save data found.");
        }
    }

    public static void ResetSaveFile()
    {
        Console.Clear();
        Console.Write("Enter the email associated with the file to delete: ");
        string email = Console.ReadLine();
        string fileName = email.Replace("@", "_at_").Replace(".", "_") + ".txt";
        string filePath = Path.Combine("Saves", fileName);

        if (File.Exists(filePath))
        {
            Console.Write("Are you sure you want to delete this save file? (y/n): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                File.Delete(filePath);
                Console.WriteLine("\nFile deleted successfully.");
            }
            else
            {
                Console.WriteLine("\nDeletion canceled.");
            }
        }
        else
        {
            Console.WriteLine("\nNo save file found for that email.");
        }
    }
}
