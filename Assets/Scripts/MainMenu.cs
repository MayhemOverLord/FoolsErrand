using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour {
    public TextMeshProUGUI nametext;
    public GameObject MainMenuPanel;
    public GameObject NameMenuPanel;
    public GameObject IntroPanel;
    public GameObject NameButton;
    static public string username;
    
    //Changes to the new game screen
    public void NewGame() {
        MainMenuPanel.SetActive(false);
        NameMenuPanel.SetActive(true);
    }

    //An empty function for if load game functionality was implemented
    public void LoadGame() {
        
    }

    //Closes the game
    public void QuitGame() {
        Application.Quit();
    }

    //Confirms the player's name and saves it
    public void NameConfirm() {
        username = nametext.text;
        IntroPanel.SetActive(true);
    }

    //Loads the main game once plot is read
    public void StoryRead(){
        SceneManager.LoadScene("GamePlay");
    }

    //Checks that the input player name is valid
    public void NameInput() {
        if(nametext.text.Length<2){
            NameButton.SetActive(false);
        }
        else{
            NameButton.SetActive(true);
        }
    }

    //Prepares the game upon starting
    void Start(){
        MainMenuPanel.SetActive(true);
        NameMenuPanel.SetActive(false);
        IntroPanel.SetActive(false);
    }

    void Update(){
        
    }
}