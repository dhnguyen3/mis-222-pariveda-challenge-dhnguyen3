using System;
using System.Media;
using System.Threading;

public static class Menu
{
    private static int selectedIndex = 0;
    private static string[] options = {
        "New Game",
        "Two Player Mode",
        "Load Game",
        "View Leaderboard",
        "Reset/Erase Save Data",
        "Exit"
    };

    private static bool showDoubleArrow = false;
    private static Thread pulseThread;
    private static bool running = true;

    public static void ShowMainMenu()
    {
        PlayBackgroundMusic();
        StartPulseEffect();

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.WriteLine("=== K-Pop Dance Revolution ===\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string arrow = showDoubleArrow ? ">>" : ">";
                    Console.WriteLine($"{arrow} {options[i]} <");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {options[i]}");
                }
            }

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    StopPulseEffect();
                    Console.Clear();
                    HandleSelection();
                    PlayBackgroundMusic();
                    StartPulseEffect();
                    break;
            }
        } while (key != ConsoleKey.Escape);

        StopPulseEffect();
        Environment.Exit(0);
    }

    private static void HandleSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                Game.StartNewGame();
                break;
            case 1:
                Game.StartTwoPlayerGame();
                break;
            case 2:
                SaveSystem.LoadGame();
                break;
            case 3:
                LeaderboardSystem.ShowLeaderboard();
                break;
            case 4:
                LeaderboardSystem.ResetSaveFile();
                break;
            case 5:
                StopPulseEffect();
                Environment.Exit(0);
                break;
        }

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }

    private static void StartPulseEffect()
    {
        running = true;
        pulseThread = new Thread(() =>
        {
            while (running)
            {
                showDoubleArrow = !showDoubleArrow;
                Thread.Sleep(700);
            }
        });
        pulseThread.Start();
    }

    private static void StopPulseEffect()
    {
        running = false;
        pulseThread?.Join();
    }

    private static void PlayBackgroundMusic()
    {
        try
        {
            SoundPlayer player = new SoundPlayer("Sounds/menu_theme.wav");
            player.PlayLooping();
        }
        catch
        {
            Console.WriteLine("⚠ Menu music file not found. Add 'menu_theme.wav' to the Sounds folder.");
        }
    }
}