using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineTree : BaseTree
{
    public override string NameOfTree => "Pine Tree";
    public override void Grow()
    {
        float size = Random.Range(2.0f, 3.0f);
        transform.localScale = Vector3.one * size;
    }
}
