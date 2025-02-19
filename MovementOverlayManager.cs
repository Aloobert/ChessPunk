using UnityEngine;
using System.Collections.Generic;

public class MovementOverlayManager : MonoBehaviour
{
    public GameObject greenBorderPrefab;
    public GameObject redBorderPrefab;
    public GameObject blueBorderPrefab;

    private List<GameObject> spawnedOverlays = new List<GameObject>();

    public void ShowOverlays(List<Vector3> validTiles, List<Vector3> invalidTiles, List<Vector3> abilityTiles)
    {
        // Clear old overlays each time
        ClearOverlays();

        // Spawn green
        foreach (var tile in validTiles)
        {
            GameObject overlay = Instantiate(greenBorderPrefab, tile, Quaternion.identity);
            TileOverlay to = overlay.GetComponent<TileOverlay>();
            to.Init(this, tile, TileOverlayType.Valid);
            spawnedOverlays.Add(overlay);
        }

        // Spawn red
        foreach (var tile in invalidTiles)
        {
            GameObject overlay = Instantiate(redBorderPrefab, tile, Quaternion.identity);
            TileOverlay to = overlay.GetComponent<TileOverlay>();
            to.Init(this, tile, TileOverlayType.Invalid);
            spawnedOverlays.Add(overlay);
        }

        // Spawn blue
        foreach (var tile in abilityTiles)
        {
            GameObject overlay = Instantiate(blueBorderPrefab, tile, Quaternion.identity);
            TileOverlay to = overlay.GetComponent<TileOverlay>();
            to.Init(this, tile, TileOverlayType.Ability);
            spawnedOverlays.Add(overlay);
        }
    }

    public void OnTileClicked(TileOverlayType type, Vector3 tilePos)
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.OnTileOverlayClicked(type, tilePos);
        }
        ClearOverlays();
    }

    public void ClearOverlays()
    {
        foreach (var overlay in spawnedOverlays)
        {
            Destroy(overlay);
        }
        spawnedOverlays.Clear();
    }
}
