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

    public void NewGame() {
        MainMenuPanel.SetActive(false);
        NameMenuPanel.SetActive(true);
    }

    public void LoadGame() {
        
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void NameConfirm() {
        username = nametext.text;
        IntroPanel.SetActive(true);
    }
    public void StoryRead(){
        SceneManager.LoadScene("GamePlay");
    }

    public void NameInput() {
        if(nametext.text.Length<2){
            NameButton.SetActive(false);
        }
        else{
            NameButton.SetActive(true);
        }
    }

    void Start(){
        MainMenuPanel.SetActive(true);
        NameMenuPanel.SetActive(false);
        IntroPanel.SetActive(false);
    }

    void Update(){
        
    }
}