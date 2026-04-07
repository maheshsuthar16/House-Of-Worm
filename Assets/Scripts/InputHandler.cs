using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public WormController worm;

    Vector2 startPos;
    bool dragging;

    float swipeThreshold = 40f;

    void Update()
    {
        // ----- TOUCH INPUT -----
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
                dragging = true;
            }

            if (touch.phase == TouchPhase.Moved && dragging)
            {
                DetectSwipe(touch.position);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                dragging = false;
            }
        }

        // ----- MOUSE INPUT (EDITOR TESTING) -----
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            dragging = true;
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            DetectSwipe(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        // ----- KEYBOARD BACKUP -----
        if (Input.GetKeyDown(KeyCode.UpArrow)) worm.SetDirection(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) worm.SetDirection(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) worm.SetDirection(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) worm.SetDirection(Vector2Int.right);
    }

    void DetectSwipe(Vector2 currentPos)
    {
        Vector2 delta = currentPos - startPos;

        if (delta.magnitude < swipeThreshold)
            return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
                worm.SetDirection(Vector2Int.right);
            else
                worm.SetDirection(Vector2Int.left);
        }
        else
        {
            if (delta.y > 0)
                worm.SetDirection(Vector2Int.up);
            else
                worm.SetDirection(Vector2Int.down);
        }

        dragging = false; // prevents multiple triggers
    }
}