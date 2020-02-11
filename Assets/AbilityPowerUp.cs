using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();
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
            GUI.ModalWindow(0, new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.6f), windowFunc, title);
        }
    }

    private void windowFunc(int id)
    {
        GUI.Label(new Rect(10, 20, Screen.width * 0.6f - 10, Screen.height * 0.6f - 10), description);
    }

    // Update is called once per frame
    void Update()
    {
        if (showTip)
        {
            if (playerInput != null && playerInput.actions["Submit"].triggered && Time.unscaledTime >= tipStart + tipDelay)
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
