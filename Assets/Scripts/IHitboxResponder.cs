using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
    void collisionedWith(Collider2D collider, int hitboxIndex = 0);

    void resetHit(int hitboxIndex = 0);
}
