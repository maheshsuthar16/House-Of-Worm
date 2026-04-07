using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WormController : MonoBehaviour
{
    [Header("Head")]
    [SerializeField] SpriteRenderer headRenderer;

    [Header("Grid Step")]
    [SerializeField] float stepInterval = 0.10f;
    [SerializeField] float stepMoveDuration = 0.10f;

    [Header("Line Body (Worm)")]
    [SerializeField] LineRenderer bodyLine;
    [Min(2)][SerializeField] int bodyLengthCells = 7;
    [SerializeField] float lineZ = -0.1f;

    [Header("Exit Suction (Follow Path)")]
    [SerializeField] float headToHoleDuration = 0.10f;
    [SerializeField] float suckStepDuration = 0.06f;

    GridManager grid;
    GameManager game;

    Vector2Int headCell;
    Vector2Int moveDir;

    bool running;
    bool exiting;

    bool blocked;

    Coroutine loop;
    Tween headTween;

    // Stored as tail -> head (last element is head)
    readonly List<Vector2Int> trail = new List<Vector2Int>();

    // =====================================================
    // SPAWN
    // =====================================================
    public void Spawn(Vector2Int startPos)
    {
        grid = FindObjectOfType<GridManager>();
        game = GameManager.Instance;

        enabled = true;
        running = true;
        exiting = false;

        blocked = true; // ✅ NEW
        moveDir = Vector2Int.zero;

        if (headTween != null && headTween.IsActive()) headTween.Kill();
        transform.DOKill(true);
        transform.localScale = Vector3.one;
        // transform.rotation = Quaternion.identity;

        headCell = startPos;
        transform.position = CellToWorld(headCell);

        if (headRenderer) headRenderer.enabled = true;

        trail.Clear();
        for (int i = 0; i < bodyLengthCells; i++)
            trail.Add(startPos);

        SetupLine();
        RefreshLine_HeadToTail();

        if (loop != null) StopCoroutine(loop);
        loop = StartCoroutine(StepLoop());
    }

    void SetupLine()
    {
        if (!bodyLine) return;

        bodyLine.enabled = true;
        bodyLine.useWorldSpace = true;
        bodyLine.positionCount = 0;
        bodyLine.alignment = LineAlignment.View;
        bodyLine.textureMode = LineTextureMode.Stretch;
        bodyLine.numCapVertices = 4;
        bodyLine.numCornerVertices = 4;


        // draw above tiles/sprites
        bodyLine.sortingOrder = 200; // increase if still behind
    }

    bool CanAcceptInput()
    {
        // Allow input only when worm is running, not exiting,
        // and currently blocked (stopped by wall)
        return running && !exiting && blocked;
    }
    // =====================================================
    // INPUT
    // =====================================================
    public void SetDirection(Vector2Int dir)
    {
        if (!CanAcceptInput()) return;
        if (!running || exiting) return;
        if (dir == Vector2Int.zero) return;

        // no diagonal
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) != 1) return;

        //  If you want: direction locked on current cell == 0
        // int here = grid.GetCell(headCell);
        // if (here == 0) return;

        moveDir = dir;

        //  NEW: if we were blocked by a wall, new direction unblocks
        blocked = false;
    }

    // =====================================================
    // STEP LOOP
    // =====================================================
    IEnumerator StepLoop()
    {
        var wait = new WaitForSeconds(stepInterval);

        while (running && !exiting)
        {
            if (moveDir != Vector2Int.zero)
            {
                //  NEW: if blocked, do nothing until player changes direction
                if (blocked)
                {
                    yield return wait;
                    continue;
                }

                Vector2Int next = headCell + moveDir;

                if (!grid.IsInside(next))
                {
                    Fail();
                    yield break;
                }

                int cell = grid.GetCell(next);

                // NEW: Wall stops movement but keeps moveDir active
                if (cell == 1 || cell == 4)
                {
                    blocked = true;   // stop stepping forward
                    yield return wait;
                    continue;
                }

                // commit
                headCell = next;

                // update trail (tail -> head)
                trail.Add(headCell);
                while (trail.Count > bodyLengthCells)
                    trail.RemoveAt(0);

                yield return AnimateHeadTo(headCell);

                RefreshLine_HeadToTail();

                if (cell == 3)
                {
                    yield return ExitSuctionRoutine_FollowPath();
                    yield break;
                }
            }

            yield return wait;
        }
    }

    IEnumerator AnimateHeadTo(Vector2Int cell)
    {
        Vector3 target = CellToWorld(cell);

        if (headTween != null && headTween.IsActive())
            headTween.Kill();

        headTween = transform.DOMove(target, stepMoveDuration).SetEase(Ease.Linear);
        yield return headTween.WaitForCompletion();
    }

    // =====================================================
    // LINE: point 0 = HEAD, last = TAIL
    // =====================================================
    void RefreshLine_HeadToTail()
    {
        if (!bodyLine) return;

        bodyLine.positionCount = trail.Count;

        // trail is tail->head, so reverse to head->tail
        for (int i = 0; i < trail.Count; i++)
        {
            Vector2Int c = trail[trail.Count - 1 - i];
            bodyLine.SetPosition(i, new Vector3(c.x, c.y, lineZ));
        }
    }

    // =====================================================
    // EXIT SUCTION: SHIFT WHOLE LINE FORWARD (NO DIAGONAL)
    // =====================================================
    IEnumerator ExitSuctionRoutine_FollowPath()
    {
        exiting = true;
        running = false;
        blocked = false;
        moveDir = Vector2Int.zero;

        Vector3 hole = new Vector3(grid.exitPosition.x, grid.exitPosition.y, 0f);

        if (headTween != null && headTween.IsActive())
            headTween.Kill();

        // head into hole
        yield return transform.DOMove(hole, headToHoleDuration)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();

        if (headRenderer) headRenderer.enabled = false;

        if (!bodyLine || bodyLine.positionCount <= 0)
        {
            enabled = false;
            game.Win();
            yield break;
        }

        // pin head of line to hole
        bodyLine.SetPosition(0, new Vector3(hole.x, hole.y, lineZ));

        // shift entire body toward hole, one "cell step" at a time
        while (bodyLine.positionCount > 1)
        {
            int count = bodyLine.positionCount;

            // snapshot current positions
            Vector3[] from = new Vector3[count];
            for (int i = 0; i < count; i++)
                from[i] = bodyLine.GetPosition(i);

            // target: each point moves to previous point position
            Vector3[] to = new Vector3[count];
            to[0] = new Vector3(hole.x, hole.y, lineZ);
            for (int i = 1; i < count; i++)
                to[i] = from[i - 1];

            float t = 0f;
            while (t < suckStepDuration)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / suckStepDuration);

                for (int i = 0; i < count; i++)
                    bodyLine.SetPosition(i, Vector3.Lerp(from[i], to[i], a));

                yield return null;
            }

            // remove tail point
            bodyLine.positionCount = count - 1;

            // keep head pinned
            bodyLine.SetPosition(0, new Vector3(hole.x, hole.y, lineZ));
        }

        // hide line
        bodyLine.positionCount = 0;
        bodyLine.enabled = false;

        enabled = false;
        game.Win();
    }

    // =====================================================
    // FAIL
    // =====================================================
    void Fail()
    {
        running = false;
        blocked = false;

        if (headRenderer) headRenderer.enabled = false;

        if (bodyLine)
        {
            bodyLine.positionCount = 0;
            bodyLine.enabled = false;
        }

        game.Fail();
    }

    Vector3 CellToWorld(Vector2Int c) => new Vector3(c.x, c.y, 0f);
}