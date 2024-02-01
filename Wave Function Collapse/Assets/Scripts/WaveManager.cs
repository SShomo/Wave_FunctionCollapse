using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int width;
    public int height;
    public Cell cell;
    private List<Cell> gridComponents;
    private Node[] nodeOpt;
    void Awake()
    {
        gridComponents = new List<Cell>();
        nodeOpt = cell.nodeOptions;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell newCell = Instantiate(cell, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
                newCell.CreateCell(false, nodeOpt);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(GetEntropy());
    }

    IEnumerator GetEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        
        

        yield return new WaitForSeconds(0.01f);
    }
}
