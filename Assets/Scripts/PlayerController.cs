using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public UnityEvent Interact;

    private Vector2 mousePosition;

    public void OnUse()
    {
        Interact.Invoke();
    }

    public void OnClick(InputValue button)
    {
        Vector2 mousePosition2D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider != null)
        {
            Board.Instance.boardState = BoardState.SHUFFLING;
            Destroy(hit.collider.gameObject);
        }
    }

    public void OnCursorMove(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }
}
