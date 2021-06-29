using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 *  Class: PlayerSaveSystem
 *  
 *  Description:
 *  Class to save player progress
 *  
 *  Author: Gianfranco Sapia, Thomas Voce
*/

public static class PlayerSaveSystem
{
    //A custom class is saved in a json
    public static void Save() {
        SaveObject saveObject = new SaveObject {
            lifes = GlobalVariables.PlayerLives,
            scores = DictionaryToArray(GlobalVariables.scores),
            dialoguesDone = GlobalVariables.dialoguesAlreadyDone
        };
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/player.txt",json);
    }

    //A custom class is loaded from a json
    public static void Load() {
        if(File.Exists(Application.dataPath + "/player.txt")) {
            string saveString = File.ReadAllText(Application.dataPath + "/player.txt");

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            GlobalVariables.PlayerLives = saveObject.lifes;
            GlobalVariables.scores = ArrayToDictionary(saveObject.scores);
            GlobalVariables.dialoguesAlreadyDone = saveObject.dialoguesDone;
        }
    }

    //Player progress to be saved
    private class SaveObject {
        public int lifes;
        public int[] scores;
        public List<int> dialoguesDone;
    }

    // methods to convert dictionary<int int> to array and viceversa (Thomas)
    public static int[] DictionaryToArray(Dictionary<int,int> d)
    {
        int[] array = new int[ Mathf.Max(new List<int>(d.Keys).ToArray()) + 1 ];
        foreach(KeyValuePair<int,int> x in d)
            array[x.Key] = x.Value;
        return array;
    }

    public static Dictionary<int, int> ArrayToDictionary(int[] array)
    {
        Dictionary<int, int> d = new Dictionary<int, int>();
        for (int i = 0; i < array.Length; i++)
            if (array[i] != 0)
                d.Add(i, array[i]);
        return d;
    }
}
