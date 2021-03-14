using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite sprite;

    public bool shifted;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Find shifting tiles
        // Tiles are shifting if they are moving
        // Mark tiles that moving so they aren't checked again
        if (GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f 
            && Board.Instance.boardState == BoardState.SHUFFLING 
            && !shifted)
        {
            Board.Instance.shiftedTiles.Add(this);
            shifted = true;
        }
    }

    private void ClearTilesInDirection(GameObject tile, Vector2 direction, List<GameObject> matchList)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(tile.transform.position, direction);

        if (hit2D.collider.gameObject.GetComponent<Tile>().sprite == tile.GetComponent<Tile>().sprite)
        {

        }
    }

    private void Update()
    {
    }
}
