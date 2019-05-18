using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int[] tilePosition = new int[2];
    public int gCost = 0;
    public int hCost = 0;
    public int fCost = 0;
    public GameObject prevObject = null;

    // Start is called before the first frame update
    void Start()
    {
        tilePosition[0] = (int)this.transform.position.x;
        tilePosition[1] = (int)this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (tilePosition[0] != (int)this.transform.position.x)
        {
            tilePosition[0] = (int)this.transform.position.x;
        }
        if (tilePosition[1] != (int)this.transform.position.z)
        {
            tilePosition[1] = (int)this.transform.position.z;
        }
    }

    public void setCosts(int[] startPosition, int[] endPosition)
    {
        int xDiff = Mathf.Abs(tilePosition[0] - startPosition[0]);
        int yDiff = Mathf.Abs(tilePosition[1] - startPosition[1]);
        gCost = xDiff + yDiff;

        xDiff = Mathf.Abs(tilePosition[0] - endPosition[0]);
        yDiff = Mathf.Abs(tilePosition[1] - endPosition[1]);
        hCost = xDiff + yDiff;

        fCost = gCost + hCost;
    }

    public void ResetCosts()
    {
        gCost = 0;
        hCost = 0;
        fCost = 0;
        prevObject = null;
    }
}
