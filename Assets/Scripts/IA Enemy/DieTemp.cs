using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieTemp : MonoBehaviour
{
    IAEnemy ia;

    private void Start()
    {
        ia = GetComponent<IAEnemy>();
        StartCoroutine("FindTargetsWithDelay", 5f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            ia.die();
        }
    }
}
