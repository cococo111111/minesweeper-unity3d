using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    public enum State { FirstTurn, Playing, Win, Lose };
    public GameObject tilePrefab;
    public Text minesText;
    public Text endgameText;
    public int numMines;
    public State state;
    const int DIMENSIONS = 10;
    const float TILE_OFFSET = 0.4f;

    private TileController[] tiles;
    private TileController[,] grid;
    private Dictionary<TileController, Position> tilePositionMap;
    private int[,] mineGrid;
    private TileController[] mineTiles;
    private List<TileController> nonMineTiles;
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
        SetMinesText();
        endgameText.text = "";
        state = State.FirstTurn;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CompleteFirstTurn(TileController tile)
    {
        AssignMines(tile);
        state = State.Playing;
    }

    public void Explode(TileController bombTile)
    {
        foreach (TileController tile in mineTiles)
        {
            if (tile != bombTile)
            {
                tile.MarkBomb();
            }
        }

        endgameText.text = "Sorry, You Lose";
        state = State.Lose;
    }

    public void CheckWin()
    {
        foreach (TileController tile in nonMineTiles)
        {
            if (tile.state != TileController.State.Uncovered)
            {
                return;
            }
        }

        state = State.Win;
        endgameText.text = "Congrats, You Win!";
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

    public void UncoverNeighbors(TileController tile)
    {
        Position pos;
        tilePositionMap.TryGetValue(tile, out pos);

        foreach (TileController neighborTile in Neighbors(pos))
        {
            if (neighborTile.state == TileController.State.Covered)
            {
                neighborTile.Uncover(true);
            }
        }
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

    private void AssignMines(TileController firstTile)
    {
        mineTiles = new TileController[numMines];
        nonMineTiles = new List<TileController>();

        foreach (TileController tile in tiles)
        {
            nonMineTiles.Add(tile);
        }
        nonMineTiles.Remove(firstTile);

        for (int i = 0; i < numMines; i++)
        {
            int index = (int)Mathf.Round(Random.Range(0, nonMineTiles.Count));
            TileController tile = nonMineTiles[index];
            nonMineTiles.RemoveAt(index);
            tile.isMined = true;
            mineTiles[i] = tile;
        }
        foreach (TileController tile in nonMineTiles)
        {
            tile.isMined = false;
        }
        nonMineTiles.Add(firstTile);

        PrecomputeMineCount();
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

        foreach (TileController tile in Neighbors(pos))
        {
            if (tile.isMined)
            {
                count++;
            }
        }

        return count;
    }

    private List<TileController> Neighbors(Position pos)
    {
        List<TileController> neighbors = new List<TileController>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Position neighbor = new Position(pos.x + x, pos.y + y);
                if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < DIMENSIONS && neighbor.y < DIMENSIONS)
                {
                    neighbors.Add(grid[neighbor.x, neighbor.y]);
                }
            }
        }

        return neighbors;
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
