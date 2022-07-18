using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {
    [Header("Structure")]
    [Tooltip("This field always has to be set, the first upgrade is what's used to initialize the resourceEntries.")]
    [SerializeField] private StructureUpgrade[] upgrades;

    [Header("Other")]
    [SerializeField] private Tile tile;

    private ResourceEntry[] resourceEntries;

    private int upgradeIndex;

    public ResourceEntry[] ResourceEntries { get => resourceEntries; set => resourceEntries = value; }

    private void Awake() {
        ResetResourceEntries();

        SetupResourceEntries();

        GetAndApplyResourceModifiers();
    }

    #region Resource Modifiers

    // Reapply resource modifiers on the tile of this structure
    public void UpdateResourceModifiers() {
        // Remove previous ResourceModifier changes
        ResetResourceEntries();

        // Create copies of the resource entries
        SetupResourceEntries();

        // Apply Resource Modifiers
        GetAndApplyResourceModifiers();
    }

    // This function creates copies of the ResourceEntry Scriptable Objects so they don't affect the original ScriptableObject
    private void SetupResourceEntries() {
        // Create list for updated resource entries
        List<ResourceEntry> _resourceEntries = new List<ResourceEntry>();
        
        // Loop through resourceEntries
        for (int i = 0; i < resourceEntries.Length; i++) {
            ResourceEntry newResourceEntry = new ResourceEntry(); // Create new resource entry

            // Set values of new resource  entry
            newResourceEntry.ResourceId = resourceEntries[i].ResourceId;
            newResourceEntry.ResourceEntryIds = resourceEntries[i].ResourceEntryIds;
            newResourceEntry.Change = resourceEntries[i].Change;
            newResourceEntry.ChangeOnTick = resourceEntries[i].ChangeOnTick;
        }

        resourceEntries = _resourceEntries.ToArray();
    }

    // Get resource modifiers on the tile of this structure and apply the modifier to the resource entries of this structure
    private void GetAndApplyResourceModifiers() {
        // Get resource Modifiers
        ResourceModifier[] resourceModifiers = tile.ResourceModifiers;

        // Loop through resource Modifiers
        for (int i = 0; i < resourceModifiers.Length; i++) {
            // Loop through resource entries
            for (int x = 0; x < resourceEntries.Length; x++) {
                // If resourceId and resourceEntryId do not match, continue to next resourceEntry
                if (!CheckResourceIdMatch(resourceModifiers[i], resourceEntries[x])) continue;
                ApplyResourceModifier(resourceModifiers[i], resourceEntries[x]);
            }
        }
    }

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
        _re.Change += _rm.Change;
        _re.Change *= _rm.PercentageChange;
    }

    // This is used by TileManagement
    public void ResetResourceEntries() {
        resourceEntries = upgrades[upgradeIndex].ResourceEntries;
    }

    #endregion
}
