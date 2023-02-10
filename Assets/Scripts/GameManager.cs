using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour{
    public Character Player;
    public List<List<int>> gridlayout;
    public Tile[] tiles;
    public Tilemap tilemap;
    public float speed = 4;
    public Transform goal;
    public int[] PlayerLocation = new int[2];
    public bool[,] tilemove = {
        {false,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,false,false,false,false,true},
        {false,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,false,true,false,false,true,false},
        {false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,false,false,true},
        {false,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,false,false,false,false,false,true,true,false}
    };

    public void GridDisplay() {
        gridlayout = new List<List<int>>();
        gridlayout.Add(new List<int>{25,13,13,13,13,23});
        gridlayout.Add(new List<int>{17,1,1,1,1,9});
        gridlayout.Add(new List<int>{17,1,1,1,1,9});
        gridlayout.Add(new List<int>{19,5,5,5,5,21});
        tilemap.ClearAllTiles();
        for (int y=0; y<gridlayout.Count; y++){
            for (int x=0; x<gridlayout[y].Count; x++){
                tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[gridlayout[y][x]]);
            }
        }

        PlayerLocate();
    }

    public void GridGen() {

    }

    public int[] PlayerLocate() {
        int[] PlayerLocation = {(int)tilemap.transform.position[0]/-2, (int)tilemap.transform.position[1]/-2};
        return PlayerLocation;
    }

    public bool MoveAllow(int direction, int[] location) {
        return tilemove[direction, gridlayout[location[1]][location[0]]];
    }

    void Start() {
        goal.SetParent(null);
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,goal.position) == 0f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal"))==1f){
                if(Input.GetAxisRaw("Horizontal")==1f){
                    if(MoveAllow(1,PlayerLocate())){
                        goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                    }
                }
                if(Input.GetAxisRaw("Horizontal")==-1f){
                    if(MoveAllow(3,PlayerLocate())){
                        goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                    }
                }
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical"))==1f){
                if(Input.GetAxisRaw("Vertical")==1f){
                    if(MoveAllow(0,PlayerLocate())){
                        goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                    }
                }
                if(Input.GetAxisRaw("Vertical")==-1f){
                    if(MoveAllow(2,PlayerLocate())){
                        goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Interaction
        }
    }
}