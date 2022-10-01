using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Area in which AI becomes aware of target if it exists
public class AlertArea : MonoBehaviour
{
    public Collider2D alertArea;
    public LayerMask layerMask;
    public HotZone hotZone;

    private AIAgent agentParent;

    private void Awake()
    {
        agentParent = GetComponentInParent<AIAgent>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((((1 << collision.gameObject.layer) & layerMask) != 0) && agentParent != null && agentParent.alertArea != null)
        {
            agentParent.SetTarget(collision.gameObject);
            gameObject.SetActive(false);
            agentParent.hotzone.gameObject.SetActive(true);
        }
    }
}
