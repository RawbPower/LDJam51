using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Area in which the target can be where the AI stays aware of it
public class HotZone : MonoBehaviour
{
    public Collider2D hotZoneArea;
    public LayerMask layerMask;

    public AIAgent agentParent;

    private void Awake()
    {
        agentParent = GetComponentInParent<AIAgent>();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) != 0)
        {
            if (agentParent.GetTarget() == collision.gameObject)
            {
                agentParent.SetTarget(null);
                gameObject.SetActive(false);
                agentParent.alertArea.gameObject.SetActive(true);
            }
        }
    }
}