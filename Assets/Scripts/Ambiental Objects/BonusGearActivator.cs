using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: BonusGearActivator
 *  
 *  Description:
 *  This script handles the Bonus Block behaviour.
 *  
 *  Author: Andrea De Seta
*/

public class BonusGearActivator : MonoBehaviour
{
    [SerializeField] private GameObject bonusGearPrefab;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private AudioSource activationSFX;
    [SerializeField] private AudioSource timerSFX;
    private GameObject _bonusGear;
    private GameObject _particle;
    public Animation anim;
    public float timeStay = 8f;
    public bool firstTime = true;
    void Start()
    {
        anim = gameObject.GetComponentInParent(typeof(Animation)) as Animation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other) {
        //It will be activated only the first time that the player jumps under it
        anim.Play("BonusBlockActivation");
        if(firstTime){
            PlayerStatistics player = other.GetComponentInParent<PlayerStatistics>();
            if (player != null)
            {
                firstTime = false;
                activationSFX.Play();
                timerSFX.Play();
                StartCoroutine(PlaySound());
                spawnBonusGears();
            }
        }
        
    }

    private IEnumerator PlaySound(){
        yield return new WaitForSeconds(timeStay);
        timerSFX.Stop();
    }

    public void spawnBonusGears(){

        //Activate a particle to make nicer the activation
        _particle = Instantiate(particlePrefab) as GameObject;
        _particle.transform.position = this.gameObject.transform.position;

        //Loop on all childs, that are the spawner of the Bonus Gears
        for (int i = 0; i < this.gameObject.transform.childCount; i++) {
            //Take the position of the spawner and give it to the position of the Bonus Gear prefab
            Transform spawner = this.gameObject.transform.GetChild(i);
            _bonusGear = Instantiate(bonusGearPrefab) as GameObject;
            GearBonus gb = _bonusGear.GetComponent<GearBonus>();
            //It will stay only for a short time, because it's a bonus
            gb.setTime(timeStay);
            _bonusGear.transform.position = spawner.position;
        }
    }
}
