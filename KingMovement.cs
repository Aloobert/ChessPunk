using UnityEngine;
using System.Collections.Generic;

public class KingMovement : HeroMovement
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

        // Single-tile cardinal => green, except if occupant is Boss => treat as ability
        AddSingleTileCardinals(ref mt);

        // If on cooldown => skip diagonal
        if (CanUseMovement())
        {
            AddDiagonalAbility(ref mt);
        }

        return mt;
    }

    private void AddSingleTileCardinals(ref MovementTileData mt)
    {
        Vector3 pos = transform.position;
        Vector3[] cardinals = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        foreach (var c in cardinals)
        {
            Vector3 tilePos = pos + c;
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
                    // Mark as ability + partial approach
                    mt.ability.Add(tilePos);
                    mt.attackBehindTile[tilePos] = pos; // stand on current pos
                }
                else
                {
                    mt.valid.Add(tilePos);
                }
            }
        }
    }

    private void AddDiagonalAbility(ref MovementTileData mt)
    {
        Vector3 pos = transform.position;
        Vector3[] diags = { new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(-1, -1, 0) };
        foreach (var d in diags)
        {
            Vector3 tilePos = pos + d;
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
