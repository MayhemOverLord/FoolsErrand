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
    public int gridsize = 14;
    public Transform goal;
    private int[,] gridstore;
    public bool squaregen = false;
    private int[] PlayerLocation = new int[2];
    private int direction;
    private List<int> alltiles = new List<int>{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44};
    private List<int> squaretiles = new List<int>{0,1,5,9,13,17,19,21,23,25};
    private bool[,] tilemove = { //The set of data that checks whether a player can move in each direction
        {false,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,false,true,false,false,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true},
        {false,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,false,false,false,false,false,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true}
    };
    private int[,] connections = { //Set of direction connection groups for each tile
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
    //Set of connections for more dynamic generation
    public List<List<List<int>>> connectClassify = new List<List<List<int>>>{
    new List<List<int>>{new List<int>{1,5,32,33,36},new List<int>{1,9,33,34,37},new List<int>{1,13,34,35,38},new List<int>{1,17,32,35,39}}, //Empty
    new List<List<int>>{new List<int>{2,15,17,19,35,39,43},new List<int>{5,6,21,32,36,40},new List<int>{7,9,10,23,33,37,41},new List<int>{11,13,14,25,34,38,42}}, //Left
    new List<List<int>>{new List<int>{3,6,9,21,34,37,40},new List<int>{7,10,13,23,35,38,41},new List<int>{11,14,17,25,32,39,42},new List<int>{5,15,19,33,36,43}}, //Right
    new List<List<int>>{new List<int>{4,7,8,14,16,18,20,28,31,38,41,42,44},new List<int>{2,4,8,11,12,20,22,29,30,39,42,43,44},new List<int>{6,8,12,22,15,16,24,26,31,36,40,43,44},new List<int>{3,4,10,12,16,18,24,27,30,37,40,41,44}}, //Both
    new List<List<int>>{new List<int>{0,10,11,12,13,22,23,24,25,26,27,29,30},new List<int>{0,14,15,16,17,18,19,24,25,26,27,28,31},new List<int>{0,2,3,4,5,18,19,20,21,26,27,28,30},new List<int>{0,6,7,8,9,20,21,22,23,26,28,29,31}}, //Wall
    new List<List<int>>{new List<int>{1,5,32,33,36,5,32,33,36},new List<int>{1,9,33,34,37,9,33,34,37},new List<int>{1,13,34,35,38,13,34,35,38},new List<int>{1,17,32,35,39,17,32,35,39}}
    };
    //Set of connections for more robust/square generation
    public List<List<List<int>>> connectDepoint = new List<List<List<int>>>{
    new List<List<int>>{new List<int>{1,5},new List<int>{1,9},new List<int>{1,13},new List<int>{1,17}}, //Empty
    new List<List<int>>{new List<int>{17,19},new List<int>{5,21},new List<int>{9,23},new List<int>{13,25}}, //Left
    new List<List<int>>{new List<int>{9,21},new List<int>{13,23},new List<int>{17,25},new List<int>{5,19}}, //Right
    new List<List<int>>{new List<int>{4,7,8,14,16,18,20},new List<int>{2,4,8,11,12,20,22},new List<int>{12,22,24},new List<int>{3,4,10,12,16,18,24}}, //Both
    new List<List<int>>{new List<int>{0,13,23,25},new List<int>{0,17,19,25},new List<int>{0,5,19,21},new List<int>{0,9,21,23}} //Wall
    };
    public void GridDisplay() {
        bool validgen = false;
        int counter = 0;
        while (!validgen) //Runs grid generation until a valid grid has been generated
        {
            GridGen();
            validgen = GridCheck();
            if(counter>=10000){
                break;
            }
            counter = counter+1;
        }
        //Outputs the tile display
        tilemap.ClearAllTiles();
        for (int y=0;y<gridsize;y++){
            for (int x=0;x<gridsize;x++){
                tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[gridstore[y,x]]);
            }
        }
        //Sets player location to the centre of the screen
        //transform.position = new Vector3Int(gridsize*-1,gridsize*-1, 0);
        //goal.position = new Vector3Int(gridsize*-1,gridsize*-1, 0);
        transform.position = new Vector3Int(-2,-2, 0);
        goal.position = new Vector3Int(-2,-2, 0);
        direction = 0;
        PlayerLocate();
    }
    private void GridStart() //Sets the initial values for the dungeon tile grid
    {
        gridstore = new int[gridsize, gridsize];
        for (int i = 0; i < gridsize; i++)
        {
            for (int j = 0; j < gridsize; j++)
            {
                if ((i <= 0) || (j <= 0) || (i >= gridsize - 1) || (j >= gridsize - 1))
                {
                    gridstore[i, j] = 0; //Solid tiles
                }
                else
                {
                    gridstore[i, j] = 100; //Unclassified tiles
                }
            }
        }
    }
    private bool GridCheck() //Checks whether the grid has been successfully created or lacks tiles.
    {
        for (int i = 0; i < gridsize; i++)
        {
            for (int j = 0; j < gridsize; j++)
            {
                if (gridstore[i, j] == 100) //If tile hasn't been classified
                {
                    return false;
                }
            }
        }
        return true;
    }
    private List<List<int>> RoomCentFinder() {
        List<List<int>> trcorners = new List<List<int>>();
        List<List<int>> blcorners = new List<List<int>>();
        List<List<int>> roomcents = new List<List<int>>();
        for (int i = 0; i < gridsize; i++)
        {
            for (int j = 0; j < gridsize; j++)
            {
                if(gridstore[i,j]==21){
                    trcorners.Add(new List<int>{i,j});
                }
            }
        }
        for(int i = 0; i<trcorners.Count;i++){
            List<int> coordinate = new List<int>{trcorners[i][0],trcorners[i][1]};
            bool finding = true;
            bool moveleft = true;
            while (finding){
                while (moveleft){
                    coordinate[1]=coordinate[1]-1;
                    if(gridstore[coordinate[0],coordinate[1]]==19){
                        moveleft=false;
                    }
                }
                coordinate[0]=coordinate[0]-1;
                if(gridstore[coordinate[0],coordinate[1]]==25){
                    finding=false;
                    blcorners.Add(coordinate);
                }
            }
            roomcents.Add(new List<int>{((int)((trcorners[i][0]+blcorners[i][0])/2d)),((int)((trcorners[i][1]+blcorners[i][1])/2d))});
        }
        return roomcents;
    }
    private void DoorMaker(){
        List<List<int>> cents = RoomCentFinder();
    }
    private void GridGen() { //Function for the generation of the dungeon layout
        //Preparation of initial variables
        GridStart();
        int[] coordinate = new int[2]{gridsize/2,gridsize/2};
        List<int> queue = new List<int>{
            coordinate[0]*gridsize+coordinate[1]
        };
        List<List<int>> carry = new List<List<int>>{
            new List<int>{coordinate[0],coordinate[1]}
        };
        Random rando = new Random();
        int roommod=(gridsize/5)+1;
        for (int i = 0; i < roommod;i++){
            gridstore[rando.Next(1,gridsize-3),rando.Next(1,gridsize-3)]=21;
        }
        bool running=true;
        int increment=0;
        while(running){
            List<int> hold = new List<int>();
            List<int> options = alltiles;
            if(squaregen){
                options=squaretiles;
            }
            if(coordinate[0]>0){
                hold = new List<int>{coordinate[0]-1,coordinate[1]};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                //Checks options based on tile from the south
                if(gridstore[hold[0],hold[1]]!=100){
                    options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 0]][0]).ToList();
                }
            }
            if(coordinate[0]<gridsize-1){
                hold = new List<int>{coordinate[0]+1,coordinate[1]};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                //Checks options based on tile from the north
                if(gridstore[hold[0],hold[1]]!=100){
                    options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 2]][2]).ToList();
                }
            }
            if(coordinate[1]>0){
                hold = new List<int>{coordinate[0],coordinate[1]-1};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                //Checks options based on tile from the west
                if(gridstore[hold[0],hold[1]]!=100){
                    options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 1]][1]).ToList();
                }
            }
            if(coordinate[1]<gridsize-1){
                hold = new List<int>{coordinate[0],coordinate[1]+1};
                if(!queue.Contains(hold[0]*gridsize+hold[1])){
                    queue.Add(hold[0]*gridsize+hold[1]);
                    carry.Add(hold);
                }
                //Checks options based on tile from the east
                if(gridstore[hold[0],hold[1]]!=100){
                    options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 3]][3]).ToList();
                }
            }
            if(gridstore[coordinate[0],coordinate[1]]==100){
                if (options.Count > 0)
                {
                    Random rand = new Random();
                    gridstore[coordinate[0], coordinate[1]] = options[rand.Next(options.Count() - 1)];
                }
            }
            if(increment==queue.Count-1){
                running=false;
            }
            else{
                increment=increment+1;
                coordinate=carry[increment].ToArray();
            }
        }
    }
    private void GridGen2() { //Function for the generation of the dungeon layout
        //Preparation of initial variables
        GridStart();
        Random rando = new Random();
        int roommod=gridsize/3;
        for (int i = 0; i < roommod;i++){
            gridstore[rando.Next(2,gridsize-2),rando.Next(2,gridsize-2)]=21;
        }
        int[] coordinate = new int[2]{gridsize/2,gridsize/2};
        bool running=true;
        while(running){
            List<int> hold = new List<int>();
            List<int> options = alltiles;
            List<int> bestop = alltiles;
            if(squaregen){
                options=squaretiles;
            }
            int checker = 0;
            for (int i = 1; i<gridsize-1;i++){
                for (int j = 1; j<gridsize-1;j++){
                    if(gridstore[i,j]==100){
                        checker=checker+1;
                        if(squaregen){
                            options=squaretiles;
                        }
                        else{
                            options=alltiles;
                        }
                        if(i>0){
                            hold = new List<int>{i-1,j};
                            //Checks options based on tile from the south
                            if(gridstore[hold[0],hold[1]]!=100){
                                options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 0]][0]).ToList();
                            }
                        }
                        if(i<gridsize-1){
                            hold = new List<int>{i+1,j};
                            //Checks options based on tile from the north
                            if(gridstore[hold[0],hold[1]]!=100){
                                options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 2]][2]).ToList();
                            }
                        }
                        if(j>0){
                            hold = new List<int>{i,j-1};
                            //Checks options based on tile from the west
                            if(gridstore[hold[0],hold[1]]!=100){
                                options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 1]][1]).ToList();
                            }
                        }
                        if(j<gridsize-1){
                            hold = new List<int>{i,j+1};
                            //Checks options based on tile from the east
                            if(gridstore[hold[0],hold[1]]!=100){
                                options = options.Intersect(connectClassify[connections[gridstore[hold[0], hold[1]], 3]][3]).ToList();
                            }
                        }
                        if(bestop.Count>options.Count){
                            bestop=options;
                            coordinate=new int[2]{i,j};
                        }
                    }
                }
            }
            Debug.Log("hehe");
            if(gridstore[coordinate[0],coordinate[1]]==100){
                Debug.Log("I got here");
                if (bestop.Count > 0)
                {
                    Random rand = new Random();
                    gridstore[coordinate[0], coordinate[1]] = bestop[rand.Next(bestop.Count() - 1)];
                }
                else{
                    running=false;
                }
            }
            if(checker==0){
                running=false;
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
        //Movement works through moving a goal object then sliding the tilemap towards it
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,goal.position) == 0f){
            if(Input.GetAxisRaw("Horizontal")!=0f){
                if(Input.GetAxisRaw("Horizontal")==1f){
                    //Moving right
                    if(MoveAllow(1,PlayerLocate())){
                        goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                        direction=1;
                    }
                }
                if(Input.GetAxisRaw("Horizontal")==-1f){
                    //Moving left
                    if(MoveAllow(3,PlayerLocate())){
                        goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                        direction=3;
                    }
                }
            }
            else if(Input.GetAxisRaw("Vertical")!=0f){
                if(Input.GetAxisRaw("Vertical")==1f){
                    //Moving up
                    if(MoveAllow(0,PlayerLocate())){
                        goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                        direction=0;
                    }
                }
                if(Input.GetAxisRaw("Vertical")==-1f){
                    //Moving down
                    if(MoveAllow(2,PlayerLocate())){
                        goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                        direction=2;
                    }
                }
            }
        }
        //This will be the section for player interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch(direction){
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            //Interaction
        }
    }
}