using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a class and not a struct so it can be used as a reference in UI code
public class Resource {
    [SerializeField] private string resourceId;
    [Space]
    [SerializeField] private float supply;
    [SerializeField] private float demand;
    [Space]
    [Tooltip("This is set by the Resource Info Scriptable Object for this resource. This value inputed here is overriden.")]
    [SerializeField] private float tickTime;

    public string ResourceId { get => resourceId; set => resourceId = value; }
    public float Supply { get => supply; set => supply = value; }
    public float Demand { get => demand; set => demand = value; }
    public float TickTime { get => tickTime; set => tickTime = value; }

    public Resource(float _supply, float _demand) {
        supply = _supply;
        demand = _demand;
    }

    public Resource() {
        supply = 0;
        demand = 0;
    }
}

public class ResourceManager : MonoBehaviour {
    #region Varibles

    [Header("Resources")]
    [SerializeField] private Resource[] resources;
    [Space]
    [Tooltip("Resource Infos must be in the same order as the resources")]
    [SerializeField] private ResourceInfo[] resourceInfos;
    [Space]
    [Tooltip("Period of time between demand changing resource supply.")]
    [SerializeField] private float tickTime;

    // Resouces are grouped by their tick time
    // First element in value list is reserved for the number of times the tick has been done, This is used in TickUpdate()
    private Dictionary<float, List<int>> resourceTicks;

    private List<ResourceEntry> resourceEntries = new List<ResourceEntry>();
    private List<int> availableResourceEntryIndex = new List<int>() { 0 }; // Which resource entry index should be used to insert new entry

    private float totalDeltaTime = 0;

    #endregion

    #region Core

    private void Awake() {
        ConfigureResources();
    }

    private void Update() {
        TickUpdate();
    }

    #endregion

    #region Core Utils

    // Called in Awake
    private void ConfigureResources() {
        // Loop through resources and set custom Tick Time.
        for (int i = 0; i < resources.Length; i++) {
            // Set TickTime to standard tickTime
            if (resourceInfos[i].CustomTickTime == 0)
                resources[i].TickTime = tickTime;
            // Set TickTime to custom tick time
            else
                resources[i].TickTime = resourceInfos[i].CustomTickTime;

            // If resourceTick already contains specified tickTime, add the index of the current resource to
            if (resourceTicks.ContainsKey(resources[i].TickTime))
                resourceTicks[(resources[i].TickTime)].Add(i);
            // Create new resourceTick and allocate first index for ticksPerformed and second for resource index
            else
                resourceTicks.Add(resources[i].TickTime, new List<int>() { 0, i });
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
            for (int x = 0; x < Mathf.FloorToInt(deltaTimeDifference % resourceTick.Value[0]); x++) {
                resourceTick.Value[0]++; // Increase ticks performed by 1

                // Loop through resources that should be updated on this tick
                for (int i = 0; i < resourceTick.Value.Count; i++) {
                    UpdateResourceTick(resourceTick.Value[i]); // Update Resource
                }
            }
        }
    }

    #endregion

    #region Resources

    public Resource GetResource(string _resourceId) {
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

    private int AddResourceEntry(ResourceEntry _resourceEntry) {
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

    private void RemoveResourceEntry(int _index) {
        Resource resource = GetResource(resourceEntries[_index].ResourceId); // Get Resource this entry modifies
        resource.Demand -= resourceEntries[_index].Change; // Subtract change to demand, reverse what was done in AddResourceEntry

        // Add index to indexes that are free to use
        availableResourceEntryIndex.Add(_index);
    }

    #endregion
}
