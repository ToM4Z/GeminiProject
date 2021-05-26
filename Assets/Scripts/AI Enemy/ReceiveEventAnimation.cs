using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveEventAnimation : MonoBehaviour
{
    [SerializeField] private AIDragon dragon;

    public void Dragon_BeatWings()
    {
        dragon.BeatWings();
    }
}
