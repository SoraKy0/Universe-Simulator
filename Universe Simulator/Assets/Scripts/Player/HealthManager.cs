using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Transporting;

public class HealthManager : NetworkBehaviour
{
    //Reference to the health bar prefab
    public GameObject healthBarObjectPrefab;
    //Holds a reference to the health bar UI
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
                    Debug.Log("DEAD"); // Says "DEAD" in the console if the health reaches 0
                }
            }
        }
    }

    // 5 health is restored every 5 seconds
    public float HealingAmount = 5f;
    public float HealingTime = 5f;
    private float nextHealingTime = 0f;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsClient)
        {
            //if your a client you will get a health bar
            instantiatedHealthBar = Instantiate(healthBarObjectPrefab, transform);
            UpdateHealthBar();
        }
    }

    //Update is called once per frame
    void Update()
    {
        //changes the health amount is you take damge then heals is health is below 100
        if (HealthAmount > 0 && HealthAmount < 100 && Time.time >= nextHealingTime)
        {
            Heal(HealingAmount);
            nextHealingTime = Time.time + HealingTime;
        }
    }

    //when the enemy object with the tag enemy chouches the player then the player takes 20 damage
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && IsOwner)
        {
            enemyDamage(20f);
        }
    }

    public void enemyDamage(float damage)
    {
        HealthAmount -= damage; //reduces health
        HealthAmount = Mathf.Max(HealthAmount, 0); //Prevents the health doesn't go below 0
    }


    public void HealPlayer(float healingAmount)
    {
        HealthAmount += healingAmount; //Increase health
        HealthAmount = Mathf.Min(HealthAmount, 100); //Prevents the health doesn't go above 100
    }

    //Update the actual of the health bar the player sees
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

    //called when player takes damage
    public void TakeDamage(float damage)
    {
        enemyDamage(damage);
    }

    //called when player heals
    public void Heal(float healingAmount)
    {
        HealPlayer(healingAmount);
    }
}
