using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Resource Entry", menuName = "Scriptable Objects/Resources/Resource Entry")]
public class ResourceEntry : ScriptableObject {
    // Unlike MonoBehaviours all variables are public by default in Scriptable Objects, the more u know
    
    [Tooltip("The source that will be changed by this entry.")]
    [SerializeField] private Resources resourceId;

    [SerializeField] private float change;
    [Tooltip("If Change On Tick is enabled then the resource will be changed every tick, if not it will be applied once.")]
    [SerializeField] private bool changeOnTick;

    public Resources ResourceId { get => resourceId; set => resourceId = value; }
    public float Change { get => change; set => change = value; }
    public bool ChangeOnTick { get => changeOnTick; set => changeOnTick = value; }
}
