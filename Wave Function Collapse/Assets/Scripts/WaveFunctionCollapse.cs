using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int dimensions;
    public Node[] nodeOpt;
    public List<Cell> gridComponents;
    public Cell cellObj;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
                newCell.CreateCell(false, nodeOpt);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.nodeOptions.Length - b.nodeOptions.Length; });

        int arrLength = tempGrid[0].nodeOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].nodeOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        Node selectedNode = cellToCollapse.nodeOptions[UnityEngine.Random.Range(0, cellToCollapse.nodeOptions.Length)];
        cellToCollapse.nodeOptions = new Node[] { selectedNode };

        Node foundNode = cellToCollapse.nodeOptions[0];
        Instantiate(foundNode, cellToCollapse.transform.position, Quaternion.identity);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Node> options = new List<Node>();
                    foreach (Node t in nodeOpt)
                    {
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in up.nodeOptions)
                        {
                            var valOption = Array.FindIndex(nodeOpt, obj => obj == possibleOptions);
                            var valid = nodeOpt[valOption].up;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in right.nodeOptions)
                        {
                            var valOption = Array.FindIndex(nodeOpt, obj => obj == possibleOptions);
                            var valid = nodeOpt[valOption].left;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in down.nodeOptions)
                        {
                            var valOption = Array.FindIndex(nodeOpt, obj => obj == possibleOptions);
                            var valid = nodeOpt[valOption].down;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in left.nodeOptions)
                        {
                            var valOption = Array.FindIndex(nodeOpt, obj => obj == possibleOptions);
                            var valid = nodeOpt[valOption].right;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    Node[] newNodeList = new Node[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newNodeList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newNodeList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    void CheckValidity(List<Node> optionList, List<Node> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
