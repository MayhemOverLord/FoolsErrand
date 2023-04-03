using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class Character : MonoBehaviour {
    public string charname;
    public int level;
    public int experience;
    public int maxhealth;
    public int currhealth;
    public int maxmana;
    public int currmana;
    public int strength;
    public int agility;
    public int constitution;
    public int defence;
    public int intelligence;
    public int wisdom;
    public int direction;
    public int spriteoffset;
    public List<List<int>> roomvisiting;
    public List<List<string>> statuses;
    public GameObject goal;
    public Sprite[] images;
    public void initCharacter(string nam, int lev, int str, int agi, int con, int def, int inte, int wis, int offset) {
        charname=nam;
        level=lev;
        experience=0;
        maxhealth = 10*con;
        currhealth = maxhealth;
        maxmana = 5*wis;
        currmana = maxmana;
        strength=str;
        agility=agi;
        constitution=con;
        defence=def;
        intelligence=inte;
        wisdom=wis;
        statuses= new List<List<string>>();
        spriteoffset=offset;
        ChangeDirection(0);
        gameObject.GetComponent<SpriteRenderer>().sprite=images[spriteoffset];
    }
    
    public int ChangeStat(string stat, int change) {
        switch(stat){
            case "str":
                strength=strength+change;
                return strength;
            case "agi":
                agility=agility+change;
                return agility;
            case "con":
                constitution=constitution+change;
                currhealth=currhealth+change*10;
                maxhealth=maxhealth+change*10;
                return constitution;
            case "def":
                defence=defence+change;
                return defence;
            case "inte":
                intelligence=intelligence+change;
                return intelligence;
            case "wis":
                wisdom=wisdom+change;
                currmana=currmana+change*5;
                maxmana=maxmana+change*5;
                return wisdom;           
        }
        return 9999999;
    }

    public int Heal(int value) {
        if (currhealth+value>maxhealth){
            currhealth=maxhealth;
        }
        else{
            currhealth=currhealth+value;
        }
        return currhealth;
    }

    public void ChangeDirection(int dire){
        direction=dire;
    }

    public int Harm(int value) {
        if (currhealth-value<0){
            currhealth=0;
        }
        else{
            currhealth=currhealth-value;
        }
        return currhealth;
    }

    public void ManaRegen(){
        currmana = currmana+(int)maxmana/50;
        if(currmana>maxmana){
            currmana=maxmana;
        }
    }
}