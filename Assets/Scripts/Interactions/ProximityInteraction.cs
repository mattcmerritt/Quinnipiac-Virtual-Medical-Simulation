using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityInteraction : Trackable
{
    // Data to manually generate interaction script components
    [SerializeField] private string NearbyObjectName;
    [SerializeField] private GameObject NearbyObject;
    [SerializeField] private Vector3 RelativeLocation;
    [SerializeField] private GameObject ProximityColliderObject;

    // Data for this event
    [SerializeField] private int IgnoreInteractLayer; 
    [SerializeField] private List<GameObject> Hands = new List<GameObject>();
    [SerializeField] private float RequiredDuration = 20;
    [SerializeField] private float FailurePenalty = 0.5f;

    protected new void Start()
    {
        base.Start();

        if (NearbyObjectName != null)
        {
            NearbyObject = GameObject.Find(NearbyObjectName);

            ProximityColliderObject = new GameObject("Proximity Detection Box");
            ProximityColliderObject.layer = IgnoreInteractLayer;
            ProximityColliderObject.transform.SetParent(NearbyObject.transform);
            ProximityColliderObject.transform.localPosition = Vector3.zero;
            ProximityColliderObject.transform.localScale = Vector3.one;

            BoxCollider proximityBox = ProximityColliderObject.AddComponent<BoxCollider>();
            proximityBox.center = RelativeLocation;
            proximityBox.isTrigger = false;

            Rigidbody proximityRigidbody = ProximityColliderObject.AddComponent<Rigidbody>();
            proximityRigidbody.useGravity = false;
            proximityRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            ChildCollisionDetector collisionDetector = ProximityColliderObject.AddComponent<ChildCollisionDetector>();
            collisionDetector.Configure(this);
            collisionDetector.OnHandEnter += OnHandEnter;
            collisionDetector.OnHandExit += OnHandExit;
        }
    }

    private void OnHandEnter(GameObject hand)
    {
        Debug.Log($"<color=green>PROXIMITY HAND ENTER OCCURRED:</color>\nTarget: {gameObject}\nHand: {hand.gameObject}");
        if (Hands.Count == 0)
        {
            Activate();
        }
        Hands.Add(hand);
    }

    private void OnHandExit(GameObject hand)
    {
        Debug.Log($"<color=green>PROXIMITY HAND EXIT OCCURRED:</color>\nTarget: {gameObject}\nHand: {hand.gameObject}");
        Hands.Remove(hand);
        if (Hands.Count == 0)
        {
            Debug.Log($"<color=green>PROXIMITY EVENT CONCLUDED:</color>\nAll hands removed.");
            // calculate accuracy based on duration
            float score = 1;
            if (Duration < RequiredDuration)
            {
                score -= FailurePenalty;
            }
            
            foreach (Prerequisite prerequisite in PrerequisiteSteps)
            {
                if (!prerequisite.CheckSatisfied())
                {
                    score -= prerequisite.GetPenalty();
                }
            }

            Deactivate(score);
            CompleteStatistic();
        }
    }

    public void SetDurationAccuracy(float duration, float accuracy)
    {
        RequiredDuration = duration;
        FailurePenalty = accuracy;
    }
}
