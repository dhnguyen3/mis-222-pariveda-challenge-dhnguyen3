public class SoundData
{
    public string FileName { get; set; }
    public int BPM { get; set; }
    public int DurationInSeconds { get; set; }

    public SoundData(string fileName, int bpm, int durationInSeconds)
    {
        FileName = fileName;
        BPM = bpm;
        DurationInSeconds = durationInSeconds;
    }
}
