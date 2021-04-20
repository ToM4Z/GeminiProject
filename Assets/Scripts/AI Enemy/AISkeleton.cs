using System.Collections;
using UnityEngine;

/*
    This script describe the Skeletons behaviur.
    Inherite by AI Enemy
*/
public class AISkeleton : AIEnemy
{
    // This triggers are the hands of the skeleton
    public AttackTrigger[] hands = new AttackTrigger[2];

    protected override void Start()
    {
        base.Start();

        status = Status.INACTIVE;

        StartCoroutine(awake());
    }

    IEnumerator awake()
    {
        yield return new WaitForSeconds(1f);

        spawn = true;
    }

    // during attack, if the hands are triggered by player, I hit him
    protected override void duringAttack()
    {
        if (hands[0].EnteredTrigger || hands[1].EnteredTrigger)
            player.GetComponent<PlayerController>().Damage(1);
    }
}
