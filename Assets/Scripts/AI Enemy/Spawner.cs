using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour, IHittable, IResettable
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public Transform spawnPoint;   // this must be placed in nav mesh area

    private List<GameObject> enemiesSpawned = new List<GameObject>();

    [SerializeField] public float timeBeforeSpawn = 6.5f;
    private float timer = 1;

    [SerializeField] public int maxEnemyToSpawn = 5;

    [SerializeField] public ParticleSystem particle;
    private Material material;

    [SerializeField] public AudioClip spawnClip, destroyClip;
    [SerializeField] public GameObject particleSpawnPrefab;
    private AudioSource audioSource;

    private bool activate = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        material = GetComponentInChildren<Renderer>().material;
        material.DisableKeyword("_EMISSION");
        particle.Stop();
    }

    void Update()
    {
        if(activate && enemiesSpawned.Count < maxEnemyToSpawn)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                Vector3 position = AIEnemy.GenerateRandomPosition(spawnPoint.position, 2, NavMesh.AllAreas);
                
                Instantiate(particleSpawnPrefab, position, Quaternion.identity);
                audioSource.PlayOneShot(spawnClip);
                enemiesSpawned.Add( Instantiate(enemyPrefab, position, Quaternion.identity));
                timer = timeBeforeSpawn;
            }

        }

        if (enemiesSpawned.Count > 0)
            CheckToDestroy();
    }

    private void CheckToDestroy()
    {
        GameObject toRemove = null;
        foreach (GameObject e in enemiesSpawned)
            if (!e.activeSelf)
            {
                toRemove = e;
                print("Find toRemove");
                break;
            }

        enemiesSpawned.Remove(toRemove);
        Destroy(toRemove);
    }

    public void hit()
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        //parte animazione
        audioSource.PlayOneShot(destroyClip);

        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }

    public void Reset()
    {
        this.gameObject.SetActive(true);
        material.DisableKeyword("_EMISSION");
        particle.Stop();
        activate = false;
        foreach (GameObject e in enemiesSpawned)
            Destroy(e);
        enemiesSpawned.Clear();
        timer = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activate = true;
            material.EnableKeyword("_EMISSION");
            particle.Play();
        }
    }

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
