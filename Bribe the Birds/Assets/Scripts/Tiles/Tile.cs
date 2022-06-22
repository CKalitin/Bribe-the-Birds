using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    // These variables should only be used for initialization of tileInfo, Also, it's wise to not use 0
    [Header("User Specified")]
    [Tooltip("These variables should only be used for initialization of tileInfo")]
    [SerializeField] private Tiles tileId;
    [Tooltip("These variables should only be used for initialization of tileInfo")]
    [SerializeField] private GameObject tileObject;

    [Header("Tile")]
    [SerializeField] private TileRule[] tileRules;

    private TileInfo tileInfo;

    public Tiles TileId { get => tileId; set => tileId = value; }
    public GameObject TileObject { get => tileObject; set => tileObject = value; }
    public TileInfo TileInfo { get => tileInfo; set => tileInfo = value; }

    void Start() {
        
    }

    void Update() {
        
    }

    public void ApplyTileRules() {
        List<Vector2Int> adjacentTiles = TileManagement.instance.GetAdjacentTilesInRadius(tileInfo.Location, 1, true);

        // Loop through all tile Rules on this tile
        for (int i = 0; i < tileRules.Length; i++) {
            // Check if tile rule is true, input tiles adjacent to current tile
            if (tileRules[i].CheckTileRule(adjacentTiles)) {
                Destroy(tileInfo.TileObject); // Destroy old tile object

                tileInfo.TileObject = Instantiate(tileRules[i].UpdatedTilePrefab, TileManagement.instance.TileLocationToWorldPosition(TileInfo.Location, 0), Quaternion.identity, tileInfo.ParentObject.transform);
                // Spawn new tile object at same position and set parent to tile's parent object
            }
        }
    }
}
