using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class OnClickPlay : MonoBehaviour
{
 [Header("Cameras")]
 // Comment because of false logic (Switched to canvas alpha)
 //[SerializeField] private Camera UI_MainMenu_Camera;
 [SerializeField] private Camera Gameplay_Camera;

 [Header("UI")]
 [SerializeField] private Button Play_Button;
 [SerializeField] private Canvas UI;

 [Header("Inputs")]
 [SerializeField] private TMP_Dropdown Dropdown_Menu;
 [SerializeField] private TMP_InputField Input_Field;

 private void Start()
 {
    Button UI_MainMenu_Play = Play_Button.GetComponent<Button>();
    UI_MainMenu_Play.onClick.AddListener(TaskOnClick);
 }

 private void Update()
 {

 }

 void TaskOnClick(){
	// Comment because of false logic (Switched to canvas alpha)
	 //UI_MainMenu_Camera.enabled = false;
	 //Gameplay_Camera.enabled = true;
    
    UI.enabled = false;
 }
}