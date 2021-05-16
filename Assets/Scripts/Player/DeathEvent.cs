using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Enum: DeathEvent 
 *  
 *  Description:
 *  This values are used in 'hurt' PlayerStatisticsController method.
 *  When an enemy or an ambiental object hurts player, he can react in base of hit type.
 *  
 *  Author: Thomas Voce
*/
public enum DeathEvent
{
    BURNED,
    FROZEN,
    FALLED_IN_VACUUM,
    HITTED
}
