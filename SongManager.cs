using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class SongManager
{
    public static string[] sounds;
    public static Dictionary<string, SoundData> SoundDataMap = new();

    static SongManager()
    {
        string soundFolder = "Sounds";
        if (Directory.Exists(soundFolder))
        {
            sounds = Directory.GetFiles(soundFolder, "*.wav")
                .Where(path => !Path.GetFileName(path).Equals("menu_theme.wav", StringComparison.OrdinalIgnoreCase))
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
        else
        {
            sounds = new string[] { };
        }

        // Add song metadata (only for actual game songs) //
        SoundDataMap["As If It's Your Last"] = new SoundData("As If It's Your Last.wav", bpm: 70, durationInSeconds: 96);
        SoundDataMap["Boy With Luv"] = new SoundData("Boy With Luv.wav", bpm: 70, durationInSeconds: 96);
        SoundDataMap["Caramelldansen"] = new SoundData("Caramelldansen.wav", bpm: 70, durationInSeconds: 73);
        SoundDataMap["Don't Wanna Cry"] = new SoundData("Don't Wanna Cry.wav", bpm: 70, durationInSeconds: 79);
        SoundDataMap["Energetic"] = new SoundData("Energetic.wav", bpm: 70, durationInSeconds: 76);
        SoundDataMap["DDU-DU DDU-DU"] = new SoundData("DDU-DU DDU-DU.wav", bpm: 70, durationInSeconds: 94);
        SoundDataMap["Hola Hola"] = new SoundData("Hola Hola.wav", bpm: 70, durationInSeconds: 86);
        SoundDataMap["Into the I-Land"] = new SoundData("Into the I-Land.wav", bpm: 70, durationInSeconds: 99);
        SoundDataMap["Just Right"] = new SoundData("Just Right.wav", bpm: 70, durationInSeconds: 89);
        SoundDataMap["Kizuna no Kiseki"] = new SoundData("Kizuna no Kiseki.wav", bpm: 70, durationInSeconds: 90);
        SoundDataMap["Learn To Meow"] = new SoundData("Learn To Meow.wav", bpm: 70, durationInSeconds: 100);
        SoundDataMap["Love Scenario"] = new SoundData("Love Scenario.wav", bpm: 70, durationInSeconds: 99);
        SoundDataMap["Lovesick Girls"] = new SoundData("Lovesick Girls.wav", bpm: 70, durationInSeconds: 84);
        SoundDataMap["Monster"] = new SoundData("Monster.wav", bpm: 70, durationInSeconds: 71);
        SoundDataMap["What Is Love"] = new SoundData("What Is Love.wav", bpm: 70, durationInSeconds: 69);
    }

    public static string SelectSong()
    {
        if (sounds.Length == 0)
        {
            Console.WriteLine("No songs found in the Sounds/ folder.");
            return "";
        }

        for (int i = 0; i < sounds.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {sounds[i]}");
        }

        int selection = -1;
        while (selection < 1 || selection > sounds.Length)
        {
            Console.Write("Select a song number: ");
            string input = Console.ReadLine();
            int.TryParse(input, out selection);
        }

        string selected = sounds[selection - 1];
        Console.WriteLine($"You selected: {selected}");
        return selected;
    }
}
