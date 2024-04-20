using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Transporting;

public class HealthManager : NetworkBehaviour
{
    // Reference to the health bar prefab
    public GameObject healthBarObjectPrefab;
    // Reference to the instantiated health bar
    private GameObject instantiatedHealthBar;

    // Current health amount
    private float healthAmount = 100f;
    public float HealthAmount
    {
        get => healthAmount;
        set
        {
            if (healthAmount != value)
            {
                healthAmount = value;
                UpdateHealthBar(); // Update the health bar when health changes
                if (healthAmount <= 0)
                {
                    Debug.Log("DEAD"); // Log "DEAD" when health reaches 0
                }
            }
        }
    }

    // Amount of health restored per healingTime
    public float HealingAmount = 5f;
    // Time interval between healing
    public float HealingTime = 5f;
    private float nextHealingTime = 0f;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsClient)
        {
            // Instantiate the health bar and update it
            instantiatedHealthBar = Instantiate(healthBarObjectPrefab, transform);
            UpdateHealthBar();

            // Ensure that the parent Health Bar object has a NetworkBehaviour component
            if (transform.parent != null && transform.parent.GetComponent<NetworkBehaviour>() == null)
            {
                transform.parent.gameObject.AddComponent<NetworkBehaviour>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If on the client and health is not full and it start to heal
        if (IsClient && HealthAmount > 0 && HealthAmount < 100 && Time.time >= nextHealingTime)
        {
            Heal(HealingAmount);
            nextHealingTime = Time.time + HealingTime;
        }
    }

    // Called when player rigidbody has begun touching enemy rigidbody
    void OnCollisionEnter(Collision collision)
    {
        // If colliding with an enemy
        if (collision.gameObject.CompareTag("Enemy") && IsOwner)
        {
            // Amount of damage to be taked passed throught the methord
            TakeDamageClient(20f);
        }
    }

    public void TakeDamageClient(float damage)
    {
        HealthAmount -= damage; // Reduce health
        HealthAmount = Mathf.Max(HealthAmount, 0); // Ensure health doesn't go below 0
    }

    // Remote procedure call to handle healing on the server side
    public void HealServerRpc(float healingAmount)
    {
        HealthAmount += healingAmount; // Increase health
        HealthAmount = Mathf.Min(HealthAmount, 100); // Ensure health doesn't exceed 100
    }

    // Update the actual of the health bar the player sees
    private void UpdateHealthBar()
    {
        if (instantiatedHealthBar != null)
        {
            Image healthFill = instantiatedHealthBar.transform.Find("Health").GetComponent<Image>();
            if (healthFill != null)
            {
                healthFill.fillAmount = HealthAmount / 100f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamageClient(damage);
    }

    public void Heal(float healingAmount)
    {
        HealServerRpc(healingAmount);
    }
}
