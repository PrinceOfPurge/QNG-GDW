using System;
using UnityEngine;
public interface IResettable
{
    void ResetForReuse();
}
public class PooledEnemy : MonoBehaviour
{
    private Action<PooledEnemy> _returnAction;
    private IResettable[] _resettables;
    private bool _returned;
    public void Init(Action<PooledEnemy> returnAction)
    {
        _returnAction = returnAction;
    }
    public void OnSpawned()
    {
        _returned = false;
        if (_resettables == null)
        {
            _resettables = GetComponents<IResettable>();
        }
        foreach (IResettable resettable in _resettables)
        {
            resettable.ResetForReuse();
        }
    }
    public void ReturnToPool()
    {
        if( _returned ) return;
            _returned = true;
        if (_returnAction != null) _returnAction(this);
        else Destroy(gameObject);
    }
}