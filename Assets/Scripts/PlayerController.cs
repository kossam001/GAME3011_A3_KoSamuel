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
    public Tile previouslySelectedTile;
    public ContactPoint2D[] adjacentTiles = new ContactPoint2D[4];

    public void OnUse()
    {
        Interact.Invoke();
    }

    public void OnClick(InputValue button)
    {
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
                        contact.collider.gameObject.GetComponent<SpriteRenderer>().color *= 0.1f;
                        adjacentSelected = true;
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

    public void OnCursorMove(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }
}
