using UnityEngine;
using System.Collections.Generic;

public class Calculation_AStar
{
    private readonly int row, column;
    private Node_AStar currentNode;
    private Node_AStar[,] node;
    private List<Node_AStar> openList;
    private List<Node_AStar> closedList;

    public Calculation_AStar(TileMap_MapData mapData)
    {
        row = mapData.row;
        column = mapData.column;

        currentNode = null;

        node = new Node_AStar[row, column];

        for (int z = 0; z < column; z++)
        {
            for (int x = 0; x < row; x++)
            {
                node[x, z] = new Node_AStar(mapData.TileData[x, z]);
            }
        }

        openList = new List<Node_AStar>();
        closedList = new List<Node_AStar>();

        FinalTrack = new List<Node_AStar>();
    }

    public List<Node_AStar> FinalTrack { get; private set; }

    public bool FindPath(TileMap_MapData.TileMap_TileData[,] tileData, TileMap_MapData.TileMap_TileData startingTile, TileMap_MapData.TileMap_TileData destinationTile)
    {
        Refresh(tileData, destinationTile);

        currentNode = node[startingTile.Index.x, startingTile.Index.z];

        closedList.Add(currentNode);

        int failureCount = row * column;

        while(true)
        {
            for (int z = currentNode.Index.z - 1; z < currentNode.Index.z + 2; z++)
            {
                for (int x = currentNode.Index.x - 1; x < currentNode.Index.x + 2; x++)
                {
                    if (NodeIndexAvailable(x, z) == false) continue;

                    if (node[x, z].Passable == false) continue;

                    if (NodeInClosedList(node[x, z]) == true) continue;

                    if (NodeInOpenList(node[x, z]) == false)
                    {
                        node[x, z].Parent = currentNode;
                        node[x, z].CalculateCostToDestination();
                        openList.Add(node[x, z]);
                    }
                    else
                    {
                        Node_AStar newData = new Node_AStar(node[x, z]);
                        newData.Parent = currentNode;
                        newData.CalculateCostToDestination();

                        if (newData.CostToDestination < node[x, z].CostToDestination)
                        {
                            node[x, z].Parent = currentNode;
                            node[x, z].CalculateCostToDestination();
                        }
                    }
                }
            }

            int lowestCost = 99999999;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].CostToDestination < lowestCost)
                {
                    lowestCost = openList[i].CostToDestination;
                    currentNode = openList[i];
                }
            }

            if (currentNode == node[destinationTile.Index.x, destinationTile.Index.z])
            {
                int whileBreaker = row * column;

                while (true)
                {
                    FinalTrack.Add(currentNode);

                    if (currentNode == node[startingTile.Index.x, startingTile.Index.z]) return true;

                    currentNode = currentNode.Parent;

                    whileBreaker--;
                    if (whileBreaker < 0) return false;
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            failureCount--;
            if (failureCount < 0) return false;
        }
    }

    private bool NodeInOpenList(Node_AStar checkedNode)
    {
        for (int i = 0; i < openList.Count; i++)
        {
            if (openList[i] == checkedNode) return true;
        }

        return false;
    }

    private void Refresh(TileMap_MapData.TileMap_TileData[,] tileData, TileMap_MapData.TileMap_TileData destinationTile)
    {
        openList.Clear();
        closedList.Clear();
        FinalTrack.Clear();
        currentNode = null;

        for (int z = 0; z < column; z++)
        {
            for (int x = 0; x < row; x++)
            {
                node[x, z].Initialize(tileData[x, z], destinationTile);
            }
        }
    }

    private bool NodeInClosedList(Node_AStar checkedNode)
    {
        for (int i = 0; i < closedList.Count; i++)
        {
            if (closedList[i] == checkedNode) return true;
        }

        return false;
    }

    private bool NodeIndexAvailable(int x, int z)
    {
        if ((x < 0) || (x >= row) || (z < 0) || (z >= column)) return false;

        return true;
    }
}
