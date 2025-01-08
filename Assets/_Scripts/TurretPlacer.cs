using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacer : MonoBehaviour {
    public TurretStatsSO attackTurretStats;
    public TurretStatsSO defenseTurretStats;
    public TurretStatsSO funTurretStats;

    public Tilemap placementTilemap;
    public Camera mainCamera;

    private TurretStatsSO currentTurretStats;
    private GameObject currentGhost;
    private bool isPlacing = false;

    private void Start() {
        // Default to Attack Turret
        currentTurretStats = attackTurretStats;
    }

    private void Update() {
        // Toggle placing mode for each turret with Z, X, C keys
        if (Input.GetKeyDown(KeyCode.Z)) {
            TogglePlacingMode(attackTurretStats);
        } else if (Input.GetKeyDown(KeyCode.X)) {
            TogglePlacingMode(defenseTurretStats);
        } else if (Input.GetKeyDown(KeyCode.C)) {
            TogglePlacingMode(funTurretStats);
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

    // Toggle placing mode for the given turret stats
    private void TogglePlacingMode(TurretStatsSO turretStats) {
        if (currentTurretStats == turretStats && isPlacing) {
            isPlacing = false;
            DestroyGhost();
        } else {
            currentTurretStats = turretStats;
            isPlacing = true;
        }
    }

    private void ShowGhostTurret(Vector3Int tilePosition) {
        if (currentGhost != null) {
            Destroy(currentGhost);
        }

        if (IsTileValidForPlacement(tilePosition)) {
            currentGhost = new GameObject("GhostTurret");
            SpriteRenderer spriteRenderer = currentGhost.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = currentTurretStats.ghostSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            currentGhost.transform.position = placementTilemap.CellToWorld(tilePosition);
        }
    }

    private bool IsTileValidForPlacement(Vector3Int tilePosition) {
        return placementTilemap.HasTile(tilePosition);
    }

    private void PlaceTurret(Vector3Int tilePosition) {
        if (IsTileValidForPlacement(tilePosition)) {
            // Access the singleton CoinManager instance
            CoinManager coinManager = CoinManager.Instance;
            if (coinManager != null && coinManager.TrySpendCoins((int)currentTurretStats.turretCost)) {
                Vector3 worldPosition = placementTilemap.CellToWorld(tilePosition);
                Instantiate(currentTurretStats.turretPrefab.gameObject, worldPosition, Quaternion.identity);
                placementTilemap.SetTile(tilePosition, null);
                isPlacing = false;
                DestroyGhost();
            } else {
                Debug.Log("Not enough coins to place turret!");
            }
        }
    }

    // Destroy the ghost turret if it exists
    private void DestroyGhost() {
        if (currentGhost != null) {
            Destroy(currentGhost);
        }
    }
}