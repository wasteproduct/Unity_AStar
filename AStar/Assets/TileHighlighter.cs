using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour {
    public struct TileIndex
    {
        public int x, z;
    }

    public GameObject tileMap;

    private float tileSize;
    private Vector3 mapPosition;
    private Calculation_AStar aStar;
    private TileIndex currentTileIndex;

    public int TilesRow { get; private set; }
    public int TilesColumn { get; private set; }
    public TileMap_MapData MapData { get; private set; }

	// Use this for initialization
	void Start () {
        TilesRow = tileMap.GetComponent<TileMap>().tilesRow;
        TilesColumn = tileMap.GetComponent<TileMap>().tilesColumn;
        tileSize = tileMap.GetComponent<TileMap>().tileSize;
        mapPosition = tileMap.GetComponent<Transform>().position;
        MapData = tileMap.GetComponent<TileMap>().mapData;

        currentTileIndex.x = MapData.GetRoom(0).CenterX;
        currentTileIndex.z = MapData.GetRoom(0).CenterZ;

        SetPosition(currentTileIndex);

        aStar = new Calculation_AStar(MapData);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentTileIndex.z++;

            if ((currentTileIndex.z >= TilesColumn) || (MapData.TileData[currentTileIndex.x, currentTileIndex.z].Passable == false))
            {
                currentTileIndex.z--;
                return;
            }

            SetPosition(currentTileIndex);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentTileIndex.z--;

            if ((currentTileIndex.z < 0) || (MapData.TileData[currentTileIndex.x, currentTileIndex.z].Passable == false))
            {
                currentTileIndex.z++;
                return;
            }

            SetPosition(currentTileIndex);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTileIndex.x--;

            if ((currentTileIndex.x < 0) || (MapData.TileData[currentTileIndex.x, currentTileIndex.z].Passable == false))
            {
                currentTileIndex.x++;
                return;
            }

            SetPosition(currentTileIndex);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentTileIndex.x++;

            if ((currentTileIndex.x >= TilesRow - 1) || (MapData.TileData[currentTileIndex.x, currentTileIndex.z].Passable == false))
            {
                currentTileIndex.x--;
                return;
            }

            SetPosition(currentTileIndex);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TileMap_MapData.TileMap_TileData destination = null;

            for (int z = 0; z < TilesColumn; z++)
            {
                for (int x = 0; x < TilesRow; x++)
                {
                    if (MapData.TileData[x, z].Type == TileMap_MapData.TileType.Destination)
                    {
                        destination = MapData.TileData[x, z];
                        break;
                    }
                }
            }

            bool pathFound = false;

            pathFound = aStar.FindPath(MapData.TileData, MapData.TileData[currentTileIndex.x, currentTileIndex.z], destination);
            
            if (pathFound == true)
            {
                for (int i = 0; i < aStar.FinalTrack.Count; i++)
                {
                    MapData.TileData[aStar.FinalTrack[i].Index.x, aStar.FinalTrack[i].Index.z].Update(TileMap_MapData.TileType.Track);
                }

                tileMap.GetComponent<TileMap>().SetupTexture();
            }
            else
            {
                Debug.Log("Let's give up.");
            }
        }
    }

    void SetPosition(TileIndex tileIndex)
    {
        GetComponent<Transform>().position = mapPosition + new Vector3((float)tileIndex.x * tileSize, 0.0f, (float)tileIndex.z * tileSize);
    }
}
