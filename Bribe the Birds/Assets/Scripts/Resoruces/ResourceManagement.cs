using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagement : MonoBehaviour {
    #region Varibles

    public static ResourceManagement instance;

    [Header("Resources")]
    [SerializeField] private Resource[] resources;
    [Space]
    [Tooltip("Period of time between changes to resource supply by demand.")]
    [SerializeField] private float tickTime;

    // Resouces are grouped by their tick time
    // First element in value list is reserved for the number of times the tick has been done, This is used in TickUpdate()
    private Dictionary<float, List<int>> resourceTicks = new Dictionary<float, List<int>>();

    private List<ResourceEntry> resourceEntries = new List<ResourceEntry>();
    private List<int> availableResourceEntryIndex = new List<int>() { 0 }; // Which resource entry index should be used to insert new entry

    private float totalDeltaTime = 0;

    #endregion

    #region Core

    private void Awake() {
        Singleton();
        ConfigureResources();
    }

    private void Update() {
        TickUpdate();
    }

    #endregion

    #region Core Utils

    // Called in Awake
    private void Singleton() {
        if (FindObjectsOfType<TileManagement>().Length > 1) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    // Called in Awake
    private void ConfigureResources() {
        // Loop through resources and set custom Tick Time.
        for (int i = 0; i < resources.Length; i++) {
            // Reset supply and demand, because it is a Scriptable Object these are saved
            resources[i].Supply = 0;
            resources[i].Demand = 0;

            // Set TickTime to standard tickTime
            if (resources[i].CustomTickTime == 0)
                resources[i].CustomTickTime = tickTime;

            // If resourceTick already contains specified tickTime, add the index of the current resource to
            if (resourceTicks.ContainsKey(resources[i].CustomTickTime)) {
                resourceTicks[resources[i].CustomTickTime].Add(i);
            } else {
                // Create new resourceTick and allocate first index for ticksPerformed and second for resource index
                resourceTicks.Add(resources[i].CustomTickTime, new List<int>() { 0, i });
                Debug.Log("New Resource Tick");
            }
                
        }
    }

    // This updates resource values based on their 
    private void TickUpdate() {
        totalDeltaTime += Time.deltaTime;
        
        foreach (KeyValuePair<float, List<int>> resourceTick in resourceTicks) {
            // Time difference between now and previous tick
            float deltaTimeDifference = totalDeltaTime - (resourceTick.Key * resourceTick.Value[0]);
            // If totalDeltaTime - (tickTime * ticksPerformed) is greater than tickTime, perform another tick
            if (deltaTimeDifference < resourceTick.Key) continue;

            // Run the resource tick the amount of times the tickTime can fit into the deltaTimeDifference
            // This is neccessary because the tickTime might be smaller than deltaTimeDifference, eg. lots of lag or tiny resourceTick
            for (int x = 0; x < Mathf.FloorToInt(deltaTimeDifference % resourceTick.Key); x++) {
                resourceTick.Value[0]++; // Increase ticks performed by 1

                // Loop through resources that should be updated on this tick
                // This starts one 1 because the 0th value is the number of times the tick update has occured
                for (int i = 1; i < resourceTick.Value.Count; i++) {
                    UpdateResourceTick(resourceTick.Value[i]); // Update Resource
                }
            }
        }
    }

    #endregion

    #region Resources

    public Resource GetResource(Resources _resourceId) {
        // Loop through resources and find resource that matches parameter id
        for (int i = 0; i < resources.Length; i++) {
            if (resources[i].ResourceId == _resourceId)
                return resources[i];
        }

        // If no resources found, return Null
        Debug.LogWarning($"Resource of Id: {_resourceId} cannot be found.");
        return null;
    }

    private void UpdateResourceTick(int _resourceIndex) {
        // Change supply by demand
        resources[_resourceIndex].Supply -= resources[_resourceIndex].Demand;
    }

    public int AddResourceEntry(ResourceEntry _resourceEntry) {
        if (_resourceEntry.ChangeOnTick) {
            int index = availableResourceEntryIndex[0]; // Get index to insert at
            resourceEntries.Insert(index, _resourceEntry); // Insert Resource Entry at index

            // If there are no availableResourceEntryIndexes left, add a new one at the end of the list
            if (availableResourceEntryIndex.Count <= 0)
                availableResourceEntryIndex.Add(resourceEntries.Count);

            Resource resource = GetResource(_resourceEntry.ResourceId); // Get Resource this entry modifies
            resource.Demand += _resourceEntry.Change; // Add change to demand

            return index;
        } else {
            Resource resource = GetResource(_resourceEntry.ResourceId); // Get Resource this entry modifies
            resource.Supply += _resourceEntry.Change; // Add change to demand

            // Return fake index because index is not used in this case, it is a single use
            // It's in situations like this where I wish a return function was not alawys needed or something, it's 4aem
            return -1;
        }
    }

    public void RemoveResourceEntry(int _index) {
        Resource resource = GetResource(resourceEntries[_index].ResourceId); // Get Resource this entry modifies
        resource.Demand -= resourceEntries[_index].Change; // Subtract change to demand, reverse what was done in AddResourceEntry

        // Add index to indexes that are free to use
        availableResourceEntryIndex.Add(_index);
    }

    #endregion
}