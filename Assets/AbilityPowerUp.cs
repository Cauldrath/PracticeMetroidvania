using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPowerUp : MonoBehaviour
{
    public string title;
    public string description;
    public string abilityFieldName;
    public float tipDelay = 1.0f;
    public PlayerScript player;

    private System.Reflection.FieldInfo abilityField;
    private bool showTip = false;
    private float tipStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (abilityFieldName != null && player != null && player.saveData != null)
        {

            System.Type type = player.saveData.GetType();
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.Name == abilityFieldName)
                {
                    abilityField = field;
                    break;
                }
            }
        }
    }

    private void OnGUI()
    {
        if (showTip)
        {
            GUI.ModalWindow(0, new Rect(100, 100, 300, 200), windowFunc, title);
        }
    }

    private void windowFunc(int id)
    {
        GUI.Label(new Rect(10, 20, 280, 190), description);
    }

    // Update is called once per frame
    void Update()
    {
        if (showTip)
        {
            if (Input.GetButtonDown("Submit") && Time.unscaledTime >= tipStart + tipDelay)
            {
                showTip = false;
                Time.timeScale = 1;
            }
        } else
        {
            if (player != null && abilityField != null)
            {
                gameObject.SetActive((bool)abilityField.GetValue(player.saveData) == false);
            }
        }
    }

    void CloseTip()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerScript>() == player)
        {
            abilityField.SetValue(player.saveData, true);
            showTip = true;
            tipStart = Time.unscaledTime;
            Time.timeScale = 0;
            player.SaveGame();
        }
    }
}
