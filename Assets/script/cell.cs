using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell : MonoBehaviour
{
    public List<cell> neightbor = new List<cell>();
    public float g = 0;
    public float f = 0;
    public cell ComeFrom = null;
    public bool visitedforDFS;
    public bool visitedforAstar;
    public List<cell> cellaround = new List<cell>();

    public List<cell> getUnvisited()
    {
        List<cell> unvisited = new List<cell>();
        foreach (var c in cellaround)
        {
            if (!c.visitedforDFS)
            {
                unvisited.Add(c);
            }
        }

        return unvisited;
    }

    
    public float getH(cell target)
    {
        float h=0;
        h = Mathf.Abs(this.transform.localPosition.x - target.transform.localPosition.x)
            +Mathf.Abs(this.transform.localPosition.y - target.transform.localPosition.y);
        return h;
    }


}
