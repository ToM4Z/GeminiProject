using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject fireFusePrefab;
    [SerializeField] private GameObject bombExplosionPrefab;
    private GameObject _fireFuse;
    private GameObject _explosionParticle;
    private Transform _fuse;
    private Transform _bomb;
    public float detonationTime = 5.0f;
    public float radiusExplosion = 1.0f;
    public float power = 5.0f;
    public float upforce = 1.0f;
    void Start()
    {
        _fuse = this.gameObject.transform.GetChild(2);
        _bomb = this.gameObject.transform;
        _fireFuse = Instantiate(fireFusePrefab);
        
        StartCoroutine(Detonation());

    }

    // Update is called once per frame
    void Update()
    {
        _fireFuse.transform.position = _fuse.position;
    }

    private IEnumerator Detonation(){
        yield return new WaitForSeconds(detonationTime);
        ExplosionDamage(_bomb.position, radiusExplosion);
    }

    void ExplosionDamage(Vector3 center, float radius)
    {
        //Instantion of the explosion particle system
        _explosionParticle = Instantiate(bombExplosionPrefab);
        _explosionParticle.transform.position = this.gameObject.transform.position;

        //I take all the colliders in the spheric overlap and I loop on them
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {   
            //If I collide with a destroyable wall, I call its function in order to destroy it
            if (hitCollider.GetComponent<DestroyableWallController>() != null){
                DestroyableWallController controller = hitCollider.GetComponent<DestroyableWallController>();
                controller.DestroyWall(power, center, radius, upforce, ForceMode.Impulse);
            }
                
        }
        
        Destroy(_fireFuse);
        Destroy(this.gameObject);

    }
}
