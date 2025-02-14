using UnityEngine;
using System.Collections.Generic;

public class BishopMovement : HeroMovement
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

        // single-tile fallback => green occupant => if occupant is boss => set ability
        AddSingleTileMoves(ref mt);

        // diagonal expansions => if not on cooldown
        if (CanUseMovement())
        {
            AddDiagonals(ref mt);
        }

        return mt;
    }

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

    private void AddDiagonals(ref MovementTileData mt)
    {
        GridManager gm = FindObjectOfType<GridManager>();
        if (!gm) return;

        Node node = gm.GetNodeFromPosition(transform.position);
        int sx = gm.GetIndexX(node.Position);
        int sy = gm.GetIndexY(node.Position);

        ExpandLine(sx, sy, 1, 1, gm, ref mt);
        ExpandLine(sx, sy, -1, 1, gm, ref mt);
        ExpandLine(sx, sy, 1, -1, gm, ref mt);
        ExpandLine(sx, sy, -1, -1, gm, ref mt);
    }

    private void ExpandLine(int x, int y, int dx, int dy, GridManager gm, ref MovementTileData mt)
    {
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
            mt.ability.Add(node.Position);
        }
    }

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
