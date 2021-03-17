using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBound : MonoBehaviour
{
    private Coroutine spawningTile;

    private void OnCollisionExit2D(Collision2D other)
    {
        if (Board.Instance.boardState == BoardState.STARTING) return;

        if (other.gameObject.CompareTag("Tile"))
        {
            if (spawningTile == null)
                StartCoroutine(SpawnTile(other.transform.position.x, other.transform.position.y));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            if (spawningTile != null)
                StopCoroutine(spawningTile);
        }
    }

    private IEnumerator SpawnTile(float x, float y)
    {
        yield return new WaitForSeconds(0.1f);

        Board.Instance.boardState = BoardState.SHUFFLING;

        Tile newTile = Board.Instance.CreateTile(x, y);

        newTile.shifted = true;
        Board.Instance.shiftedTiles.Add(newTile);

        spawningTile = null;
    }
}
