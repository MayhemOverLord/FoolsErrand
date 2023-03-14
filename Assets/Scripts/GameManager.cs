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
    public int enemycount = 4;
    public Transform goal;
    private int[,] gridstore;
    public bool squaregen = true;
    private int[] PlayerLocation = new int[2];
    private int direction;
    private int depth = 0;
    private List<List<int>> trcorners;
    private List<List<int>> blcorners;
    private int[] laddercord;
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
    private List<List<List<int>>> connectClassify = new List<List<List<int>>>{
    new List<List<int>>{new List<int>{1,5,32,33,36},new List<int>{1,9,33,34,37},new List<int>{1,13,34,35,38},new List<int>{1,17,32,35,39}}, //Empty
    new List<List<int>>{new List<int>{2,15,17,19,35,39,43},new List<int>{5,6,21,32,36,40},new List<int>{7,9,10,23,33,37,41},new List<int>{11,13,14,25,34,38,42}}, //Left
    new List<List<int>>{new List<int>{3,6,9,21,34,37,40},new List<int>{7,10,13,23,35,38,41},new List<int>{11,14,17,25,32,39,42},new List<int>{5,15,19,33,36,43}}, //Right
    new List<List<int>>{new List<int>{4,7,8,14,16,18,20,28,31,38,41,42,44},new List<int>{2,4,8,11,12,20,22,29,30,39,42,43,44},new List<int>{6,8,12,22,15,16,24,26,31,36,40,43,44},new List<int>{3,4,10,12,16,18,24,27,30,37,40,41,44}}, //Both
    new List<List<int>>{new List<int>{0,10,11,12,13,22,23,24,25,26,27,29,30},new List<int>{0,14,15,16,17,18,19,24,25,26,27,28,31},new List<int>{0,2,3,4,5,18,19,20,21,26,27,28,30},new List<int>{0,6,7,8,9,20,21,22,23,26,28,29,31}}, //Wall
    new List<List<int>>{new List<int>{1,5,32,33,36,5,32,33,36},new List<int>{1,9,33,34,37,9,33,34,37},new List<int>{1,13,34,35,38,13,34,35,38},new List<int>{1,17,32,35,39,17,32,35,39}}
    };
    public GameObject enemyprefab;
    private List<GameObject> enemies;
    public GameObject popupwindow;
    public TextMeshProUGUI popuptext;
    public GameObject popupops;
    public GameObject descendops;
    public GameObject battleops;
    private bool popped=false;
    //Function for displaying the dungeon layout
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
        DoorMaker();
        if(enemies!=null){
            for (int i =enemies.Count-1;i>=0;i--){
                Destroy(enemies[i].GetComponent<Character>().goal);
                Destroy(enemies[i]);
            }
        }
        enemies = new List<GameObject>();
        //Outputs the tile display
        tilemap.ClearAllTiles();
        LadderMake();
        for (int y=0;y<gridsize;y++){
            for (int x=0;x<gridsize;x++){
                tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[gridstore[y,x]]);
                if (y==laddercord[0] & x == laddercord[1]){
                    tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[45]);
                }
            }
        }
        PlayerLocation = PlayerSpawn().ToArray();
        transform.position = new Vector3Int(-2*PlayerLocation[1],-2*PlayerLocation[0], 0);
        goal.position = new Vector3Int(-2*PlayerLocation[1],-2*PlayerLocation[0], 0);
        direction = 0;
        EnemySpawn();
    }
    //Initialises the dungeon layout storage with a valid base
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
    private void LadderMake(){
        List<List<int>> Validtiles = new List<List<int>>();
        for (int y=1; y<gridsize-1;y++){
            for (int x=1; x<gridsize-1;x++){
                if(gridstore[y,x]==1){
                    Validtiles.Add(new List<int>{y,x});
                }
            }
        }
        Random rando = new Random();
        int hold = rando.Next(0,Validtiles.Count-1);
        laddercord = new int[2]{Validtiles[hold][0],Validtiles[hold][1]};
    }
    private int[] PlayerSpawn(){
        List<List<int>> Validtiles = new List<List<int>>();
        for (int i=1; i<gridsize-1;i++){
            for (int j=1; j<gridsize-1;j++){
                if (gridstore[i,j]!=0){
                    Validtiles.Add(new List<int>{i,j});
                }
            }
        }
        Random rando = new Random();
        int hold = rando.Next(0,Validtiles.Count-1);
        return new int[2]{Validtiles[hold][0],Validtiles[hold][1]};
    }
    private void EnemySpawn(){
        if(enemies.Count<enemycount){
            List<int> rooms = new List<int>();
            int playroom = RoomFinder(PlayerLocation);
            for (int i=0;i<trcorners.Count;i++){
                if (playroom!=i){
                    rooms.Add(i);
                }
            }
            int enemyroom = 0;
            Random rando = new Random();
            int [] enemylocate= new int[]{0,0};
            bool finding=true;
            int counter=0;
            while(finding){
                finding=false;
                enemyroom = rooms[rando.Next(0,rooms.Count-1)];
                enemylocate = new int[]{rando.Next(blcorners[enemyroom][1],trcorners[enemyroom][1]),rando.Next(blcorners[enemyroom][0],trcorners[enemyroom][0])};
                for (int i=0;i<enemies.Count;i++){
                    if((enemies[i].transform.position.x-tilemap.transform.position.x)/2==enemylocate[0] & (enemies[i].transform.position.y-tilemap.transform.position.y)/2==enemylocate[1]){
                        finding=true;
                    }
                    if((enemies[i].transform.position.x-tilemap.transform.position.x)/2==PlayerLocation[1] & (enemies[i].transform.position.y-tilemap.transform.position.y)/2==PlayerLocation[0]){
                        finding=true;
                    }
                }
                counter=counter+1;
                if(counter>=100000){
                    finding=false;
                }
            }
            enemies.Add(Instantiate(enemyprefab, new Vector3(0,0,0), Quaternion.identity));
            enemies[enemies.Count-1].transform.SetParent(tilemap.transform);
            enemies[enemies.Count-1].transform.position =  new Vector3(2*enemylocate[0]+tilemap.transform.position.x,2*enemylocate[1]+tilemap.transform.position.y,0);
            EnemyStats(enemies[enemies.Count-1]);
            enemies[enemies.Count-1].GetComponent<Character>().goal.transform.SetParent(tilemap.transform);
            if(enemies.Count<enemycount){
                EnemySpawn();
            }
        }
    }
    private void EnemyStats(GameObject enemy){
        Random rando = new Random();
        int monsternum=rando.Next(0,4);
        string name="Enemy";
        double[] stats = new double[6];
        switch(monsternum){
            //Bandit
            case 0:
                name = "Bandit";
                stats = new double[6]{5+depth,5+depth,5+depth,5+depth,5+depth,5+depth};
                break;
            //Orc
            case 1:
                name = "Orc";
                stats = new double[6]{10+depth*2,1+depth*0.2,6+depth*1.2,7+depth*1.4,2+depth*0.4,4+depth*0.8};
                break;
            //Goblin
            case 2:
                name = "Goblin";
                stats = new double[6]{2+depth*0.4,10+depth*2,4+depth*0.8,4+depth*0.8,5+depth,5+depth};
                break;
            //Slime
            case 3:
                name = "Slime";
                stats = new double[6]{4+depth*0.8,5+depth,6+depth*1.2,10+depth*2,2+depth*0.4,3+depth*0.6};
                break;
            //Golem
            case 4:
                name = "Golem";
                stats = new double[6]{4+depth*0.8,3+depth*0.6,10+depth*2,10+depth*2,0,3+depth*0.6};
                break;    
        }
        enemy.GetComponent<Character>().initCharacter(name,depth,(int)stats[0],(int)stats[1],(int)stats[2],(int)stats[3],(int)stats[4],(int)stats[5]);
        //name, strength, agility, constitution, defence, intelligence, wisdom
    }
    private int RoomFinder(int[] location){
        for (int i=0;i<trcorners.Count;i++){
            if((location[1]<=trcorners[i][0]) & (location[0]<=trcorners[i][1])){
                if((location[1]>=blcorners[i][0]) & (location[0]>=blcorners[i][1])){
                    return i;
                }
            }
        }
        return trcorners.Count;
    }
    //This function checks that the dungeon layout contains only valid tiles
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
    //This function finds the centre for each room
    private List<List<int>> RoomCentFinder() {
        trcorners = new List<List<int>>();
        blcorners = new List<List<int>>();
        List<List<int>> roomcents = new List<List<int>>();
        //Finds all the top right corners of rooms
        for (int i = 0; i < gridsize; i++)
        {
            for (int j = 0; j < gridsize; j++)
            {
                if(gridstore[i,j]==21){
                    trcorners.Add(new List<int>{i,j});
                }
            }
        }
        //Traverses the walls of each room until the bottom left corner can be found
        for(int i = 0; i<trcorners.Count;i++){
            List<int> coordinate = new List<int>{trcorners[i][0],trcorners[i][1]};
            bool finding = true;
            bool moveleft = true;
            while (finding){
                while (moveleft){
                    coordinate[1]=coordinate[1]-1;
                    if(gridstore[coordinate[0],coordinate[1]]==19 || gridstore[coordinate[0],coordinate[1]]==15 || gridstore[coordinate[0],coordinate[1]]==2){
                        moveleft=false;
                    }
                }
                coordinate[0]=coordinate[0]-1;
                if(gridstore[coordinate[0],coordinate[1]]==25 || gridstore[coordinate[0],coordinate[1]]==7 || gridstore[coordinate[0],coordinate[1]]==10){
                    finding=false;
                    blcorners.Add(coordinate);
                }
            }
            //Calculates the centre of each room
            roomcents.Add(new List<int>{((int)((trcorners[i][0]+blcorners[i][0])/2d)),((int)((trcorners[i][1]+blcorners[i][1])/2d))});
        }
        return roomcents;
    }
    //Function for burrowing through walls to create new corridors
    private bool DoorHole(List<int> currloc, int direction){
        switch(direction){
            case 0:
                switch(gridstore[currloc[0],currloc[1]]){
                    case 0:
                        gridstore[currloc[0],currloc[1]]=31;
                        return true;
                    case 2:
                        gridstore[currloc[0],currloc[1]]=43;
                        return true;
                    case 3:
                        gridstore[currloc[0],currloc[1]]=40;
                        return true;
                    case 4:
                        gridstore[currloc[0],currloc[1]]=44;
                        return true;
                    case 5:
                        gridstore[currloc[0],currloc[1]]=36;
                        return true;
                    case 18:
                        gridstore[currloc[0],currloc[1]]=16;
                        return true;
                    case 19:
                        gridstore[currloc[0],currloc[1]]=15;
                        return true;
                    case 20:
                        gridstore[currloc[0],currloc[1]]=8;
                        return true;
                    case 21:
                        gridstore[currloc[0],currloc[1]]=6;
                        return true;
                    case 30:
                        gridstore[currloc[0],currloc[1]]=12;
                        return true;
                }
                break;
            case 1:
                switch(gridstore[currloc[0],currloc[1]]){
                    case 0:
                        gridstore[currloc[0],currloc[1]]=30;
                        return true;
                    case 6:
                        gridstore[currloc[0],currloc[1]]=40;
                        return true;
                    case 7:
                        gridstore[currloc[0],currloc[1]]=41;
                        return true;
                    case 8:
                        gridstore[currloc[0],currloc[1]]=44;
                        return true;
                    case 9:
                        gridstore[currloc[0],currloc[1]]=37;
                        return true;
                    case 20:
                        gridstore[currloc[0],currloc[1]]=4;
                        return true;
                    case 21:
                        gridstore[currloc[0],currloc[1]]=3;
                        return true;
                    case 22:
                        gridstore[currloc[0],currloc[1]]=12;
                        return true;
                    case 23:
                        gridstore[currloc[0],currloc[1]]=10;
                        return true;
                    case 31:
                        gridstore[currloc[0],currloc[1]]=16;
                        return true;
                }
                break;
            case 2:
                switch(gridstore[currloc[0],currloc[1]]){
                    case 0:
                        gridstore[currloc[0],currloc[1]]=31;
                        return true;
                    case 10:
                        gridstore[currloc[0],currloc[1]]=41;
                        return true;
                    case 11:
                        gridstore[currloc[0],currloc[1]]=42;
                        return true;
                    case 12:
                        gridstore[currloc[0],currloc[1]]=44;
                        return true;
                    case 13:
                        gridstore[currloc[0],currloc[1]]=38;
                        return true;
                    case 22:
                        gridstore[currloc[0],currloc[1]]=8;
                        return true;
                    case 23:
                        gridstore[currloc[0],currloc[1]]=7;
                        return true;
                    case 24:
                        gridstore[currloc[0],currloc[1]]=16;
                        return true;
                    case 25:
                        gridstore[currloc[0],currloc[1]]=14;
                        return true;
                    case 30:
                        gridstore[currloc[0],currloc[1]]=4;
                        return true;
                }
                break;
            case 3:
                switch(gridstore[currloc[0],currloc[1]]){
                    case 0:
                        gridstore[currloc[0],currloc[1]]=30;
                        return true;
                    case 14:
                        gridstore[currloc[0],currloc[1]]=42;
                        return true;
                    case 15:
                        gridstore[currloc[0],currloc[1]]=43;
                        return true;
                    case 16:
                        gridstore[currloc[0],currloc[1]]=44;
                        return true;
                    case 17:
                        gridstore[currloc[0],currloc[1]]=39;
                        return true;
                    case 18:
                        gridstore[currloc[0],currloc[1]]=4;
                        return true;
                    case 19:
                        gridstore[currloc[0],currloc[1]]=2;
                        return true;
                    case 24:
                        gridstore[currloc[0],currloc[1]]=12;
                        return true;
                    case 25:
                        gridstore[currloc[0],currloc[1]]=11;
                        return true;
                    case 31:
                        gridstore[currloc[0],currloc[1]]=8;
                        return true;
                }
                break;
        }
        return false;
    }
    private void DoorMaker(){
        List<List<int>> cents = RoomCentFinder();
        List<List<int>> connects = Shortestroomfind(cents);
        for (int i=0;i<connects.Count;i++){
            if ((connects[i][0]!=connects[connects[i][1]][1]) || (connects[i][1]>connects[i][0])){
                List<int> difference = new List<int>{cents[connects[i][1]][0]-cents[connects[i][0]][0],cents[connects[i][1]][1]-cents[connects[i][0]][1]};
                List<int> currloc = new List<int>{cents[connects[i][0]][0],cents[connects[i][0]][1]};
                bool hormoved = false;
                bool vermoved = false;
                bool updire = true;
                bool rightdire = true;
                bool vertlast = false;
                if(difference[0]<0){
                    updire=false;
                }
                if(difference[1]<0){
                    rightdire=false;
                }
                while(!((currloc[0]==cents[connects[i][1]][0]) & (currloc[1]==cents[connects[i][1]][1]))){
                    if (((Math.Abs(difference[0])<Math.Abs(difference[1])) & (difference[0]!=0))|| (difference[1]==0)){
                        //Moving vertically
                        if(hormoved){
                            if (rightdire){
                                DoorHole(currloc,3);
                            }
                            else{
                                DoorHole(currloc,1);
                            }
                            hormoved=false;
                        }
                        if(!updire){
                            //Moving down
                            DoorHole(currloc,2);
                            if(vermoved){
                                vermoved = DoorHole(currloc,0);
                            }
                            vermoved=true;
                            difference[0]=difference[0]+1;
                            currloc[0]=currloc[0]-1;
                        }
                        else{
                            //Moving up
                            DoorHole(currloc,0);
                            if(vermoved){
                                vermoved = DoorHole(currloc,2);
                            }
                            vermoved=true;
                            difference[0]=difference[0]-1;
                            currloc[0]=currloc[0]+1;
                        }
                    }
                    else{
                        //Moving horizontally
                        if(vermoved){
                            if (updire){
                                DoorHole(currloc,2);
                            }
                            else{
                                DoorHole(currloc,0);
                            }
                            vermoved=false;
                        }
                        if(!rightdire){
                            //Moving left
                            DoorHole(currloc,3);
                            if(hormoved){
                                hormoved = DoorHole(currloc,1);
                            }
                            hormoved=true;
                            difference[1]=difference[1]+1;
                            currloc[1]=currloc[1]-1;
                        }
                        else{
                            //Moving right
                            DoorHole(currloc,1);
                            if(hormoved){
                                hormoved = DoorHole(currloc,3);
                            }
                            hormoved=true;
                            difference[1]=difference[1]-1;
                            currloc[1]=currloc[1]+1;
                        }
                    }
                    if (!vertlast){
                        if (difference[1]==0){
                            if (difference[0]!=0){
                                vertlast=true;
                            }
                        }
                    }
                }
                if (vertlast){
                    if(updire){
                        DoorHole(currloc,2);
                    }
                    else{
                        DoorHole(currloc,0);
                    }
                }
                else{
                    if(rightdire){
                        DoorHole(currloc,3);
                    }
                    else{
                        DoorHole(currloc,1);
                    }
                }
            }
        }
        EdgeSmoother();
    }
    private void EdgeSmoother(){
        for(int i = 1; i<gridsize-1;i++){
            for(int j =1; j<gridsize -1; j++){
                switch (gridstore[i,j]){
                    case 4:
                        if(connectClassify[4][3].Contains(gridstore[i,j-1])){
                            gridstore[i,j]=18;
                        }
                        if(connectClassify[4][1].Contains(gridstore[i,j+1])){
                            gridstore[i,j]=20;
                        }
                        break;
                    case 8:
                        if(connectClassify[4][2].Contains(gridstore[i-1,j])){
                            gridstore[i,j]=22;
                        }
                        if(connectClassify[4][0].Contains(gridstore[i+1,j])){
                            gridstore[i,j]=20;
                        }
                        break;
                    case 12:
                        if(connectClassify[4][3].Contains(gridstore[i,j-1])){
                            gridstore[i,j]=24;
                        }
                        if(connectClassify[4][1].Contains(gridstore[i,j+1])){
                            gridstore[i,j]=22;
                        }
                        break;
                    case 16:
                        if(connectClassify[4][2].Contains(gridstore[i-1,j])){
                            gridstore[i,j]=24;
                        }
                        if(connectClassify[4][0].Contains(gridstore[i+1,j])){
                            gridstore[i,j]=18;
                        }
                        break;
                    case 44:
                        if (connectClassify[4][0].Contains(gridstore[i+1,j])){
                            gridstore[i,j]=4;
                        }
                        if (connectClassify[4][1].Contains(gridstore[i,j+1])){
                            gridstore[i,j]=8;
                        }
                        if (connectClassify[4][2].Contains(gridstore[i-1,j])){
                            gridstore[i,j]=12;
                        }
                        if (connectClassify[4][3].Contains(gridstore[i,j-1])){
                            gridstore[i,j]=16;
                        }
                        break;
                }
            }
        }
    }
    //This function finds the room closest to each room
    private List<List<int>> Shortestroomfind(List<List<int>> cents){
        List<List<int>> shortconnects = new List<List<int>>();
        for(int i=0;i<cents.Count;i++){
            int bestconnect=0;
            int bestdist = Int32.MaxValue;
            for(int j=0;j<cents.Count;j++){
                if(i!=j){
                    double dist = Math.Sqrt((cents[i][0]-cents[j][0])*(cents[i][0]-cents[j][0])+(cents[i][1]-cents[j][1])*(cents[i][1]-cents[j][1]));
                    if(bestdist>dist){
                        bestdist=(int)dist;
                        bestconnect=j;
                    }
                }
            }
            shortconnects.Add(new List<int>{i,bestconnect});
        }
        shortconnects=SecondConnects(shortconnects,cents);
        return shortconnects;
    }
    //Connects any rooms that haven't previously been connected to the network
    private List<List<int>> SecondConnects(List<List<int>> shortconnects, List<List<int>> cents) {
        List<int> connected = new List<int>{0};
        List<int> notconnect = new List<int>();
        for(int i=0;i<shortconnects.Count;i++){
            bool delving = true;
            int hold=i;
            List<int> visited= new List<int>{};
            while(delving){
                for(int j = 0; j<shortconnects.Count;j++){
                    if((shortconnects[j][0]==hold) & (!visited.Contains(j))){
                        hold=j;
                        visited.Add(j);
                        break;
                    }
                }
                if(connected.Contains(shortconnects[hold][1])){
                    if(!connected.Contains(shortconnects[hold][0])){
                        connected.Add(shortconnects[hold][0]);
                    }
                    else{
                        delving=false;
                    }
                }
                else{
                    if(connected.Contains(shortconnects[hold][0])){
                        connected.Add(shortconnects[hold][1]);
                    }
                    else{
                        delving=false;
                    }
                }
                hold=shortconnects[hold][1];
            }
        }
        for(int i=0;i<cents.Count;i++){
            if(!connected.Contains(i)){
                notconnect.Add(i);
            }
        }
        if(notconnect.Count!=0){
            List<int> newconnect = new List<int>();
            int bestdist=10000000;
            double dist;
            for(int i=0;i<connected.Count;i++){
                for(int j=0;j<notconnect.Count;j++){
                    dist = Math.Sqrt((cents[connected[i]][0]-cents[notconnect[j]][0])*(cents[connected[i]][0]-cents[notconnect[j]][0])+(cents[connected[i]][1]-cents[notconnect[j]][1])*(cents[connected[i]][1]-cents[notconnect[j]][1]));
                    if(dist<bestdist){
                        bestdist=(int)dist;
                        newconnect = new List<int> {notconnect[j],connected[i]};
                    }
                }
            }
            connected.Add(newconnect[1]);
            notconnect.Remove(newconnect[1]);
            int keep=shortconnects.Count;
            for(int i=0;i<shortconnects.Count;i++){
                if(shortconnects[i][0]==newconnect[0]){
                    keep=i;
                }
            }
            shortconnects.Insert(keep,newconnect);
            for (int i=0;i<shortconnects.Count;i++){
            }
            if(notconnect.Count!=0){
                shortconnects=SecondConnects(shortconnects,cents);
            }
        }
        return shortconnects;
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
    //Flawed function, lowest entropy has the desire to naturally fill the area with block cells, when forcing rooms it will make minimum sized rooms
    private void GridGenLowestEntropy() { //Function for the generation of the dungeon layout
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
            if(gridstore[coordinate[0],coordinate[1]]==100){
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

    private int[] PlayerLocate() {
        int[] PlayerLocation = {(int)tilemap.transform.position[0]/-2, (int)tilemap.transform.position[1]/-2};
        return PlayerLocation;
    }

    private bool MoveAllow(int direction, int[] location) {
        return tilemove[direction, gridstore[location[1],location[0]]];
    }
    //public GameObject popupwindow;
    //public TextMeshProUGUI popuptext;
    //public GameObject popupops;
    //public GameObject descendops;
    //public GameObject battleops;
    private void LandOnLadder(){
        int [] goallocate = new int[2]{(int)goal.position.x/-2,(int)goal.position.y/-2};
        if(goallocate[1]==laddercord[0] & goallocate[0]==laddercord[1]){
            popped=true;
            popuptext.text="You have found a ladder down to the next layer! Would you like to descend?";
            popupwindow.SetActive(true);
            descendops.SetActive(true);
            popupops.SetActive(false);
            battleops.SetActive(false);
        }
    }

    public void Descend(){
        popupwindow.SetActive(false);
        popped=false;
        depth=depth+1;
        GridDisplay();
    }

    public void NoDescend(){
        popupwindow.SetActive(false);
        popped=false;
    }

    private bool nearbyneighbour(int x, int y){
        for(int i=0;i<enemies.Count;i++){
            if ((enemies[i].GetComponent<Character>().goal.transform.position[0]-tilemap.transform.position.x)/2==x & (enemies[i].GetComponent<Character>().goal.transform.position[1]-tilemap.transform.position.y)/2==y){
                return false;
            }
        }
        return true;
    }
    private bool direqueuecheck(List<List<int>> queue,int increment){
        bool checking=true;
        for(int i=0;i<queue.Count;i++){
            if((queue[i][0]==queue[increment][0]) & (queue[i][1]==queue[increment][1]) & (i!=increment)){
                checking=false;
            }
        }
        return checking;
    }

    private int direchoose(int[] enemy, int[] player){
        List<List<int>> queue = new List<List<int>> {new List<int>{enemy[0],enemy[1],4}};
        int[] goalcords = new int[2] { (int)goal.position.x/-2, (int)goal.position.y/-2};
        bool searching = true;
        int increment = 0;
        while (searching){
            if ((goalcords[0] == queue[increment][0]) & (goalcords[1] == queue[increment][1]))
            {
                searching = false;
                return queue[increment][2];
            }
            //CHECKING NORTH
            if (MoveAllow(0, new int[2] {queue[increment][0], queue[increment][1]})){
                if(direqueuecheck(queue,increment)){
                    if(increment == 0)
                    {
                        queue.Add(new List<int>{queue[increment][0], queue[increment][1] + 1, 0});
                    }
                    else
                    {
                        queue.Add(new List<int>{queue[increment][0], queue[increment][1] + 1, queue[increment][2]});
                    }
                }
            }
            //CHECKING EAST
            if (MoveAllow(1, new int[2] {queue[increment][0], queue[increment][1] }))
            {
                if(direqueuecheck(queue,increment)){
                    if (increment == 0)
                    {
                        queue.Add(new List<int>{queue[increment][0]+1, queue[increment][1], 1});
                    }
                    else
                    {
                        queue.Add(new List<int>{queue[increment][0]+1, queue[increment][1], queue[increment][2]});
                    }
                }
            }
            //CHECKING SOUTH
            if (MoveAllow(2, new int[2] {queue[increment][0], queue[increment][1] }))
            {
                if(direqueuecheck(queue,increment)){
                    if (increment == 0)
                    {
                        queue.Add(new List<int>{queue[increment][0], queue[increment][1] - 1, 2});
                    }
                    else
                    {
                        queue.Add(new List<int>{queue[increment][0], queue[increment][1] - 1, queue[increment][2]});
                    }
                }
            }
            //CHECKING WEST
            if (MoveAllow(3, new int[2] {queue[increment][0], queue[increment][1] }))
            {
                if(direqueuecheck(queue,increment)){
                    if (increment == 0)
                    {
                        queue.Add(new List<int>{queue[increment][0]-1, queue[increment][1], 3});
                    }
                    else
                    {
                        queue.Add(new List<int>{queue[increment][0]-1, queue[increment][1], queue[increment][2]});
                    }
                }
            }
            increment = increment + 1;
            if(increment>=queue.Count){
                searching=false;
            }
        }
        return 5;
    }

    private void EnemyMove(){
        if(enemies!=null){
            Random rando = new Random();
            for (int i=0;i<enemies.Count;i++){
                List<int> direoptions = new List<int>();
                int[] enemylocation = new int[2]{(int)(enemies[i].transform.position.x-tilemap.transform.position.x)/2,(int)(enemies[i].transform.position.y-tilemap.transform.position.y)/2};
                int[] playerlocation = PlayerLocate();
                int dire = 0;
                if (Math.Sqrt((playerlocation[0] - enemylocation[0]) * (playerlocation[0] - enemylocation[0]) + (playerlocation[1] - enemylocation[1]) * (playerlocation[1] - enemylocation[1])) <= 5)
                {
                    dire=direchoose(enemylocation, playerlocation);
                    Debug.Log(dire);
                }
                else
                {
                    if (MoveAllow(0, enemylocation))
                    {
                        direoptions.Add(0);
                    }
                    if (MoveAllow(1, enemylocation))
                    {
                        direoptions.Add(1);
                    }
                    if (MoveAllow(2, enemylocation))
                    {
                        direoptions.Add(2);
                    }
                    if (MoveAllow(3, enemylocation))
                    {
                        direoptions.Add(3);
                    }
                    dire = direoptions[rando.Next(0, direoptions.Count)];
                }
                switch(dire){
                    case 0:
                        if(nearbyneighbour(enemylocation[0],enemylocation[1]+1)){
                            enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(0f, speed*-0.5f, 0f);
                        }
                        break;
                    case 1:
                        if(nearbyneighbour(enemylocation[0]+1,enemylocation[1])){
                            enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(speed*-0.5f, 0f, 0f);
                        }
                        break;
                    case 2:
                        if(nearbyneighbour(enemylocation[0],enemylocation[1]-1)){
                            enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(0f, speed*0.5f, 0f);
                        }
                        break;
                    case 3:
                        if(nearbyneighbour(enemylocation[0]-1,enemylocation[1])){
                            enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(speed*0.5f, 0f, 0f);
                        }
                        break;
                    case 4:
                        //RUN CODE HERE FOR INITIALISING COMBAT
                        break;
                    case 5:
                        //Nothing happens here, it can't get to the player
                        break;
                }
            }
        }
    }
    
    private void EnemyMoving(){
        if(enemies!=null){
            for (int i=0;i<enemies.Count;i++){
                enemies[i].transform.position = Vector3.MoveTowards(enemies[i].transform.position, enemies[i].GetComponent<Character>().goal.transform.position, speed*Time.deltaTime);
            }
        }
    }

    //public void CombatStart(){}

    void Start() {
        depth=0;
        GridDisplay();
        goal.SetParent(null);
    }

    void Update() {
        //Movement works through moving a goal object then sliding the tilemap towards it
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        EnemyMoving();
        if(!popped){
            if(Vector3.Distance(transform.position,goal.position) == 0f){
                if(Input.GetAxisRaw("Horizontal")!=0f){
                    if(Input.GetAxisRaw("Horizontal")==1f){
                        //Moving right
                        if(MoveAllow(1,PlayerLocate())){
                            goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                            direction=1;
                            EnemyMove();
                            LandOnLadder();
                        }
                    }
                    if(Input.GetAxisRaw("Horizontal")==-1f){
                        //Moving left
                        if(MoveAllow(3,PlayerLocate())){
                            goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                            direction=3;
                            EnemyMove();
                            LandOnLadder();
                        }
                    }
                }
                else if(Input.GetAxisRaw("Vertical")!=0f){
                    if(Input.GetAxisRaw("Vertical")==1f){
                        //Moving up
                        if(MoveAllow(0,PlayerLocate())){
                            goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                            direction=0;
                            EnemyMove();
                            LandOnLadder();
                        }
                    }
                    if(Input.GetAxisRaw("Vertical")==-1f){
                        //Moving down
                        if(MoveAllow(2,PlayerLocate())){
                            goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                            direction=2;
                            EnemyMove();
                            LandOnLadder();
                        }
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