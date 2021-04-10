using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGearActivator : MonoBehaviour
{
    [SerializeField] private GameObject bonusGearPrefab;
    [SerializeField] private GameObject particlePrefab;
    private GameObject _bonusGear;
    private GameObject _particle;
    public Animation anim;
    public float timeStay = 5f;
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
        anim.Play("BonusBlockActivation");
        if(firstTime){
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                spawnBonusGears();
            }
            firstTime = false;
        }
        
    }

    public void spawnBonusGears(){
        Debug.Log("Ou");
        _particle = Instantiate(particlePrefab) as GameObject;
        _particle.transform.position = this.gameObject.transform.position;
        for (int i = 0; i < this.gameObject.transform.childCount; i++) {
            Transform spawner = this.gameObject.transform.GetChild(i);
            Debug.Log(i);
            _bonusGear = Instantiate(bonusGearPrefab) as GameObject;
            GearBonus gb = _bonusGear.GetComponent<GearBonus>();
            gb.setTime(timeStay);
            _bonusGear.transform.position = spawner.position;
        }
    }
}
