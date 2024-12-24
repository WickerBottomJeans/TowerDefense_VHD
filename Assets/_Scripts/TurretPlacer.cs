using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacer : MonoBehaviour {
    public GameObject turretPrefab;
    public GameObject ghostTurretPrefab;
    public Tilemap placementTilemap; // New Tilemap for turret placement zones
    public Camera mainCamera;

    private bool isPlacing = false;
    private GameObject currentGhost;

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            isPlacing = !isPlacing;
            if (!isPlacing && currentGhost != null) {
                Destroy(currentGhost);
            }
        }

        if (isPlacing) {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePosition = placementTilemap.WorldToCell(mouseWorldPosition);

            ShowGhostTurret(tilePosition);

            if (Input.GetMouseButtonDown(0)) {
                PlaceTurret(tilePosition);
            }
        }
    }

    void ShowGhostTurret(Vector3Int tilePosition) {
        if (currentGhost != null) {
            Destroy(currentGhost);
        }

        if (IsTileValidForPlacement(tilePosition)) {
            currentGhost = Instantiate(ghostTurretPrefab, placementTilemap.CellToWorld(tilePosition), Quaternion.identity);
        }
    }

    bool IsTileValidForPlacement(Vector3Int tilePosition) {
        // Check if the tile exists in the placement Tilemap
        return placementTilemap.HasTile(tilePosition);
    }

    void PlaceTurret(Vector3Int tilePosition) {
        if (IsTileValidForPlacement(tilePosition)) {
            Vector3 worldPosition = placementTilemap.CellToWorld(tilePosition);
            Instantiate(turretPrefab, worldPosition, Quaternion.identity);
            placementTilemap.SetTile(tilePosition, null); // Optional: Clear the placement tile if turrets can't stack
        }
    }
}
