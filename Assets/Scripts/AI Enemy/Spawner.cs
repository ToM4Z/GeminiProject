using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IHittable, IResettable
{
    [SerializeField] public GameObject enemy;
    [SerializeField] public Transform placeOnNavMeshToSpawn;

    private List<GameObject> enemySpawned = new List<GameObject>();

    [SerializeField] public float timeBeforeSpawn = 6.5f;
    private float timer = 0;

    [SerializeField] public int maxEnemyToSpawn = 5;

    private bool activate = true;

    void Update()
    {
        if(activate && enemySpawned.Count < maxEnemyToSpawn)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                enemySpawned.Add( Instantiate(enemy, 
                                            AIEnemy.GenerateRandomPosition(placeOnNavMeshToSpawn.position, 3, -1),
                                            Quaternion.identity));
                timer = timeBeforeSpawn;
            }
        }  
    }

    public void hit()
    {
        this.gameObject.SetActive(false);
    }

    public void Reset()
    {
        this.gameObject.SetActive(true);
        foreach (GameObject e in enemySpawned)
            Destroy(e);
        timer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            activate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            activate = false;
    }
}
