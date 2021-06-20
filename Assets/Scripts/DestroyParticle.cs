using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioSource soundClip;
    void Start()
    {
        
        if(soundClip != null)
            soundClip.Play();
            
        //I destroy it after 5 seconds in order to delete it from the scene and to not occupy memory
        Destroy(this.gameObject, 5.0f);
    }
}
