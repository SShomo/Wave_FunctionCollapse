using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class WaveManager : MonoBehaviour
{
    public int width;
    public int height;
    private int dimensions;
    public Cell _cell;
    private List<Cell> gridComponents;
    private Node[] nodeOpt;
    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        nodeOpt = _cell.nodeOptions;
        dimensions = height * width;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell newCell = Instantiate(_cell, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
                newCell.CreateCell(false, nodeOpt);
                gridComponents.Add(newCell);
                gridComponents[gridComponents.Count - 1].positon = new Vector2(x, y);
                gridComponents[gridComponents.Count - 1].index = gridComponents.Count - 1;
            }
        }
        StartCoroutine(GetEntropy());
    }

    //Check which tiles have the fewest options and collapse it
    IEnumerator GetEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        List<Cell> lowestCount = new List<Cell>();
        tempGrid.RemoveAll(c => c.collapsed);
        if(tempGrid.Count > 0)
        {
            int lowest = tempGrid[0].nodeOptions.Length;

            //Creates a list of the cells with the lowest entropy
            for(int i = 0; i < tempGrid.Count; i++)
            {
                if (tempGrid[i].nodeOptions.Length < lowest)
                {
                    lowestCount.Clear();
                    lowestCount.Add(tempGrid[i]);
                    lowest = tempGrid[i].nodeOptions.Length;
                }
                else if(tempGrid[i].nodeOptions.Length == lowest)
                    lowestCount.Add(tempGrid[i]);
            }

            yield return new WaitForSeconds(0.01f);
            CollapseCell(lowestCount);
        }
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        Cell cellToCollapse = tempGrid[randIndex];

        //collapses cell
        cellToCollapse.collapsed = true;
        //chooses a random node of the collapsed cell out of the possible nodes
        Node selectedNode = cellToCollapse.nodeOptions[UnityEngine.Random.Range(0, cellToCollapse.nodeOptions.Length)];
        cellToCollapse.node = selectedNode;

        Quaternion rot = GetRotation(cellToCollapse, selectedNode);

        Instantiate(selectedNode, cellToCollapse.transform.position, rot);
        ChangeCells(cellToCollapse);
    }

    Quaternion GetRotation(Cell collapsed, Node select)
    {
        Quaternion rotate = Quaternion.identity;
        if (collapsed.positon.y > 0) // up
        {
            int ind = ((int)collapsed.positon.y - 1) * width + (int)collapsed.positon.x;
            if (gridComponents[ind].collapsed)
            {
                if (gridComponents[ind].node.up_socket == select.down_socket)
                    rotate.z = 0;
                else if (gridComponents[ind].node.up_socket == select.left_socket)
                {
                    rotate.z = 90;
                    float t = select.up_socket;
                    select.up_socket = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = t;
                }
                else if (gridComponents[ind].node.up_socket == select.up_socket)
                {
                    rotate.z = 180;
                    float t = select.up_socket;
                    float te = select.right_socket;
                    select.right_socket = select.left_socket;
                    select.left_socket = te;
                    select.up_socket = select.down_socket;
                    select.down_socket = t;
                }
                else if (gridComponents[ind].node.up_socket == select.right_socket)
                {
                    rotate.z = 270;
                    float t = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = select.up_socket;
                    select.up_socket = t;
                }
            }
        }
        else if (collapsed.positon.x < width - 1)//right
        {
            int ind = (int)collapsed.positon.y * width + ((int)collapsed.positon.x + 1);
            if (gridComponents[ind].collapsed)
            {
                if (gridComponents[ind].node.left_socket == select.right_socket)
                    rotate.z = 0;
                else if (gridComponents[ind].node.left_socket == select.down_socket)
                {
                    rotate.z = 90;
                    float t = select.up_socket;
                    select.up_socket = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = t;
                }
                else if (gridComponents[ind].node.left_socket == select.left_socket)
                {
                    rotate.z = 180;
                    float t = select.up_socket;
                    float te = select.left_socket;
                    select.up_socket = select.down_socket;
                    select.left_socket = select.right_socket;
                    select.down_socket = t;
                    select.right_socket = te;
                }
                else if (gridComponents[ind].node.left_socket == select.up_socket)
                {
                    rotate.z = 270;
                    float t = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = select.up_socket;
                    select.up_socket = t;
                }
            }
        }
        else if (collapsed.positon.y < width - 1)
        {
            int ind = ((int)collapsed.positon.y + 1) * width + (int)collapsed.positon.x;
            if (gridComponents[ind].collapsed)
            {
                if (gridComponents[ind].node.left_socket == select.right_socket)
                    rotate.z = 0;
                else if (gridComponents[ind].node.left_socket == select.down_socket)
                {
                    rotate.z = 90;
                    float t = select.up_socket;
                    select.up_socket = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = t;
                }
                else if (gridComponents[ind].node.left_socket == select.left_socket)
                {
                    rotate.z = 90;
                    float t = select.up_socket;
                    select.up_socket = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = t;
                }
                else if (gridComponents[ind].node.left_socket == select.up_socket)
                {
                    rotate.z = 270;
                    float t = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = select.up_socket;
                    select.up_socket = t;
                }
            }
        }
        else if (collapsed.positon.x > 0)
        {
            int ind = (int)collapsed.positon.y * width + ((int)collapsed.positon.x - 1);
            if (gridComponents[ind].collapsed)
            {
                if (gridComponents[ind].node.right_socket == select.left_socket)
                    rotate.z = 0;
                else if (gridComponents[ind].node.right_socket == select.up_socket)
                {
                    rotate.z = 90;
                    float t = select.up_socket;
                    select.up_socket = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = t;
                }
                else if (gridComponents[ind].node.right_socket == select.right_socket)
                {
                    rotate.z = 180;
                    float t = select.up_socket;
                    float te = select.left_socket;
                    select.up_socket = select.down_socket;
                    select.left_socket = select.right_socket;
                    select.down_socket = t;
                    select.right_socket = te;
                }
                else if (gridComponents[ind].node.right_socket == select.down_socket)
                {
                    rotate.z = 270;
                    float t = select.right_socket;
                    select.right_socket = select.down_socket;
                    select.down_socket = select.left_socket;
                    select.left_socket = select.up_socket;
                    select.up_socket = t;
                }
            }
        }
        return rotate;
    }

    void ChangeCells(Cell cell)
    {
        List<Node> options = new List<Node>();
        List<float> temp = new List<float>();
        temp.Add(cell.node.up_socket);
        temp.Add(cell.node.down_socket);
        temp.Add(cell.node.left_socket);
        temp.Add(cell.node.left_socket);

        if (cell.positon.y > 0) // up
        {
            int ind = ((int)cell.positon.y - 1) * width + (int)cell.positon.x ;
            foreach (Node n in gridComponents[ind].nodeOptions)
            {
                if (temp.Contains(n.up_socket) || temp.Contains(n.down_socket) || temp.Contains(n.left_socket) || temp.Contains(n.right_socket))
                {
                    options.Add(n);
                }
            }
            gridComponents[ind].nodeOptions = options.ToArray();
            options.Clear();
        }
        if (cell.positon.x < width - 1)
        {
            int ind = (int)cell.positon.y * width + ((int)cell.positon.x + 1);
            foreach (Node n in gridComponents[ind].nodeOptions)
            {
                if (temp.Contains(n.up_socket) || temp.Contains(n.down_socket) || temp.Contains(n.left_socket) || temp.Contains(n.right_socket))
                {
                    options.Add(n);
                }
            }
            gridComponents[ind].nodeOptions = options.ToArray();
            options.Clear();
        }
        if (cell.positon.y < width - 1)
        {
            int ind = ((int)cell.positon.y + 1) * width + (int)cell.positon.x;
            foreach (Node n in gridComponents[ind].nodeOptions)
            {
                if (temp.Contains(n.up_socket) || temp.Contains(n.down_socket) || temp.Contains(n.left_socket) || temp.Contains(n.right_socket))
                {
                    options.Add(n);
                }
            }
            gridComponents[ind].nodeOptions = options.ToArray();
            options.Clear();
        }
        if (cell.positon.x > 0)
        {
            int ind = (int)cell.positon.y * width + ((int)cell.positon.x - 1);
            foreach (Node n in gridComponents[ind].nodeOptions)
            {
                if (temp.Contains(n.up_socket) || temp.Contains(n.down_socket) || temp.Contains(n.left_socket) || temp.Contains(n.right_socket))
                {
                    options.Add(n);
                }
            }
            gridComponents[ind].nodeOptions = options.ToArray();
            options.Clear();
        }

        iterations++;
        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(GetEntropy());
        }
    }
}
