using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    public GameObject tilePrefab;
    public Text minesText;
    public int numMines;
    const int DIMENSIONS = 10;
    const float TILE_OFFSET = 0.5f;

    private TileController[] tiles;
    private TileController[,] grid;
    private Dictionary<TileController, Position> tilePositionMap;
    private int[,] mineGrid;
    private int flaggedMines;

    // Use this for initialization
    void Start()
    {
        tiles = new TileController[DIMENSIONS * DIMENSIONS];
        grid = new TileController[DIMENSIONS, DIMENSIONS];
        tilePositionMap = new Dictionary<TileController, Position>();
        mineGrid = new int[DIMENSIONS, DIMENSIONS];
        flaggedMines = numMines;
        CreateTiles();
        AssignMines();
        PrecomputeMineCount();
        SetMinesText();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Explode()
    {
        Debug.Log("Boom!");
    }

    public int GetMineCount(TileController tile)
    {
        Position pos;
        tilePositionMap.TryGetValue(tile, out pos);
        return mineGrid[pos.x, pos.y];
    }

    public void IncrementMines()
    {
        flaggedMines++;
        SetMinesText();
    }

    public void DecrementMines()
    {
        flaggedMines--;
        SetMinesText();
    }

    private void CreateTiles()
    {
        float cameraOffset = DIMENSIONS / 2.0f * TILE_OFFSET;
        Vector3 offset = new Vector3(0.0f, 0.0f, transform.position.z);
        int id = 0;

        for (int x = 0; x < DIMENSIONS; x++)
        {
            for (int y = 0; y < DIMENSIONS; y++)
            {
                offset.x = transform.position.x - cameraOffset + TILE_OFFSET * x;
                offset.y = transform.position.y - cameraOffset + TILE_OFFSET * y;
                TileController tile = ((GameObject)Instantiate(tilePrefab, offset, transform.rotation)).GetComponent<TileController>();
                tile.transform.parent = transform;
                tile.gameObject.SetActive(true);
                tile.id = id;
                tiles[id] = tile;
                grid[x, y] = tile;
                tilePositionMap.Add(tile, new Position(x, y));
                id++;
            }
        }
    }

    private void AssignMines()
    {
        List<TileController> unassignedTiles = new List<TileController>();
        foreach (TileController tile in tiles)
        {
            unassignedTiles.Add(tile);
        }

        for (int i = 0; i < numMines; i++)
        {
            int index = (int)Mathf.Round(Random.Range(0, unassignedTiles.Count));
            TileController tile = unassignedTiles[index];
            unassignedTiles.RemoveAt(index);
            tile.isMined = true;
        }
        foreach (TileController tile in unassignedTiles)
        {
            tile.isMined = false;
        }
    }

    private void PrecomputeMineCount()
    {
        for (int x = 0; x < DIMENSIONS; x++)
        {
            for (int y = 0; y < DIMENSIONS; y++)
            {
                mineGrid[x, y] = ComputeMineCount(new Position(x, y));
            }
        }
    }

    private int ComputeMineCount(Position pos)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Position neighbor = new Position(pos.x + x, pos.y + y);
                if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < DIMENSIONS && neighbor.y < DIMENSIONS)
                {
                    if (grid[neighbor.x, neighbor.y].isMined)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private void SetMinesText()
    {
        minesText.text = "Mines: " + flaggedMines;
    }

    private void PrintBoard()
    {
        foreach (TileController tile in tiles)
        {
            Debug.Log(tile.id + " (" + tile.GetHashCode() + "): " + tile.isMined);
        }
    }
}
