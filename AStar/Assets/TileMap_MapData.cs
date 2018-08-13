using UnityEngine;
using System.Collections.Generic;

public class TileMap_MapData {
    public struct TileIndex
    {
        public readonly int x, z;

        public TileIndex(int xIndex, int zIndex)
        {
            x = xIndex;
            z = zIndex;
        }
    }

    public enum TileType
    {
        Block,
        Plane,
        Track,
        Destination
    }

    public class TileMap_TileData
    {
        private readonly TileType originalType;

        public TileMap_TileData(TileType tileType, int x, int z)
        {
            originalType = tileType;
            Update(originalType);

            Index = new TileIndex(x, z);
        }

        public TileIndex Index { get; private set; }
        public bool Passable { get; set; }
        public TileType Type { get; private set; }

        public void Update(TileType newType)
        {
            Type = newType;

            switch (Type)
            {
                case TileType.Block:
                    Passable = false;
                    break;
                case TileType.Plane:
                    Passable = true;
                    break;
                case TileType.Track:
                    Passable = true;
                    break;
                case TileType.Destination:
                    Passable = true;
                    break;
            }
        }
        public void Update_Returning()
        {
            Type = originalType;
        }
    }

    public TileMap_TileData[,] TileData { get; set; }
    List<TileMap_Room> rooms;
    public readonly int row, column;
    const int invalidIndex = -1;

    public TileMap_MapData(int tilesRow, int tilesColumn)
    {
        row = tilesRow;
        column = tilesColumn;

        TileData = new TileMap_TileData[row, column];

        for (int z = 0; z < column; z++)
        {
            for (int x = 0; x < row; x++)
            {
                TileData[x, z] = new TileMap_TileData(TileType.Block, x, z);
            }
        }
        
        CreateRooms();
    }
    
    public TileMap_Room GetRoom(int roomNumber)
    {
        if (roomNumber >= rooms.Count) return null;

        return rooms[roomNumber];
    }

    void CreateRooms()
    {
        int maximumRoomsNumber = (row / 10) * (column / 10);

        if (maximumRoomsNumber == 0) return;

        rooms = new List<TileMap_Room>();

        int failureCount = 0;
        int minimumRoomWidth = row / 10 * 2;
        int maximumRoomWidth = row / 10 * 3;
        int minimumRoomHeight = column / 10 * 2;
        int maximumRoomHeight = column / 10 * 3;
        while (true)
        {
            int roomWidth = Random.Range(minimumRoomWidth, maximumRoomWidth);
            int roomHeight = Random.Range(minimumRoomHeight, maximumRoomHeight);
            int roomLeft = Random.Range(1, row - 1 - maximumRoomWidth);
            int roomBottom = Random.Range(1, column - 1 - maximumRoomHeight);

            TileMap_Room newRoom = new TileMap_Room(roomLeft, roomBottom, roomWidth, roomHeight);

            bool roomsOverlapping = false;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms.Count <= 0) break;

                roomsOverlapping = newRoom.Overlapping(rooms[i]);

                if (roomsOverlapping == true) break;
            }

            if (roomsOverlapping == true)
            {
                failureCount++;
            }
            else
            {
                rooms.Add(newRoom);
            }

            if ((rooms.Count >= maximumRoomsNumber) || (failureCount >= maximumRoomsNumber * 2)) break;
        }

        SetupRooms();
    }

    void SetupRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            int right = rooms[i].Left + rooms[i].Width;
            int top = rooms[i].Bottom + rooms[i].Height;
            for (int z = rooms[i].Bottom; z < top; z++)
            {
                for (int x = rooms[i].Left; x < right; x++)
                {
                    TileData[x, z].Update(TileType.Plane);
                }
            }
        }

        ConnectRooms();
    }

    void ConnectRooms()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            if (rooms[i].Connected == true) continue;

            int rowIncrementor = (rooms[i].CenterX < rooms[i + 1].CenterX) ? 1 : -1;
            int columnIncrementor = (rooms[i].CenterZ < rooms[i + 1].CenterZ) ? 1 : -1;

            int x = rooms[i].CenterX;
            int z = rooms[i].CenterZ;

            while (true)
            {
                TileData[x, z].Update(TileType.Plane);

                if (x == rooms[i + 1].CenterX) break;

                x += rowIncrementor;
            }

            while (true)
            {
                TileData[x, z].Update(TileType.Plane);

                if (z == rooms[i + 1].CenterZ) break;

                z += columnIncrementor;
            }

            rooms[i].Connected = true;
        }

        SetupDestination(rooms[rooms.Count - 1].CenterX, rooms[rooms.Count - 1].CenterZ);
    }

    void SetupDestination(int x, int z)
    {
        TileData[x, z].Update(TileType.Destination);
    }
}
