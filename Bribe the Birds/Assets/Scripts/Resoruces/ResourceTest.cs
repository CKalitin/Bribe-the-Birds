using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTest : MonoBehaviour {
    [SerializeField] private ResourceEntry resourceEntry;

    private int resourceEntryId = -1;

    void Start() {
        Debug.Log($"Id: {resourceEntryId}, Supply: {ResourceManagement.instance.GetResource(Resources.Gold).Supply}, Demand: {ResourceManagement.instance.GetResource(Resources.Gold).Demand}");
        resourceEntryId = ResourceManagement.instance.AddResourceEntry(resourceEntry);
        StartCoroutine(RemoveResourceEntry());
    }

    void Update() {
        Debug.Log($"Id: {resourceEntryId}, Supply: {ResourceManagement.instance.GetResource(Resources.Gold).Supply}, Demand: {ResourceManagement.instance.GetResource(Resources.Gold).Demand}");
    }

    private IEnumerator RemoveResourceEntry() {
        yield return new WaitForSeconds(10f);

        ResourceManagement.instance.RemoveResourceEntry(resourceEntryId);
    }
}
