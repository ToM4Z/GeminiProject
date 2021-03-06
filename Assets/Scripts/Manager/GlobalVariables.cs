using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
 *  Class: GlobalVariables
 *  
 *  Description:
 *  This class contains constants and global variables
 *  
 *  Author: Thomas Voce
*/
public static class GlobalVariables
{

    public static bool isPaused = false;
    public static bool isRespawning = false;
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

    // After player arrive to gameover screen, I reset lives to this variable
    public static readonly int PlayerLivesToReset = 3;
    // max HP to reset when player respawn
    public static readonly int PlayerHPToReset = 3;

    // Actual Player lives
    private static int _playerLives = 3;
    public static int PlayerLives
    {
        get { return _playerLives; }
        set {_playerLives = value; }
    }

    // constant messages
    public const string DEATH = "DEATH";
    public const string RESET = "RESET";
    public const string ENABLE_INPUT = "ENABLE_INPUT";
    public const string TOGGLE_AUDIO_ON_OFF = "TOGGLE_AUDIO_ON_OFF";
    public const string AUDIO_SETTINGS_CHANGED = "AUDIO_SETTINGS_CHANGED";

    // Dictionary containing level scores
    public static Dictionary<int, int> scores = new Dictionary<int, int>();

    // score assigned for each gear / gearBonus
    public static int GearScore = 10, GearBonusScore = 50;

    public static int ACTUAL_SCENE => SceneManager.GetActiveScene().buildIndex;

    public const int NUMBER_OF_LEVELS = 3;
    public const int MAIN_MENU_SCENE = 0;
    public const int HUB_SCENE = 4;
    public const int LEVEL_1_SCENE = 1;
    public const int LEVEL_2_SCENE = 2;
    public const int LEVEL_3_SCENE = 3;

    public static readonly string 
        PATH_MUSIC = "Musics/",
        MAIN_MENU_MUSIC     = PATH_MUSIC + "MenuMusic",
        GAMEOVER_MUSIC      = PATH_MUSIC + "GameOverMusic",
        VICTORY_MUSIC       = PATH_MUSIC + "VictoryMusic"
        ;

    public static readonly string[] LEVEL_MUSICS =
    {
        PATH_MUSIC + "LV1 OST",
        PATH_MUSIC + "LV2 OST",
        PATH_MUSIC + "LV3 OST",
        PATH_MUSIC + "HUB OST",
    };

    // methods that return the music of actual level (if it has)
    public static string GetLevelMusic() 
    { 
        return (ACTUAL_SCENE < 1 || ACTUAL_SCENE > 4) ?
            LEVEL_MUSICS[0]
            :
            LEVEL_MUSICS[ ACTUAL_SCENE - 1 ]; 
    }

    // list of dialogue identifiers already triggered
    public static List<int> dialoguesAlreadyDone = new List<int>();

    public static bool isDialogueDone(int x) { return dialoguesAlreadyDone.Contains(x); }

    public static void addDialogueDone(int x) { dialoguesAlreadyDone.Add(x); }


    public static readonly string[] Dialogues = new string[]
    {
        // [0, 1] testo per visualizzare il punteggio
        "Record: ", 
        "Non hai fatto ancora nessun punteggio!\nDatti una mossa!!",

        // [2, 7] Hub, D1 all'inizio della partita
        "Stupido Timmy, ti mando ad esplorare nuove dimensioni al posto mio e la prima cosa che fai e' rompere l'astronave!!",
        "Per poter tornare qui e farti resettare, devi trovare tutti i componenti sparsi in giro e portarli qui.",
        "Perfino tu non dovresti avere difficolta', tanto per muoverti devi solo usare WASD o l'analogico sinistro.",
        "Per guardarti intorno usa il Mouse oppure l'analogico destro.",
        "Suppongo che per cambiare zona ti devi ficcare in uno di quei tubi.",
        "Nel frattempo io vado a scolarmi una bottiglia di whisky.",

        // [8, 8] Hub, D2, davanti ad un tubo
        "Stupido Timmy, devi accovacciarti con il tasto C, o B del Joypad, per poter entrare in quel tubo.",

        // [9, 15] Hub, D3, quando tutti e 3 i livelli sono stati completati
        "Mmmm, non vedo altri tubi in cui ficcarsi.",
        "Mi sa che il gioco per il momento finisce qui.",
        "Ma dato che non hai ottenuto abbastanza pezzi di ricambio, mi sa che dovrai rimanere li.",
        "Non verro' di certo io a riprenderti!",
        "Buona permanenza!!",
        "Ah un'altra cosa... Il Gemini team ti ringrazia per aver giocato!",
        "Crediti:\nThomas Voce\nGianfranco Sapia\nAndrea De Seta",

        // [16, 18] Level 1, D4, all'inizio
        "Dannazione, ma che razza di mondo e' questo?!",
        "Per proseguire dovrai saltare... Premi SPAZIO o il tasto A del Joypad.",
        "Effettivamente avrei potuto montarti dei razzi, ma per un rottame come te sarebbe uno spreco!",

        // [19, 20] Level 1, D5, davanti il BonusBlock
        "Ehi, quella e' una cassa Bonus!",
        "Attivala saltandoci di sotto per ottenere ingranaggi extra!",

        // [21, 21] Level 1, D6, davanti il muro distruggibile
        "Mmm... quel muro sembra fragile... Li vicino c'e' una bomba, usa il tasto E, o Y del Joypad, per piazzarla e distruggere quel muro.",

        // [22, 25] Level 1, D7, davanti al fungo
        "I tuoi radar mi segnalano che qui sopra c'e' molta attivita'... ti consiglio di prepararti!",
        "Se qualche creatura ti volesse attaccare, usa il tasto sinistro del mouse, o X del Joypad, per attaccare.",
        "Oppure puoi sempre ricorrere all'antica arte della fuga...",
        "Quando sei in movimento, usa il tasto C, o B del Joypad, per eseguire una scivolata.",

        // [26, 27] Level 1, D8, davanti allo scheletro della caverna
        "Bene, hai trovato una batteria! Con quella puoi recuperare un HP!",
        "Non rilevo pericoli, raccoglila pure!!",

        // [28, 28] Level 1, D9, davanti alla vita
        "Hai trovato una vita! Vabbe e' inutile, tanto morirai miseramente...",

        // [29, 29] Level 2, D8, davanti al primo masso
        "Preparati a fare una bellissima e tranquillissima passeggiata... ;)",

    }; 

}