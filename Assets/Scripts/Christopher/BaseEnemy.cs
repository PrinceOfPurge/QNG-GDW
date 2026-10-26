// base enemy
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{

    public abstract void attack();
    [SerializeField] protected float health = 25f;

}
