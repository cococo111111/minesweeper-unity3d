using UnityEngine;
using System.Collections.Generic;

public class TileController : MonoBehaviour
{
    public int id { get; set; }
    public bool isMined { get; set; }

    private bool isFlagged = false;
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> spriteMap;
    private BoardController board;

    // Use this for initialization
    void Start()
    {
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

    private void Uncover()
    {
        if (isMined)
        {
            board.Explode();
        }
        else
        {
            int mines = board.GetMineCount(this);
            string spriteName = "tile";
            string[] words = {"", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight"};
            if (mines < 9)
            {
                spriteName += words[mines];
                ReplaceSprite(spriteName);
            }
            else
            {
                Debug.Log("Error, Can't have more than 8 mines: " + mines);
            }
        }
    }

    private void SetFlag()
    {
        Debug.Log("Clicked: " + isMined);
        if (isFlagged)
        {
            isFlagged = false;
            ReplaceSprite("tile");
        }
        else
        {
            isFlagged = true;
            ReplaceSprite("tileFlag");
        }
    }
}
