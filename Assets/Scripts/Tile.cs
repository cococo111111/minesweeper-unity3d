using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public enum State { Covered, Flagged, Uncovered, Exploded };
    public int id { get; set; }
    public bool isMined { get; set; }
    public State state;

    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> spriteMap;
    private Board board;

    // Use this for initialization
    void Start()
    {
        state = State.Covered;
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadSprites();
        board = transform.parent.gameObject.GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseOver()
    {
        if (board.state != Board.State.Playing && board.state != Board.State.FirstTurn)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (board.state == Board.State.FirstTurn)
            {
                board.CompleteFirstTurn(this);
            }
            Uncover();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SetFlag();
        }
    }

    public void LoadSprites()
    {
        spriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        foreach (Sprite sprite in sprites)
        {
            spriteMap.Add(sprite.name, sprite);
        }
    }

    public void MarkBomb()
    {
        ReplaceSprite("tileBombMark");
    }

    public void Uncover(bool boardAction = false)
    {
        if (state == State.Flagged)
        {
            return;
        }
        else if (isMined)
        {
            state = State.Exploded;
            ReplaceSprite("tileExplosion");
            board.Explode(this);
        }
        else if (state == State.Uncovered && !boardAction)
        {
            board.UncoverUnflaggedNeighbors(this);
        }
        else if (state == State.Covered)
        {
            int mines = board.GetMineCount(this);
            string spriteName = "tile";
            string[] words = { "Pressed", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight" };
            if (mines < 9)
            {
                state = State.Uncovered;
                spriteName += words[mines];
                ReplaceSprite(spriteName);
                if (mines == 0)
                {
                    board.UncoverNeighbors(this);
                }
                board.CheckWin();
            }
            else
            {
                Debug.Log("Error, Can't have more than 8 mines: " + mines);
            }
        }
    }

    private void ReplaceSprite(string name)
    {
        Sprite sprite;
        spriteMap.TryGetValue(name, out sprite);
        spriteRenderer.sprite = sprite;
    }

    private void SetFlag()
    {
        if (state == State.Flagged)
        {
            board.IncrementMines();
            state = State.Covered;
            ReplaceSprite("tile");
        }
        else if (state == State.Covered)
        {
            board.DecrementMines();
            state = State.Flagged;
            ReplaceSprite("tileFlag");
        }
    }
}
