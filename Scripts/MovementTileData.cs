using System.Collections.Generic;
using UnityEngine;

public struct MovementTileData
{
    public List<Vector3> valid;     // Green fallback
    public List<Vector3> invalid;   // Red blocked
    public List<Vector3> ability;   // Blue special
    public Dictionary<Vector3, Vector3> attackBehindTile;
}
