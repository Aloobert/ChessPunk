using UnityEngine;
using System.Collections.Generic;

public class PawnMovement : HeroMovement
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

        // single-tile => green
        AddSingleTileMoves(ref mt);

        // 2-tile => blue if not on cooldown
        if (CanUseMovement())
        {
            AddTwoTileJumps(ref mt);
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
                // occupant check
                Collider2D occupant = Physics2D.OverlapPoint(tilePos);
                if (occupant != null && occupant.CompareTag("Boss"))
                {
                    mt.ability.Add(tilePos);
                    // behind tile => pos
                    mt.attackBehindTile[tilePos] = pos;
                }
                else
                {
                    mt.valid.Add(tilePos);
                }
            }
        }
    }

    // 2-tile jumps
    private void AddTwoTileJumps(ref MovementTileData mt)
    {
        Vector3 pos = transform.position;
        // up,down,left,right * 2
        Vector3[] doubles = { Vector3.up * 2, Vector3.down * 2, Vector3.left * 2, Vector3.right * 2 };

        foreach (var off in doubles)
        {
            // Check the middle tile if you do NOT want to jump over pillars
            Vector3 mid = pos + (off / 2); // e.g. if off= (0,2)
            if (IsBlocked(mid))
            {
                // If the middle tile is blocked, we can't jump
                Vector3 tilePos = pos + off;
                mt.invalid.Add(tilePos);
                continue;
            }

            Vector3 tilePos2 = pos + off;
            if (IsBlocked(tilePos2))
            {
                mt.invalid.Add(tilePos2);
            }
            else
            {
                Collider2D occupant = Physics2D.OverlapPoint(tilePos2);
                if (occupant != null && occupant.CompareTag("Boss"))
                {
                    mt.ability.Add(tilePos2);
                    // behind tile => the middle tile or the original pos?
                    // If you want the behind tile to be the (mid) tile, do:
                    Node behindNode = FindObjectOfType<GridManager>().GetNodeFromPosition(mid);
                    mt.attackBehindTile[tilePos2] = behindNode.Position;
                }
                else
                {
                    mt.ability.Add(tilePos2);
                }
            }
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
