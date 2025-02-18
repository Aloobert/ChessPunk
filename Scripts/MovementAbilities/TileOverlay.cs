using UnityEngine;

public enum TileOverlayType
{
    Valid,
    Invalid,
    Ability
}

public class TileOverlay : MonoBehaviour
{
    private MovementOverlayManager overlayManager;
    private Vector3 tilePosition;
    private TileOverlayType overlayType;

    public void Init(MovementOverlayManager manager, Vector3 position, TileOverlayType type)
    {
        overlayManager = manager;
        tilePosition = position;
        overlayType = type;
    }

    void OnMouseDown()
    {
        // When the user clicks this overlay
        overlayManager.OnTileClicked(overlayType, tilePosition);
    }
}
