using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
    public int tilesRow;
    public int tilesColumn;
    public float tileSize;
    public Texture2D terrainTiles;
    public int tileResolution;

    [HideInInspector]
    public TileMap_MapData mapData;

    // Use this for initialization
    void Start() {
        CreateMap();
    }

    // Update is called once per frame
    void Update() {

    }

    public void CreateMap()
    {
        int verticesX = tilesRow + 1;
        int verticesZ = tilesColumn + 1;
        int verticesNumber = verticesX * verticesZ;

        Vector3[] vertices = new Vector3[verticesNumber];
        Vector3[] normals = new Vector3[verticesNumber];
        Vector2[] uv = new Vector2[verticesNumber];

        int x, z;
        int index = 0;
        for (z = 0; z < verticesZ; z++)
        {
            for (x = 0; x < verticesX; x++)
            {
                vertices[index] = new Vector3((float)x * tileSize, 0.0f, (float)z * tileSize);
                normals[index] = Vector3.up;
                uv[index] = new Vector2((float)x / (float)tilesRow, (float)z / (float)tilesColumn);

                index++;
            }
        }
        index = 0;

        int tilesNumber = tilesRow * tilesColumn;
        int[] triangles = new int[tilesNumber * 6];
        int triangleIndex = 0;

        for (z = 0; z < tilesColumn; z++)
        {
            for (x = 0; x < tilesRow; x++)
            {
                triangles[triangleIndex] = index;
                triangleIndex++;
                triangles[triangleIndex] = index + verticesX;
                triangleIndex++;
                triangles[triangleIndex] = index + verticesX + 1;
                triangleIndex++;
                triangles[triangleIndex] = index;
                triangleIndex++;
                triangles[triangleIndex] = index + verticesX + 1;
                triangleIndex++;
                triangles[triangleIndex] = index + 1;
                triangleIndex++;

                index++;
            }

            index++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        SetupMapData();

        SetupTexture();
    }

    void SetupMapData()
    {
        mapData = new TileMap_MapData(tilesRow, tilesColumn);
    }

    public void SetupTexture()
    {
        int textureWidth = tilesRow * tileResolution;
        int textureHeight = tilesColumn * tileResolution;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Color[][] tileTextures = DistributeTexture();

        for (int z = 0; z < tilesColumn; z++)
        {
            for (int x = 0; x < tilesRow; x++)
            {
                Color[] colors = tileTextures[0];

                switch (mapData.TileData[x, z].Type)
                {
                    case TileMap_MapData.TileType.Block:
                        colors = tileTextures[3];
                        break;
                    case TileMap_MapData.TileType.Plane:
                        colors = tileTextures[2];
                        break;
                    case TileMap_MapData.TileType.Track:
                        colors = tileTextures[1];
                        break;
                    case TileMap_MapData.TileType.Destination:
                        colors = tileTextures[0];
                        break;
                }

                texture.SetPixels(x * tileResolution, z * tileResolution, tileResolution, tileResolution, colors);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    Color[][] DistributeTexture()
    {
        int textureRow = terrainTiles.width / tileResolution;
        int textureColumn = terrainTiles.height / tileResolution;

        Color[][] pixelSet = new Color[textureRow * textureColumn][];

        for (int y = 0; y < textureColumn; y++)
        {
            for (int x = 0; x < textureRow; x++)
            {
                pixelSet[y * textureRow + x] =
                    terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }

        return pixelSet;
    }
}
