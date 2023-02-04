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
    public Character(string nam, int str, int agi, int con, int def, int inte, int wis) {
        charname=nam;
        level=1;
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
                return constitution;
            case "def":
                defence=defence+change;
                return defence;
            case "inte":
                intelligence=intelligence+change;
                return intelligence;
            case "wis":
                wisdom=wisdom+change;
                return wisdom;           
        }
        return 9999999;
    }
    public int heal(int value) {
        if (currhealth+value>maxhealth){
            currhealth=maxhealth;
        }
        else{
            currhealth=currhealth+value;
        }
        return currhealth;
    }
    public int harm(int value) {
        if (currhealth-value<0){
            currhealth=0;
        }
        else{
            currhealth=currhealth-value;
        }
        return currhealth;
    }
}