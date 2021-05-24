using System.Collections;
using UnityEngine;

/*
 *  Class: AISkeleton
 *  
 *  Description:
 *  This script describe the Skeleton behaviur.
 *  Inherite by AI Enemy
 *  
 *  Author: Thomas Voce
*/
public class AISkeleton : AIEnemy
{
    // This triggers are the hands of the skeleton
    [SerializeField]
    private AttackTrigger[] hands = new AttackTrigger[2];

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

    // at the beginning of the attack, I activate the attack triggers
    protected override void startAttack()
    {
        base.startAttack();
        hands[0].EnableTrigger();
        hands[1].EnableTrigger();
    }

    // during attack, if the hands are triggered by player, I hit him
    protected override void duringAttack()
    {
        if (hands[0].EnteredTrigger || hands[1].EnteredTrigger)
            PlayerStatisticsController.instance.hurt(DeathEvent.HITTED);
    }

    // at the end of the attack, I deactivate the attack triggers
    protected override void stopAttack()
    {
        base.stopAttack();
        hands[0].DisableTrigger();
        hands[1].DisableTrigger();
    }

    public override void Reset()
    {
        base.Reset();

        status = Status.INACTIVE;

        StartCoroutine(awake());
    }
}
