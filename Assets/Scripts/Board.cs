using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform rightBound;
    public Transform leftBound;
    public Transform bottomBound;
    
    public int rows;
    public int columns;

    public GameObject tileTemplate;
    public List<Sprite> tileSprites;

    // Start is called before the first frame update
    void Start()
    {
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
                tile.GetComponent<SpriteRenderer>().sprite = tileSprites[Random.Range(0, tileSprites.Count)];
            }
        }
    }
}
