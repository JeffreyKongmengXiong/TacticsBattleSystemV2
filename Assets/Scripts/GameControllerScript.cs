using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    // The Main Goal of the Game Controller is to manage pretty much everything to do with the game's mechanics

    [SerializeField]
    private GameObject floorObject;

    ////////////////////////////////////// [BOARD VARIABLES] //////////////////////////////////////
    public GameObject[,] boardGrid = new GameObject[10, 10];
    public List<GameObject> tileList = new List<GameObject>();


    ////////////////////////////////////// [UNIT VARIABLES] ///////////////////////////////////////
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();

    ////////////////////////////////////// [MOVE VARIABLES] ///////////////////////////////////////
    public bool findingMovement = false;
    public bool movingUnit = false;

    ////////////////////////////////////// [CURSOR VARIABLES] ///////////////////////////////////////
    [SerializeField]
    private GameObject cursorObject;
    public CursorScript cursorScript;
    public GameObject currentTileObject;
    public GameObject currentUnitObject;
    public UnitScript currentUnitScript;
    public int[] currentPosition;
    public GameObject selectedUnitObject;
    public UnitScript selectedUnitScript;

    // Menus can probably be it's own script and object. I might change that later
    ////////////////////////////////////// [MENU VARIABLES] ///////////////////////////////////////
    public bool isMenuOpened;
    public GameObject optionMenuIndicator;
    
    public Canvas playerMenu;
    public PlayerMenuScript playerMenuScript;
    public bool isPlayerMenuOpened;
    public int playerMenuOptions = 0;

    public int direction = 0;

    // Create the map and setups the cursor script reference
    void Start()
    {
        CreateMap();
        cursorScript = cursorObject.GetComponent<CursorScript>();
        playerMenuScript = playerMenu.gameObject.GetComponent<PlayerMenuScript>();
    }
    
    void Update()
    {
        // Updates the main unit list
        UpdateUnitList();

        // this will try to get the current unit's information that the cursor is hovering over
        try
        {
            currentUnitScript = currentUnitObject.GetComponent<UnitScript>();
            currentPosition = currentUnitScript.currentPosition;
        }
        // If it can't then we'll just make it null.
        catch
        {
            currentUnitScript = null;
            currentPosition = null;
        }

        // if the menu is open, and we're not moving the character then we continue
        if (isMenuOpened == true && !findingMovement)
        {
            // This will check to see if the menu opened is the player menu, if it is then we continue
            if (isPlayerMenuOpened)
            {
                // This fuction will navigate through the playermenu
                NavigatingPlayerMenu();
            }
        }
        // If the menu is open, and we are able to move the character, then that means they have chosen the movement option so now
        // we will allow the player to move the currently selected unit.
        else if (isMenuOpened == true && findingMovement)
        {
            if (!movingUnit)
            {
                CheckMovement();
            }
            else
            {
                selectedUnitScript.MovePath();
            }
        }
        // If there is not menu open then we continue
        else
        {
            if (playerList.Contains(currentUnitObject))
            {
                // The next following if statements will check to see if the current unit the cursor is hovering over is in the player list
                // and if it is then we will open the player menu for the player unit.
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (isMenuOpened == false)
                    {
                        isMenuOpened = true;
                        isPlayerMenuOpened = true;
                        OpenPlayerMenu();
                    }
                }
                // If they want to redo their movement then this will let them redo it
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    if (selectedUnitObject != null)
                    {
                        if (isMenuOpened == false && selectedUnitScript.moved == true)
                        {
                            playerMenuScript.movementButton.interactable = true;
                            selectedUnitScript.transform.position = selectedUnitScript.originalPositionVector;
                            cursorScript.transform.position = selectedUnitScript.originalPositionVector;
                            selectedUnitScript.moved = false;
                        }
                    }
                }
            }
        }

        if (currentUnitObject != null)
        {
            if (playerList.Contains(currentUnitObject))
            {
                if (Input.GetKey(KeyCode.X))
                {
                    cursorScript.canMoveCursor = false;
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        Debug.Log("This worked?");
                        if (direction == 0)
                        {
                            currentUnitScript.direction = 1;
                        }
                        else if (direction == 1)
                        {
                            currentUnitScript.direction = 2;
                        }
                        else if (direction == 2)
                        {
                            currentUnitScript.direction = 3;
                        }
                        else if (direction == 3)
                        {
                            currentUnitScript.direction = 0;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (direction == 0)
                        {
                            currentUnitScript.direction = 2;
                        }
                        else if (direction == 1)
                        {
                            currentUnitScript.direction = 3;
                        }
                        else if (direction == 2)
                        {
                            currentUnitScript.direction = 0;
                        }
                        else if (direction == 3)
                        {
                            currentUnitScript.direction = 1;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (direction == 0)
                        {
                            currentUnitScript.direction = 3;
                        }
                        else if (direction == 1)
                        {
                            currentUnitScript.direction = 0;
                        }
                        else if (direction == 2)
                        {
                            currentUnitScript.direction = 1;
                        }
                        else if (direction == 3)
                        {
                            currentUnitScript.direction = 2;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (direction == 0)
                        {
                            currentUnitScript.direction = 0;
                        }
                        else if (direction == 1)
                        {
                            currentUnitScript.direction = 1;
                        }
                        else if (direction == 2)
                        {
                            currentUnitScript.direction = 2;
                        }
                        else if (direction == 3)
                        {
                            currentUnitScript.direction = 3;
                        }
                    }
                }
                else if (Input.GetKeyUp(KeyCode.X))
                {
                    cursorScript.canMoveCursor = true;
                }
            }
        }
    }

    // This updates the unit list, this probably isn't needed unless one of the units summons another unit so unless we do that later then this is useless.
    public void UpdateUnitList()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (!unitList.Contains(playerList[i]))
            {
                unitList.Add(playerList[i]);
            }
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (!unitList.Contains(enemyList[i]))
            {
                unitList.Add(enemyList[i]);
            }
        }
    }

    // This creates the map, just a randomly generated one, nothing too special to see here
    public void CreateMap()
    {
        for (int z = 0; z < 10; z++)
        {
            for (int x = 0; x < 10; x++)
            {
                int check = Random.Range(0, 5);
                float height = -0.5f;
                if (check == 0 || check == 4)
                {
                    height += Random.Range(-1, 2);
                }
                GameObject newBlock = Instantiate(floorObject, new Vector3((float)x, (float)height, (float)z), Quaternion.identity);
                newBlock.name = "[" + x + ", " + z + ", " + (height + 1.5f) + "]";
                boardGrid[x, z] = newBlock;
                tileList.Add(newBlock);
            }
        }
    }
    
    // This is the code to navigate through the player menu
    public void NavigatingPlayerMenu()
    {
        // In the menu, if the player presses down the up and down key then they will move the menu indicator for other options.
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (playerMenuOptions < 2)
            {
                playerMenuOptions++;
                optionMenuIndicator.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -35);
            }
            else
            {
                playerMenuOptions = 0;
                optionMenuIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 150);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (playerMenuOptions > 0)
            {
                playerMenuOptions--;
                optionMenuIndicator.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 35);
            }
            else
            {
                playerMenuOptions = 2;
                optionMenuIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 150-70);
            }
        }
        // If the player presses Z they will select an option
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // option '0' is the movement option so now we activate the MovementSelectionMode Fuction and set moving to true
            // so that we can move the cursor again. we also close the menu temporarily so that we can see where the unit can
            // move.
            if (playerMenuOptions == 0 && selectedUnitScript.moved == false)
            {
                playerMenu.gameObject.SetActive(false);
                selectedUnitScript.GetMoveableTiles();
                cursorScript.canMoveCursor = true;
                findingMovement = true;
            }
            // option '2' is to close the menu and return to the map view, pressing X does the same
            else if (playerMenuOptions == 2)
            {
                isMenuOpened = false;
                isPlayerMenuOpened = false;
                cursorScript.canMoveCursor = true;
                playerMenu.gameObject.SetActive(false);
            }
        }
        // same as above
        if (Input.GetKeyDown(KeyCode.X))
        {
            isMenuOpened = false;
            isPlayerMenuOpened = false;
            cursorScript.canMoveCursor = true;
            playerMenu.gameObject.SetActive(false);
        }
    }
    
    // If the player selected movement in the player menu then this will check to see if they will actually move.
    public void CheckMovement()
    {
        // X will exit the movement mode and resetting variables back to the main player menu
        if (Input.GetKeyDown(KeyCode.X))
        {
            findingMovement = false;
            playerMenu.gameObject.SetActive(true);
            cursorScript.canMoveCursor = false;
            selectedUnitScript.moveableTiles.Clear();
            selectedUnitScript.DeleteMovementIndicator();
            cursorScript.transform.position = new Vector3(selectedUnitScript.transform.position.x, 0, selectedUnitScript.transform.position.z);
        }

        // Z will move then to the location where the cursor is as long as they are hovering over an approved area.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (selectedUnitScript.moveableTiles.Contains(currentTileObject))
            {
                cursorScript.canMoveCursor = false;

                // This will move the selected unit to the new location
                selectedUnitScript.GetMovementPath(selectedUnitScript.originalPosition, currentTileObject.GetComponent<TileScript>().tilePosition);
                movingUnit = true;

                selectedUnitScript.DeleteMovementIndicator();
            }
        }
    }

    // This opens the player menu
    public void OpenPlayerMenu()
    {
        isMenuOpened = true;
        cursorScript.canMoveCursor = false;
        playerMenu.gameObject.SetActive(true);
        playerMenuOptions = 0;
        optionMenuIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 150);

        selectedUnitObject = currentUnitObject;
        selectedUnitScript = currentUnitScript;
        cursorScript.changePosition = new Vector3(selectedUnitScript.transform.position.x,
                                                    0,
                                                    selectedUnitScript.transform.position.z);

        if (selectedUnitScript.moved == true)
        {
            playerMenuScript.movementButton.interactable = false;
        }
    }

}