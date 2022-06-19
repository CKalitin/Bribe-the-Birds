using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileManagement : MonoBehaviour {
    #region Variables

    [Header("Tiles")]
    private Dictionary<Vector2Int, TileStruct> tiles = new Dictionary<Vector2Int, TileStruct>();

    [SerializeField] private GameObject tilePrefab;

    private struct TileStruct {
        private Vector2Int location;
        private GameObject gameObject;

        public Vector2Int Location { get => location; set => location = value; }
        public GameObject GameObject { get => gameObject; set => gameObject = value; }

        public TileStruct(Vector2Int _location, GameObject _gameObject) {
            location = _location;
            gameObject = _gameObject;
        }
    }

    [Header("Specify")]
    [Tooltip("Tile Dimensions")]
    [SerializeField] private Vector2 tileDim = new Vector2(4.25f, 5f); // Tile dimensions

    #endregion

    #region Core

    void Start() {
        GenerateTiles();

        List<Vector2Int> tileLocs = GetAdjacentTilesInRadius(new Vector2Int(10, 8), 5);
        StartCoroutine(DestroyTiles(tileLocs));
    }

    void Update() {
        
    }

    private void GenerateTiles() {
        for (int x = 0; x < 21; x++) {
            for (int y = 0; y < 21; y++) {
                Vector2Int tileLoc = SpawnTile(tilePrefab, new Vector2Int(x, y));
                // Update location text on spawned tile
                tiles[tileLoc].GameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({x}, {y})";
            }
        }
    }

    private IEnumerator DestroyTiles(List<Vector2Int> tileLocs) {
        //float waitTime = 0.05f;d

        //yield return new WaitForSeconds(2);

        for (int i = 0; i < tileLocs.Count; i++) {
            //yield return new WaitForSeconds(waitTime);
            DeleteTile(tileLocs[i]);
        }

        yield return new WaitForSeconds(0.1f);
    }

    #endregion

    #region Tiles

    private Vector2Int SpawnTile(GameObject _tilePrefab, Vector2Int _location) {
        if (tiles.ContainsKey(_location)) { return new Vector2Int(-999999999, -999999999); }

        GameObject tileObject = Instantiate(_tilePrefab, TileLocationToWorldPosition(_location, 0), Quaternion.identity);
        tiles.Add(_location, new TileStruct(_location, tileObject));

        return _location;
    }

    private void DeleteTile(Vector2Int _location) {
        // Check if tile exists at location
        if (!tiles.ContainsKey(_location)) {
            Debug.LogWarning("No tile to destroy at location: " + _location);
            return;
        }

        Destroy(tiles[_location].GameObject); // Delete tile gameObject
        tiles.Remove(_location); // Remove tileStruct from tile structs list (memory leak???, do i need to destroy the struct???, I hope c# handles it)
    }

    #endregion

    #region Utils

    // Returns all Tile Locations around Center Tile in Radius
    // Does not Return Center Tile Location, does not Return Tile Locations without a Tile
    private List<Vector2Int> GetAdjacentTilesInRadius(Vector2Int _loc, int _r) {
        // _r = radius

        List<Vector2Int> output = new List<Vector2Int>();

        int max = _r * 2 + 1;

        for (int y = 0; y < _r * 2 + 1; y++) {
            int offset = Mathf.RoundToInt((Mathf.Abs(y - _r) - 0.1f) / 2); // Results eg: (0, 1) = 0 offset, (2, 3) = 1 offset, (4, 5) = 2 offset, ...
            offset += Mathf.Abs((_loc.y) % 2) * Mathf.Abs((y - _r) % 2); // If the locaction is odd and the distance to the center is odd, subtract 1, this fixes a bug
            
            int rowLength = offset + max - Mathf.Abs(y - _r); // Add the maximum value - the distance to the center to the offset (to the offset so the for loop works)
            
            for (int x = offset ; x < rowLength; x++) {
                Vector2Int loc = new Vector2Int(x - _r + _loc.x, y - _r + _loc.y); // Convert from for-loop location to world location

                if (loc.x == _loc.x && loc.y == _loc.y) { continue; } // If the current iteration is the center tile then skip it
                if (!tiles.ContainsKey(loc)) { continue; } // If there is no tile at the location skip it

                output.Add(loc); // Add location to output
            }
        }

        return output;
    }

    // Convert tile location to position in world units
    private Vector3 TileLocationToWorldPosition(Vector2Int _loc, float _y) {
        return new Vector3(_loc.x * tileDim.x + (tileDim.x / 2 * (_loc.y % 2)), _y, _loc.y * (tileDim.y * 0.75f));

        /*** How it's made: ***/
        // _loc = location
        // _loc.x * tileDim.x seperates the tiles on the x axis
        // + (tiledim.x / 2) Adds half of the length of the tile to the x axis for "in-between" tiles that aren't on the exact x
        // (_loc.y % 2) sees if y is divisible by 2, if it is use the line above ^^^, if not multiply by 0
        // The tiles are seperated on the y axis by 0.75, every other tile (eg. multiples of 2) are 1.5 units apart, the "in-between" tiles are only 0.75 units up

        // Look at coordinate system section here for more info and examples: https://www.redblobgames.com/grids/hexagons/
    }

    #endregion
}
