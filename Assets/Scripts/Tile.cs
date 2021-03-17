using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool shifted;
    public bool swapped;

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
            Board.Instance.currentlyShiftingTiles++;
            
            shifted = true;

            StartShifting();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (!IsInvoking(nameof(ShiftHorizontal)))
        //    Invoke(nameof(ShiftHorizontal), 1.5f);

        if (GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f
            && Board.Instance.boardState == BoardState.SHUFFLING
            && !shifted)
        {
            Board.Instance.shiftedTiles.Add(this);
            Board.Instance.currentlyShiftingTiles++;

            shifted = true;

            StartShifting();
        }
    }

    private void ShiftHorizontal()
    {
        shifted = true;

        Vector3 raycastOrigin = new Vector3(transform.position.x, transform.position.y - 1);
        RaycastHit2D[] neightborHits = Physics2D.RaycastAll(raycastOrigin, Vector2.right, 1f);
        RaycastHit2D[] selfHits = Physics2D.RaycastAll(transform.position, Vector2.right, 1f);

        if (neightborHits.Length >= 2 && selfHits.Length < 2)
        {
            //Board.Instance.shiftedTiles.Add(this);
            //Board.Instance.currentlyShiftingTiles++;

            transform.position = new Vector2(transform.position.x + 1, transform.position.y);
        }
    }

    public void StartShifting()
    {
        Invoke(nameof(EndShifting), 0.5f);
    }

    private void EndShifting()
    {
        Board.Instance.currentlyShiftingTiles--;
    }

}
