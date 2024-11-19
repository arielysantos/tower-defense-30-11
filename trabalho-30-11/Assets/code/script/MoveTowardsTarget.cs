using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveTowardsTarget : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void MoveToTarget()  // Nome do método alterado para MoveToTarget
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            //UpdateTarget(); // Descomente se existir o método UpdateTarget
        }
    }

    void Update()
    {
        MoveToTarget();  // Chama o método renomeado dentro do Update
    }
}



