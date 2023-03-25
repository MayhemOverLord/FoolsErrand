using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class NPC : MonoBehaviour {
    public List<string> Dialogue;
    public string PostDialogue;
    public string Type;
    public int DialogueSteps;
    public List<int[]> Boundaries;
    public void initNPC(List<string> Dia, string Post, string Typ, int[] coords) {
        Dialogue=Dia;
        PostDialogue=Post;
        Type=Typ;
        DialogueSteps=-1;
        if(Type=="BOSS"){
            Boundaries = new List<int[]>{
                new int[2]{coords[0]-1,coords[1]},
                new int[2]{coords[0],coords[1]},
                new int[2]{coords[0]+1,coords[1]},
                new int[2]{coords[0]-1,coords[1]+1},
                new int[2]{coords[0],coords[1]+1},
                new int[2]{coords[0]+1,coords[1]+1},
            };
        }
        else{
            Boundaries = new List<int[]>{new int[2]{coords[0],coords[1]}};
        }
    }
    public string DiaRead(){
        DialogueSteps++;
        if(DialogueSteps>=Dialogue.Count){
            DialogueSteps=-1;
            return PostDialogue;
        }
        return Dialogue[DialogueSteps];
    }
}