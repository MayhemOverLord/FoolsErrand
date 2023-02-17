using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using Random = System.Random;

public class GameManager : MonoBehaviour{
    public Character Player;
    public Tile[] tiles;
    public Tilemap tilemap;
    public float speed = 4;
    public int gridsize = 10;
    public Transform goal;
    public int[,] gridstore;
    public int[] PlayerLocation = new int[2];
    public List<int> alltiles = new List<int>{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44};
    public bool[,] tilemove = {
        {false,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,false,true,false,false,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,false,false,false,false,false,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true}
    };
    public int[,] connections = {
        {4,4,4,4}, //0 N
        {0,0,0,0}, //1 A
        {4,1,2,3}, //2 B1
        {4,3,1,2}, //3 B2
        {4,3,3,3}, //4 B3
        {4,1,0,2}, //5 B
        {3,4,1,2}, //6 C1
        {2,4,3,1}, //7 C2
        {3,4,3,3}, //8 C3
        {2,4,1,0}, //9 C
        {2,3,4,1}, //10 D1
        {1,2,4,3}, //11 D2
        {3,3,4,3}, //12 D3
        {0,2,4,1}, //13 D
        {1,2,3,4}, //14 E1
        {3,1,2,4}, //15 E2
        {3,3,3,4}, //16 E3
        {1,0,2,4}, //17 E
        {4,3,3,4}, //18 F1
        {4,1,2,4}, //19 F
        {4,4,3,3}, //20 G1
        {4,4,1,2}, //21 G
        {3,4,4,3}, //22 H1
        {2,4,4,1}, //23 H
        {3,3,4,4}, //24 I1
        {1,2,4,4}, //25 I
        {3,4,4,4}, //26 J
        {4,3,4,4}, //27 K
        {4,4,3,4}, //28 L
        {4,4,4,3}, //29 M
        {4,3,4,3}, //30 O
        {3,4,3,4}, //31 P
        {1,0,0,2}, //32 Q1
        {2,1,0,0}, //33 Q2
        {0,2,1,0}, //34 Q3
        {0,0,2,1}, //35 Q4
        {3,1,0,2}, //36 Q5
        {2,3,1,0}, //37 Q6
        {0,2,3,1}, //38 Q7
        {1,0,2,3}, //39 Q8
        {3,3,1,2}, //40 Q9
        {2,3,3,1}, //41 Q10
        {1,2,3,3}, //42 Q11
        {3,1,2,3}, //43 Q12
        {3,3,3,3} //44 Q13
    };
    public List<List<List<int>>> connectClassify = new List<List<List<int>>>{
    new List<List<int>>{new List<int>{1,5,32,33,36},new List<int>{1,9,33,34,37},new List<int>{1,13,34,35,38},new List<int>{1,17,32,35,39}}, //Empty
    new List<List<int>>{new List<int>{2,15,17,19,35,39,43},new List<int>{5,6,21,32,36,40},new List<int>{7,9,10,23,33,37,41},new List<int>{11,13,14,25,34,38,42}}, //Left
    new List<List<int>>{new List<int>{3,6,9,21,34,37,40},new List<int>{7,10,13,23,35,38,41},new List<int>{11,14,17,25,32,39,42},new List<int>{5,15,19,33,36,43}}, //Right
    new List<List<int>>{new List<int>{4,7,8,14,16,18,20,28,31,38,41,42,44},new List<int>{2,4,8,11,12,20,22,29,30,39,42,43,44},new List<int>{12,22,24,26,31,36,40,43,44},new List<int>{3,4,10,12,16,18,24,27,30,37,40,41,44}}, //Both
    new List<List<int>>{new List<int>{0,10,11,12,13,22,23,24,25,26,27,29,30},new List<int>{0,14,15,16,17,18,19,24,25,26,27,28,31},new List<int>{0,2,3,4,5,18,19,20,21,26,27,28,30},new List<int>{0,6,7,8,9,20,21,22,23,26,28,29,31}} //Wall
    };
    public void GridDisplay() {
        tilemap.ClearAllTiles();
        GridGen();
        for (int y=0;y<gridsize;y++){
            for (int x=0;x<gridsize;x++){
                Debug.Log(x.ToString()+" , "+y.ToString());
                Debug.Log(tiles[gridstore[y,x]].ToString());
                tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[gridstore[y,x]]);
            }
        }
        PlayerLocate();
    }
    public void GridGen() {
        gridstore = new int[gridsize,gridsize];
        for(int i=0;i<gridsize;i++){
            for(int j=0;j<gridsize;j++){
                if((i<=2)||(j<=2)||(i>=gridsize-3)||(j>=gridsize-3)){
                //    gridstore[i,j]=0;
                    gridstore[i,j]=100;
                }
                else{
                    gridstore[i,j]=100;
                }
            }
        }
        int[] coordinate = new int[2]{gridsize/2,gridsize/2};
        List<int> queue = new List<int>{
            coordinate[0]*gridsize+coordinate[1]
        };
        List<List<int>> carry = new List<List<int>>{
            new List<int>{coordinate[0],coordinate[1]}
        };
        gridstore[gridsize/2,gridsize/2]=21;
        bool running=true;
        int increment=0;
        while(running){
            List<int> hold = new List<int>();
            Debug.Log(queue.Count);
            List<int> options = alltiles;
            if(coordinate[0]>0){
                hold = new List<int>{coordinate[0]-1,coordinate[1]};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                if(gridstore[hold[0],hold[1]]!=100){
                    options=options.Intersect(connectClassify[connections[gridstore[hold[0],hold[1]],1]][1]).ToList();
                }
            }
            if(coordinate[0]<gridsize-1){
                hold = new List<int>{coordinate[0]+1,coordinate[1]};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                if(gridstore[hold[0],hold[1]]!=100){
                    options=options.Intersect(connectClassify[connections[gridstore[hold[0],hold[1]],3]][3]).ToList();
                }
            }
            if(coordinate[1]>0){
                hold = new List<int>{coordinate[0],coordinate[1]-1};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                if(gridstore[hold[0],hold[1]]!=100){
                    options=options.Intersect(connectClassify[connections[gridstore[hold[0],hold[1]],0]][0]).ToList();
                }
            }
            if(coordinate[1]<gridsize-1){
                hold = new List<int>{coordinate[0],coordinate[1]+1};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                if(gridstore[hold[0],hold[1]]!=100){
                    options=options.Intersect(connectClassify[connections[gridstore[hold[0],hold[1]],2]][2]).ToList();
                }
            }
            if (options.Count<1){
                Debug.Log(coordinate[0]);
                Debug.Log(coordinate[1]);
            }
            if(gridstore[coordinate[0],coordinate[1]]==100){
                Random rand = new Random();
                gridstore[coordinate[0],coordinate[1]]=options[rand.Next(options.Count()-1)];
            }
            if(increment==queue.Count-1){
                running=false;
            }
            else{
                increment=increment+1;
                coordinate=carry[increment].ToArray();
                Debug.Log(queue.Count);
            }
        }
    }
    public int[] PlayerLocate() {
        int[] PlayerLocation = {(int)tilemap.transform.position[0]/-2, (int)tilemap.transform.position[1]/-2};
        return PlayerLocation;
    }

    public bool MoveAllow(int direction, int[] location) {
        return tilemove[direction, gridstore[location[1],location[0]]];
    }

    void Start() {
        goal.SetParent(null);
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,goal.position) == 0f){
            if(Input.GetAxisRaw("Horizontal")!=0f){
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
            else if(Input.GetAxisRaw("Vertical")!=0f){
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