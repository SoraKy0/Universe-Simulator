using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private Image healthBar;
    public float healthAmount = 100f;
    public float healingAmount = 5f;
    public float healingTime = 5f;
    private float nextHealingTime = 0f;

    void Start()
    {
        GameObject healthBarObject = GameObject.FindWithTag("HealthBar");
        if (healthBarObject != null)
        {
            healthBar = healthBarObject.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Health bar UI element not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (healthAmount <= 0)
        {
            Debug.Log("DEAD");
            return;
        }

        
        if (healthAmount < 100 && Time.time >= nextHealingTime)
        {
            Heal(healingAmount);
            nextHealingTime = Time.time + healingTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20f);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        
        healthAmount = Mathf.Max(healthAmount, 0);

        healthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Min(healthAmount, 100);
        healthBar.fillAmount = healthAmount / 100f;
    }
}
