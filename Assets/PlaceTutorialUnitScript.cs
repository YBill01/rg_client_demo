using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaceTutorialUnitScript : MonoBehaviour
{
    public Vector2 position;
    public int deckID;

    [SerializeField]
    private GameObject tutorialCardPlayPrefab;

    private UnityEvent _placeComplete = new UnityEvent();

    public UnityEvent PlaceComplete { get => _placeComplete; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupDragUnit()
    {

    }
}
