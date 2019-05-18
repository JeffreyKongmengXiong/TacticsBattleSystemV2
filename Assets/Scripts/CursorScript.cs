using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public GameObject cameraObject;
    private CameraScript cameraScript;

    public GameObject gc;
    private GameControllerScript gcScript;

    public GameObject highlightObject;
    public GameObject selectorObj;

    public Vector3 changePosition;

    [Range(1.0f, 20f)]
    public float panSpeed = 3f;
    

    public bool isRotating;
    public bool canMoveCursor = true;


    void Start()
    {
        // This will create the red highlight beneath the cursor to show which tile the cursor is active on, but we will turn it off for now until it is needed.
        highlightObject = Instantiate(highlightObject, new Vector3(0, 0, 0), Quaternion.identity);
        highlightObject.SetActive(false);

        // This will make retrieving the camera script and Game Controller scripts much easier and make the code less cluttered.
        cameraScript = cameraObject.GetComponent<CameraScript>();
        gcScript = gc.GetComponent<GameControllerScript>();

        isRotating = false;
    }

    void Update()
    {
        // At the beginning of the tick, the new position will be the current position and by the end of this tick, it will have either found a
        // new position to be possess or stayed put in it's current position.
        changePosition = transform.position;

        // This will increase the panning speed of the camera if the X button is held down
        if (Input.GetKeyDown(KeyCode.X))
        {
            panSpeed = panSpeed* 2;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            panSpeed = panSpeed/2;
        }

        // This will check to see if it is allowed to move
        if (canMoveCursor == true)
        {
            changePosition = PanMovement(changePosition);
        }

        // if we are moving the unit then we will move the cursor with the unit moving
        if (gcScript.movingUnit == true)
        {
            changePosition = Vector3.Lerp(this.transform.position, gcScript.selectedUnitObject.transform.position, 0.25f);
        }

        // This will call a function that will rotate the camera
        RotateCamera();
        
        transform.position = changePosition;
    }

    // This will check collision with every collider with the trigger boolean ticked to true and a rigidbody.
    void OnTriggerStay(Collider col)
    {
        // First we will start with defining the colliding object
        GameObject tempObject = col.gameObject;

        // If this colliding object belongs to the list that holds all of the tiles on the map grid then we continue
        if (gcScript.tileList.Contains(tempObject))
        {
            // currentTileObject will keep track of which tile on the map grid the cursor is highlighting
            gcScript.currentTileObject = col.gameObject;
            
            // most of what is happening next is for cosmetics and troubleshooting, what's happening is making sure that the cursor is above the tile floating
            // and making sure that the highlightObject is on the tile so the user knows which tile the cursor will activate when prompt too.
            float temp = 1f + gcScript.currentTileObject.transform.position.y;
            Vector3 tempVect = new Vector3(this.transform.position.x, temp, this.transform.position.z);
            this.transform.position = tempVect;
            highlightObject.SetActive(true);
            highlightObject.transform.position = new Vector3(tempObject.transform.position.x, tempObject.transform.position.y + 0.502f, tempObject.transform.position.z);
        }

        // If this colliding object belongs to the list that holds all units then we continue
        if (gcScript.unitList.Contains(tempObject))
        {
            // currentUnitObject will keep track of the unit the cursor is highlighting
            gcScript.currentUnitObject = col.gameObject;
        }
    }

    // This will check when the cursor leaves the colliding trigger object
    void OnTriggerExit(Collider col)
    {
        // All we have to do change the current tile and unit to null
        gcScript.currentTileObject = null;
        gcScript.currentUnitObject = null;
    }

    public Vector3 PanMovement(Vector3 newPosition)
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (gcScript.direction == 0)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 1)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 2)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 3)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (gcScript.direction == 0)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 1)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 2)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 3)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (gcScript.direction == 0)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 1)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 2)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 3)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (gcScript.direction == 0)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 1)
            {
                newPosition.z += panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 2)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x += panSpeed * Time.deltaTime;
            }
            else if (gcScript.direction == 3)
            {
                newPosition.z -= panSpeed * Time.deltaTime;
                newPosition.x -= panSpeed * Time.deltaTime;
            }
        }

        return newPosition;
    }

    public void RotateCamera()
    {
        if (Input.GetKey("a"))
        {
            if (isRotating == false && transform.rotation == cameraObject.transform.rotation)
            {
                isRotating = true;
                transform.Rotate(0, 90, 0);
                if (gcScript.direction < 3)
                {
                    gcScript.direction++;
                }
                else
                {
                    gcScript.direction = 0;
                }
            }
        }


        if (Input.GetKey("s"))
        {
            if (isRotating == false && transform.rotation == cameraObject.transform.rotation)
            {
                isRotating = true;
                transform.Rotate(0, -90, 0);
                if (gcScript.direction > 0)
                {
                    gcScript.direction--;
                }
                else
                {
                    gcScript.direction = 3;
                }
            }
        }
    }

   
}
