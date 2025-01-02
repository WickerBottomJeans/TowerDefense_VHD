using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacer : MonoBehaviour {
    public GameObject attackTurretPrefab;
    public GameObject defenseTurretPrefab;
    public GameObject funTurretPrefab;

    // Ghost prefabs for each turret type
    public GameObject ghostAttackTurretPrefab;

    public GameObject ghostDefenseTurretPrefab;
    public GameObject ghostFunTurretPrefab;

    public Tilemap placementTilemap;
    public Camera mainCamera;

    private GameObject currentTurretPrefab;
    private GameObject currentGhost;
    private GameObject currentGhostPrefab;
    private bool isPlacing = false;

    private void Start() {
        // Default to Attack Turret
        currentTurretPrefab = attackTurretPrefab;
        currentGhostPrefab = ghostAttackTurretPrefab;
    }

    private void Update() {
        // Toggle placing mode for each turret with Z, X, C keys
        if (Input.GetKeyDown(KeyCode.Z)) {
            TogglePlacingMode(attackTurretPrefab, ghostAttackTurretPrefab);
        } else if (Input.GetKeyDown(KeyCode.X)) {
            TogglePlacingMode(defenseTurretPrefab, ghostDefenseTurretPrefab);
        } else if (Input.GetKeyDown(KeyCode.C)) {
            TogglePlacingMode(funTurretPrefab, ghostFunTurretPrefab);
        }

        // If placing mode is active, allow turret placement
        if (isPlacing) {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePosition = placementTilemap.WorldToCell(mouseWorldPosition);

            ShowGhostTurret(tilePosition);

            // Place the turret when left-clicking
            if (Input.GetMouseButtonDown(0)) {
                PlaceTurret(tilePosition);
            }
        }
    }

    // Toggle placing mode for the given turret prefab and ghost
    private void TogglePlacingMode(GameObject turretPrefab, GameObject ghostPrefab) {
        if (currentTurretPrefab == turretPrefab && isPlacing) {
            // If already in placing mode for this turret, disable placing mode
            isPlacing = false;
            if (currentGhost != null) {
                Destroy(currentGhost);
            }
        } else {
            // If switching turret or toggling on placing mode for this turret
            currentTurretPrefab = turretPrefab;
            currentGhostPrefab = ghostPrefab;
            isPlacing = true;
        }
    }

    // Show a ghost turret preview
    private void ShowGhostTurret(Vector3Int tilePosition) {
        if (currentGhost != null) {
            Destroy(currentGhost);
        }

        if (IsTileValidForPlacement(tilePosition)) {
            currentGhost = Instantiate(currentGhostPrefab, placementTilemap.CellToWorld(tilePosition), Quaternion.identity);
        }
    }

    // Check if the tile is valid for placement
    private bool IsTileValidForPlacement(Vector3Int tilePosition) {
        // Check if the tile exists and is not blocked
        return placementTilemap.HasTile(tilePosition);
    }

    // Place the selected turret on the tile
    private void PlaceTurret(Vector3Int tilePosition) {
        if (IsTileValidForPlacement(tilePosition)) {
            Vector3 worldPosition = placementTilemap.CellToWorld(tilePosition);
            Instantiate(currentTurretPrefab, worldPosition, Quaternion.identity);
            placementTilemap.SetTile(tilePosition, null); // Optionally clear the tile after placement

            // Turn off placing mode after successful placement
            isPlacing = false;

            // Remove the ghost turret
            if (currentGhost != null) {
                Destroy(currentGhost);
            }
        }
    }
}