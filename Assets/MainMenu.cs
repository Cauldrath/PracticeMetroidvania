using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public int savesToShow = 5;
    public UnityEngine.UI.Image savePanel;

    private List<SavedGame> savedGames = new List<SavedGame>();
    private List<UnityEngine.UI.Image> savePanels = new List<UnityEngine.UI.Image>();
    private bool deleting = false;
    private float lastAxis = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (savePanel)
        {
            for (int saveLoop = 0; saveLoop < savesToShow; saveLoop++)
            {
                SavedGame newSave = SavedGame.LoadGame(saveLoop);
                if (newSave == null)
                {
                    newSave = new SavedGame();
                }
                savedGames.Add(newSave);
                UnityEngine.UI.Image newPanel = (UnityEngine.UI.Image)Object.Instantiate(savePanel, transform);
                newPanel.rectTransform.anchoredPosition = new Vector2(0, -60 - 110 * saveLoop);
                setUpPanel(newSave, newPanel);
                savePanels.Add(newPanel);
            }
        } else
        {
            // If there isn't a panel to render, just load the save in the current slot
            SavedGame newSave = SavedGame.LoadGame(SavedGame.saveSlot);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

    private void OnGUI()
    {
        for (int saveLoop = 0; saveLoop < savePanels.Count; saveLoop++)
        {
            savePanels[saveLoop].color = new Color(1, 1, 1, saveLoop == SavedGame.saveSlot ? 1 : 0.5f);
        }

        if (deleting)
        {
            GUI.ModalWindow(savesToShow, new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.6f), deleteWindow, "Confirm Delete");
        }
    }

    private void deleteWindow(int id)
    {
        GUI.Label(new Rect(10, 20, Screen.width * 0.6f - 10, Screen.height * 0.6f - 20), "Are you sure you want to delete this save?");
    }

    private void setUpPanel(SavedGame save, UnityEngine.UI.Image panel)
    {
        panel.transform.Find("DoubleJump")?.gameObject.SetActive(save.hasDoubleJump);
        panel.transform.Find("AirDash")?.gameObject.SetActive(save.hasAirDash);
        panel.transform.Find("DownStab")?.gameObject.SetActive(save.hasDownStab);
        panel.transform.Find("HighJump")?.gameObject.SetActive(save.hasHighJump);
        panel.transform.Find("EnergyAbsorb")?.gameObject.SetActive(save.hasEnergyAbsorb);
        panel.transform.Find("WallClimb")?.gameObject.SetActive(save.hasWallClimb);
        panel.transform.Find("Uppercut")?.gameObject.SetActive(save.hasUppercut);
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (deleting)
            {
                SavedGame.DeleteSave(SavedGame.saveSlot);
                savedGames[SavedGame.saveSlot] = new SavedGame();
                setUpPanel(savedGames[SavedGame.saveSlot], savePanels[SavedGame.saveSlot]);
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
