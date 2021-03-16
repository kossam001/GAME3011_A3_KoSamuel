using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    SELECTION,
    SHUFFLING,
    CLEARING
}

public class Board : MonoBehaviour
{
    private static Board instance;
    public static Board Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);

        else
            instance = this;
    }

    public BoardState boardState = BoardState.SELECTION;

    public Transform rightBound;
    public Transform leftBound;
    public Transform bottomBound;
    
    public int rows;
    public int columns;

    public GameObject tileTemplate;
    public List<Sprite> tileSprites;

    public List<Tile> shiftedTiles;
    public int currentlyShiftingTiles;
    public List<Tile> allMatches;

    private Coroutine clearingRoutine;

    // Start is called before the first frame update
    void Start()
    {
        allMatches = new List<Tile>();

        SetBounds(rows, columns);
        CreateTiles(0.5f, 0.5f);
    }

    private void SetBounds(int rows, int columns)
    {
        rightBound.position = new Vector3(columns, 0.0f);

        leftBound.localScale = new Vector3(1.0f, rows);
        rightBound.localScale = new Vector3(1.0f, rows);

        bottomBound.localScale = new Vector3(columns, 1.0f);
    }

    private void CreateTiles(float xOffset, float yOffset)
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile = Instantiate(tileTemplate, new Vector3(x + xOffset, y + yOffset), Quaternion.identity);
                tile.transform.SetParent(transform);
                AssignSprite(tile);
            }
        }
    }

    private void AssignSprite(GameObject tile)
    {
        RaycastHit2D[] leftHit = Physics2D.RaycastAll(tile.transform.position, Vector2.left, 2.0f);
        RaycastHit2D[] downHit = Physics2D.RaycastAll(tile.transform.position, Vector2.down, 2.0f);

        RaycastHit2D[][] hitChecks = { leftHit, downHit };

        List<Sprite> validSprites = new List<Sprite>(tileSprites);

        foreach (RaycastHit2D[] hitDirections in hitChecks)
        {
            Sprite comparisonSprite = null;

            for (int i = 0; i < hitDirections.Length; i++)
            {
                // 
                if (hitDirections[i].collider.GetComponent<Tile>() != null)
                {
                    if (comparisonSprite == null)
                        comparisonSprite = hitDirections[i].collider.GetComponent<SpriteRenderer>().sprite;
                    else if (comparisonSprite == hitDirections[i].collider.GetComponent<SpriteRenderer>().sprite)
                    {
                        validSprites.Remove(hitDirections[i].collider.GetComponent<SpriteRenderer>().sprite);
                    }
                }
            }
        }

        tile.GetComponent<SpriteRenderer>().sprite = validSprites[Random.Range(0, validSprites.Count)];
    }

    private IEnumerator FindMatches()
    {
        foreach (Tile tile in shiftedTiles)
        {
            Vector2[] checkHorizontalDirections = { Vector2.left, Vector2.right }; // Up not necessary
            Vector2[] checkVerticalDirections = { Vector2.up, Vector2.down }; // Up not necessary

            yield return StartCoroutine(CheckInDirections(checkHorizontalDirections, tile));
            yield return StartCoroutine(CheckInDirections(checkVerticalDirections, tile));

            tile.shifted = false; // Checked
        }

        shiftedTiles.Clear();

        for (int i = 0; i < allMatches.Count; i++)
        {
            if (allMatches[i] != null)
                Destroy(allMatches[i].gameObject);
        }

        allMatches.Clear();

        clearingRoutine = null;
        boardState = BoardState.SHUFFLING;
    }

    private IEnumerator CheckInDirections(Vector2[] directions, Tile tile)
    {
        List<Tile> matches = new List<Tile>();
        
        matches.Add(tile);

        Tile matchingTile = tile;

        // Find all matching tiles in the row left and right of the tile
        foreach (Vector2 direction in directions)
        {
            while (matchingTile != null)
            {
                matchingTile = CheckInDirection(matchingTile, direction);

                if (matchingTile != null)
                    matches.Add(matchingTile);

                yield return null;
            }

            // Go back to starting tile and check opposite direction
            matchingTile = tile;
        }

        if (matches.Count < 3)
            matches.Clear();

        else
            allMatches.AddRange(matches);
    }

    private Tile CheckInDirection(Tile tile, Vector2 direction)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(tile.transform.position, direction, 1.0f);

        foreach (RaycastHit2D result in hit)
        {
            if (result.collider.gameObject.GetComponent<Tile>() != null)
            {
                if (result.collider.gameObject.GetComponent<SpriteRenderer>().sprite == tile.GetComponent<SpriteRenderer>().sprite && result.distance > 0.0f)
                {
                    return result.collider.gameObject.GetComponent<Tile>();
                }
            }
        }

        return null;
    }

    private void CheckShuffling()
    {
        if (currentlyShiftingTiles <= 0)
            boardState = BoardState.SELECTION;
    }

    private void Update()
    {
        switch(boardState)
        {
            case BoardState.SELECTION:
                break;
            case BoardState.SHUFFLING:
                if (currentlyShiftingTiles <= 0 && shiftedTiles.Count > 0)
                    boardState = BoardState.CLEARING;
                else
                {
                    if (!IsInvoking(nameof(CheckShuffling)))
                        Invoke(nameof(CheckShuffling), 2.0f);
                }                        

                break;
            case BoardState.CLEARING:
                if (clearingRoutine != null) return;

                clearingRoutine = StartCoroutine(FindMatches());
                break;
        }
    }
}
