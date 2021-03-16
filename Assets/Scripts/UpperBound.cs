using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBound : MonoBehaviour
{
    private Coroutine spawningTile;

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            if (spawningTile == null)
                StartCoroutine(SpawnTile(other.transform.position.x, other.transform.position.y));
        }
    }

    private IEnumerator SpawnTile(float x, float y)
    {
        yield return new WaitForSeconds(0.1f);

        Board.Instance.CreateTile(x, y);

        spawningTile = null;
    }
}
