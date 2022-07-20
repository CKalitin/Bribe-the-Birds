using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {
    #region Variables

    [Header("Upgrades")]
    [Tooltip("This field always has to be set, the first upgrade is what's used to initialize the resourceEntries.")]
    [SerializeField] private StructureUpgrade[] upgrades;
    
    [Tooltip("This is only public so you can see the value, changing it won't do anything.\nbtw hey there future chris")]
    public int upgradeIndex = 0;

    [Header("Other")]
    [SerializeField] private Tile tile;
    public string upkeepCost;

    /*** Resources don't need to be public ***/
    private ResourceEntry[] resourceEntries;
    private List<ResourceModifier> appliedResourceModifiers = new List<ResourceModifier>(); // This is used to get which ResourceModifiers don't need to be updated again

    public ResourceEntry[] ResourceEntries { get => resourceEntries; set => resourceEntries = value; }

    #endregion

    #region Core

    private void Awake() {
        InitVars();

        InitializeResourceEntries();
        InstantiateCopiesOfResourceEntries();

        if (TileManagement.instance.SpawningComplete)
            GetAndApplyResourceModifiers();
    }

    private void Update() {
        if (resourceEntries.Length > 0) {
            upkeepCost = resourceEntries[0].Change.ToString();
        } else {
            upkeepCost = resourceEntries.Length.ToString();
        }
    }

    private void InitVars() {
        if (transform.parent.GetComponent<Tile>()) {
            tile = transform.parent.GetComponent<Tile>();
            transform.parent.GetComponent<Tile>().Structures.Add(this);
        }
    }

    private void OnDisable() {
        tile.Structures.Remove(this);
    }

    #endregion

    #region Public Resource Modifier Functions

    // Reapply resource modifiers on the tile of this structure
    public void UpdateResourceModifiers() {
        //ResetResourceEntries(); // Remove previous ResourceModifier changes
        //InstantiateCopiesOfResourceEntries(); // Create copies of the resource entries

        // Apply Resource Modifiers
        GetAndApplyResourceModifiers();
    }

    // This is public because it's used by TileManagement
    public void InitializeResourceEntries() {
        resourceEntries = upgrades[upgradeIndex].ResourceEntries;
    }

    #endregion

    #region Resource Modifiers

    // This function creates copies of the ResourceEntry Scriptable Objects so they don't affect the original ScriptableObject
    private void InstantiateCopiesOfResourceEntries() {
        // Create list for updated resource entries
        ResourceEntry[] _resourceEntries = new ResourceEntry[resourceEntries.Length];
        
        // Loop through resourceEntries
        for (int i = 0; i < resourceEntries.Length; i++) {
            ResourceEntry newResourceEntry = ScriptableObject.CreateInstance<ResourceEntry>(); // Create new resource entry

            // Set values of new resource  entry
            newResourceEntry.ResourceId = resourceEntries[i].ResourceId;
            newResourceEntry.ResourceEntryIds = resourceEntries[i].ResourceEntryIds;
            newResourceEntry.Change = resourceEntries[i].Change;
            newResourceEntry.ChangeOnTick = resourceEntries[i].ChangeOnTick;

            _resourceEntries[i] = newResourceEntry;
        }

        resourceEntries = _resourceEntries;
    }

    // Get resource modifiers on the tile of this structure and apply the modifier to the resource entries of this structure
    private void GetAndApplyResourceModifiers() {
        // Get resource Modifiers
        List<ResourceModifier> tileResourceModifiers = tile.ResourceModifiers;

        List<ResourceModifier> applyResourceModifiers = new List<ResourceModifier>(); // These are the resource modifiers that need to be added
        List<ResourceModifier> removeResourceModifiers = new List<ResourceModifier>(); // These are the resource modifiers that need to be removed

        // Loop through ResourceModifiers on the tile and see which need to be added to this structure
        for (int i = 0; i < tile.ResourceModifiers.Count; i++) {
            if (!appliedResourceModifiers.Contains(tile.ResourceModifiers[i])) {
                applyResourceModifiers.Add(tile.ResourceModifiers[i]);
                appliedResourceModifiers.Add(tile.ResourceModifiers[i]);
            }
        }

        // Loop through ResourceModifiers on this Structure and see which are not on the Tile and set them to be removed
        for (int i = 0; i < appliedResourceModifiers.Count; i++) {
            if (!tile.ResourceModifiers.Contains(appliedResourceModifiers[i])) {
                removeResourceModifiers.Add(appliedResourceModifiers[i]);
            }
        }

        int iters = 0; // DEBUG
        // Loop through resource entries
        for (int x = 0; x < resourceEntries.Length; x++) {
            // Apply resource modifiers
            for (int i = 0; i < applyResourceModifiers.Count; i++) {
                // If resourceId and resourceEntryId do not match, continue to next resourceEntry
                if (!CheckResourceIdMatch(applyResourceModifiers[i], resourceEntries[x])) continue;
                ApplyResourceModifier(applyResourceModifiers[i], resourceEntries[x]);
                iters++; // DEBUG
            }

            // Check to Remove resource modifiers from this resource entry
            for (int i = 0; i < removeResourceModifiers.Count; i++) {
                // If resourceId and resourceEntryId do not match, continue to next resourceEntry
                if (!CheckResourceIdMatch(removeResourceModifiers[i], resourceEntries[x])) continue;
                ApplyResourceModifier(removeResourceModifiers[i], resourceEntries[x]);
                appliedResourceModifiers.Remove(removeResourceModifiers[i]);
                iters++; // DEBUG
            }
        }

        if (PlayerPrefs.GetInt("bool", 0) < 0) Debug.Log("iters:" + iters); // DEBUG
        PlayerPrefs.SetInt("bool", PlayerPrefs.GetInt("bool", 0) + 1); // DEBUG
    }

    #endregion

    #region Resource Modifier Utils

    // Check if resourceIds and resourceEntryIds match between Resource Modifiers and ResourceEntries
    private bool CheckResourceIdMatch(ResourceModifier _rm, ResourceEntry _re) {
        // Check if the resourceId's don't match
        if (_rm.ResourceIdTarget != _re.ResourceId) return false;

        bool matchFound = false;
        // Check if resourceEntry has a matching resourceEntryId
        for (int i = 0; i < _re.ResourceEntryIds.Length; i++) {
            // Check if there is a match
            if (_re.ResourceEntryIds[i] == _rm.ResourceEntryIdTarget) matchFound = true;
        }

        return matchFound;
    }

    private void ApplyResourceModifier(ResourceModifier _rm, ResourceEntry _re) {
        float change = 0f;
        change += _rm.Change;
        if (GetDefaultResourceEntry(_re) != null)
            change += GetDefaultResourceEntry(_re).Change * _rm.PercentageChange; // Get default change value so multiple modifiers do not stack

        _re.Change += change;
        int value = PlayerPrefs.GetInt("iters") + 1; // DEBUG
        PlayerPrefs.SetInt("iters", value); // DEBUG
    }

    private void RemoveResourceModifier(ResourceModifier _rm, ResourceEntry _re) {
        float change = 0f;
        change -= _rm.Change;
        if (GetDefaultResourceEntry(_re) != null)
            change -= GetDefaultResourceEntry(_re).Change * _rm.PercentageChange; // Get default change value so multiple modifiers do not stack

        _re.Change += change;
        int value = PlayerPrefs.GetInt("iters") + 1; // DEBUG
        PlayerPrefs.SetInt("iters", value); // DEBUG
    }

    // Returns default value of resource entry (default from upgrade)
    private ResourceEntry GetDefaultResourceEntry(ResourceEntry _re) {
        for (int i = 0; i < resourceEntries.Length; i++) {
            if (resourceEntries[i].ResourceId != _re.ResourceId) continue;
            if (resourceEntries[i].ResourceEntryIds != _re.ResourceEntryIds) continue;
            return resourceEntries[i];
        }
        return null;
    }

    #endregion
}
