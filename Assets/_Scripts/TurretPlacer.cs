using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class TurretPlacer : MonoBehaviour {
    public Button DefButton;
    public Button AtkButton;
    public Button FunButton;

    public TurretStatsSO attackTurretStats;
    public TurretStatsSO defenseTurretStats;
    public TurretStatsSO funTurretStats;

    public Tilemap placementTilemap;
    public Camera mainCamera;

    private TilemapRenderer placementTilemapRenderer; // Reference to TilemapRenderer
    private TurretStatsSO currentTurretStats;
    private GameObject currentGhost;
    private bool isPlacing = false;

    private void Start() {
        // Default to Attack Turret
        currentTurretStats = attackTurretStats;

        DefButton.GetComponentInChildren<TMP_Text>().text = $"{defenseTurretStats.turretCost}";
        AtkButton.GetComponentInChildren<TMP_Text>().text = $"{attackTurretStats.turretCost}";
        FunButton.GetComponentInChildren<TMP_Text>().text = $"{funTurretStats.turretCost}";

        DefButton.onClick.AddListener(() => TogglePlacingMode(defenseTurretStats));
        AtkButton.onClick.AddListener(() => TogglePlacingMode(attackTurretStats));
        FunButton.onClick.AddListener(() => TogglePlacingMode(funTurretStats));

        placementTilemapRenderer = placementTilemap.GetComponent<TilemapRenderer>(); // Get the TilemapRenderer
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            TogglePlacingMode(attackTurretStats);
        } else if (Input.GetKeyDown(KeyCode.X)) {
            TogglePlacingMode(defenseTurretStats);
        } else if (Input.GetKeyDown(KeyCode.C)) {
            TogglePlacingMode(funTurretStats);
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

    private void TogglePlacingMode(TurretStatsSO turretStats) {
        if (currentTurretStats == turretStats && isPlacing) {
            isPlacing = false;
            DestroyGhost();
            SetTilemapRenderOrder(-6); // Reset render order when placing mode is off
        } else {
            currentTurretStats = turretStats;
            isPlacing = true;
            SetTilemapRenderOrder(10); // Increase render order during placing mode
        }
    }

    private void SetTilemapRenderOrder(int order) {
        if (placementTilemapRenderer != null) {
            placementTilemapRenderer.sortingOrder = order;
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

            // Adjust position to bottom-center of the tile
            Vector3 adjustedPosition = placementTilemap.CellToWorld(tilePosition) + new Vector3(placementTilemap.cellSize.x / 2, 0, 0);
            currentGhost.transform.position = adjustedPosition;
        }
    }

    private bool IsTileValidForPlacement(Vector3Int tilePosition) {
        return placementTilemap.HasTile(tilePosition);
    }

    private void PlaceTurret(Vector3Int tilePosition) {
        if (IsTileValidForPlacement(tilePosition)) {
            CoinManager coinManager = CoinManager.Instance;
            if (coinManager != null && coinManager.TrySpendCoins((int)currentTurretStats.turretCost)) {
                // Adjust position to bottom-center of the tile
                Vector3 worldPosition = placementTilemap.CellToWorld(tilePosition) + new Vector3(placementTilemap.cellSize.x / 2, 0, 0);
                Instantiate(currentTurretStats.turretPrefab.gameObject, worldPosition, Quaternion.identity);
                placementTilemap.SetTile(tilePosition, null);
                isPlacing = false;
                DestroyGhost();
                SetTilemapRenderOrder(-6); // Reset render order after placing
            } else {
                Debug.Log("Not enough coins to place turret!");
            }
        }
    }

    private void DestroyGhost() {
        if (currentGhost != null) {
            Destroy(currentGhost);
        }
    }
}