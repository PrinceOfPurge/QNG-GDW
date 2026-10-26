using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTree : MonoBehaviour
{
   public abstract string NameOfTree { get; }
    public abstract void Grow();
}
