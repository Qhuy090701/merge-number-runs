using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapHealth : MonoBehaviour
{

    [SerializeField] private float maxhealth;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = maxhealth;
    }

    void Update()
    {

    }

    public void TakeDamage(float dame)
    {
        currentHealth -= dame;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
