using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool shifted;

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

            Invoke(nameof(EndShifting), 0.1f);
        }
    }

    private void EndShifting()
    {
        Board.Instance.currentlyShiftingTiles--;
    }
}
