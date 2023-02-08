using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
public class GameManager : MonoBehaviour{
    public Character Player;
    public List<List<int>> gridlayout = new List<List<int>>();
    public Sprite[] sprites;
    public Tilemap tilemap;
    public void GridDisplay() {
        tilemap.ClearAllTiles();
        for (int y=0; y<gridlayout.Count;y++){
            for (int x=0; x<gridlayout[y].Count;x++){
            }
        }
    }
    public void GridGen() {

    }
}