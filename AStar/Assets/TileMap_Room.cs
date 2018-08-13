public class TileMap_Room {
    private int left;
    private int bottom;
    private int width;
    private int height;
    private int right;
    private int top;
    private int centerX;
    private int centerZ;
    
    public int Left { get { return left; } }
    public int Bottom { get { return bottom; } }
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int Right { get { return right; } }
    public int Top { get { return top; } }
    public int CenterX { get { return centerX; } }
    public int CenterZ { get { return centerZ; } }

    public bool Connected { get; set; }

    public TileMap_Room(int roomLeft, int roomBottom, int roomWidth, int roomHeight)
    {
        left = roomLeft;
        bottom = roomBottom;
        width = roomWidth;
        height = roomHeight;

        right = left + width - 1;
        top = bottom + height - 1;

        centerX = left + width / 2;
        centerZ = bottom + height / 2;

        Connected = false;
    }

    public bool Overlapping(TileMap_Room otherRoom)
    {
        if (left > otherRoom.right + 2) return false;
        if (right < otherRoom.left - 2) return false;
        if (bottom > otherRoom.top + 2) return false;
        if (top < otherRoom.bottom - 2) return false;

        return true;
    }
}
