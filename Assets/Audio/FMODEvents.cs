using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity; 

public class FMODEvents : MonoBehaviour
{
    
    [field: Header("Ambience")]
    //[field: SerializeField] public EventReference ambience { get; private set; }
    
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }
    
    [field: Header("Player SFX")]
    //[field: SerializeField] public EventReference AfterImage { get; private set; }
    
    [field: Header("World SFX")]
    //[field: SerializeField] public EventReference grinding { get; private set; }
    
    
    [Header("Narrative Dialogue")]
    public FMODUnity.EventReference[] narrativeLines;

    

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager in scene.");
        }
        instance = this;
    }
    
}
