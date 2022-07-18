using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceModifierApplier : MonoBehaviour {
    [Header("Resource Modidifer Applier")]
    [SerializeField] private ResourceModifier[] resourceModifiers;
    [SerializeField] private int effectRadius;

    [Header("Other")]
    [SerializeField] Tile tile;

    public void ApplyResourceModifiers() {
        // Get all adjacent tiles 
        List<Vector2Int> locs = TileManagement.instance.GetAdjacentTilesInRadius(tile.TileInfo.Location, effectRadius);

        // Loop through adjacent tiles
        for (int i = 0; i < locs.Count; i++) {
            Tile targetTile = TileManagement.instance.GetTileAtLocation(locs[i]).Tile;

            targetTile.ResourceModifiers = resourceModifiers;
        }
    }
}
