using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 positon;
    public int index;
    public int rotation = 1; //1 = original, 2 = turn right, 3 is upside down, 4 is turn right
    
    public bool collapsed;
    public Node[] nodeOptions;
    public Node node;

    public void CreateCell(bool collapsedState, Node[] nodes)
    {
        collapsed = collapsedState;
        nodeOptions = nodes;
    }

    public void RecreateCell(Node[] nodes)
    {
        nodeOptions = nodes;
    }

    public void GetNodeOptions()
    {

    }

    public void IsCollapsed() 
    {

    }
}
