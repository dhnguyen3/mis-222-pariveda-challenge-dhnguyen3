using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public static class Game
{
    public static readonly string[] directions = { "←", "↑", "↓", "→" };
    public static readonly ConsoleKey[] player1Keys = { ConsoleKey.A, ConsoleKey.W, ConsoleKey.S, ConsoleKey.D };
    public static readonly ConsoleKey[] player2Keys = { ConsoleKey.J, ConsoleKey.I, ConsoleKey.K, ConsoleKey.L };
    public static readonly Random random = new Random();

    private const int LaneHeight = 15;
    private const int ArrowSize = 3;
    private const int ColumnsPerPlayer = 4;
    private const int ColumnWidth = 6;
    private const int TargetZonePosition = 3;

    public class Arrow
    {
        public string Direction;
        public int Y;
        public bool Hit;
        public int PlayerId;

        public Arrow(string direction, int playerId = 1)
        {
            Direction = direction;
            Y = LaneHeight + ArrowSize;
            Hit = false;
            PlayerId = playerId;
        }
    }

    public static void StartNewGame()
    {
        Console.CursorVisible = true;
        FullClear();
        Console.WriteLine("Welcome to K-Pop Dance Revolution!");
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();
        Console.Write("Enter your email: ");
        string email = Console.ReadLine();

        Player player = new Player(name, email);
        string selectedSound = SongManager.SelectSong();
        if (string.IsNullOrEmpty(selectedSound)) return;

        SoundData songData = SongManager.SoundDataMap[selectedSound];
        SoundManager.PlaySong(selectedSound);

        Console.WriteLine($"\nStarting '{selectedSound}'...");
        Console.WriteLine("Match the arrows as they reach the target zone!");
        Console.WriteLine("Controls: A (←), W (↑), S (↓), D (→)");
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);
        Console.CursorVisible = false;

        int beatIntervalMs = 60000 / songData.BPM;
        Stopwatch stopwatch = Stopwatch.StartNew();
        int score = 0, combo = 0, lives = 5;

        List<Arrow> arrows = new List<Arrow>();
        int frame = 0, minSpacing = 10, lastArrowFrame = -minSpacing;
        string message = "";
        int messageTimer = 0;

        while (stopwatch.Elapsed.TotalSeconds < songData.DurationInSeconds && lives > 0)
        {
            if (frame - lastArrowFrame >= minSpacing)
            {
                arrows.Add(new Arrow(directions[random.Next(directions.Length)]));
                lastArrowFrame = frame;
            }

            FullClear();
            Console.WriteLine($"Score: {score}   Combo: {combo}   Lives: {lives}\n");

            string targetLine = string.Join(" ", Enumerable.Repeat("[═══]", ColumnsPerPlayer));
            Console.WriteLine(targetLine);
            Console.WriteLine(targetLine);

            string[][] lane = new string[LaneHeight][];
            for (int i = 0; i < lane.Length; i++)
                lane[i] = new string[ColumnsPerPlayer];

            foreach (var arrow in arrows)
            {
                if (!arrow.Hit && arrow.Y < LaneHeight + ArrowSize)
                {
                    int col = Array.IndexOf(directions, arrow.Direction);
                    string[] art = GetCompactArrowArt(arrow.Direction);

                    for (int i = 0; i < art.Length; i++)
                    {
                        int row = arrow.Y - (art.Length - 1 - i);
                        if (row >= 0 && row < lane.Length && string.IsNullOrEmpty(lane[row][col]))
                            lane[row][col] = art[i];
                    }
                }
            }

            for (int row = 0; row < lane.Length; row++)
            {
                for (int col = 0; col < ColumnsPerPlayer; col++)
                {
                    Console.ForegroundColor = GetArrowColor(directions[col]);
                    Console.Write((lane[row][col] ?? new string(' ', ColumnWidth)).PadRight(ColumnWidth));
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(message))
            {
                Console.ForegroundColor = message.Contains("Perfect") ? ConsoleColor.Cyan : ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }

            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                string pressed = key switch
                {
                    ConsoleKey.A => "←", ConsoleKey.W => "↑",
                    ConsoleKey.S => "↓", ConsoleKey.D => "→",
                    _ => null
                };

                bool hit = false;
                foreach (var arrow in arrows.ToArray())
                {
                    if (!arrow.Hit && arrow.Y <= TargetZonePosition && arrow.Y >= 0 && pressed == arrow.Direction)
                    {
                        arrow.Hit = true;
                        hit = true;
                        combo++;
                        score += 100 + (combo * 10);
                        message = "Perfect!";
                        break;
                    }
                }

                if (!hit && pressed != null)
                {
                    combo = 0;
                    lives--;
                    message = "Miss!";
                }
                messageTimer = 8;
            }

            foreach (var arrow in arrows) arrow.Y--;
            arrows.RemoveAll(a => a.Y < -ArrowSize || a.Hit);

            if (messageTimer > 0 && --messageTimer == 0)
                message = "";

            frame++;
            Thread.Sleep(beatIntervalMs / 4);
        }

        SoundManager.StopSong();
        Thread.Sleep(1000);
        FullClear();
        CenterText("══════════════════════════");
        CenterText("       GAME OVER!");
        CenterText($"Final Score: {score}");
        CenterText($"Max Combo: {combo}");

        Console.WriteLine("\nWould you like to save your score? (y/n)");
        if (Console.ReadLine().Trim().ToLower() == "y")
        {
            SaveSystem.SaveGame(player, selectedSound, score, combo);
            Console.WriteLine("Score saved successfully!");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
        Console.CursorVisible = true;
    }

    public static void StartTwoPlayerGame()
    {
        Console.CursorVisible = true;
        FullClear();
        
        Console.WriteLine("=== Two Player Mode ===");
        Console.Write("Player 1 - Enter your name: ");
        string name1 = Console.ReadLine();
        Console.Write("Player 1 - Enter your email: ");
        string email1 = Console.ReadLine();
        Console.Write("Player 2 - Enter your name: ");
        string name2 = Console.ReadLine();
        Console.Write("Player 2 - Enter your email: ");
        string email2 = Console.ReadLine();

        Player player1 = new Player(name1, email1);
        Player player2 = new Player(name2, email2);

        string selectedSound = SongManager.SelectSong();
        if (string.IsNullOrEmpty(selectedSound)) return;

        SoundData songData = SongManager.SoundDataMap[selectedSound];
        SoundManager.PlaySong(selectedSound);

        Console.WriteLine("\nStarting Two Player Match...");
        Console.WriteLine("Player 1: WASD | Player 2: IJKL");
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);
        Console.CursorVisible = false;

        int beatIntervalMs = 60000 / songData.BPM;
        Stopwatch stopwatch = Stopwatch.StartNew();
        int score1 = 0, combo1 = 0, lives1 = 5;
        int score2 = 0, combo2 = 0, lives2 = 5;

        List<Arrow> arrows = new List<Arrow>();
        int frame = 0, minSpacing = 10, lastArrowFrame = -minSpacing;
        string p1Message = "", p2Message = "";
        int messageTimer = 0;

        while (stopwatch.Elapsed.TotalSeconds < songData.DurationInSeconds && (lives1 > 0 || lives2 > 0))
        {
            if (frame - lastArrowFrame >= minSpacing)
            {
                arrows.Add(new Arrow(directions[random.Next(directions.Length)], 1));
                arrows.Add(new Arrow(directions[random.Next(directions.Length)], 2));
                lastArrowFrame = frame;
            }

            FullClear();
            
            int consoleWidth = Console.WindowWidth;
            int halfWidth = consoleWidth / 2;
            int laneWidth = ColumnsPerPlayer * ColumnWidth;
            int padding = Math.Max(1, (halfWidth - laneWidth - 1) / 2);

            string player1Header = $"P1: {player1.Name.Shorten(8)} | Score: {score1}";
            string player2Header = $"P2: {player2.Name.Shorten(8)} | Score: {score2}";
            
            Console.Write(player1Header.PadRight(halfWidth - 1));
            Console.Write("║");
            Console.WriteLine(player2Header.PadLeft(halfWidth - 1));

            string targetBox = string.Join(" ", Enumerable.Repeat("[═══]", ColumnsPerPlayer));
            
            Console.Write(new string(' ', padding));
            Console.Write(targetBox);
            Console.Write(new string(' ', padding));
            Console.Write("║");
            Console.Write(new string(' ', padding));
            Console.Write(targetBox);
            Console.WriteLine(new string(' ', padding));

            Console.Write(new string(' ', padding));
            Console.Write(targetBox);
            Console.Write(new string(' ', padding));
            Console.Write("║");
            Console.Write(new string(' ', padding));
            Console.Write(targetBox);
            Console.WriteLine(new string(' ', padding));

            string[][] lane = new string[LaneHeight][];
            for (int i = 0; i < lane.Length; i++)
                lane[i] = new string[ColumnsPerPlayer * 2];

            foreach (var arrow in arrows)
            {
                if (!arrow.Hit && arrow.Y < LaneHeight + ArrowSize)
                {
                    int col = Array.IndexOf(directions, arrow.Direction);
                    int offset = (arrow.PlayerId == 1) ? 0 : ColumnsPerPlayer;
                    string[] art = GetCompactArrowArt(arrow.Direction);
                    
                    for (int i = 0; i < art.Length; i++)
                    {
                        int row = arrow.Y - (art.Length - 1 - i);
                        if (row >= 0 && row < lane.Length && string.IsNullOrEmpty(lane[row][col + offset]))
                            lane[row][col + offset] = art[i];
                    }
                }
            }

            for (int row = 0; row < lane.Length; row++)
            {
                Console.Write(new string(' ', padding));
                
                for (int col = 0; col < ColumnsPerPlayer; col++)
                {
                    Console.ForegroundColor = GetArrowColor(directions[col]);
                    Console.Write((lane[row][col] ?? new string(' ', ColumnWidth)).PadRight(ColumnWidth));
                    Console.ResetColor();
                }
                
                Console.Write(new string(' ', padding) + "║" + new string(' ', padding));
                
                for (int col = ColumnsPerPlayer; col < ColumnsPerPlayer * 2; col++)
                {
                    Console.ForegroundColor = GetArrowColor(directions[col % ColumnsPerPlayer]);
                    Console.Write((lane[row][col] ?? new string(' ', ColumnWidth)).PadRight(ColumnWidth));
                    Console.ResetColor();
                }
                
                Console.WriteLine(new string(' ', padding));
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write((p1Message + new string(' ', 10)).Substring(0, 10).PadRight(halfWidth));
            Console.ResetColor();
            Console.Write(" ║ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine((p2Message + new string(' ', 10)).Substring(0, 10).PadLeft(halfWidth - 3));
            Console.ResetColor();

            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                string pressed1 = null, pressed2 = null;

                if (player1Keys.Contains(key))
                    pressed1 = directions[Array.IndexOf(player1Keys, key) % 4];
                else if (player2Keys.Contains(key))
                    pressed2 = directions[Array.IndexOf(player2Keys, key) % 4];

                bool p1Hit = false, p2Hit = false;
                foreach (var arrow in arrows.ToArray())
                {
                    if (!arrow.Hit && arrow.Y <= TargetZonePosition && arrow.Y >= 0)
                    {
                        if (arrow.PlayerId == 1 && pressed1 == arrow.Direction)
                        {
                            arrow.Hit = true;
                            p1Hit = true;
                            combo1++;
                            score1 += 100 + (combo1 * 10);
                            p1Message = "Perfect!";
                        }
                        else if (arrow.PlayerId == 2 && pressed2 == arrow.Direction)
                        {
                            arrow.Hit = true;
                            p2Hit = true;
                            combo2++;
                            score2 += 100 + (combo2 * 10);
                            p2Message = "Perfect!";
                        }
                    }
                }

                if (pressed1 != null && !p1Hit) { combo1 = 0; lives1--; p1Message = "Miss!"; }
                if (pressed2 != null && !p2Hit) { combo2 = 0; lives2--; p2Message = "Miss!"; }
                
                messageTimer = 8;
            }

            foreach (var arrow in arrows) arrow.Y--;
            arrows.RemoveAll(a => a.Y < -ArrowSize || a.Hit);
            
            if (messageTimer > 0 && --messageTimer == 0)
                p1Message = p2Message = "";

            frame++;
            Thread.Sleep(beatIntervalMs / 4);
        }

        SoundManager.StopSong();
        Thread.Sleep(1000);
        FullClear();
        CenterText("══════════════════════════");
        CenterText("       GAME OVER!");
        CenterText($"Player 1: {score1}  Player 2: {score2}");
        CenterText(score1 > score2 ? $"{player1.Name} Wins! 🏆" : 
                  score2 > score1 ? $"{player2.Name} Wins! 🏆" : "It's a Tie! 🤝");

        Console.WriteLine("\nWould you like to save both scores? (y/n)");
        if (Console.ReadLine().Trim().ToLower() == "y")
        {
            SaveSystem.SaveGame(player1, $"{selectedSound} (P1)", score1, combo1);
            SaveSystem.SaveGame(player2, $"{selectedSound} (P2)", score2, combo2);
            Console.WriteLine("Scores saved successfully!");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
        Console.CursorVisible = true;
    }

    private static string[] GetCompactArrowArt(string direction) => direction switch
    {
        "↑" => new[] { " ^ ", "/^\\", " ^ " },
        "↓" => new[] { " v ", "\\v/", " v " },
        "←" => new[] { " < ", "<<=", " < " },
        "→" => new[] { " > ", "=>>", " > " },
        _ => new[] { " ? " }
    };

    public static ConsoleColor GetArrowColor(string direction) => direction switch
    {
        "←" => ConsoleColor.Red, "↑" => ConsoleColor.Blue,
        "↓" => ConsoleColor.Green, "→" => ConsoleColor.Yellow,
        _ => ConsoleColor.White
    };

    private static string Shorten(this string str, int maxLength) =>
        str.Length > maxLength ? str.Substring(0, maxLength - 2) + ".." : str;

    public static void CenterText(string text)
    {
        int width = Console.WindowWidth;
        int leftPadding = Math.Max(0, (width - text.Length) / 2);
        Console.WriteLine(new string(' ', leftPadding) + text);
    }

    public static void FullClear()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth * Console.WindowHeight));
        Console.SetCursorPosition(0, 0);
    }
}