using Code;
using Code.DeathMessages;
using Ez;
using UnityEngine;

public class FishingNet : MonoBehaviour
{
    [SerializeField] private GameObject turtle;
    private Collider turtleCollider;
    private CauseOfDeath _causeOfDeath = CauseOfDeath.Fishing;

    private void Start()
    {
        turtleCollider = turtle.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == turtleCollider)
        {
            other.gameObject.Send<TurtleVitalSystems>(_=>_.Die(_causeOfDeath));
        }
    }
}
