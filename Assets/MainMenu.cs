using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public int savesToShow = 3;
    public GUIStyle windowStyle;
    public GUIStyle labelStyle;

    private List<SavedGame> savedGames = new List<SavedGame>();
    private bool deleting = false;
    private float lastAxis = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int saveLoop = 0; saveLoop < savesToShow; saveLoop++)
        {
            SavedGame newSave = SavedGame.LoadGame(saveLoop);
            if (newSave != null)
            {
                savedGames.Add(newSave);
            } else
            {
                savedGames.Add(new SavedGame());
            }
        }
    }

    private void OnGUI()
    {
        for (int saveLoop = 0; saveLoop < savesToShow; saveLoop++)
        {
            GUI.Window(saveLoop, new Rect(100, 25 + 50 * saveLoop, 300, 50), saveWindow, "Save " + (saveLoop + 1) + (saveLoop == SavedGame.saveSlot ? "*" : ""), windowStyle);
        }
        if (deleting)
        {
            GUI.ModalWindow(savesToShow, new Rect(150, 150, 300, 100), deleteWindow, "Confirm Delete");
        }
    }

    private void deleteWindow(int id)
    {
        GUI.Label(new Rect(10, 20, 280, 80), "Are you sure you want to delete this save?", labelStyle);
    }

    private void saveWindow(int id)
    {
        string abilityString = "";
        SavedGame currentSave = savedGames[id];
        if (currentSave.hasDoubleJump)
        {
            abilityString += 'J';
        }
        if (currentSave.hasAirDash)
        {
            abilityString += 'A';
        }
        if (currentSave.hasDownStab)
        {
            abilityString += 'D';
        }
        if (currentSave.hasEnergyAbsorb)
        {
            abilityString += 'E';
        }
        if (currentSave.hasHighJump)
        {
            abilityString += 'H';
        }
        if (currentSave.hasUppercut)
        {
            abilityString += 'U';
        }
        if (currentSave.hasWallClimb)
        {
            abilityString += 'W';
        }
        GUI.Label(new Rect(10, 10, 280, 30), abilityString);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (deleting)
            {
                SavedGame.DeleteSave(SavedGame.saveSlot);
                savedGames[SavedGame.saveSlot] = new SavedGame();
                deleting = false;
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            deleting = !deleting;
        }
        if (Input.GetAxis("Vertical") < 0 && lastAxis >= 0)
        {
            SavedGame.saveSlot++;
            if (SavedGame.saveSlot >= savesToShow)
            {
                SavedGame.saveSlot -= savesToShow;
            }
        }
        if (Input.GetAxis("Vertical") > 0 && lastAxis <= 0)
        {
            SavedGame.saveSlot--;
            if (SavedGame.saveSlot < 0)
            {
                SavedGame.saveSlot += savesToShow;
            }
        }
        lastAxis = Input.GetAxis("Vertical");
    }
}
