## 🚀 How to Run (With Audio)
1. **Install Dependencies**:
   - Windows: Install [.NET 6+ SDK](https://dotnet.microsoft.com/download).
   - Windows: Or run "dotnet add package System.Windows.Extensions" in Cmder (for easy installment if you do not wan to do the first one)
   - Mac/Linux: Install .NET SDK and `NAudio`:
     ```bash
     brew install --cask dotnet-sdk  # Mac
     dotnet add package NAudio      # For cross-platform audio
     ```

2. **Audio Files**:
   - Place `.wav` files in `./Sounds/` (e.g., `menu_theme.wav`).

3. **Run the Game**:
   ```bash
   dotnet run


**Mac Users**:
- dotnet add package NAudio
- Replace the contents of SoundManager.cs with:
  using NAudio.Wave;

public static class SoundManager
{
    private static WaveOutEvent? outputDevice;
    
    public static void PlaySong(string songName)
    {
        outputDevice?.Stop();
        var audioFile = new AudioFileReader($"Sounds/{songName}");
        outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFile);
        outputDevice.Play();
    }
    
    public static void StopSong()
    {
        outputDevice?.Stop();
    }
}
