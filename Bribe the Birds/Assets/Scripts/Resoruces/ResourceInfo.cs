using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Resource Info", menuName = "Scriptable Objects/Resources/Resource Info")]
public class ResourceInfo : ScriptableObject {
    // Unlike MonoBehaviours all variables are public by default in Scriptable Objects, the more u know

    [SerializeField] private Resources resourceId;
    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private string resourceDisplayName;

    public Resources ResourceId { get => resourceId; set => resourceId = value; }
    public Image Icon { get => icon; set => icon = value; }
    public string ResourceDisplayName { get => resourceDisplayName; set => resourceDisplayName = value; }
}
