using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Resource Info", menuName = "Scriptable Objects/Resource Info")]
public class ResourceInfo : ScriptableObject {
    // Unlike MonoBehaviours all variables are public by default in Scriptable Objects, the more u know

    [SerializeField] private string resourceId;
    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private string resourceName;
    [Space]
    [Tooltip("Leave as 0 to use standard Resource Manager tick time.")]
    [SerializeField] private float customTickTime;

    public string ResourceId { get => resourceId; set => resourceId = value; }
    public Image Icon { get => icon; set => icon = value; }
    public string ResourceName { get => resourceName; set => resourceName = value; }
    public float CustomTickTime { get => customTickTime; set => customTickTime = value; }
}
