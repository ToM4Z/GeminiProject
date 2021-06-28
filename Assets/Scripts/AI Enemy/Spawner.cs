using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *  Class: Spawner
 *  
 *  Description:
 *  When player is so close to this gameobject, spawn an enemy every 'timeBeforeSpawn' seconds
 *  
 *  Author: Thomas Voce
*/

public class Spawner : MonoBehaviour, IHittable, IResettable
{
    // Enemy prefab to spawn
    [SerializeField] public GameObject enemyPrefab;

    // This transform must be placed on a NavMesh Area
    [SerializeField] public Transform spawnPoint;

    // List of enemies spawned, it's used to check if they are alive
    private List<GameObject> enemiesSpawned = new List<GameObject>();

    // Spawn timer
    [SerializeField] public float timeBeforeSpawn = 6.5f;
    private float timer = 1;

    // Max limit of enemy to spawn
    [SerializeField] public int maxEnemyToSpawn = 5;

    // Particle effect of when go is activated
    [SerializeField] public ParticleSystem particle;

    // I change material 
    private Material material;

    // AudioClips
    [SerializeField] public AudioClip spawnClip, destroyClip;

    // Particle effect to use when spawn enemy
    [SerializeField] public GameObject particleSpawnPrefab;

    private AudioSource audioSource;
    private Animator animator;

    private bool activate = false;
    private bool hitted = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        material = GetComponentInChildren<Renderer>().material;

        // set GO turned off
        material.DisableKeyword("_EMISSION");
        particle.Stop();
    }

    void Update()
    {
        // if the GO is activated and I can spawn another enemy
        if(activate && enemiesSpawned.Count < maxEnemyToSpawn)
        {
            timer -= Time.deltaTime;

            // and the timer is over
            if(timer <= 0)
            {
                // I spawn an enemy around the spawnpoint
                Vector3 position = AIEnemy.GenerateRandomPosition(spawnPoint.position, 2, NavMesh.AllAreas);
                
                Instantiate(particleSpawnPrefab, position, Quaternion.identity);
                audioSource.PlayOneShot(spawnClip);

                // A spawned enemy don't have to drop gears
                GameObject e = Instantiate(enemyPrefab, position, Quaternion.identity);
                e.GetComponent<AIEnemy>().canDropItem = false;
                enemiesSpawned.Add(e);

                timer = timeBeforeSpawn;
            }

        }

        if (enemiesSpawned.Count > 0)
            CheckToDestroy();
    }

    // I remove an enemy for frame if it's disactivated (died)
    private void CheckToDestroy()
    {
        GameObject toRemove = null;
        foreach (GameObject e in enemiesSpawned)
            if (!e.activeSelf)
            {
                toRemove = e;
                break;
            }

        enemiesSpawned.Remove(toRemove);
        Destroy(toRemove);
    }

    // if I hitted, I disable colliders, I drop a gear
    public bool hit()
    {
        if (hitted)
            return false;

        hitted = true;
        activate = false;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        GameObject gear = Instantiate(Managers.Enemies.DropItem, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity) as GameObject;
        gear.GetComponent<Gear>().ActivateFallDown();

        particle.Stop();
        animator.Play("Despawn");
        audioSource.PlayOneShot(destroyClip);

        return true;
    }

    // This method is called from the Despawn animation
    // After the animation is done, I reset the localscale and then I disable me
    public void Disable()
    {
        gameObject.transform.GetChild(0).localScale = Vector3.one;
        this.gameObject.SetActive(false);
    }

    // This method is called from EnemiesManager,
    // this method reset all gameobject data
    public void Reset()
    {
        this.gameObject.SetActive(true);
        material.DisableKeyword("_EMISSION");
        particle.Stop();
        activate = false;
        hitted = false;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        foreach (GameObject e in enemiesSpawned)
            Destroy(e);
        enemiesSpawned.Clear();
        timer = 1;
    }

    // when player is near to spawner, I activate it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activate = true;
            material.EnableKeyword("_EMISSION");
            particle.Play();
        }
    }

    // when player is far to spawner, I disactivate it
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activate = false;
            material.DisableKeyword("_EMISSION");
            particle.Stop();
        }
    }
}
