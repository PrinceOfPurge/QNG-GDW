using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTree : BaseTree
{
    public override string NameOfTree => "Oak Tree";
    public override void Grow()
    {
        float size = Random.Range(1.0f, 2.0f);
        transform.localScale = Vector3.one * size;
    }
}
