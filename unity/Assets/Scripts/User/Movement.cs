using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    public float moveSpeed = 10f;
    
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);
        
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
    }
}
