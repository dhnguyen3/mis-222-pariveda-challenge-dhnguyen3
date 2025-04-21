using System;
using System.IO;
using System.Media;

public static class SoundManager
{
    private static SoundPlayer player;

    public static void PlaySong(string soundName)
    {
        if (string.IsNullOrEmpty(soundName)) return;

        if (!SongManager.SoundDataMap.ContainsKey(soundName))
        {
            Console.WriteLine($"No sound data found for: {soundName}");
            return;
        }

        string filePath = Path.Combine("Sounds", SongManager.SoundDataMap[soundName].FileName);

        if (File.Exists(filePath))
        {
            try
            {
                player = new SoundPlayer(filePath);
                player.Play(); // Asynchronous
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Audio file not found: {filePath}");
        }
    }

    public static void StopSong()
    {
        player?.Stop();
    }
}
