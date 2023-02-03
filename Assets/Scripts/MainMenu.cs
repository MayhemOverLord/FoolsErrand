using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenu : MonoBehaviour {
    public TextMeshProUGUI nametext;
    public GameObject MainMenuPanel;
    public GameObject NameMenuPanel;
    public GameObject IntroPanel;
    public string username;
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
        Debug.Log(username);
        SceneManager.LoadScene("GamePlay");
    }
    public void NameInput() {
        Debug.Log(nametext.text.Length);
        if (nametext.text.Length>8) {
            nametext.text=nametext.text.Remove(nametext.text.Length-1);
            Debug.Log(nametext.text.Length);
        }
    }
    void start(){
        MainMenuPanel.SetActive(true);
        NameMenuPanel.SetActive(false);
        IntroPanel.SetActive(false);
    }
    void update(){

    }
}
//MAKE ANOTHER FUNCTION FOR TESTING ON NAME INPUT TO LIMIT THE SIZE OF TEXT