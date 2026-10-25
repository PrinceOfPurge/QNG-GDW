using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : Command
{

    PlayerShoot playershoot;

    public ShootCommand(PlayerShoot _playershoot)
    {
        playershoot = _playershoot;
    }


    public override void Execute()
    {
        playershoot.Shoot();
    }


    public override void Undo()
    {
        throw new System.NotImplementedException();
    }


}
