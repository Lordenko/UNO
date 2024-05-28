using System;
using System.Windows.Media;

public class MusicPlayer
{
    private static MusicPlayer instance = null;
    private MediaPlayer mediaPlayer;

    private MusicPlayer()
    {
        mediaPlayer = new MediaPlayer();
        mediaPlayer.MediaEnded += MediaPlayer_MediaEnded; // Подія для повторного відтворення
        mediaPlayer.Open(new Uri("pack://application:,,,/Data/Tigo.mp3")); // Відкриття файлу
        mediaPlayer.Volume = 0.5; // Регулювання гучності за замовчуванням
    }

    public static MusicPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MusicPlayer();
            }
            return instance;
        }
    }

    public void Play()
    {
        mediaPlayer.Play(); // Відтворення музики
    }

    public void Stop()
    {
        mediaPlayer.Stop(); // Зупинка музики
    }

    public void SetVolume(double volume)
    {
        mediaPlayer.Volume = volume; // Встановлення гучності
    }

    private void MediaPlayer_MediaEnded(object sender, EventArgs e)
    {
        mediaPlayer.Position = TimeSpan.Zero; // Повторне відтворення з початку
        mediaPlayer.Play();
    }
}
