using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public TileBase groundTile;
    public Tilemap groundTilemap;

    public TileBase wallTile;
    public Tilemap wallTilemap;

    public int width;
    public int height;
    public float chanceToChangeDirection = 0.5f;
    public float chanceToTurnAround = 0.05f;
    public float chanceWalkerSpawn = 0.05f;
    public float chanceWalkerDestroy = 0.05f;
    public float chanceFor2x2 = 0.5f;
    public float chanceFor3x3 = 0.01f;
    public int maxTiles = 110;

    public PathfindingGrid pathfinderGrid;

    bool gridSet = false;

    int[,] map;
    int maxWalkers = 10;
    int tiles = 0;

    struct Walker
    {
        public Vector2Int direction;
        public Vector2Int position;
    }

    List<Walker> walkers;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        CreateTileMap();
        //pathfinderGrid.Init();
    }

    void GenerateMap()
    {
        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 1;
            }
        }

        tiles = 0;

        walkers = new List<Walker>();
        Walker newWalker = new Walker();
        newWalker.direction = RandomDirection();
        Vector2Int spawnPos = new Vector2Int(Mathf.RoundToInt(width * 0.5f), Mathf.RoundToInt(height * 0.5f));
        newWalker.position = spawnPos;

        walkers.Add(newWalker);

        RandomWalk();
    }

    void RandomWalk()
    {
        int iterations = 0;

        while (iterations < 100000)
        {
            // Create floor for each walker
            foreach (Walker walker in walkers)
            {
                if (Random.value < chanceFor3x3)
                {
                    PlaceRoom3x3(walker);
                }
                else if (Random.value < chanceFor2x2)
                {
                    PlaceRoom2x2(walker);
                }
                else
                {
                    PlaceFloor(walker.position);
                }
            }

            // Chance: destroy walker
            int numWalkers = walkers.Count;
            for (int i = 0; i < numWalkers; i++)
            {
                if (Random.value < chanceWalkerDestroy && numWalkers > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                }
            }

            // Chance walker picks new direction
            numWalkers = walkers.Count;
            for (int i = 0; i < numWalkers; i++)
            {
                if (Random.value < chanceToChangeDirection)
                {
                    Walker walker = walkers[i];
                    walker.direction = ChangeDirection(walkers[i].direction);
                    walkers[i] = walker;
                }
            }

            // Chance: Spawn new walker
            for (int i = 0; i < numWalkers; i++)
            {
                if (Random.value < chanceWalkerSpawn && numWalkers < maxWalkers)
                {
                    Walker newWalker = new Walker();
                    newWalker.direction = ChangeDirection(walkers[i].direction);
                    newWalker.position = walkers[i].position;
                    walkers.Add(newWalker);
                }
            }

            // Move walkers
            numWalkers = walkers.Count;
            for (int i = 0; i < numWalkers; i++)
            {
                Walker walker = walkers[i];
                walker.position += walker.direction;
                walkers[i] = walker;
            }

            for (int i = 0; i < numWalkers; i++)
            {
                Walker walker = walkers[i];
                walker.position.x = Mathf.Clamp(walker.position.x, 1, width - 1);
                walker.position.y = Mathf.Clamp(walker.position.y, 1, height - 1);
                walkers[i] = walker;
            }

            if (tiles >= maxTiles)
            {
                break;
            }

            iterations++;
        }
    }

    void CreateTileMap()
    {
        Vector3Int offset = -new Vector3Int(Mathf.RoundToInt(width * 0.5f), Mathf.RoundToInt(height * 0.5f), 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    wallTilemap.SetTile(offset + new Vector3Int(x, y, 0), wallTile);
                }
                else
                {
                    groundTilemap.SetTile(offset + new Vector3Int(x, y, 0), groundTile);
                }
            }
        }
    }

    void PlaceRoom2x2(Walker walker)
    {
        Vector2Int xLims, yLims;
        if (walker.direction == Vector2Int.down)
        {
            if (Random.value < 0.5f)
            {
                xLims = new Vector2Int(0, 1);
            }
            else
            {
                xLims = new Vector2Int(-1, 0);
            }
            yLims = new Vector2Int(-1, 0);
        }
        else if (walker.direction == Vector2Int.left)
        {
            xLims = new Vector2Int(0, 1);
            if (Random.value < 0.5f)
            {
                yLims = new Vector2Int(0, 1);
            }
            else
            {
                yLims = new Vector2Int(-1, 0);
            }
        }
        else if (walker.direction == Vector2Int.up)
        {
            if (Random.value < 0.5f)
            {
                xLims = new Vector2Int(0, 1);
            }
            else
            {
                xLims = new Vector2Int(-1, 0);
            }
            yLims = new Vector2Int(0, 1);
        }
        else
        {
            xLims = new Vector2Int(-1, 0);
            if (Random.value < 0.5f)
            {
                yLims = new Vector2Int(0, 1);
            }
            else
            {
                yLims = new Vector2Int(-1, 0);
            }
        }

        for (int i = xLims[0]; i <= xLims[1]; i++)
        {
            for (int j = yLims[0]; j <= yLims[1]; j++)
            {
                PlaceFloor(walker.position + new Vector2Int(i, j));
            }
        }
    }

    void PlaceRoom3x3(Walker walker)
    {
        Vector2Int xLims, yLims;
        if (walker.direction == Vector2Int.down)
        {
            xLims = new Vector2Int(-1, 1);
            yLims = new Vector2Int(-2, 0);
        }
        else if (walker.direction == Vector2Int.left)
        {
            xLims = new Vector2Int(0, 2);
            yLims = new Vector2Int(-1, 1);
        }
        else if (walker.direction == Vector2Int.up)
        {
            xLims = new Vector2Int(-1, 1);
            yLims = new Vector2Int(0, 2);
        }
        else
        {
            xLims = new Vector2Int(-2, 0);
            yLims = new Vector2Int(-1, 1);
        }

        for (int i = xLims[0]; i <= xLims[1]; i++)
        {
            for (int j = yLims[0]; j <= yLims[1]; j++)
            {
                PlaceFloor(walker.position + new Vector2Int(i, j));
            }
        }
    }

    void PlaceFloor(Vector2Int position)
    {
        if (map[position.x, position.y] != 0 && position.x >= 0 && position.y >= 0 && position.x < width && position.y < height)
        {
            map[position.x, position.y] = 0;
            tiles++;
        }
    }

    Vector2Int RandomDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2Int.down;
            case 1:
                return Vector2Int.left;
            case 2:
                return Vector2Int.up;
            default:
                return Vector2Int.right;
        }
    }

    Vector2Int ChangeDirection(Vector2Int currentDirection)
    {
        if (Random.value < chanceToTurnAround)
        {
            if (currentDirection == Vector2Int.down)
            {
                return Vector2Int.up;
            }
            else if (currentDirection == Vector2Int.left)
            {
                return Vector2Int.right;
            }
            else if (currentDirection == Vector2Int.up)
            {
                return Vector2Int.down;
            }
            else
            {
                return Vector2Int.left;
            }
        }
        else
        {
            if (Random.value < 0.5f)
            {
                if (currentDirection == Vector2Int.down)
                {
                    return Vector2Int.right;
                }
                else if (currentDirection == Vector2Int.left)
                {
                    return Vector2Int.down;
                }
                else if (currentDirection == Vector2Int.up)
                {
                    return Vector2Int.left;
                }
                else
                {
                    return Vector2Int.up;
                }
            }
            else
            {
                if (currentDirection == Vector2Int.down)
                {
                    return Vector2Int.left;
                }
                else if (currentDirection == Vector2Int.left)
                {
                    return Vector2Int.up;
                }
                else if (currentDirection == Vector2Int.up)
                {
                    return Vector2Int.right;
                }
                else
                {
                    return Vector2Int.down;
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!gridSet)
        {
            if (pathfinderGrid != null)
            {
                pathfinderGrid.Init();
            }
            gridSet = true;
        }
    }
}
