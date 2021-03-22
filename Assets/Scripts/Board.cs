using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BoardState
{
    STARTING,
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

    public BoardState boardState = BoardState.STARTING;

    public Transform rightBound;
    public Transform leftBound;
    public Transform bottomBound;
    public Transform upperBound;
    
    public int rows;
    public int columns;

    public GameObject tileTemplate;
    public List<Sprite> tileSprites;
    public List<int> tilePoints;

    public List<Tile> shiftedTiles;
    public int currentlyShiftingTiles;
    public List<Tile> allMatches;

    private Coroutine clearingRoutine;

    [SerializeField] private int score;
    [SerializeField] private TMP_Text scoreLabel;

    public int maxImmoveableTilesCount = 0;
    private int immoveableTileCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        allMatches = new List<Tile>();

        SetBounds(rows, columns);
        CreateTiles(0.5f, 0.5f);

        boardState = BoardState.SELECTION;
    }

    private void SetBounds(int rows, int columns)
    {
        rightBound.position = new Vector3(columns, 0.0f);
        upperBound.position = new Vector3(0.0f, rows - 0.5f);

        leftBound.localScale = new Vector3(1.0f, rows);
        rightBound.localScale = new Vector3(1.0f, rows);

        bottomBound.localScale = new Vector3(columns, 1.0f);
        upperBound.localScale = new Vector3(columns, 1.0f);
    }

    private void CreateTiles(float xOffset, float yOffset)
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                CreateTile(x + xOffset, y + yOffset);
            }
        }
    }

    public Tile CreateTile(float x, float y)
    {
        GameObject tile = Instantiate(tileTemplate, new Vector3(x, y), Quaternion.identity);
        tile.transform.SetParent(transform);

        // Create Immoveable tiles
        if (Random.Range(0.0f, 1.0f) < 0.1f && immoveableTileCount <= maxImmoveableTilesCount)
        {
            immoveableTileCount++;
            tile.GetComponent<Tile>().isImmoveable = true;
            tile.tag = "Untagged";
        }
        else   
            AssignSprite(tile);

        return tile.GetComponent<Tile>();
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

        int randomInt = Random.Range(0, validSprites.Count);
        tile.GetComponent<SpriteRenderer>().sprite = validSprites[randomInt];
        tile.GetComponent<Tile>().points = tilePoints[randomInt];
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
            {
                allMatches[i].gameObject.GetComponent<Tile>().swapped = false;
                score += allMatches[i].points;
                scoreLabel.text = score.ToString();

                Destroy(allMatches[i].gameObject);
            }
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
            if (result.collider.gameObject.GetComponent<Tile>() != null &&
                result.collider.gameObject.CompareTag("Tile"))
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
