using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public int RevealAmount = 5;

//#nullable enable
    public PowerUp Active { get; private set; }
//#nullable disable


    PowerUp[] powerUps;

    public void GMAwake()
    {
        powerUps = new PowerUp[] { new Shovel(), new Reveal(RevealAmount) };
        foreach (PowerUp p in powerUps)
            p.Amount = 3;
    }

    public void Activate(int i )
    {
        if (Active != powerUps[i]  && powerUps[i].Amount > 0)
            Active = powerUps[i];
        else
            Active = null;
    }

    public void Use(TileLetter tileLetter)
    {
        if(Active != null)
        {
            Active.Use(tileLetter);
            Active = null;
        }
    }
}
