using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public GameObject gc;
    private GameControllerScript gcScript;
    public GameObject body;
    public GameObject sprite;

    public GameObject collisionObject;
    public int[] currentPosition = new int[2] { -1, -1 };
    public int[] originalPosition = new int[2] { -1, -1 };
    public Vector3 originalPositionVector;

    public bool moved;
    public int movement;

    public int direction = 0;
    
    public List<GameObject> moveableTiles = new List<GameObject>();
    public GameObject movementIndicator;
    public List<GameObject> movementIndicatorList = new List<GameObject>();

    public List<GameObject> path = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        gcScript = gc.GetComponent<GameControllerScript>();
        body = this.transform.GetChild(0).transform.gameObject;
        sprite = this.transform.GetChild(1).transform.gameObject;
        originalPosition = new int[] { (int)this.transform.position.x, (int)this.transform.position.z };
        originalPositionVector = new Vector3(this.originalPosition[0], 0, this.originalPosition[1]);
    }

    // Update is called once per frame
    void Update()
    {
        collisionObject = body.GetComponent<UnitCollisionScript>().collisionObject;
        SpriteRotate();   
    }

    // This fuction will take the unit's movement distance variable and use that to gather the tiles that the unit can travel too.
    public void GetMoveableTiles()
    {
        // first, we get the unit's movement distance or how far they can travel in one turn
        int movementLimit = movement;

        // Initializing list we are going to use later
        List<GameObject> newAdjacentTiles = new List<GameObject>();
        List<GameObject> prevAdjacentTiles = new List<GameObject>();

        // This will make a list of the temporary adjacent tiles
        List<GameObject> tempTiles = GetAdjacentTiles(originalPosition);

        // This will check too see if the adjacent tiles is being occupied by a unit already, if it is then we remove it from a potential place to move
        for (int j = 0; j < gcScript.unitList.Count; j++)
        {
            if (tempTiles.Contains(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject))
            {
                tempTiles.Remove(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject);
            }
        }

        // with those adjacent tiles registered, we will then go through the list and add those to two list,
        // the moveableTiles list will keep track of the tiles that the unit can move
        // the prevAdjacentTiles list will hold the newly approve adjacent tiles
        for (int i = 0; i < tempTiles.Count; i++)
        {
            moveableTiles.Add(tempTiles[i]);
            prevAdjacentTiles.Add(tempTiles[i]);
        }

        // This while loop will loop until all of the movement options within the movement distances is found
        while (movementLimit > 0)
        {
            // First, we will have the to go through the previous adjacent tiles one by one and get their adjacent tiles as well
            for (int i = 0; i < prevAdjacentTiles.Count; i++)
            {
                // This temporary tile will hold the adjacent tiles before they are approved
                tempTiles = new List<GameObject>(GetAdjacentTiles(new int[] { (int)prevAdjacentTiles[i].transform.position.x, (int)prevAdjacentTiles[i].transform.position.z }));
                // This will check too see if the adjacent tiles is being occupied by a unit already, if it is then we remove it from a potential place to move
                for (int j = 0; j < gcScript.unitList.Count; j++)
                {
                    if (tempTiles.Contains(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject))
                    {
                        tempTiles.Remove(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject);
                    }
                }
                for (int j = 0; j < tempTiles.Count; j++)
                {
                    // This will see if they are already in the moveableTiles list, if they aren't then they are approved to be added
                    if (!moveableTiles.Contains(tempTiles[j]))
                    {
                        moveableTiles.Add(tempTiles[j]);
                        newAdjacentTiles.Add(tempTiles[j]);
                    }
                }
            }
            // Then we minus 1 from the movement counter
            movementLimit -= 1;

            // Now that we set the previous adjacent tile list for the next loop and clear the other list
            prevAdjacentTiles = new List<GameObject>(newAdjacentTiles);
            newAdjacentTiles.Clear();
            tempTiles.Clear();
        }
        // This will create the movement indicators
        CreateMovmenetIndicator();
    }

    public void CreateMovmenetIndicator()
    {
        // This will create movement tile indicators that will show which tile is a movement platform. mostly for visual.
        for (int i = 0; i < moveableTiles.Count; i++)
        {
            movementIndicatorList.Add(Instantiate(movementIndicator, moveableTiles[i].transform.position + new Vector3(0, 0.501f, 0), Quaternion.identity));
        }
    }

    public void DeleteMovementIndicator()
    {
        moveableTiles.Clear();
        for (int i = 0; i < movementIndicatorList.Count; i++)
        {
            Destroy(movementIndicatorList[i]);
        }
        movementIndicatorList.Clear();
    }

    public void GetMovementPath(int[] startPosition, int[] endPosition)
    {

        for (int i = 0; i < gcScript.tileList.Count; i++)
        {
            gcScript.tileList[i].GetComponent<TileScript>().ResetCosts();
        }

        bool found = false;
        
        List<GameObject> adjacentTiles = new List<GameObject>();
        List<GameObject> searchedTiles = new List<GameObject>();
        GameObject lowestFCost = gcScript.boardGrid[startPosition[0], startPosition[1]];

        while (!found)
        {
            // From the lowest FCost, we find the adjacent tiles
            List<GameObject> temptiles = new List<GameObject>(GetAdjacentTiles(new int[] { (int)lowestFCost.transform.position.x, (int)lowestFCost.transform.position.z }));
            
            // Then we look to see if there is an object already occupying the tiles, if there is then we delete it
            for (int j = 0; j < gcScript.unitList.Count; j++)
            {
                if (temptiles.Contains(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject))
                {
                    temptiles.Remove(gcScript.unitList[j].GetComponent<UnitScript>().collisionObject);
                }
            }

            // If the temptiles are already in the adjacent tiles list then we will remove them
            for (int j = 0; j < adjacentTiles.Count; j++)
            {
                if (temptiles.Contains(adjacentTiles[j]))
                {
                    temptiles.Remove(adjacentTiles[j]);
                }
            }

            // Now we do the same, but for the searched list
            for (int j = 0; j < searchedTiles.Count; j++)
            {
                if (temptiles.Contains(searchedTiles[j]))
                {
                    temptiles.Remove(searchedTiles[j]);
                }
            }

            // Next we check to see if the adjacent tiles are in the spectrum of our movement area we defined in an earlier function
            for (int j = 0; j < temptiles.Count; j++)
            {
                // If they are, then we set their previous tiles to the lowest FCost so we can follow it back to the original location we also set the Costs
                if (moveableTiles.Contains(temptiles[j]))
                {
                    temptiles[j].GetComponent<TileScript>().prevObject = lowestFCost;
                    temptiles[j].GetComponent<TileScript>().setCosts(startPosition, endPosition);
                    adjacentTiles.Add(temptiles[j]);
                }
            }

            // Now that we added the approved tiles to the master list of adjacent tiles we will now find a new Lowest FCost
            lowestFCost = adjacentTiles[0];
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                if (adjacentTiles[i].GetComponent<TileScript>().fCost < lowestFCost.GetComponent<TileScript>().fCost)
                {
                    lowestFCost = adjacentTiles[i];
                }
            }
            
            // With the lowest FCost found, we can add it to the list that keeps track of the quickest path and remove it from the previous adjacent list
            adjacentTiles.Remove(lowestFCost);
            searchedTiles.Add(lowestFCost);

            // Here we will check to see if we found the end, if we did the we say that we found it and end the while loop
            if (lowestFCost == gcScript.boardGrid[endPosition[0], endPosition[1]])
            {
                found = true;
            }

            // did we found the end? if no then we repeat!
        }

        // After finding the most optimal path, we will put that path into a list.
        // List<GameObject> path = new List<GameObject>();
        while (lowestFCost.GetComponent<TileScript>().prevObject != null)
        {
            path.Add(lowestFCost);
            lowestFCost = lowestFCost.GetComponent<TileScript>().prevObject;
        }
    }

    public void MovePath()
    {
        if (path.Count > 0)
        {
            GameObject nextPositionObject = path[path.Count - 1];
            Vector3 nextPosition = new Vector3(nextPositionObject.transform.position.x, this.transform.position.y, nextPositionObject.transform.position.z);
            if ((this.transform.position.x >= nextPosition.x + 0.01f || this.transform.position.x <= nextPosition.x - 0.01f) || 
                (this.transform.position.z >= nextPosition.z + 0.01f || this.transform.position.z <= nextPosition.z - 0.01f))
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, 0.1f);
            }
            else if (!(this.transform.position.x >= nextPosition.x + 0.01f || this.transform.position.x <= nextPosition.x - 0.01f) && 
                !(this.transform.position.z >= nextPosition.z + 0.01f || this.transform.position.z <= nextPosition.z - 0.01f))
            {
                this.transform.position = nextPosition;
                path.Remove(path[path.Count - 1]);
            }
        }
        else
        {
            moved = true;
            gcScript.cursorScript.canMoveCursor = true;
            gcScript.cursorScript.transform.position = gcScript.selectedUnitObject.transform.position;
            gcScript.movingUnit = false;
            gcScript.findingMovement = false;
            gcScript.isMenuOpened = false;
            gcScript.isPlayerMenuOpened = false;
        }
    }

    public void SpriteRotate()
    {
        if (gcScript.direction == 0)
        {
            if (direction == 0)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 1)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 2)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 3)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
        }
        else if (gcScript.direction == 1)
        {
            if (direction == 0)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 1)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 2)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 3)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
        }
        else if (gcScript.direction == 2)
        {
            if (direction == 0)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 1)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 2)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 3)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
        }
        else if (gcScript.direction == 3)
        {
            if (direction == 0)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 1)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 1);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 2)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x > 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
            else if (direction == 3)
            {
                sprite.transform.GetChild(0).GetComponent<Animator>().SetInteger("direction", 0);

                if (sprite.transform.GetChild(0).transform.localScale.x < 0)
                {
                    sprite.transform.GetChild(0).transform.localScale = new Vector3(-sprite.transform.GetChild(0).transform.localScale.x,
                                                                                     sprite.transform.GetChild(0).transform.localScale.y,
                                                                                     sprite.transform.GetChild(0).transform.localScale.z);
                }
            }
        }
    }

    // This will get the adjacent tiles for a certain position
    public List<GameObject> GetAdjacentTiles(int[] blockPosition)
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        try
        {
            adjacentTiles.Add(gcScript.boardGrid[blockPosition[0] + 1, blockPosition[1]]);
        }
        catch { }
        try
        {
            adjacentTiles.Add(gcScript.boardGrid[blockPosition[0] - 1, blockPosition[1]]);
        }
        catch { }
        try
        {
            adjacentTiles.Add(gcScript.boardGrid[blockPosition[0], blockPosition[1] + 1]);
        }
        catch { }
        try
        {
            adjacentTiles.Add(gcScript.boardGrid[blockPosition[0], blockPosition[1] - 1]);
        }
        catch { }
        return adjacentTiles;
    }
}
