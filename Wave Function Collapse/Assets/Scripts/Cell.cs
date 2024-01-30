using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Node[] nodeOptions;

    public void CreateCell(bool collapsedState, Node[] nodes)
    {
        collapsed = collapsedState;
        nodeOptions = nodes;
    }    

    public void RecreateCell(Node[] nodes)
    {
        nodeOptions = nodes;
    }
}
