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
    private Animator animator;

    private bool activate = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
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

    private void CheckToDestroy()   // I remove an enemy for frame if it's disactivated
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

    public void hit()
    {
        activate = false;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefab/Gear.prefab", typeof(GameObject));
        GameObject gear = Instantiate(prefab, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity) as GameObject;
        gear.GetComponent<Rigidbody>().useGravity = true;
        gear.GetComponent<Rigidbody>().AddExplosionForce(5f, transform.position, 4f, 1f, ForceMode.Impulse);

        particle.Stop();
        animator.Play("Despawn");
        audioSource.PlayOneShot(destroyClip);
    }

    public void Disable()
    {
        gameObject.transform.GetChild(0).localScale = Vector3.one;
        this.gameObject.SetActive(false);
    }

    public void Reset()
    {
        this.gameObject.SetActive(true);
        material.DisableKeyword("_EMISSION");
        particle.Stop();
        activate = false;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

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
