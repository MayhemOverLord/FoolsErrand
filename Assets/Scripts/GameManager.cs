using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

public class GameManager : MonoBehaviour{
    public Canvas canvas;
    public GameObject Player;
    public Tile[] tiles;
    public Tilemap tilemap;
    //Speed for moving across tiles
    public float speed = 4;
    private bool tileactive = false;
    //Maximum width/length of a dungeon and maximum enemy count
    public int gridsize = 14;
    public int enemycount = 4;
    public Transform goal;
    private int[,] gridstore;
    public bool squaregen = true;
    private int[] PlayerLocation = new int[2];
    private int direction;
    //Values used for determining enemy difficulty
    public int depth = 0;
    private int wins = 0;
    private int bosslevel=10;
    //Lists used for storing room dimensions
    private List<List<int>> trcorners;
    private List<List<int>> blcorners;
    private List<List<int>> roomcenters;
    //List of all tiles
    private List<int> alltiles = new List<int>{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44};
    //List of tiles used in wave collapse generation
    private List<int> squaretiles = new List<int>{0,1,5,9,13,17,19,21,23,25};
    //Arrays for whether a player can move directionally per tile
    private bool[,] tilemove = {
        {false,false,false,false}, //0 N
        {true,true,true,true}, //1 A
        {false,true,true,true}, //2 B1
        {false,true,true,true}, //3 B2
        {false,true,true,true}, //4 B3 
        {false,true,true,true}, //5 B
        {true,false,true,true}, //6 C1
        {true,false,true,true}, //7 C2
        {true,false,true,true}, //8 C3
        {true,false,true,true}, //9 C
        {true,true,false,true}, //10 D1
        {true,true,false,true}, //11 D2
        {true,true,false,true}, //12 D3
        {true,true,false,true}, //13 D
        {true,true,true,false}, //14 E1
        {true,true,true,false}, //15 E2
        {true,true,true,false}, //16 E3
        {true,true,true,false}, //17 E
        {false,true,true,false}, //18 F1
        {false,true,true,false}, //19 F
        {false,false,true,true}, //20 G1
        {false,false,true,true}, //21 G
        {true,false,false,true}, //22 H1
        {true,false,false,true}, //23 H
        {true,true,false,false}, //24 I1 
        {true,true,false,false}, //25 I
        {true,false,false,false}, //26 J
        {false,true,false,false}, //27 K
        {false,false,true,false}, //28 L
        {false,false,false,true}, //29 M
        {false,true,false,true}, //30 O
        {true,false,true,false}, //31 P
        {true,true,true,true}, //32 Q1
        {true,true,true,true}, //33 Q2
        {true,true,true,true}, //34 Q3
        {true,true,true,true}, //35 Q4
        {true,true,true,true}, //36 Q5
        {true,true,true,true}, //37 Q6
        {true,true,true,true}, //38 Q7
        {true,true,true,true}, //39 Q8
        {true,true,true,true}, //40 Q9
        {true,true,true,true}, //41 Q10
        {true,true,true,true}, //42 Q11
        {true,true,true,true}, //43 Q12
        {true,true,true,true}, //44 Q13
        {true,true,true,true}, //45 Ladder
        {true,true,true,true}, //46 Magma
        {true,true,true,true}, //47 Ice
        {true,true,true,true} //48 Spike
    };
    //Array for determining what sets of tiles can connect to a tile in each direction
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
    //Set of connections between tiles for more dynamic generation
    private List<List<List<int>>> connectClassify = new List<List<List<int>>>{
    new List<List<int>>{new List<int>{1,5,32,33,36},new List<int>{1,9,33,34,37},new List<int>{1,13,34,35,38},new List<int>{1,17,32,35,39}}, //Empty
    new List<List<int>>{new List<int>{2,15,17,19,35,39,43},new List<int>{5,6,21,32,36,40},new List<int>{7,9,10,23,33,37,41},new List<int>{11,13,14,25,34,38,42}}, //Left
    new List<List<int>>{new List<int>{3,6,9,21,34,37,40},new List<int>{7,10,13,23,35,38,41},new List<int>{11,14,17,25,32,39,42},new List<int>{5,15,19,33,36,43}}, //Right
    new List<List<int>>{new List<int>{4,7,8,14,16,18,20,28,31,38,41,42,44},new List<int>{2,4,8,11,12,20,22,29,30,39,42,43,44},new List<int>{6,8,12,22,15,16,24,26,31,36,40,43,44},new List<int>{3,4,10,12,16,18,24,27,30,37,40,41,44}}, //Both
    new List<List<int>>{new List<int>{0,10,11,12,13,22,23,24,25,26,27,29,30},new List<int>{0,14,15,16,17,18,19,24,25,26,27,28,31},new List<int>{0,2,3,4,5,18,19,20,21,26,27,28,30},new List<int>{0,6,7,8,9,20,21,22,23,26,28,29,31}}, //Wall
    new List<List<int>>{new List<int>{1,5,32,33,36,5,32,33,36},new List<int>{1,9,33,34,37,9,33,34,37},new List<int>{1,13,34,35,38,13,34,35,38},new List<int>{1,17,32,35,39,17,32,35,39}}
    };
    //UI objects
    public GameObject enemyprefab;
    private List<GameObject> enemies;
    public GameObject popupwindow;
    public TextMeshProUGUI popuptext;
    public GameObject nextdiabutton;
    public TextMeshProUGUI nextdiatext;
    public GameObject popupops;
    public GameObject descendops;
    public GameObject battleops;
    public GameObject stuckops;
    //Whether there is currently a popup on screen, preventing player movement
    private bool popped=false;
    private GameObject combatenemy;
    private int combatenemyindex;
    public GameObject playericon;
    public GameObject enemyicon;
    public GameObject combatscreen;
    public GameObject combatoptions;
    public GameObject combatspells;
    public GameObject combattextpanel;
    public TextMeshProUGUI combattext;
    public TextMeshProUGUI playername;
    public TextMeshProUGUI enemyname;
    public TextMeshProUGUI playerhealth;
    public TextMeshProUGUI playermana;
    public TextMeshProUGUI enemyhealth;
    public TextMeshProUGUI enemymana;
    public GameObject playerhealthbar;
    public GameObject playermanabar;
    public GameObject enemyhealthbar;
    public GameObject enemymanabar;
    //Combat related values
    private List<int> turnqueue;
    private int turns;
    private bool playerfirst;
    private int currentturn;
    private bool fled=false;
    private bool turneffect=false;
    private int expgained=0;
    public int statpoints=0;
    private int tempspell;
    public GameObject spellviewpanel;
    public GameObject spellcastbutton;
    public TextMeshProUGUI spelldescription;
    //List of usable spells
    private List<List<string>> spelllist = new List<List<string>>{
        new List<string>{"Heal Wounds","50","Heals the user by restoring some lost health"},
        new List<string>{"Breathe Fire","40","Applies burning to enemy, dealing damage each enemy turn"},
        new List<string>{"Arcane Knife","25","Damages the enemy with a magical blade"},
        new List<string>{"Power Charge","35","Empowers the user with physical and magical strength, increasing attack power"},
        new List<string>{"Wild Surge","45","A risky move with a high chance of missing but great damage capability"}
    };

    //Objects for experience gained/level up panel
    public TextMeshProUGUI exptext;
    public GameObject exppanel;
    public GameObject strpanel;
    public TextMeshProUGUI strtext;
    public GameObject agipanel;
    public TextMeshProUGUI agitext;
    public GameObject conpanel;
    public TextMeshProUGUI context;
    public GameObject defpanel;
    public TextMeshProUGUI deftext;
    public GameObject intpanel;
    public TextMeshProUGUI inttext;
    public GameObject wispanel;
    public TextMeshProUGUI wistext;
    public GameObject donebutton;

    public GameObject NPCprefab;
    public List<GameObject> NPCs;
    private bool bossfight;
    private bool npcfight;
    private int interactindex;
    public GameObject gameoverpanel;
    public TextMeshProUGUI gameovertext;

    //Top left player stats UI objects
    public TextMeshProUGUI UInametext;
    public TextMeshProUGUI UIHPtext;
    public TextMeshProUGUI UImanatext;
    public TextMeshProUGUI UIEXPtext;
    public TextMeshProUGUI UIdepthtext;
    
    //Function for displaying the dungeon layout
    public void GridDisplay() {
        bossfight=false;
        npcfight=false;
        UIstats();
        tileactive=true;
        bool normgen=!(depth==0 || depth==bosslevel);
        bool validgen = false;
        int counter = 0;
        //Clears all preexisting enemies and NPCs
        if(enemies!=null){
            for (int i =enemies.Count-1;i>=0;i--){
                Destroy(enemies[i].GetComponent<Character>().goal);
                Destroy(enemies[i]);
            }
        }
        if(NPCs!=null){
            for (int i =NPCs.Count-1;i>=0;i--){
                Destroy(NPCs[i].GetComponent<Character>().goal);
                Destroy(NPCs[i]);
            }
        }
        NPCs = new List<GameObject>();
        enemies = new List<GameObject>();
        if(normgen){
            //Creates a normal dungeon layer
            while (!validgen) //Runs grid generation until a valid grid has been generated
            {
                GridGen();
                validgen = GridCheck();
                if(counter>=10000){
                    break;
                }
                counter = counter+1;
            }
            if (counter>=10000){
                GridDetermined();
                PlayerLocation = PlayerSpawn().ToArray();
            }
            else{
                DoorMaker();
                PlayerLocation = PlayerSpawn().ToArray();
                LadderMake();
                TileGen();
                EnemySpawn();
            }
        }
        else{
            //Creates either the lobby layer or the boss layer
            GridDetermined();
        }
        //Outputs the tile display
        tilemap.ClearAllTiles();
        for (int y=0;y<gridsize;y++){
            for (int x=0;x<gridsize;x++){
                tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[gridstore[y,x]]);
                if(gridstore[y,x]==48){
                    tilemap.SetTile(new Vector3Int(x*2+1, y*2+1, 0), tiles[1]);
                }
            }
        }
        //Sets the player location on screen
        transform.position = new Vector3Int(-2*PlayerLocation[1],-2*PlayerLocation[0], 0);
        goal.position = new Vector3Int(-2*PlayerLocation[1],-2*PlayerLocation[0], 0);
        direction = 0;
    }
    private void GridDetermined(){
        //Starting Room
        if(depth==0){
            //Creates the lobby room layout
            GridTut();
            PlayerLocation=new int[2]{0,2};
            //Guide NPC
            NPCs.Add(Instantiate(NPCprefab, new Vector3(0,0,0), Quaternion.identity));
            NPCs[NPCs.Count-1].transform.SetParent(tilemap.transform);
            NPCs[NPCs.Count-1].transform.position =  new Vector3(2*2+tilemap.transform.position.x,2*4+tilemap.transform.position.y,-1);
            NPCs[NPCs.Count-1].GetComponent<Character>().initCharacter("Guide",100,1000,1000,1000,1000,1000,1000,1);
            NPCs[NPCs.Count-1].GetComponent<NPC>().initNPC(new List<string>{"Guide: Welcome to the dungeon, this is where the villain is hiding,",
            "Guide: I've heard that the villain is hiding on layer "+bosslevel+"!",
            "Guide: You can descend down the ladder to reach new layers of the dungeon,",
            "Guide: Be careful though, there are monsters that grow stronger the deeper you go.",
            "Guide: I wish you luck, the kingdom depends on you!"
            },"Exit","NPC", new int[2]{2,4});

            //Combat Guide NPC
            NPCs.Add(Instantiate(NPCprefab, new Vector3(0,0,0), Quaternion.identity));
            NPCs[NPCs.Count-1].transform.SetParent(tilemap.transform);
            NPCs[NPCs.Count-1].transform.position =  new Vector3(2*6+tilemap.transform.position.x,2*4+tilemap.transform.position.y,-1);
            NPCs[NPCs.Count-1].GetComponent<Character>().initCharacter("Guide",100,1000,1000,1000,1000,1000,1000,1);
            NPCs[NPCs.Count-1].GetComponent<NPC>().initNPC(new List<string>{"Combat Teacher: I guess I should probably explain what your stats are right?",
            "Combat Teacher: Your strength stat affects how much damage you deal when using a regular attack,",
            "Combat Teacher: Agility determines how often you attack before your opponent does,",
            "Combat Teacher: Constitution will increase current and maximum health by 10 each,",
            "Combat Teacher: Defence helps to reduce the damage from any attacks coming your way,",
            "Combat Teacher: Intelligence determines the power of your magical abilities,",
            "Combat Teacher: Wisdom will give you mana capacity and determine your mana regen,",
            "Combat Teacher: Well looks like that's it for stats, go chat to the dummy if you want to know how combat works!"
            },"Exit","NPC", new int[2]{6,4});

            //Dummy NPC
            NPCs.Add(Instantiate(NPCprefab, new Vector3(0,0,0), Quaternion.identity));
            NPCs[NPCs.Count-1].transform.SetParent(tilemap.transform);
            NPCs[NPCs.Count-1].transform.position =  new Vector3(2*8+tilemap.transform.position.x,2*4+tilemap.transform.position.y,-1);
            int agil = Player.GetComponent<Character>().agility;
            if(agil==0){
                agil=10;
            }
            NPCs[NPCs.Count-1].GetComponent<Character>().initCharacter("Dummy",1,0,agil-1,100,5,1,1,8);
            NPCs[NPCs.Count-1].GetComponent<NPC>().initNPC(new List<string>{"Dummy: Hello! I am the combat dummy, want to learn how fighting works?",
            "Dummy: Fighting is all about who's fastest, if you're 5 times faster than your opponent, you can hit them 5 times before they can fight back!",
            "Dummy: Your basic attack option uses your strength against your enemy's defence to reduce the enemy's health points,",
            "Dummy: If you manage to fight well enough to drop your enemy to 0 hit points you win!",
            "Dummy: But if they drop you to 0 hit points you are beaten and end up back here, so don't let that happen...",
            "Dummy: Defending will halve the next attack you receive from an opponent, keeping you safe a little longer!",
            "Dummy: Fleeing is guaranteed if you're faster than your opponent, but the faster they are more than you the less your chances of escape!",
            "Dummy: Spells are powered by your intelligence, clicking on each spell in the spell menu will give you a description,",
            "Dummy: Spells are fuelled by mana, it is refueled a little at the end of each of your turns and on each step you take in the dungeon.",
            "Dummy: I think we've covered just about everything, why not try out a few hits on me?",
            },"Fight","NPC", new int[2]{8,4});
        }
        //Boss Room
        else{
            if(depth%bosslevel==0){
                //Creates the boss room
                GridBoss();
                PlayerLocation=new int[2]{0,4};
                NPCs.Add(Instantiate(NPCprefab, new Vector3(0,0,0), Quaternion.identity));
                NPCs[NPCs.Count-1].transform.SetParent(tilemap.transform);
                NPCs[NPCs.Count-1].transform.position =  new Vector3(2*4+tilemap.transform.position.x,2*4+tilemap.transform.position.y,-1);
                EnemyStats(NPCs[NPCs.Count-1],true);
                NPCs[NPCs.Count-1].GetComponent<NPC>().initNPC(new List<string>{BossDialogue(NPCs[NPCs.Count-1])},"Fight","BOSS", new int[2]{4,4});
                NPCs[NPCs.Count-1].transform.localScale *= 3;
                NPCs[NPCs.Count-1].GetComponent<Character>().goal.transform.SetParent(tilemap.transform);
            }
            else{
                //Creates a predetermined room if generation fails
                BrokenGrid();
                PlayerLocation=new int[2]{2,2};
            }
        }
    }

    private string BossDialogue(GameObject NPC){
        //Returns boss dialogue determined by what enemy type it is
        switch(NPC.GetComponent<Character>().charname){
            //Bandit
            case "Bandit":
                return "I'm the king of this dungeon, get out of here!";
            //Orc
            case "Orc":
                return "Human filth, I'll rip your kind to shreds!";
            //Goblin
            case "Goblin":
                return "HEHEHEHE A LITTLE HUMAN HAS FALLEN INTO MY PIT?!";
            //Slime
            case "Slime":
                return "*squishy slime noises*";
            //Golem
            case "Golem":
                return "INTRUDER DETECTED: INITIATING ATTACK SEQUENCE"; 
            case "Wizard":
                if(wins==0){
                    return "It's too late fool, I have forever scarred this land, now you shall die!";
                }
                else{
                    return "You've beaten me already, but I've prepared myself for a rematch!";
                }
        }
        return "You've picked a fight with the wrong person, prepare to die!";
    }

    private void BrokenGrid(){
        //Creates a predetermined dungeon layer in case of generation errors
        gridstore = new int[gridsize, gridsize];
        for (int i = 0; i < gridsize; i++)
        {
            List<int> hold = new List<int>();
            switch(i){
                case 0:
                    hold = new List<int>{0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    break;
                case 1:
                    hold = new List<int>{0,25,13,13,13,13,13,13,23,25,13,13,23,0};
                    break;
                case 2:
                    hold = new List<int>{0,17,1,1,1,1,1,1,37,39,46,46,9,0};
                    break;
                case 3:
                    hold = new List<int>{0,19,5,5,5,36,5,5,21,17,46,46,9,0};
                    break;
                case 4:
                    hold = new List<int>{0,25,13,13,13,38,13,23,0,17,46,46,9,0};
                    break;
                case 5:
                    hold = new List<int>{0,17,47,47,47,47,47,9,0,17,46,46,9,0};
                    break;
                case 6:
                    hold = new List<int>{0,17,47,47,47,47,47,9,0,19,5,5,21,0};
                    break;
                case 7:
                    hold = new List<int>{0,17,47,47,47,47,47,9,0,25,13,13,23,0};
                    break;
                case 8:
                    hold = new List<int>{0,19,5,5,36,5,5,3,30,39,1,1,9,0};
                    break;
                case 9:
                    hold = new List<int>{0,25,13,13,38,13,13,23,0,17,1,1,9,0};
                    break;
                case 10:
                    hold = new List<int>{0,17,1,1,1,1,1,9,0,17,1,1,9,0};
                    break;
                case 11:
                    hold = new List<int>{0,17,1,1,1,1,1,9,0,17,1,45,9,0};
                    break;
                case 12:
                    hold = new List<int>{0,19,5,5,5,5,5,21,0,19,5,5,21,0};
                    break;
                case 13:
                    hold = new List<int>{0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    break;
            }
            for (int j = hold.Count-1; j < gridsize; j++)
            {
                hold.Add(0);
            }
            for (int j = 0; j < gridsize; j++)
            {
                gridstore[i,j] = hold[j]; //Solid tiles
            }
        }
    }
    private void GridTut()
    {
        //Creates the lobby layer
        gridstore = new int[gridsize, gridsize];
        for (int i = 0; i < gridsize; i++)
        {
            List<int> hold = new List<int>();
            switch(i){
                case 0:
                    hold = new List<int>{25,13,13,13,23,0,0,0,0};
                    break;
                case 1:
                    hold = new List<int>{17,1,1,1,9,0,25,13,23};
                    break;
                case 2:
                    hold = new List<int>{17,1,1,1,9,0,17,45,9};
                    break;
                case 3:
                    hold = new List<int>{17,1,1,1,37,30,39,1,9};
                    break;
                case 4:
                    hold = new List<int>{19,5,5,5,21,0,19,5,21};
                    break;
            }
            for (int j = hold.Count-1; j < gridsize; j++)
            {
                hold.Add(0);
            }
            for (int j = 0; j < gridsize; j++)
            {
                gridstore[i,j] = hold[j]; //Solid tiles
            }
        }
    }
    private void GridBoss()
    {
        //Creates the boss room layer
        gridstore = new int[gridsize, gridsize];
        for (int i = 0; i < gridsize; i++)
        {
            List<int> hold = new List<int>();
            switch(i){
                case 0:
                    hold = new List<int>{25,13,13,13,13,13,13,13,23};
                    break;
                case 1:
                    hold = new List<int>{19,32,1,1,1,1,1,33,21};
                    break;
                case 2:
                    hold = new List<int>{0,19,32,1,1,1,33,21,0};
                    break;
                case 3:
                    hold = new List<int>{0,0,19,32,1,33,21,0,0};
                    break;
                case 4:
                    hold = new List<int>{0,0,0,17,1,9,0,0,0};
                    break;
                case 5:
                    hold = new List<int>{0,0,0,19,5,21,0,0,0};
                    break;
            }
            for (int j = hold.Count-1; j < gridsize; j++)
            {
                hold.Add(0);
            }
            for (int j = 0; j < gridsize; j++)
            {
                gridstore[i,j] = hold[j]; //Solid tiles
            }
        }
    }

    //Initialises the dungeon layout storage with a valid base
    private void GridStart() //Sets the initial values for the dungeon tile grid
    {
        gridstore = new int[gridsize, gridsize];
        for (int i = 0; i < gridsize; i++)
        {
            for (int j = 0; j < gridsize; j++)
            {
                if ((i <= 0) | (j <= 0) | (i >= gridsize - 1) | (j >= gridsize - 1))
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

    //Chooses a random place in the dungeon and adds a ladder
    private void LadderMake(){
        List<List<int>> Validtiles = ValidTiler();
        Random rando = new Random();
        int hold = rando.Next(0,Validtiles.Count-1);
        gridstore[Validtiles[hold][0],Validtiles[hold][1]]=45;
    }
    
    //Checks if this tile is a valid part of the dungeon floor
    private List<List<int>> ValidTiler(){
        int[] playerlocation = PlayerLocate();
        List<List<int>> Validtiles = new List<List<int>>();
        for (int y=1; y<gridsize-1;y++){
            for (int x=1; x<gridsize-1;x++){
                if(gridstore[y,x]==1 & !(playerlocation[0]==x & playerlocation[1]==y)){
                    Validtiles.Add(new List<int>{y,x});
                }
            }
        }
        return Validtiles;
    }

    //Chooses a valid start point for the player
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

    //Selects a valid location for enemies to appear
    private int[] EnemyValid(){
        int[] playerlocation = PlayerLocate();
        List<List<int>> Validtiles = new List<List<int>>();
        for (int y=1; y<gridsize-1;y++){
            for (int x=1; x<gridsize-1;x++){
                if(gridstore[y,x]!=0 && ((playerlocation[0]-6>x || playerlocation[0]+6<x) || (playerlocation[1]+2<y || playerlocation[1]-2>y))){
                    Validtiles.Add(new List<int>{y,x});
                }
            }
        }
        Random rando = new Random();
        int hold = rando.Next(0,Validtiles.Count-1);
        return new int[2]{Validtiles[hold][1],Validtiles[hold][0]};
    }

    //Creates an instance of an enemy
    private void EnemySpawn(){
        if(enemies.Count<enemycount){
            Random rando = new Random();
            int [] enemylocate= new int[]{0,0};
            bool finding=true;
            int counter=0;
            while(finding){
                finding=false;
                enemylocate = EnemyValid();
                for (int i=0;i<enemies.Count;i++){
                    if((enemies[i].transform.position.x-tilemap.transform.position.x)/2==enemylocate[0] && (enemies[i].transform.position.y-tilemap.transform.position.y)/2==enemylocate[1]){
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
            enemies[enemies.Count-1].transform.position =  new Vector3(2*enemylocate[0]+tilemap.transform.position.x,2*enemylocate[1]+tilemap.transform.position.y,-3);
            EnemyStats(enemies[enemies.Count-1],false);
            enemies[enemies.Count-1].GetComponent<Character>().goal.transform.SetParent(tilemap.transform);
            if(enemies.Count<enemycount){
                EnemySpawn();
            }
        }
    }

    //Gives an enemy its stats based on the depth and their enemy type
    private void EnemyStats(GameObject enemy, bool boss){
        Random rando = new Random();
        int monsternum=rando.Next(0,6);
        string name="Enemy";
        double[] stats = new double[6];
        double modifier = (depth+wins*bosslevel);
        if(boss){
            modifier=modifier*1.2;
            if(wins==0){
                monsternum=5;
            }
        }
        switch(monsternum){
            //Bandit
            case 0:
                name = "Bandit";
                stats = new double[7]{5+modifier,5+modifier,5+modifier,5+modifier,5+modifier,5+modifier,2};
                break;
            //Orc
            case 1:
                name = "Orc";
                stats = new double[7]{10+modifier*2,1+modifier*0.2,6+modifier*1.2,7+modifier*1.4,2+modifier*0.4,4+modifier*0.8,3};
                break;
            //Goblin
            case 2:
                name = "Goblin";
                stats = new double[7]{5+modifier,10+modifier*2,4+modifier*0.8,4+modifier*0.8,5+modifier,2+modifier*0.4,4};
                break;
            //Slime
            case 3:
                name = "Slime";
                stats = new double[7]{4+modifier*0.8,5+modifier,6+modifier*1.2,10+modifier*2,2+modifier*0.4,3+modifier*0.6,5};
                break;
            //Golem
            case 4:
                name = "Golem";
                stats = new double[7]{4+modifier*0.8,3+modifier*0.6,10+modifier*2,10+modifier*2,0,3+modifier*0.6,6};
                break;   
            case 5:
                name = "Wizard";
                stats = new double[7]{5+modifier,4+modifier*0.8,8+modifier*1.6,6+modifier*1.2,5+modifier,5+modifier,7};
                break; 
        }
        enemy.GetComponent<Character>().initCharacter(name,depth,(int)stats[0],(int)stats[1],(int)stats[2],(int)stats[3],(int)stats[4],(int)stats[5],(int)stats[6]);
        if(monsternum==5){
            enemy.GetComponent<Character>().roomvisiting=roomcenters;
        }
        //name, strength, agility, constitution, defence, intelligence, wisdom, spriteoffset
    }

    //Finds which room a location is inside
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
                    if(gridstore[coordinate[0],coordinate[1]]==19 | gridstore[coordinate[0],coordinate[1]]==15 | gridstore[coordinate[0],coordinate[1]]==2){
                        moveleft=false;
                    }
                }
                coordinate[0]=coordinate[0]-1;
                if(gridstore[coordinate[0],coordinate[1]]==25 | gridstore[coordinate[0],coordinate[1]]==7 | gridstore[coordinate[0],coordinate[1]]==10){
                    finding=false;
                    blcorners.Add(coordinate);
                }
            }
            //Calculates the centre of each room
            roomcents.Add(new List<int>{((int)((trcorners[i][0]+blcorners[i][0])/2d)),((int)((trcorners[i][1]+blcorners[i][1])/2d))});
        }
        roomcenters=roomcents;
        return roomcents;
    }

    //Function for burrowing through walls to create new corridors
    private bool DoorHole(List<int> currloc, int direction){
        switch(direction){
            //North
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
            //East
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
            //South
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
            //West
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

    //Function for connecting separate rooms with doors and corridors
    private void DoorMaker(){
        List<List<int>> cents = RoomCentFinder();
        List<List<int>> connects = Shortestroomfind(cents);
        for (int i=0;i<connects.Count;i++){
            if ((connects[i][0]!=connects[connects[i][1]][1]) | (connects[i][1]>connects[i][0])){
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
                //While burrower hasn't yet reached target location
                while(!((currloc[0]==cents[connects[i][1]][0]) & (currloc[1]==cents[connects[i][1]][1]))){
                    if (((Math.Abs(difference[0])<Math.Abs(difference[1])) & (difference[0]!=0))| (difference[1]==0)){
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

    //Fixes any corridor generation overlap issues
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
    //This function does not run in the generation of a dungeon due to flaw
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

    //Returns the location of the player
    private int[] PlayerLocate() {
        int[] PlayerLocation = {(int)tilemap.transform.position[0]/-2, (int)tilemap.transform.position[1]/-2};
        return PlayerLocation;
    }

    //Checks if the entity is allowed to move
    private bool MoveAllow(int direction, int[] location) {
        int[] coord=new int[2]{location[0],location[1]};
        switch(direction){
            case 0:
                coord[1]=location[1]+1;
                break;
            case 1:
                coord[0]=location[0]+1;
                break;
            case 2:
                coord[1]=location[1]-1;
                break;
            case 3:
                coord[0]=location[0]-1;
                break;
        }
        bool movevalid = tilemove[gridstore[location[1],location[0]],direction];
        bool touchedinter = false;
        if(NPCs!=null){
            for (int i = 0;i<NPCs.Count;i++){
                List<int[]> holdbound = NPCs[i].GetComponent<NPC>().Boundaries;
                for (int j=0;j<holdbound.Count;j++){
                    if(holdbound[j][0]==coord[0] & holdbound[j][1]==coord[1]){
                        touchedinter=true;
                    }
                }
            } 
        }
        return ((movevalid) && !(touchedinter));
    }

    //Function for calling unique tile generation
    private void TileGen(){
        Random rando = new Random();
        //Random generation between either magma or ice going first as they compete for the same tiles
        int magmaice = rando.Next(1,3);
        TileMaker(45+magmaice);
        TileMaker(45+3-magmaice);
        TileMaker(48);
    }

    //Generates unique tile type
    private void TileMaker(int type){
        int freespots=OpenCounter();
        if(freespots>7){
            Random rando = new Random();
            List<List<int>> validtiles = ValidTiler();
            int randindex = rando.Next(0,validtiles.Count);
            //Spike trap generation
            if(type!=48){
                List<List<int>> queue = new List<List<int>>();
                int counting = 0;
                bool failed = false;
                while(queue.Count<4){
                    randindex = rando.Next(0,validtiles.Count);
                    queue = ConnectedEmpty(validtiles[randindex]);
                    counting++;
                    if(counting == 1000){
                        failed=true;
                        break;
                    }
                }
                if(!failed){
                    List<int> coord = queue[rando.Next(0,queue.Count)];
                    int boundary = queue.Count;
                    if(queue.Count>7){
                        boundary = queue.Count/2;
                    }
                    int tiles = rando.Next(3,boundary);
                    int currtiles=0;
                    gridstore[coord[0],coord[1]]=type;
                    Growingtile(new List<List<int>>{new List<int>{coord[0],coord[1]}},currtiles,tiles,type);
                }
            }
            //Magma and ice tile generation
            else{
                int spikes = rando.Next(1,4);
                int currspike = 0;
                while(currspike<spikes){
                    randindex = rando.Next(0,validtiles.Count);
                    if(gridstore[validtiles[randindex][0],validtiles[randindex][1]]!=type){
                        currspike++;
                        gridstore[validtiles[randindex][0],validtiles[randindex][1]]=type;
                    }
                }
            }
        }
    }

    //Function for growing a spread of unique tiles, used for magma and ice type tiles
    private void Growingtile(List<List<int>> visited, int currtile, int maxtile, int type){
        List<List<int>> store = new List<List<int>>();
        for(int i=0;i<visited.Count;i++){
            //ChECKING NORTH
            if (gridstore[visited[i][0]+1,visited[i][1]]==1){
                if(EmptyQueueCheck(visited,new List<int>{visited[i][0]+1,visited[i][1]})){
                    store.Add(new List<int>{visited[i][0]+1, visited[i][1]});
                }
            }
            //ChECKING EAST
            if (gridstore[visited[i][0],visited[i][1]+1]==1){
                if(EmptyQueueCheck(visited,new List<int>{visited[i][0],visited[i][1]+1})){
                    store.Add(new List<int>{visited[i][0], visited[i][1]+1});
                }
            }
            //ChECKING SOUTH
            if (gridstore[visited[i][0]-1,visited[i][1]]==1){
                if(EmptyQueueCheck(visited,new List<int>{visited[i][0]-1,visited[i][1]})){
                    store.Add(new List<int>{visited[i][0]-1, visited[i][1]});
                }
            }
            //ChECKING WEST
            if (gridstore[visited[i][0],visited[i][1]-1]==1){
                if(EmptyQueueCheck(visited,new List<int>{visited[i][0],visited[i][1]-1})){
                    store.Add(new List<int>{visited[i][0], visited[i][1]-1});
                }
            }
        }
        Random rando = new Random();
        int randint = rando.Next(0,store.Count);
        visited.Add(new List<int>{store[randint][0],store[randint][1]});
        gridstore[store[randint][0],store[randint][1]]=type;
        currtile++;
        if(currtile<maxtile){
            Growingtile(visited, currtile, maxtile, type);
        }
    }

    //Checks that the neighbouring tile is not already in the unique tile spread
    private bool EmptyQueueCheck(List<List<int>> queue,List<int> coordinate){
        bool checking=true;
        for(int i=0;i<queue.Count;i++){
            if((queue[i][0]==coordinate[0]) && (queue[i][1]==coordinate[1])){
                checking=false;
            }
        }
        return checking;
    }

    //Adds tiles to the spread of empty tiles that can be changed by a neighbouring unique tile
    private List<List<int>> ConnectedEmpty(List<int> opener){
        List<List<int>> queue = new List<List<int>>();
        queue.Add(opener);
        bool connecting = true;
        int index = 0;
        while(connecting){
            //CHECKING NORTH
            if (gridstore[queue[index][0]+1,queue[index][1]]==1){
                if(EmptyQueueCheck(queue,new List<int>{queue[index][0]+1,queue[index][1]})){
                    queue.Add(new List<int>{queue[index][0]+1, queue[index][1]});
                }
            }
            //CHECKING EAST
            if (gridstore[queue[index][0],queue[index][1]+1]==1){
                if(EmptyQueueCheck(queue,new List<int>{queue[index][0],queue[index][1]+1})){
                    queue.Add(new List<int>{queue[index][0], queue[index][1]+1});
                }
            }
            //CHECKING SOUTH
            if (gridstore[queue[index][0]-1,queue[index][1]]==1){
                if(EmptyQueueCheck(queue,new List<int>{queue[index][0]-1,queue[index][1]})){
                    queue.Add(new List<int>{queue[index][0]-1, queue[index][1]});
                }
            }
            //CHECKING WEST
            if (gridstore[queue[index][0],queue[index][1]-1]==1){
                if(EmptyQueueCheck(queue,new List<int>{queue[index][0],queue[index][1]-1})){
                    queue.Add(new List<int>{queue[index][0], queue[index][1]-1});
                }
            }
            index = index + 1;
            if(index>=queue.Count){
                connecting=false;
            }
        }
        return queue;
    }

    //Checks the amount of tiles that have an open space
    private int OpenCounter(){
        int counter=0;
        for (int i=0;i<gridsize;i++){
            for(int j=0;j<gridsize;j++){
                if(gridstore[i,j]==1){
                    counter++;
                }
            }
        }
        return counter;
    }
    //Function for performing desired behaviour for unique tile types
    private void LandOnTile(){
        int [] goallocate = new int[2]{(int)goal.position.x/-2,(int)goal.position.y/-2};
        //Ladder
        bool caught = CheckCaught();
        if(!caught){
            if(gridstore[goallocate[1],goallocate[0]]==45){
                popped=true;
                popuptext.text="You have found a ladder down to the next layer! Would you like to descend?";
                popupwindow.SetActive(true);
                descendops.SetActive(true);
                popupops.SetActive(false);
                battleops.SetActive(false);
                stuckops.SetActive(false);
            }
        }
        //Magma
        if(gridstore[goallocate[1],goallocate[0]]==46){
            Player.GetComponent<Character>().Harm(5);
            UIstats();
        }
        //Ice
        if(gridstore[goallocate[1],goallocate[0]]==47){
            if (!caught){
                switch(direction){
                    case 0:
                        goal.position = goal.position - new Vector3(0f, speed*0.5f, 0f);
                        break;
                    case 1:
                        goal.position = goal.position - new Vector3(speed*0.5f, 0f, 0f);
                        break;
                    case 2:
                        goal.position = goal.position - new Vector3(0f, -1*speed*0.5f, 0f);
                        break;
                    case 3:
                        goal.position = goal.position - new Vector3(-1*speed*0.5f, 0f, 0f);
                        break;
                }
                LandOnTile();
                CheckCaught();
            }
        }
        //Spike
        if(gridstore[goallocate[1],goallocate[0]]==48){
            Player.GetComponent<Character>().Harm(5);
            UIstats();
            tilemap.SetTile(new Vector3Int(goallocate[0]*2+1, goallocate[1]*2+1, 0), tiles[48]);
        }
        //If tile damages reduces the player to 0 health, game over
        if(Player.GetComponent<Character>().currhealth==0){
            GameOver();
        }
    }

    //The function for when a player descends to the next layer
    public void Descend(){
        popupwindow.SetActive(false);
        popped=false;
        depth=depth+1;
        GridDisplay();
    }

    //When a player decides not to go down the ladder
    public void NoDescend(){
        popupwindow.SetActive(false);
        popped=false;
    }

    //Creates the text popup for when a player clicks the emergency stuck button
    public void Stuck(){
        popped=true;
        popuptext.text="This button will return you to start with health and mana restored. Would you like to return?";
        stuckops.SetActive(true);
        popupwindow.SetActive(true);
        battleops.SetActive(false);
        nextdiabutton.SetActive(false);
        descendops.SetActive(false);
        popupops.SetActive(false);
    }
    
    //Creates the popup for combat starting
    private void combatpopup(){
        popped=true;
        popuptext.text=("You have been attacked by a "+combatenemy.GetComponent<Character>().charname+"!");
        popupwindow.SetActive(true);
        battleops.SetActive(true);
        stuckops.SetActive(false);
        nextdiabutton.SetActive(false);
        descendops.SetActive(false);
        popupops.SetActive(false);
    }

    //The function for finding the next location to get to in an enemy's patrol route
    private int[] Patrol(int[] enemylocation, int i){
        int[] goalcords = new int[2]{0,0};
        if(enemies[i].GetComponent<Character>().roomvisiting==null){
            enemies[i].GetComponent<Character>().roomvisiting=roomcenters;
        }
        List<List<int>> visiting = enemies[i].GetComponent<Character>().roomvisiting;
        int bestdist=10000000;
        for (int j=0;j<visiting.Count;j++){
            if(enemylocation[0]==visiting[i][1] && enemylocation[1]==visiting[i][0]){
                enemies[i].GetComponent<Character>().roomvisiting.Remove(enemies[i].GetComponent<Character>().roomvisiting[i]);
            }
            else{
                double dist;
                dist = Math.Sqrt((enemylocation[0]-visiting[i][1])*(enemylocation[0]-visiting[i][1])+(enemylocation[1]-visiting[i][0])*(enemylocation[1]-visiting[i][0]));
                if(dist<bestdist){
                    bestdist=(int)dist;
                    goalcords = new int[2]{visiting[i][1],visiting[i][0]};
                }
            }
        }
        return goalcords;
    }

    //Checks if there are any enemies in an enemy's predicted step
    private bool nearbyneighbour(int x, int y){
        for(int i=0;i<enemies.Count;i++){
            if ((enemies[i].GetComponent<Character>().goal.transform.position[0]-tilemap.transform.position.x)/2==x && (enemies[i].GetComponent<Character>().goal.transform.position[1]-tilemap.transform.position.y)/2==y){
                return false;
            }
        }
        return true;
    }

    //Checks if a location is already in the list of locations for movement planning
    private bool direqueuecheck(List<List<int>> queue,int increment){
        bool checking=true;
        for(int i=0;i<queue.Count;i++){
            if((queue[i][0]==queue[increment][0]) && (queue[i][1]==queue[increment][1]) && (i!=increment)){
                checking=false;
            }
        }
        return checking;
    }

    //Function for choosing the direction to move for an enemy
    private int direchoose(int[] enemy, int[] player, int i, int smart){
        List<List<int>> queue = new List<List<int>> {new List<int>{enemy[0],enemy[1],4}};
        int[] goalcords = new int[2]{0,0};
        if(Math.Sqrt((player[0] - enemy[0]) * (player[0] - enemy[0]) + (player[1] - enemy[1]) * (player[1] - enemy[1])) <= 5){
            //Predicts player movement
            if(smart>=3){
                goalcords = new int[2] { (int)goal.position.x/-2, (int)goal.position.y/-2};
            }
            //Chases player location
            else{
                goalcords = new int[2] { (int)transform.position.x/-2, (int)transform.position.y/-2};
            }
        }
        else{
            //Enemey follows patrol route
            goalcords=Patrol(enemy,i);
        }
        bool searching = true;
        int increment = 0;
        while (searching){
            if(smart<=2)
            {
                if (((int)goal.position.x/-2 == queue[increment][0]) && ((int)goal.position.y/-2 == queue[increment][1]))
                {
                    searching = false;
                    return queue[increment][2];
                }
            }
            if ((goalcords[0] == queue[increment][0]) && (goalcords[1] == queue[increment][1]))
            {
                searching = false;
                return queue[increment][2];
            }
            else{
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
        }
        return randomdire(enemy);
    }

    //Prepares the array of valid directions to move in
    private int randomdire(int[] enemy){
        Random rando = new Random();
        List<int> direoptions = new List<int>();
        if (MoveAllow(0, enemy) & nearbyneighbour(enemy[0],enemy[1]+1))
        {
            direoptions.Add(0);
        }
        if (MoveAllow(1, enemy) & nearbyneighbour(enemy[0]+1,enemy[1]))
        {
            direoptions.Add(1);
        }
        if (MoveAllow(2, enemy) & nearbyneighbour(enemy[0],enemy[1]-1))
        {
            direoptions.Add(2);
        }
        if (MoveAllow(3, enemy) & nearbyneighbour(enemy[0]-1,enemy[1]))
        {
            direoptions.Add(3);
        }
        if(direoptions.Count==0){
            direoptions.Add(4);
        }
        return direoptions[rando.Next(0, direoptions.Count)];
    }

    //Chooses a random direction for the player to move in
    private void RandomMove(int[] enemylocation, int[] playerlocation, int i){
        int dire = randomdire(enemylocation);
        //Enemy doesn't move if player lands on them
        if ((enemylocation[0] == (int)goal.position.x/-2) && (enemylocation[1] == (int)goal.position.y/-2)){

        }
        //Moves in a random direction of available options
        else{
            switch(dire){
                case 0:
                    enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(0f, speed*-0.5f, 0f);
                    break;
                case 1:
                    enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(speed*-0.5f, 0f, 0f);
                    break;
                case 2:
                    enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(0f, speed*0.5f, 0f);
                    break;
                case 3:
                    enemies[i].GetComponent<Character>().goal.transform.position = enemies[i].GetComponent<Character>().goal.transform.position - new Vector3(speed*0.5f, 0f, 0f);
                    break;
                case 4:
                    //Nowhere to move
                    break;
            }
        }
    }

    //Moves the enemy if they are able to onto the best tile for reaching the player
    private void ChaseMove(int[] enemylocation, int[] playerlocation,int i, int smart){
        int dire=direchoose(enemylocation, playerlocation, i,smart);
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
                break;
        }
    }

    //Gets the movement type of an enemy
    private int GetSmart(int i){
        switch (enemies[i].GetComponent<Character>().charname){
            //Bandit
            case "Bandit":
                return 3;
            //Orc
            case "Orc":
                return 2;
            //Goblin
            case "Goblin":
                return 2;
            //Slime
            case "Slime":
                return 0;
            //Golem
            case "Golem":
                return 1;
            case "Wizard":
                return 4;
        }
        return 0;
    }

    //Chooses which style of movement an enemy will do based on proximity to player and the enemy movement type
    private void EnemyMove(){
        if(enemies!=null){
            for (int i=0;i<enemies.Count;i++){
                int[] enemylocation = new int[2]{(int)(enemies[i].transform.position.x-tilemap.transform.position.x)/2,(int)(enemies[i].transform.position.y-tilemap.transform.position.y)/2};
                int[] playerlocation = PlayerLocate();
                int smart = GetSmart(i);
                if(smart==4){
                    //Patrols for the player then chases at close proximity
                    ChaseMove(enemylocation,playerlocation,i,smart);
                }
                if(smart==2 || smart==3){
                    //Chases at close proximity but moves randomly when not nearby
                    if (Math.Sqrt((playerlocation[0] - enemylocation[0]) * (playerlocation[0] - enemylocation[0]) + (playerlocation[1] - enemylocation[1]) * (playerlocation[1] - enemylocation[1])) <= 5)
                    {
                        ChaseMove(enemylocation,playerlocation,i,smart);
                    }
                    else
                    {
                        RandomMove(enemylocation,playerlocation,i);
                    }
                }
                //Chases the player when at close range, otherwise doesn't move
                if(smart==1){
                    if (Math.Sqrt((playerlocation[0] - enemylocation[0]) * (playerlocation[0] - enemylocation[0]) + (playerlocation[1] - enemylocation[1]) * (playerlocation[1] - enemylocation[1])) <= 5)
                    {
                        ChaseMove(enemylocation,playerlocation,i,smart);
                    }
                }
                //Moves randomly
                if(smart==0){
                    RandomMove(enemylocation,playerlocation,i);
                }
            }
        }
    }
    
    //Moves all enemies towards their chosen tile
    private void EnemyMoving(){
        if(enemies!=null){
            for (int i=0;i<enemies.Count;i++){
                enemies[i].transform.position = Vector3.MoveTowards(enemies[i].transform.position, enemies[i].GetComponent<Character>().goal.transform.position, speed*Time.deltaTime);
            }
        }
    }

    //Destroys an entity
    private void Kill(int i){
        if(bossfight){
            //Destroys a boss
            Destroy(NPCs[i].GetComponent<Character>().goal);
            Destroy(NPCs[i]);
            NPCs.Remove(NPCs[i]);
        }
        else{
            //Destroys an enemy and creates a new one
            if(!npcfight){
                Destroy(enemies[i].GetComponent<Character>().goal);
                Destroy(enemies[i]);
                enemies.Remove(enemies[i]);
                EnemySpawn();
            }
        }
    }

    //Checks if player has been caught by an enemy
    private bool CheckCaught(){
        if(enemies!=null){
            int[] playerlocation = {(int)goal.transform.position[0]/-2, (int)goal.transform.position[1]/-2};
            for (int i=0;i<enemies.Count;i++){
                int[] enemylocation = new int[2]{(int)(enemies[i].GetComponent<Character>().goal.transform.position.x-tilemap.transform.position.x)/2,(int)(enemies[i].GetComponent<Character>().goal.transform.position.y-tilemap.transform.position.y)/2};
                if(playerlocation[0]==enemylocation[0] & playerlocation[1]==enemylocation[1]){
                    //Finds which enemy collided with player and calls for the combat starting popup
                    combatenemy=enemies[i];
                    combatenemyindex=i;
                    combatpopup();
                    return true;
                }
            }
        }
        return false;
    }

    //Begins combat
    public void CombatStart(){
        fled=false;
        //Prepares combat UI
        battleops.SetActive(false);
        playericon.GetComponent<Image>().sprite=Player.GetComponent<Character>().images[Player.GetComponent<Character>().spriteoffset];
        enemyicon.GetComponent<Image>().sprite=combatenemy.GetComponent<Character>().images[combatenemy.GetComponent<Character>().spriteoffset];
        popupwindow.SetActive(false);
        combatscreen.SetActive(true);
        combattextpanel.SetActive(true);
        combattext.text=("Battle has begun with the "+combatenemy.GetComponent<Character>().charname+"!");
        playername.text=(Player.GetComponent<Character>().charname);
        enemyname.text=(combatenemy.GetComponent<Character>().charname);
        CombatUI();
        //Calculates turn order
        if(Player.GetComponent<Character>().agility>combatenemy.GetComponent<Character>().agility){
            turns=(int)(Player.GetComponent<Character>().agility/combatenemy.GetComponent<Character>().agility);
            currentturn=0;
            playerfirst=true;
        }
        else{
            turns=(int)(combatenemy.GetComponent<Character>().agility/Player.GetComponent<Character>().agility);
            currentturn=0;
            playerfirst=false;
        }
    }

    //Checks whose turn it is
    private int TurnCheck(){
        if(currentturn%(turns+1)==0){
            if(playerfirst){
                //Enemy turn
                return 0;
            }
            else{
                //Player turn
                return 1;
            }
        }
        else{
            if(playerfirst){
                //Player turn
                return 1;
            }
            else{
                //Enemy turn
                return 0;
            }
        }
    }

    //Carries out status effect at the end of a turn
    private void DoEffect(){
        turneffect=true;
        //Player turn
        if(TurnCheck()==1){
            //Checks for burning status
            int burningval=CheckStatus(false,"Burning");
            if(burningval!=-1){
                //Applies burning effect
                Player.GetComponent<Character>().Harm(5);
                Player.GetComponent<Character>().statuses[burningval][2]=(int.Parse(Player.GetComponent<Character>().statuses[burningval][2])-1).ToString();
                if(Player.GetComponent<Character>().statuses[burningval][2]=="0"){
                    Player.GetComponent<Character>().statuses.RemoveAt(burningval);
                    combattext.text=("You took 5 damage from your burns! Your burn has ended!");
                }
                else{
                    combattext.text=("You took 5 damage from your burns!");
                }
                combattextpanel.SetActive(true);
            }
            //Checks for charged status
            int chargedval=CheckStatus(false,"Charged");
            if(chargedval!=-1){
                //Reduces charged counter
                Player.GetComponent<Character>().statuses[chargedval][2]=(int.Parse(Player.GetComponent<Character>().statuses[chargedval][2])-1).ToString();
                if(Player.GetComponent<Character>().statuses[chargedval][2]=="0"){
                    Player.GetComponent<Character>().statuses.RemoveAt(chargedval);
                }
            }
            //If no status effects move onto next turn
            if(chargedval+burningval==-2){
                NextTurn();
            }
        }
        //Enemy turn
        else{
            //Checks if enemy has burning status
            int burningval=CheckStatus(true,"Burning");
            if(burningval!=-1){
                //Applies burning effect
                combatenemy.GetComponent<Character>().Harm(5);
                combatenemy.GetComponent<Character>().statuses[burningval][2]=(int.Parse(combatenemy.GetComponent<Character>().statuses[burningval][2])-1).ToString();
                if(combatenemy.GetComponent<Character>().statuses[burningval][2]=="0"){
                    combatenemy.GetComponent<Character>().statuses.RemoveAt(burningval);
                    combattext.text=(combatenemy.GetComponent<Character>().charname+" took 5 damage from their burns! Their burn has ended!");
                }
                else{
                    combattext.text=(combatenemy.GetComponent<Character>().charname+" took 5 damage from their burns!");
                    
                }
                combattextpanel.SetActive(true);
            }
            //Checks if enemy has charged status
            int chargedval=CheckStatus(true,"Charged");
            if(chargedval!=-1){
                //Reduces charged counter
                combatenemy.GetComponent<Character>().statuses[chargedval][2]=(int.Parse(combatenemy.GetComponent<Character>().statuses[chargedval][2])-1).ToString();
                if(combatenemy.GetComponent<Character>().statuses[chargedval][2]=="0"){
                    combatenemy.GetComponent<Character>().statuses.RemoveAt(chargedval);
                }
            }
            //If no status effects move onto next turn
            if(chargedval+burningval==-2){
                NextTurn();
            }
        }
    }

    //Function for moving onto the next turn or checks if combat should end
    public void NextTurn(){
        //Checks if secondary entity turn
        combattextpanel.SetActive(false);
        if(fled){
            endcombat(true);
        }
        if(Player.GetComponent<Character>().currhealth==0){
            endcombat(false);
        }
        if(combatenemy.GetComponent<Character>().currhealth==0){
            endcombat(true);
        }
        //Changes turn
        if((!fled) && (Player.GetComponent<Character>().currhealth!=0) && (combatenemy.GetComponent<Character>().currhealth!=0)){
            if(turneffect){
                currentturn++;
                if(TurnCheck()==1){
                    Player.GetComponent<Character>().ManaRegen();
                    CombatUI();
                    PlayerTurn();
                    turneffect=false;
                }
                else{
                    combatenemy.GetComponent<Character>().ManaRegen();
                    CombatUI();
                    EnemyTurn();
                    turneffect=false;
                }
            }
            //Performs status effect
            else{
                DoEffect();
            }
        }
    }

    //Provides the exp menu after ending combat and gives level up options if needed
    private void endcombat(bool success){
        UIstats();
        if(npcfight){
            Player.GetComponent<Character>().currmana=Player.GetComponent<Character>().maxmana;
        }
        Player.GetComponent<Character>().statuses=new List<List<string>>();
        Random rando = new Random();
        exppanel.SetActive(true);
        donebutton.SetActive(true);
        combatscreen.SetActive(false);
        strpanel.SetActive(false);
        agipanel.SetActive(false);
        conpanel.SetActive(false);
        defpanel.SetActive(false);
        intpanel.SetActive(false);
        wispanel.SetActive(false);
        exptext.text="exp gained: 0";
        if(success){
            if(fled){
                exptext.text=("ran away\nexp gained 0");
            }
            else{
                int neededexp = (int)(Math.Pow(1.2,Player.GetComponent<Character>().level)*500);
                //Player gains no exp for fighting the Dummy NPC
                if(npcfight){
                    expgained = 0;
                }
                if(bossfight || !(npcfight)){
                    expgained=rando.Next((depth+wins*10)*100,(depth+1+wins*10)*100);
                }
                exptext.text=("exp gained: "+expgained);
                Player.GetComponent<Character>().experience=Player.GetComponent<Character>().experience+expgained;
                //Provides level up stat options
                if (Player.GetComponent<Character>().experience>neededexp){
                    Player.GetComponent<Character>().experience=Player.GetComponent<Character>().experience-neededexp;
                    Player.GetComponent<Character>().level=Player.GetComponent<Character>().level+1;
                    statpoints=3;
                    donebutton.SetActive(false);
                    strpanel.SetActive(true);
                    agipanel.SetActive(true);
                    conpanel.SetActive(true);
                    defpanel.SetActive(true);
                    intpanel.SetActive(true);
                    wispanel.SetActive(true);
                    UpdateStatsView();
                }
            }
        }
        else{
            exppanel.SetActive(false);
            GameOver();
        }
    }

    //Function for aftermath of combat
    public void CombatResult(){
        UIstats();
        if(bossfight){
            //Ends combat, destroys the boss and shows the winning screen
            popped=false;
            Kill(interactindex);
            exppanel.SetActive(false);
            GameOver();
        }
        else{
            //Ends combat and destroys the enemy
            popped=false;
            Kill(combatenemyindex);
            exppanel.SetActive(false);
        }
    }

    //Provides the game over screen for either winning or losing
    private void GameOver(){
        gameoverpanel.SetActive(true);
        if(Player.GetComponent<Character>().currhealth==0){
            gameovertext.text=("Game over\n\nYou failed to beat the game but all is not lost, you still have a chance to save the day\n\n"+
            "get back into the dungeon, become stronger and beat the boss!");
        }
        else{
            wins++;
            gameovertext.text=("Game over\n\nyou have beaten the game, you defeated the boss and you have saved the day.\n\n"+
            "don't want it to be over?\n\ngo back into the dungeon, fight the boss again, except this time it's stronger!");
        }
    }

    //Sends the player back to the lobby layer of the dungeon
    public void SendBack(){
        popped=false;
        gameoverpanel.SetActive(false);
        depth=0;
        Player.GetComponent<Character>().currhealth=Player.GetComponent<Character>().maxhealth;
        Player.GetComponent<Character>().currmana=Player.GetComponent<Character>().maxmana;
        GridDisplay();
    }

    //Resets combat UI for the player turn
    private void PlayerTurn(){
        combatoptions.SetActive(true);
    }

    //Returns the amount of available spells that the current enemy can cast
    private int AvailableSpells(){
        int curr=combatenemy.GetComponent<Character>().currmana;
        List<int> costs = new List<int>{50,40,25,35,45};
        int spellcount=0;
        for(int i=0;i<costs.Count;i++){
            if(curr>=costs[i]){
                spellcount++;
            }
        }
        return spellcount;
    }

    //Choose the weighted random behaviour of an enemy
    private void EnemyTurn(){
        //bandit orc goblin slime golem
        Random rando = new Random();
        int spellcount = AvailableSpells();
        int chooseattack=0;
        int choosedefend=0;
        int choosespell=0;
        int chooseflee=0;
        //Regular enemy behaviour
        if(depth!=bosslevel){
            switch(combatenemy.GetComponent<Character>().charname){
                case "Bandit":
                    if((combatenemy.GetComponent<Character>().currhealth*100)/combatenemy.GetComponent<Character>().maxhealth<=20){
                        //35% attack, 45% defend, 20% flee
                        chooseattack=35;
                        choosedefend=80;
                        chooseflee=100;
                    }
                    else{
                        //50% attack, 45% defend, 5% flee
                        chooseattack=50;
                        choosedefend=95;
                        chooseflee=100;
                    }
                    break;
                case "Orc":
                    if((combatenemy.GetComponent<Character>().currhealth*100)/combatenemy.GetComponent<Character>().maxhealth<=20){
                        //95% attack, 5% flee
                        chooseattack=95;
                        chooseflee=100;
                    }
                    else{
                        //90% attack, 10% defend
                        chooseattack=90;
                        choosedefend=100;
                    }
                    break;
                case "Goblin":
                    //70% attack, 20% defend, 10% flee
                    if((combatenemy.GetComponent<Character>().currhealth*100)/combatenemy.GetComponent<Character>().maxhealth<=30){
                        //70% attack, 15% defend, 15% flee
                        chooseattack=70;
                        choosedefend=85;
                        chooseflee=100;
                    }
                    else{
                        //75% attack, 20% defend, 5% flee
                        chooseattack=75;
                        choosedefend=95;
                        chooseflee=100;
                    }
                    break;
                case "Slime":
                    if((combatenemy.GetComponent<Character>().currhealth*100)/combatenemy.GetComponent<Character>().maxhealth<=25){
                        //60% attack, 15% defend, 25% flee
                        chooseattack=60;
                        choosedefend=75;
                        chooseflee=100;
                    }
                    else{
                        //60% attack, 30% defend, 10% flee
                        chooseattack=60;
                        choosedefend=90;
                        chooseflee=100;
                    }
                    break;
                case "Golem":
                    //25% attack, 70% defend, 5% flee
                    chooseattack=25;
                    choosedefend=95;
                    chooseflee=100;
                    break;
                case "Wizard":
                    //70% attack, 5-30% defend, 0-25% spell
                    chooseattack=70;
                    choosedefend=100-spellcount*5;
                    choosespell=100;
                    break;
                case "Dummy":
                    chooseattack=50;
                    choosedefend=100;
                    break;
            }
        }
        //Boss enemy behaviour
        else{
            switch(combatenemy.GetComponent<Character>().charname){
                case "Bandit":
                    if(combatenemy.GetComponent<Character>().currmana>=35){
                        chooseattack=60;
                        choosedefend=90;
                        choosespell=100;
                    }
                    else{
                        //55% attack, 45% defend
                        chooseattack=55;
                        choosedefend=100;
                    }
                    break;
                case "Orc":
                    if(combatenemy.GetComponent<Character>().currmana>=35){
                        chooseattack=85;
                        choosespell=100;
                    }
                    else{
                        //90% attack, 10% defend
                        chooseattack=90;
                        choosedefend=100;
                    }
                    break;
                case "Goblin":
                    if(combatenemy.GetComponent<Character>().currmana>=35){
                        //60% attack, 20% defend, 20% spell
                        chooseattack=60;
                        choosedefend=80;
                        choosespell=100;
                    }
                    else{
                        //75% attack, 25% defend
                        chooseattack=75;
                        choosedefend=100;
                    }
                    break;
                case "Slime":
                    if(combatenemy.GetComponent<Character>().currmana>=50){
                        //80% attack, 20% defend
                        chooseattack=80;
                        choosespell=100;
                    }
                    else{
                        //80% attack, 20% defend
                        chooseattack=80;
                        choosedefend=100;
                    }
                    break;
                case "Golem":
                    if(combatenemy.GetComponent<Character>().currmana>=40){
                        //30% attack, 50% defend, 20% spell
                        chooseattack=30;
                        choosedefend=80;
                        choosespell=100;
                    }
                    else{
                        //30% attack, 70% defend
                        chooseattack=30;
                        choosedefend=100;
                    }
                    break;
                case "Wizard":
                    //75% attack, 0-25% defend, 0-25% spell
                    chooseattack=75;
                    choosedefend=100-spellcount*5;
                    choosespell=100;
                    break;
            }
        }
        //Determines which behaviour is performed on the random chance
        int roll = rando.Next(1,101);
        if(roll<=chooseattack){
            Attack();
        }
        else{
            if(roll<=choosedefend){
                Defend();
            }
            else{
                if(choosespell<chooseflee){
                    if(roll<=choosespell){
                        EnemySpells();
                    }
                    else{
                        Flee();
                    }
                }
                else{
                    if(roll<=chooseflee){
                        Flee();
                    }
                    else{
                        EnemySpells();
                    }
                }
            }
        }
    }

    //Chooses a spell at random for the list of spells currently castable by the enemy
    private void EnemySpells(){
        List<int> options = new List<int>();
        int currmana=combatenemy.GetComponent<Character>().currmana;
        switch(combatenemy.GetComponent<Character>().charname){
            case "Bandit":
                if(currmana>=35){
                    options.Add(3);
                }
                break;
            case "Orc":
                if(currmana>=35){
                    options.Add(3);
                }
                break;
            case "Goblin":
                if(currmana>=25){
                    options.Add(2);
                }
                break;
            case "Slime":
                if((currmana>=50) && (combatenemy.GetComponent<Character>().currhealth<combatenemy.GetComponent<Character>().maxhealth)){
                    options.Add(0);
                }
                break;
            case "Golem":
                if(currmana>=40){
                    options.Add(1);
                }
                break;
            case "Wizard":
                if((currmana>=50) && (combatenemy.GetComponent<Character>().currhealth<combatenemy.GetComponent<Character>().maxhealth)){
                    options.Add(0);
                }
                if(currmana>=40){
                    options.Add(1);
                }
                if(currmana>=25){
                    options.Add(2);
                }
                if(currmana>=35){
                    options.Add(3);
                }
                if(currmana>=45){
                    options.Add(4);
                }
                break;
        }
        Random rando = new Random();
        tempspell=options[rando.Next(0,options.Count)];
        Spells();
    }

    //Checks if an entity currently has a particular status or not
    private int CheckStatus(bool playerenemy,string status){
        int found=-1;
        if(playerenemy){//Enemy
            for(int i=0;i<combatenemy.GetComponent<Character>().statuses.Count;i++){
                if(combatenemy.GetComponent<Character>().statuses[i][0]==status){
                    found=i;
                }
            }
        }
        else{//Player
            for(int i=0;i<Player.GetComponent<Character>().statuses.Count;i++){
                if(Player.GetComponent<Character>().statuses[i][0]==status){
                    found=i;
                }
            }
        }
        return found;
    }

    //Function for performing a spell attack with intelligence as the attack stat
    private void SpellAttack(int power, string spellname){
        int damage = 0;
        //Player turn
        if(TurnCheck()==1){
            int defence=combatenemy.GetComponent<Character>().defence;
            int attackstat=Player.GetComponent<Character>().intelligence;
            //Checks if player has charged status
            int chargefound=CheckStatus(false,"Charged");
            if(chargefound!=-1){
                attackstat = (int)(attackstat*1.2);
            }
            //Checks if enemy has defended status
            int defendfound=CheckStatus(true,"Defending");
            if(defendfound!=-1){
                defence=defence*2;
                combatenemy.GetComponent<Character>().statuses.RemoveAt(defendfound);
            }
            //Damage calculation
            damage = (int)(((2*Player.GetComponent<Character>().level)/3+2)*power*attackstat/defence)/10;
            combatenemy.GetComponent<Character>().Harm(damage);
            combattext.text=("Your "+spellname+" hit the " +combatenemy.GetComponent<Character>().charname+" for "+damage+" damage!");
        }
        //Enemy turn
        else{
            int defence=Player.GetComponent<Character>().defence;
            int attackstat=combatenemy.GetComponent<Character>().intelligence;
            //Checks if enemy has charged status
            int chargefound=CheckStatus(true,"Charged");
            if(chargefound!=-1){
                attackstat = (int)(attackstat*1.2);
            }
            //Checks if player has defended status
            int defendfound=CheckStatus(false,"Defending");
            if(defendfound!=-1){
                defence=defence*2;
                Player.GetComponent<Character>().statuses.RemoveAt(defendfound);
            }
            //Damage calculation
            damage = (int)(((2*combatenemy.GetComponent<Character>().level)/3+2)*power*attackstat/defence)/10;
            Player.GetComponent<Character>().Harm(damage);
            combattext.text=("You were attacked by the "+combatenemy.GetComponent<Character>().charname+"'s "+spellname+" for "+damage+" damage!");
        }
        combattextpanel.SetActive(true);
        CombatUI();
    }

    //Performs a normal attack against a target using strength as the attack stat
    public void Attack(){
        int damage = 0;
        //Player turn
        if(TurnCheck()==1){
            int defence=combatenemy.GetComponent<Character>().defence;
            int attackstat=Player.GetComponent<Character>().strength;
            //Checks if player has charged status
            int chargefound=CheckStatus(false,"Charged");
            if(chargefound!=-1){
                attackstat = (int)(attackstat*1.2);
            }
            //Checks if enemy has defending status
            int defendfound=CheckStatus(true,"Defending");
            if(defendfound!=-1){
                defence=defence*2;
                combatenemy.GetComponent<Character>().statuses.RemoveAt(defendfound);
            }
            //Calculates and deals damage
            damage = (int)(((2*Player.GetComponent<Character>().level)/3+2)*40*attackstat/defence)/10;
            combatenemy.GetComponent<Character>().Harm(damage);
            combattext.text=("You attacked the "+combatenemy.GetComponent<Character>().charname+" for "+damage+" damage!");
        }
        //Enemy turn
        else{
            int defence=Player.GetComponent<Character>().defence;
            int attackstat=combatenemy.GetComponent<Character>().strength;
            //Checks if enemy has charged status
            int chargefound=CheckStatus(true,"Charged");
            if(chargefound!=-1){
                attackstat = (int)(attackstat*1.2);
            }
            //Checks if player has defending status
            int defendfound=CheckStatus(false,"Defending");
            if(defendfound!=-1){
                defence=defence*2;
                Player.GetComponent<Character>().statuses.RemoveAt(defendfound);
            }
            //Calculates and deals damage
            damage = (int)(((2*combatenemy.GetComponent<Character>().level)/3+2)*40*attackstat/defence)/10;
            Player.GetComponent<Character>().Harm(damage);
            combattext.text=("You were attacked by the "+combatenemy.GetComponent<Character>().charname+" for "+damage+" damage!");
        }
        combattextpanel.SetActive(true);
        CombatUI();
    }

    //Adds the defending status to the user
    public void Defend(){
        //Player turn
        if(TurnCheck()==1){
            Player.GetComponent<Character>().statuses.Add(new List<string>{"Defending","Attack","1"});
            combattext.text=("You have defended yourself against an attack!");
        }
        //Enemy turn
        else{
            combatenemy.GetComponent<Character>().statuses.Add(new List<string>{"Defending","Attack","1"});
            combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" has defended itself against an attack!");
        }
        combattextpanel.SetActive(true);
    }

    //Creates a spell description for currently selected spell
    public void Spellview(int spellchoice){
        spellviewpanel.SetActive(true);
        tempspell=spellchoice;
        spelldescription.text = ("Spell: "+spelllist[tempspell][0]+"\nCost: "+spelllist[tempspell][1]+" mana\n" +spelllist[tempspell][2]);
        if(Player.GetComponent<Character>().currmana>=int.Parse(spelllist[tempspell][1])){
            spellcastbutton.SetActive(true);
        }
        else{
            spellcastbutton.SetActive(false);
        }
    }

    //Performs the effect for a spell
    public void Spells(){
        Random rando = new Random();
        //Player turn
        if(TurnCheck()==1){
            Player.GetComponent<Character>().currmana = Player.GetComponent<Character>().currmana - int.Parse(spelllist[tempspell][1]);
            switch(tempspell){
                case 0: //Heal Wounds, heals the user for 3 x their intelligence stat
                    int healing = 3*Player.GetComponent<Character>().intelligence;
                    Player.GetComponent<Character>().Heal(healing);
                    combattext.text=("You have healed yourself for "+healing+" health!");
                    combattextpanel.SetActive(true);
                    break;
                case 1: //Breathe Fire, applies burning status to target if they don't already have it
                    if(CheckStatus(true,"Burning")==-1){
                        combatenemy.GetComponent<Character>().statuses.Add(new List<string>{"Burning","Turns",rando.Next(2,6).ToString()});
                        combattext.text=("Your fire breath set the "+combatenemy.GetComponent<Character>().charname+" on fire!");
                    }
                    else{
                        combattext.text=("You cast breathe fire but the "+combatenemy.GetComponent<Character>().charname+" is already on fire!");
                    }
                    combattextpanel.SetActive(true);
                    break;
                case 2: //Arcane Knife, deals a 40 power attack
                    SpellAttack(40,"Arcane Knife");
                    break;
                case 3: //Power Charge, applies the charged status to the user
                    if(CheckStatus(false,"Charged")==-1){
                        Player.GetComponent<Character>().statuses.Add(new List<string>{"Charged","Turns","10"});
                        combattext.text=("Your power charge fills you with fighting energy!");
                    }
                    else{
                        combattext.text=("Your power charge failed as you are already charged!");
                    }
                    combattextpanel.SetActive(true);
                    break;
                case 4: //Wild Surge, has a 33% chance of dealing a 120 power attack
                    int chance = rando.Next(1,101);
                    if(chance<=33){
                        SpellAttack(120,"Wild Surge");
                    }
                    else{
                        combattext.text=("Your attempted wild surge spell failed!");
                        combattextpanel.SetActive(true);
                    }
                    break;
            }
        }
        //Enemy turn
        else{
            combatenemy.GetComponent<Character>().currmana = combatenemy.GetComponent<Character>().currmana - int.Parse(spelllist[tempspell][1]);
            switch(tempspell){
                case 0: //Heal Wounds, heals the user for 3 x their intelligence stat
                    int healing = 3*combatenemy.GetComponent<Character>().intelligence;
                    combatenemy.GetComponent<Character>().Heal(healing);
                    combattext.text=(combatenemy.GetComponent<Character>().charname+" healed themself for "+healing+" health!");
                    combattextpanel.SetActive(true);
                    break;
                case 1: //Breathe Fire, applies burning status to target if they don't already have it
                    if(CheckStatus(false,"Burning")==-1){
                        Player.GetComponent<Character>().statuses.Add(new List<string>{"Burning","Turns",rando.Next(2,6).ToString()});
                        combattext.text=("The "+combatenemy.GetComponent<Character>().charname+"'s fire breath set you on fire!");
                    }
                    else{
                        combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" cast breathe fire but you are already on fire!");
                    }
                    combattextpanel.SetActive(true);
                    break;
                case 2: //Arcane Knife, deals a 40 power attack
                    SpellAttack(40,"Arcane Knife");
                    break;
                case 3: //Power Charge, applies the charged status to the user
                    if(CheckStatus(true,"Charged")==-1){
                        combatenemy.GetComponent<Character>().statuses.Add(new List<string>{"Charged","Turns","10"});
                        combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" used power charge to strengthen themself!");
                    }
                    else{
                        combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" used power charge but they were already strengthened!");
                    }
                    combattextpanel.SetActive(true);
                    break;
                case 4: //Wild Surge, has a 33% chance of dealing a 120 power attack
                    int chance = rando.Next(1,101);
                    if(chance<=33){
                        SpellAttack(120,"Wild Surge");
                    }
                    else{
                        combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" attempted to use wild surge but failed!");
                        combattextpanel.SetActive(true);
                    }
                    break;
            }
        }
        CombatUI();
    }

    //User attempts to free from combat, automatic success if faster, chance of success if slower
    public void Flee(){
        int playspeed=Player.GetComponent<Character>().agility;
        int enemspeed=combatenemy.GetComponent<Character>().agility;
        int chance=0;
        Random rando = new Random();
        int roll = rando.Next(1,101);
        combattextpanel.SetActive(true);
        if(TurnCheck()==1){
            //Player Turn
            chance=(int)((playspeed*100)/enemspeed);
            if((roll<=chance) && !(bossfight)){
                //Successfully fled
                combattext.text=("You have successfully fled from combat!");
                fled=true;
            }
            else{
                //Failed to flee
                combattext.text=("You were not able to flee from combat!");
            }
        }
        else{
            //Enemy Turn
            chance=(int)((enemspeed*100)/playspeed);
            if(roll<=chance){
                //Successfully fled
                combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" has successfully run away!");
                fled=true;
            }
            else{
                //Failed to flee
                combattext.text=("The "+combatenemy.GetComponent<Character>().charname+" tried to run away but failed!");
            }
        }
    }

    //Function for calculating the top left Heads Up Display stats
    private void UIstats(){
        UInametext.text = (Player.GetComponent<Character>().charname+" - LVL "+Player.GetComponent<Character>().level);
        UIHPtext.text = ("Health: "+Player.GetComponent<Character>().currhealth+"/"+Player.GetComponent<Character>().maxhealth);
        UImanatext.text = ("Mana: "+Player.GetComponent<Character>().currmana+"/"+Player.GetComponent<Character>().maxmana);
        UIEXPtext.text = ("EXP: "+Player.GetComponent<Character>().experience+"/"+(int)(Math.Pow(1.2,Player.GetComponent<Character>().level)*500));
        UIdepthtext.text =("Depth: "+depth);
    }

    //Updates usable stat points when used
    public void UpdateStats(string Option){
        if(statpoints>0){
            statpoints--;
            Player.GetComponent<Character>().ChangeStat(Option,1);
            UpdateStatsView();
        }
    }

    //Updates stat allocation display upon change
    private void UpdateStatsView(){
        exptext.text=("exp gained: "+expgained+"\nlevel up\nusable points: "+statpoints);
        strtext.text=("strength: "+Player.GetComponent<Character>().strength);
        agitext.text=("agility: "+Player.GetComponent<Character>().agility);
        context.text=("constitution: "+Player.GetComponent<Character>().constitution);
        deftext.text=("defence: "+Player.GetComponent<Character>().defence);
        inttext.text=("intelligence: "+Player.GetComponent<Character>().intelligence);
        wistext.text=("wisdom: "+Player.GetComponent<Character>().wisdom);
        donebutton.SetActive(false);
        strpanel.SetActive(true);
        agipanel.SetActive(true);
        conpanel.SetActive(true);
        defpanel.SetActive(true);
        intpanel.SetActive(true);
        wispanel.SetActive(true);
        if(statpoints==0){
            donebutton.SetActive(true);
        }
    }

    //Sets the combat UI and updates the health and mana bars for player and enemy stats when needed
    private void CombatUI(){
        playerhealth.text=(Player.GetComponent<Character>().currhealth+"/"+Player.GetComponent<Character>().maxhealth);
        playermana.text=(Player.GetComponent<Character>().currmana+"/"+Player.GetComponent<Character>().maxmana);
        enemyhealth.text=(combatenemy.GetComponent<Character>().currhealth+"/"+combatenemy.GetComponent<Character>().maxhealth);
        enemymana.text=(combatenemy.GetComponent<Character>().currmana+"/"+combatenemy.GetComponent<Character>().maxmana);

        int playerhealthratio=(int)((Player.GetComponent<Character>().currhealth*100)/Player.GetComponent<Character>().maxhealth);
        int enemyhealthratio=(int)((combatenemy.GetComponent<Character>().currhealth*100)/combatenemy.GetComponent<Character>().maxhealth);
        float playerhealthdiff=playerhealthbar.GetComponent<RectTransform>().rect.width;
        float enemyhealthdiff=enemyhealthbar.GetComponent<RectTransform>().rect.width;

        playerhealthbar.GetComponent<RectTransform>().sizeDelta = new Vector2 (playerhealthratio*3, 50);
        enemyhealthbar.GetComponent<RectTransform>().sizeDelta = new Vector2 (enemyhealthratio*3, 50);
        playerhealthdiff=(playerhealthdiff-playerhealthbar.GetComponent<RectTransform>().rect.width)/3;
        enemyhealthdiff=(enemyhealthdiff-enemyhealthbar.GetComponent<RectTransform>().rect.width)/3;
        playerhealthbar.transform.position -= new Vector3(playerhealthdiff*1.5f, 0, 0);
        enemyhealthbar.transform.position -= new Vector3(enemyhealthdiff*1.5f, 0, 0);

        int playermanaratio=(int)((Player.GetComponent<Character>().currmana*100)/Player.GetComponent<Character>().maxmana);
        int enemymanaratio=(int)((combatenemy.GetComponent<Character>().currmana*100)/combatenemy.GetComponent<Character>().maxmana);
        float playermanadiff=playermanabar.GetComponent<RectTransform>().rect.width;
        float enemymanadiff=enemymanabar.GetComponent<RectTransform>().rect.width;

        playermanabar.GetComponent<RectTransform>().sizeDelta = new Vector2 (playermanaratio*3, 50);
        enemymanabar.GetComponent<RectTransform>().sizeDelta = new Vector2 (enemymanaratio*3, 50);
        playermanadiff=(playermanadiff-playermanabar.GetComponent<RectTransform>().rect.width)/3;
        enemymanadiff=(enemymanadiff-enemymanabar.GetComponent<RectTransform>().rect.width)/3;
        playermanabar.transform.position -= new Vector3(playermanadiff*1.5f, 0, 0);
        enemymanabar.transform.position -= new Vector3(enemymanadiff*1.5f, 0, 0);
    }

    //Interacts with target in front of player
    private void Interact(int[] coords){
        if(NPCs!=null){
            for (int i = 0;i<NPCs.Count;i++){
                List<int[]> holdbound = NPCs[i].GetComponent<NPC>().Boundaries;
                for (int j=0;j<holdbound.Count;j++){
                    if(holdbound[j][0]==coords[0] & holdbound[j][1]==coords[1]){
                        interactindex=i;
                        popped=true;
                    }
                }
            } 
        }
        if(popped){
            DialogueScroll();
        }
    }

    //Scrolls through dialogue window with NPC
    public void DialogueScroll(){
        string result = NPCs[interactindex].GetComponent<NPC>().DiaRead();
        popuptext.text=result;
        popupwindow.SetActive(true);
        nextdiabutton.SetActive(true);
        battleops.SetActive(false);
        stuckops.SetActive(false);
        descendops.SetActive(false);
        popupops.SetActive(false);
        if(NPCs[interactindex].GetComponent<NPC>().DialogueSteps+1>=NPCs[interactindex].GetComponent<NPC>().Dialogue.Count){
            nextdiatext.text=NPCs[interactindex].GetComponent<NPC>().PostDialogue;
        }
        else{
            nextdiatext.text="Next";
        }
        if(result=="Exit"){
            nextdiabutton.SetActive(false);
            popupwindow.SetActive(false);
            popped=false;
        }
        if(result=="Fight"){
            nextdiabutton.SetActive(false);
            popupwindow.SetActive(false);
            combatenemy=NPCs[interactindex];
            npcfight=true;
            if(combatenemy.GetComponent<NPC>().Type=="BOSS"){
                bossfight=true;
            }

            combatpopup();
        }
    }

    //Prepares the start of the main game upon loading
    void Start() {
        depth=0;
        wins=0;
        popped=false;
        GridDisplay();
        goal.SetParent(null);
        Player.GetComponent<Character>().initCharacter(MainMenu.username,1,10,10,10,10,10,10,0);
        bossfight=false;
        npcfight=false;
        UIstats();
    }

    //Update function used for detection of player input
    void Update() {
        //Movement works through moving a goal object then sliding the tilemap towards it
        transform.position = Vector3.MoveTowards(transform.position, goal.position, speed*Time.deltaTime);
        EnemyMoving();
        if(!popped){
            if(Vector3.Distance(transform.position,goal.position) == 0f){
                if(tileactive){
                    if(Input.GetAxisRaw("Horizontal")!=0f){
                        if(Input.GetAxisRaw("Horizontal")==1f){
                            //Moving right
                            direction=1;
                            if(MoveAllow(1,PlayerLocate())){
                                tileactive=false;
                                goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                                Player.GetComponent<Character>().ManaRegen();
                                UIstats();
                                EnemyMove();
                            }
                        }
                        if(Input.GetAxisRaw("Horizontal")==-1f){
                            //Moving left
                            direction=3;
                            if(MoveAllow(3,PlayerLocate())){
                                tileactive=false;
                                goal.position = goal.position - new Vector3(Input.GetAxisRaw("Horizontal")*speed*0.5f, 0f, 0f);
                                Player.GetComponent<Character>().ManaRegen();
                                UIstats();
                                EnemyMove();
                            }
                        }
                    }
                    else if(Input.GetAxisRaw("Vertical")!=0f){
                        if(Input.GetAxisRaw("Vertical")==1f){
                            //Moving up
                            direction=0;
                            if(MoveAllow(0,PlayerLocate())){
                                tileactive=false;
                                goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                                Player.GetComponent<Character>().ManaRegen();
                                UIstats();
                                EnemyMove();
                            }
                        }
                        if(Input.GetAxisRaw("Vertical")==-1f){
                            //Moving down
                            direction=2;
                            if(MoveAllow(2,PlayerLocate())){
                                tileactive=false;
                                goal.position = goal.position - new Vector3(0f, Input.GetAxisRaw("Vertical")*speed*0.5f, 0f);
                                Player.GetComponent<Character>().ManaRegen();
                                UIstats();
                                EnemyMove();
                            }
                        }
                    }
                }
                else{
                    //Checks if caught by enemy then if landed on unique tile
                    tileactive=true;
                    CheckCaught();
                    LandOnTile();
                }
                //Player interacts with the object directly in front of them
                if (Input.GetKeyDown(KeyCode.E))
                {
                    int[] intercords = PlayerLocate();
                    switch(direction){
                        case 0:
                            Interact(new int[2]{intercords[0],intercords[1]+1});
                            break;
                        case 1:
                            Interact(new int[2]{intercords[0]+1,intercords[1]});
                            break;
                        case 2:
                            Interact(new int[2]{intercords[0],intercords[1]-1});
                            break;
                        case 3:
                            Interact(new int[2]{intercords[0]-1,intercords[1]});
                            break;
                    }
                }
            }
        }
    }
}