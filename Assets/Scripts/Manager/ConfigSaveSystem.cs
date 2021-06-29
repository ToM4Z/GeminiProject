using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 *  Class: ConfigSaveSystem
 *  
 *  Description:
 *  this manager handle the savesystem for the config of the game
 *  
 *  Author: Gianfranco Sapia
*/
public static class ConfigSaveSystem
{
    //A custom class is saved in a json
    public static void Save() {
        SaveObject saveObject = new SaveObject {
            soundVolume = GlobalVariables.SoundVolume,
            musicVolume = GlobalVariables.MusicVolume
        };
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/config.txt",json);
    }

    //A custom class is loaded from a json is the file exist
    public static void Load() {
        if (File.Exists(Application.dataPath + "/config.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/config.txt");

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            GlobalVariables.SoundVolume = saveObject.soundVolume;
            GlobalVariables.MusicVolume = saveObject.musicVolume;
        }
        else
            Save();
    }

    private class SaveObject {
        public float soundVolume;
        public float musicVolume;

    }
}
