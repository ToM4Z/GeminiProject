using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLives = 3;
    public int hp = 5;
    public int goldCoinCount = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseGoldCoins(){
        goldCoinCount++;
        if (goldCoinCount == 100){
            playerLives++;
            goldCoinCount = 0;
        }
    }
}
