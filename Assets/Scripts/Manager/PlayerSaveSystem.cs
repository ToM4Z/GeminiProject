using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PlayerSaveSystem
{
    public static void Save() {
        SaveObject saveObject = new SaveObject {
            lifes = GlobalVariables.PlayerLives,
            scores = GlobalVariables.scores,
            dialoguesDone = GlobalVariables.dialoguesAlreadyDone
        };
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/player.txt",json);
    }

    public static void Load() {
        if(File.Exists(Application.dataPath + "/player.txt")) {
            string saveString = File.ReadAllText(Application.dataPath + "/player.txt");

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            GlobalVariables.PlayerLives = saveObject.lifes;
            GlobalVariables.scores = saveObject.scores;
            GlobalVariables.dialoguesAlreadyDone = saveObject.dialoguesDone;
        }
    }

    private class SaveObject {
        public int lifes;
        public Dictionary<int, int> scores;
        public List<int> dialoguesDone;

    }
}
