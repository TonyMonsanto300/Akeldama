using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBillboard : MonoBehaviour {

    void Update() {
        // Get direction to player
        Vector3 direction = Camera.main.transform.position - transform.position;

        // Lock rotation to the Y-axis by zeroing out X and Z components
        direction.y = 0;

        // Flip the X and Z to mirror the direction across the X-axis
        direction.x = -direction.x;
        direction.z = -direction.z;

        // Update the rotation of the object to face the player, but remain upright
        if (direction != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
