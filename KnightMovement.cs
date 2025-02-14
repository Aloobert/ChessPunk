using UnityEngine;
using System.Collections.Generic;

public class KnightMovement : HeroMovement
{
    private Vector3[] knightMoves =
    {
        new Vector3(2,1,0),
        new Vector3(2,-1,0),
        new Vector3(-2,1,0),
        new Vector3(-2,-1,0),
        new Vector3(1,2,0),
        new Vector3(1,-2,0),
        new Vector3(-1,2,0),
        new Vector3(-1,-2,0),
    };

    public override MovementTileData GetMovementTileData()
    {
        MovementTileData mt = new MovementTileData
        {
            valid = new List<Vector3>(),
            invalid = new List<Vector3>(),
            ability = new List<Vector3>(),
            attackBehindTile = new Dictionary<Vector3, Vector3>()
        };

        // Single-tile fallback => green
        AddSingleTileMoves(ref mt);

        if (CanUseMovement())
        {
            // L-shaped => ability
            AddKnightMoves(ref mt);
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
                    mt.attackBehindTile[tilePos] = pos;
                }
                else
                {
                    mt.valid.Add(tilePos);
                }
            }
        }
    }

    private void AddKnightMoves(ref MovementTileData mt)
    {
        Vector3 pos = transform.position;
        foreach (var move in knightMoves)
        {
            Vector3 tilePos = pos + move;
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
                    // behind tile => for Knight, typically pos is the behind tile
                    mt.attackBehindTile[tilePos] = pos;
                }
                else
                {
                    mt.ability.Add(tilePos);
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
