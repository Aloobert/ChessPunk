using UnityEngine;
using System.Collections.Generic;

public class RookMovement : HeroMovement
{
    public override MovementTileData GetMovementTileData()
    {
        // Initialize the MovementTileData struct
        MovementTileData mt = new MovementTileData
        {
            valid = new List<Vector3>(),
            invalid = new List<Vector3>(),
            ability = new List<Vector3>(),
            attackBehindTile = new Dictionary<Vector3, Vector3>()
        };

        // 1) Single-tile fallback => green
        AddSingleTileMoves(ref mt);

        // 2) If not on cooldown, do multi-tile expansions => blue
        if (CanUseMovement())
        {
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm)
            {
                Node currentNode = gm.GetNodeFromPosition(transform.position);
                int startX = gm.GetIndexX(currentNode.Position);
                int startY = gm.GetIndexY(currentNode.Position);

                // Up
                ExpandLine(startX, startY, 0, 1, gm, ref mt);
                // Down
                ExpandLine(startX, startY, 0, -1, gm, ref mt);
                // Left
                ExpandLine(startX, startY, -1, 0, gm, ref mt);
                // Right
                ExpandLine(startX, startY, 1, 0, gm, ref mt);
            }
        }

        return mt;
    }

    /// <summary>
    /// Adds single-tile moves as Green fallback. If occupant is Boss, we put it in Blue + partial approach.
    /// </summary>
    private void AddSingleTileMoves(ref MovementTileData mt)
    {
        Vector3 pos = transform.position;
        Vector3[] singleDirs = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        foreach (var dir in singleDirs)
        {
            Vector3 tilePos = pos + dir;
            if (IsBlocked(tilePos))
            {
                mt.invalid.Add(tilePos);
            }
            else
            {
                // Check occupant
                Collider2D occupant = Physics2D.OverlapPoint(tilePos);
                if (occupant != null && occupant.CompareTag("Boss"))
                {
                    // Mark as Blue => partial approach
                    mt.ability.Add(tilePos);
                    // behind tile => current pos
                    mt.attackBehindTile[tilePos] = pos;
                }
                else
                {
                    // Otherwise, normal fallback => green
                    mt.valid.Add(tilePos);
                }
            }
        }
    }

    /// <summary>
    /// Expands in a straight line (multi-tile). If occupant is Boss, add that tile to Blue + partial approach, then stop expansions.
    /// </summary>
    private void ExpandLine(int startX, int startY, int dx, int dy, GridManager gm, ref MovementTileData mt)
    {
        int x = startX;
        int y = startY;

        while (true)
        {
            int prevX = x;
            int prevY = y;

            x += dx;
            y += dy;

            // Out of bounds check
            if (x < 0 || x >= gm.GetGridSizeX() || y < 0 || y >= gm.GetGridSizeY())
                break;

            Node node = gm.GetNode(x, y);
            if (!node.Walkable)
            {
                // Mark as red if a pillar or invisible boundary
                mt.invalid.Add(node.Position);
                break;
            }

            // occupant check
            Collider2D occupant = Physics2D.OverlapPoint(node.Position);
            if (occupant != null && occupant.CompareTag("Boss"))
            {
                // Mark occupant tile as blue => partial approach
                mt.ability.Add(node.Position);

                // The tile behind occupant is the node we came from => prevX, prevY
                Node behindNode = gm.GetNode(prevX, prevY);
                mt.attackBehindTile[node.Position] = behindNode.Position;

                // Stop expansions
                break;
            }

            // normal tile => add to ability => blue
            mt.ability.Add(node.Position);
        }
    }

    /// <summary>
    /// Checks if the tile is blocked by a pillar or out-of-bounds
    /// </summary>
    private bool IsBlocked(Vector3 tilePos)
    {
        // occupant
        Collider2D hit = Physics2D.OverlapPoint(tilePos);
        if (!hit)
        {
            // no occupant => check node
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Node node = gm.GetNodeFromPosition(tilePos);
                if (!node.Walkable) return true;
            }
            return false;
        }
        if (hit.CompareTag("Pillar")) return true;
        return false;
    }

    public override void PerformMovement() { }
}
