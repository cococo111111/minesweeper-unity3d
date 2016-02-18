using UnityEngine;
using System.Collections.Generic;

public class TileController : MonoBehaviour
{
    public enum State { Covered, Flagged, Uncovered, Exploded };
    public int id { get; set; }
    public bool isMined { get; set; }
    public State state;

    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> spriteMap;
    private BoardController board;

    // Use this for initialization
    void Start()
    {
        state = State.Covered;
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadSprites();
        board = transform.parent.gameObject.GetComponent<BoardController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseOver()
    {
        if (board.state != BoardController.State.Playing) {
            Debug.Log("Can't play after game is done.");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
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

    public void ReplaceSprite(string name)
    {
        Sprite sprite;
        spriteMap.TryGetValue(name, out sprite);
        spriteRenderer.sprite = sprite;
    }

    public void Uncover(bool empty = false)
    {
        if (state == State.Flagged)
        {
            Debug.Log("Can't click on flagged tiles.");
        }
        else if (isMined && !empty)
        {
            state = State.Exploded;
            ReplaceSprite("tileExplosion");
            board.Explode(this);
        }
        else
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
                    board.UncoverEmptyNeighbors(this);
                }
                board.CheckWin();
            }
            else
            {
                Debug.Log("Error, Can't have more than 8 mines: " + mines);
            }
        }
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
