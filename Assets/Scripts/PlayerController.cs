using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public UnityEvent Interact;

    private Vector2 mousePosition;

    public Tile selectedTile;
    public Tile tileToSwapWith;
    public ContactPoint2D[] adjacentTiles = new ContactPoint2D[4];

    public void OnUse()
    {
        Interact.Invoke();
    }

    public void OnClick(InputValue button)
    {
        if (Board.Instance.boardState != BoardState.SELECTION) return;

        Vector2 mousePosition2D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Tile"))
        {
            //Board.Instance.boardState = BoardState.SHUFFLING;
            //Destroy(hit.collider.gameObject);

            if (selectedTile == null)
            {
                selectedTile = hit.collider.gameObject.GetComponent<Tile>();
                selectedTile.gameObject.GetComponent<SpriteRenderer>().color *= 0.5f;

                hit.collider.GetContacts(adjacentTiles);
            }
            else
            {
                ContactPoint2D[] contacts = new ContactPoint2D[4];
                hit.collider.GetContacts(contacts);

                bool adjacentSelected = false;

                foreach (ContactPoint2D contact in adjacentTiles)
                {
                    if (ReferenceEquals(contact.collider, hit.collider))
                    {
                        tileToSwapWith = contact.collider.GetComponent<Tile>();
                        Vector2 tempPosition = selectedTile.transform.position;

                        // Swap position
                        selectedTile.transform.position = tileToSwapWith.transform.position;
                        tileToSwapWith.transform.position = tempPosition;

                        // Mark shifted
                        selectedTile.shifted = true;
                        Board.Instance.shiftedTiles.Add(selectedTile);

                        tileToSwapWith.shifted = true;
                        Board.Instance.shiftedTiles.Add(tileToSwapWith);

                        Board.Instance.boardState = BoardState.CLEARING;

                        adjacentSelected = true;
                        tileToSwapWith.swapped = true;
                        selectedTile.swapped = true;

                        InvokeRepeating(nameof(SwapBack), 0.1f, 0.1f);
                    }
                }

                if (!adjacentSelected)
                {
                    selectedTile.gameObject.GetComponent<SpriteRenderer>().color *= 2.0f;
                    selectedTile = null;
                }
            }
        }
    }

    public void SwapBack()
    {
        if (selectedTile == null || tileToSwapWith == null || selectedTile.isBomb || tileToSwapWith.isBomb)
        {
            //if (selectedTile.isBomb)
            //{
            //    Board.Instance.score += selectedTile.points;
            //    Destroy(selectedTile.gameObject);
            //}
            //else if (tileToSwapWith.isBomb)
            //{
            //    Board.Instance.score += tileToSwapWith.points;
            //    Destroy(tileToSwapWith.gameObject);
            //}

            if (selectedTile != null)
            {
                selectedTile.gameObject.GetComponent<SpriteRenderer>().color *= 2.0f;
                selectedTile.swapped = false;
            }

            if (tileToSwapWith != null)
                tileToSwapWith.swapped = false;

            CancelInvoke(nameof(SwapBack));
            return;
        }
        else if (!selectedTile.shifted && Board.Instance.boardState != BoardState.CLEARING)
        {
            selectedTile.gameObject.GetComponent<SpriteRenderer>().color *= 2.0f;

            Vector2 tempPosition = selectedTile.transform.position;

            // Swap position
            selectedTile.transform.position = tileToSwapWith.transform.position;
            tileToSwapWith.transform.position = tempPosition;

            tileToSwapWith.swapped = true;
            selectedTile.swapped = true;

            selectedTile = null;
            tileToSwapWith = null;

            CancelInvoke(nameof(SwapBack));
        }
    }

    public void OnCursorMove(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }
}
