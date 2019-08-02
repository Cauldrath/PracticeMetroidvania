using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedGame
{
    public int version = 1;
    public int savedPoint = 0;
    public bool hasDoubleJump = false;
    public bool hasAirDash = false;
    public bool hasDownStab = false;
    public bool hasHighJump = false;
    public bool hasEnergyAbsorb = false;
    public bool hasWallClimb = false;
    public bool hasUppercut = false;
    public List<bool> bossesKilled = new List<bool>();
    public List<bool> healthUpgrades = new List<bool>();
    public List<bool> damageUpgrades = new List<bool>();
}
