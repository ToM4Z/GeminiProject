/*
 *  Class: GameEvent
 *  
 *  Description:
 *  This class contain some constants and global variables
 *  
 *  Author: Thomas Voce
*/
public static class GlobalVariables
{

    public static bool isPaused = false;
    public static bool Win = false;
    public static bool GameOver = false;

    private static bool _soundMute = false;
    public static bool SoundMute
    {
        get { return _soundMute; }
        set
        {
            _soundMute = value;
            Messenger.Broadcast(AUDIO_SETTINGS_CHANGED, MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    private static float _soundVolume = 1;
    public static float SoundVolume
    {
        get { return _soundVolume; }
        set 
        {
            _soundVolume = value;
            Messenger.Broadcast(AUDIO_SETTINGS_CHANGED, MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    private static float _musicVolume = 1;
    public static float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            Messenger.Broadcast(AUDIO_SETTINGS_CHANGED, MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    public static int SceneToLoad;

    public const string DEATH = "DEATH";
    public const string RESET = "RESET";
    public const string ENABLE_INPUT = "ENABLE_INPUT";
    public const string TOGGLE_AUDIO_ON_OFF = "TOGGLE_AUDIO_ON_OFF";
    public const string AUDIO_SETTINGS_CHANGED = "AUDIO_SETTINGS_CHANGED";


    public const int MAIN_MENU_SCENE = 0;
    public const int HUB_SCENE = 1;
    public const int LEVEL_1_SCENE = 2;
    public const int LEVEL_2_SCENE = 3;
    public const int LEVEL_3_SCENE = 4;
}