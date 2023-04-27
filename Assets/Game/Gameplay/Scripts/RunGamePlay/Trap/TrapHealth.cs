using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public Text healthText;

    void Start()
    {
        currentHealth = maxHealth;
        healthText = Instantiate(healthText, transform.position, Quaternion.identity);
        healthText.transform.SetParent(transform);
        healthText.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthText.text = "Health: " + currentHealth.ToString();
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
