using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject targetObject;
    private CursorScript targetScript;
    private Transform target;

    public GameObject cameraObject;
    public Camera cameraCamera;

    public GameObject gc;
    public GameControllerScript gcScript;

    [Range(0.01f, 1.0f)]
    public float panSmoothFactor = 0.1f;

    [Range(0.01f, 1.0f)]
    public float rotateSmoothFactor = 0.1f;
    
    [Range(0.01f, 1.0f)]
    public float zoomSmoothFactor = 0.075f;

    public int zoomPhase = 1;

    private bool zooming = false;
    private float zoomDestination;
    public Transform zoomPosition1;
    public Transform zoomPosition2;
    private Transform positionDestination;

    void Start()
    {
        gcScript = gc.GetComponent<GameControllerScript>();
        cameraObject = this.transform.GetChild(0).transform.gameObject;
        cameraCamera = cameraObject.GetComponent<Camera>();
        targetScript = targetObject.GetComponent<CursorScript>();
        target = targetObject.transform;
    }

    private void Update()
    {
        // zoom stuff
        if (Input.GetKeyDown(KeyCode.C) && zooming == false)
        {
            if (zoomPhase == 1)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize - 2;
                zoomPhase--;
                positionDestination = zoomPosition1;
            }
            else if (zoomPhase == 2)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize - 3;
                zoomPhase--;
                positionDestination = zoomPosition1;
            }
            else if (zoomPhase == 3)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize - 3;
                zoomPhase--;
                positionDestination = zoomPosition2;
            }
            else if (zoomPhase == 0)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize + 8;
                zoomPhase = 3;
                positionDestination = zoomPosition2;
            }
        }
        else if (Input.GetKey(KeyCode.D) && zooming == false)
        {
            if (zoomPhase == 1)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize + 3;
                zoomPhase++;
                positionDestination = zoomPosition2;
            }
            else if (zoomPhase == 2)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize + 3;
                zoomPhase++;
                positionDestination = zoomPosition2;
            }
            else if (zoomPhase == 3)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize - 8;
                zoomPhase = 0;
                positionDestination = zoomPosition1;
            }
            else if (zoomPhase == 0)
            {
                zooming = true;
                zoomDestination = cameraCamera.orthographicSize + 2;
                zoomPhase++;
                positionDestination = zoomPosition1;
            }
        }
    }

    void LateUpdate()
    {
        // basically, im following the cursor's position with a lerp
        transform.position = Vector3.Lerp(transform.position, target.position, panSmoothFactor);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotateSmoothFactor);

        for (int i = 0; i < gcScript.unitList.Count; i++)
        {
            gcScript.unitList[i].GetComponent<UnitScript>().sprite.transform.rotation = Quaternion.Lerp(gcScript.unitList[i].GetComponent<UnitScript>().sprite.transform.rotation, target.rotation, rotateSmoothFactor);
        }

        if (targetScript.isRotating == true && transform.rotation == target.rotation)
        {
            targetScript.isRotating = false;
        }

        // more zooming stuff, this will zoom in and out with a lerp and will also adjust the camera angle for certain zooms
        if (zooming == true)
        {
            if (cameraCamera.orthographicSize >= zoomDestination + 0.01f || cameraCamera.orthographicSize <= zoomDestination - 0.01f)
            {
                cameraCamera.orthographicSize = Mathf.Lerp(cameraCamera.orthographicSize, zoomDestination, zoomSmoothFactor);
                cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, positionDestination.position, zoomSmoothFactor);
                cameraObject.transform.rotation = Quaternion.Lerp(cameraObject.transform.rotation, positionDestination.rotation, zoomSmoothFactor);
            }
            else
            {
                cameraCamera.orthographicSize = (int)(cameraCamera.orthographicSize + 0.5f);
                cameraObject.transform.position = positionDestination.position;
                cameraObject.transform.rotation = positionDestination.rotation;
                zooming = false;
            }
        }

    }
}