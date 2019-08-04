using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class VersionedSave
{
    public int version = 0;
    public System.Object saveData = new System.Object();

    public static SavedGame deversion(VersionedSave versionedSave)
    {
        // if versionedSave.version is less than the current version, convert it here
        return (SavedGame)versionedSave.saveData;
    }
}

[System.Serializable]
public class SavedGame
{
    public static int saveSlot = 0;

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

    public static void SaveGame(SavedGame saveData, int saveSlot)
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/save" + saveSlot + ".dat");
            VersionedSave versionedSave = new VersionedSave();
            versionedSave.saveData = saveData;
            bf.Serialize(file, versionedSave);
        }
        catch (Exception e)
        {
            if (e != null)
            {
                // unable to save
            }
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    public static SavedGame LoadGame(int saveSlot)
    {
        FileStream file = null;
        SavedGame saveData = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/save" + saveSlot + ".dat", FileMode.Open);
            VersionedSave versionedSave = (VersionedSave)bf.Deserialize(file);
            saveData = VersionedSave.deversion(versionedSave);
        }
        catch (Exception e)
        {
            if (e != null)
            {
                // unable to load
            }
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
        return saveData;
    }

    public static void DeleteSave(int saveSlot)
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/save" + saveSlot + ".dat");
        }
        catch (Exception e)
        {
            if (e != null)
            {
                // unable to delete
            }
        }
    }
}
