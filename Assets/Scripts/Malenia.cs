using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Malenia : Enemy
{
    public override void Update()
    {
        base.Update();
        LookAtTarget(0.2f);
    }
}
