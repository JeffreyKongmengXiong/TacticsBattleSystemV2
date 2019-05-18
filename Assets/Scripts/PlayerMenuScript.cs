using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuScript : MonoBehaviour
{
    public GameObject gc;
    private GameControllerScript gcScript;

    public Button movementButton;


    // Start is called before the first frame update
    void Start()
    {
        gcScript = gc.GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {   

    }

    public void ToggleMovementButton()
    {
        movementButton.interactable = !movementButton.interactable;
    }
}
