using UnityEngine;
using System.Collections.Generic;


public abstract class HeroMovement : MonoBehaviour
{
    public float moveTime = 0.2f;
    public float rotationSpeed = 200f;
    public int movementCooldown = 0; // 5-turn cooldown if used

    protected bool isMoving = false;
    protected PlayerController player;

    protected virtual void Start()
    {
        player = GetComponent<PlayerController>();
        if (!player)
        {
            Debug.LogError("PlayerController not found on the same object as HeroMovement.");
        }
    }

    public abstract MovementTileData GetMovementTileData();

    public virtual void PerformMovement() { /* immediate input approach if desired */ }

    public bool CanUseMovement() => (movementCooldown == 0);

    public void ResetCooldown() => movementCooldown = 5;
}
