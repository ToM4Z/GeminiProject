using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: IHittable
 *  
 *  Description:
 *  This interface encloses all entities that are hittable.
 *  
 *  Author: Thomas Voce
*/

public interface IHittable
{
    public bool hit();
}
