using UnityEngine;
using System.Collections.Generic;

public class QueenMovement : HeroMovement
{
    public override MovementTileData GetMovementTileData()
    {
        MovementTileData mt = new MovementTileData
        {
            valid = new List<Vector3>(),
            invalid = new List<Vector3>(),
            ability = new List<Vector3>(),
            attackBehindTile = new Dictionary<Vector3, Vector3>()
        };

        // 1) Single-tile fallback => green
        AddSingleTileMoves(ref mt);

        // 2) If not on cooldown => multi-tile expansions => blue
        if (CanUseMovement())
        {
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm)
            {
                Node currentNode = gm.GetNodeFromPosition(transform.position);
                int startX = gm.GetIndexX(currentNode.Position);
                int startY = gm.GetIndexY(currentNode.Position);

                // up
                ExpandLine(startX, startY, 0, 1, gm, ref mt);
                // down
                ExpandLine(startX, startY, 0, -1, gm, ref mt);
                // left
                ExpandLine(startX, startY, -1, 0, gm, ref mt);
                // right
                ExpandLine(startX, startY, 1, 0, gm, ref mt);
                // diagonals
                ExpandLine(startX, startY, 1, 1, gm, ref mt);
                ExpandLine(startX, startY, -1, 1, gm, ref mt);
                ExpandLine(startX, startY, 1, -1, gm, ref mt);
                ExpandLine(startX, startY, -1, -1, gm, ref mt);
            }
        }

        return mt;
    }

    /// <summary>
    /// Single-tile fallback => if occupant is Boss => treat as blue
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
                Collider2D occupant = Physics2D.OverlapPoint(tilePos);
                if (occupant != null && occupant.CompareTag("Boss"))
                {
                    // occupant => put in ability + partial approach
                    mt.ability.Add(tilePos);
                    mt.attackBehindTile[tilePos] = pos;
                }
                else
                {
                    mt.valid.Add(tilePos);
                }
            }
        }
    }

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

            if (x < 0 || x >= gm.GetGridSizeX() || y < 0 || y >= gm.GetGridSizeY())
                break;

            Node node = gm.GetNode(x, y);
            if (!node.Walkable)
            {
                mt.invalid.Add(node.Position);
                break;
            }

            Collider2D occupant = Physics2D.OverlapPoint(node.Position);
            if (occupant != null && occupant.CompareTag("Boss"))
            {
                mt.ability.Add(node.Position);
                Node behindNode = gm.GetNode(prevX, prevY);
                mt.attackBehindTile[node.Position] = behindNode.Position;
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
        Collider2D hit = Physics2D.OverlapPoint(tilePos);
        if (!hit)
        {
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
