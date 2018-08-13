using UnityEngine;

public class Node_AStar
{
    public Node_AStar(TileMap_MapData.TileMap_TileData correspondingTile)
    {
        Index = correspondingTile.Index;
    }

    public Node_AStar(Node_AStar copiedNode)
    {
        this.Index = copiedNode.Index;
        this.Parent = copiedNode.Parent;
        this.Passable = copiedNode.Passable;
        this.DistanceFromStart = copiedNode.DistanceFromStart;
        this.DistanceToDestination = copiedNode.DistanceToDestination;
        this.CostToDestination = copiedNode.CostToDestination;
    }

    public TileMap_MapData.TileIndex Index { get; private set; }
    public Node_AStar Parent { get; set; }
    public bool Passable { get; private set; }
    public int DistanceFromStart { get; private set; }
    public int DistanceToDestination { get; private set; }
    public int CostToDestination { get; private set; }

    public void Initialize(TileMap_MapData.TileMap_TileData correspondingTile, TileMap_MapData.TileMap_TileData destinationTile)
    {
        Parent = null;
        Passable = correspondingTile.Passable;

        CalculateDistanceToDestination(destinationTile);
    }

    public void CalculateCostToDestination()
    {
        CalculateDistanceFromStart();

        CostToDestination = DistanceFromStart + DistanceToDestination;
    }

    private void CalculateDistanceFromStart()
    {
        if ((this.Parent.Index.x - this.Index.x != 0) && (this.Parent.Index.z - this.Index.z != 0))
        {
            this.DistanceFromStart = this.Parent.DistanceFromStart + 14;
        }
        else
        {
            this.DistanceFromStart = this.Parent.DistanceFromStart + 10;
        }
    }

    private void CalculateDistanceToDestination(TileMap_MapData.TileMap_TileData destinationTile)
    {
        int destinationX = destinationTile.Index.x;
        int destinationZ = destinationTile.Index.z;

        int xDistance = Mathf.Abs(destinationX - Index.x);
        int zDistance = Mathf.Abs(destinationZ - Index.z);

        if (xDistance - zDistance == 0)
        {
            DistanceToDestination = 14 * zDistance;
        }
        else
        {
            int linearDistance = Mathf.Abs(xDistance - zDistance);
            int furtherAxis = (xDistance - zDistance > 0) ? xDistance : zDistance;
            int diagonalDistance = furtherAxis - linearDistance;

            DistanceToDestination = linearDistance * 10 + diagonalDistance * 14;
        }
    }
}
