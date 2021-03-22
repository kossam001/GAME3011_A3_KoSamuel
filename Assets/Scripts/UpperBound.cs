using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBound : MonoBehaviour
{
    private Coroutine spawningTile;

    private void OnCollisionExit2D(Collision2D other)
    {
        if (Board.Instance.boardState == BoardState.STARTING) return;

        if (!other.gameObject.GetComponent<Tile>().swapped)
        {
            if (spawningTile == null)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(other.transform.position, Vector2.down);

                if (hits.Length == Board.Instance.rows + 1) return;

                Board.Instance.boardState = BoardState.SHUFFLING;

                Tile newTile = Board.Instance.CreateTile(other.transform.position.x, other.transform.position.y);

                newTile.shifted = true;
                Board.Instance.shiftedTiles.Add(newTile);
                Board.Instance.currentlyShiftingTiles++;
                newTile.StartShifting();

                spawningTile = null;
            }
        }
    }

    private IEnumerator SpawnTile(float x, float y)
    {
        yield return new WaitForSeconds(0.1f);
    }
}
