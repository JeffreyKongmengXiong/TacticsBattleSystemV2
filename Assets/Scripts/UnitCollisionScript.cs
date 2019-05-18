using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollisionScript : MonoBehaviour
{
    public GameObject gc;
    private GameControllerScript gcScript;

    public GameObject collisionObject;

    public GameObject parent;
    private UnitScript parentScript;

    // Start is called before the first frame update
    void Start()
    {
        gcScript = gc.GetComponent<GameControllerScript>();
        parent = this.transform.parent.gameObject;
        parentScript = parent.GetComponent<UnitScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider col)
    {
        if (!gcScript.movingUnit)
        {
            GameObject tempObject = col.gameObject;
            if (gcScript.tileList.Contains(tempObject))
            {
                collisionObject = col.gameObject;
                parent.transform.position = new Vector3(tempObject.transform.position.x, tempObject.transform.position.y + 1.5f, tempObject.transform.position.z);
                parentScript.currentPosition = new int[2] { (int)tempObject.transform.position.x, (int)tempObject.transform.position.z };
            }
        }
        else
        {
            GameObject tempObject = col.gameObject;
            if (gcScript.tileList.Contains(tempObject))
            {
                collisionObject = col.gameObject;
                parent.transform.position = new Vector3(parent.transform.position.x, tempObject.transform.position.y + 1.5f, parent.transform.position.z);
            }
        }
    }
}
