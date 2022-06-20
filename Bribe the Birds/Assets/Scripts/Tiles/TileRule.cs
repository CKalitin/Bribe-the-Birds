using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Tile Rule", menuName ="Tile Rule")]
public class TileRule : ScriptableObject {
    [SerializeField] private string intendedTileId;
    [Space]
    [SerializeField] private GameObject updatedTilePrefab;

    [Tooltip("This is the list of tiles needed to use the updatedTilePrefab.\n" +
        "They should be in order from the bottom y first, then moving left, like the GetAdjacentTiles function.\n" +
        "Leave Blank for no tile required.")]
<<<<<<< HEAD
    [SerializeField] private string[] requiredAdjacentTileIds = new string[6];

    public string IntendedTileId { get => intendedTileId; set => intendedTileId = value; }
    public GameObject UpdatedTilePrefab { get => updatedTilePrefab; set => updatedTilePrefab = value; }
    public string[] RequiredAdjacentTiles { get => requiredAdjacentTileIds; set => requiredAdjacentTileIds = value; }
=======
    [SerializeField] private string[] requiredAdjacentTileIDs;

    public string IntendedTileId { get => intendedTileId; set => intendedTileId = value; }
    public GameObject UpdatedTilePrefab { get => updatedTilePrefab; set => updatedTilePrefab = value; }
    public string[] AdjacentTiles { get => requiredAdjacentTileIDs; set => requiredAdjacentTileIDs = value; }
>>>>>>> f2c421a52b3442ebf13bbd1081dcfb4607bc4744

    // Return true is updatedTilePrefab should be used
    public bool CheckTileRule(List<Vector2Int> _adjacentTileLocs) {
        // Output is false by default so find a tile that is correct and return true
        for (int i = 0; i < _adjacentTileLocs.Count; i++) {
<<<<<<< HEAD
            if (requiredAdjacentTileIds[i] == "") continue; // If there is no TileRule for specified tile
            if (!TileManagement.instance.GetTile(_adjacentTileLocs[i]).Spawned) continue; // If the tile is not spawned (there is no tile at the location)
            if (TileManagement.instance.GetTile(_adjacentTileLocs[i]).TileId != requiredAdjacentTileIds[i]) continue; // If tile ID is correct
=======
            if (requiredAdjacentTileIDs[i] == "") continue; // If there is no TileRule for specified tile
            if (!TileManagement.instance.GetTile(_adjacentTileLocs[i]).Spawned) continue; // If the tile is not spawned (there is no tile at the location)
            if (TileManagement.instance.GetTile(_adjacentTileLocs[i]).TileId != requiredAdjacentTileIDs[i]) continue; // If tile ID is correct
>>>>>>> f2c421a52b3442ebf13bbd1081dcfb4607bc4744

            return true;
        }

        return false;
    }
}
