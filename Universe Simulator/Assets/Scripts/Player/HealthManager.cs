using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Transporting;

public class HealthManager : NetworkBehaviour
{
    public GameObject healthBarPrefab;
    private GameObject instantiatedHealthBar;

    private float _healthAmount = 100f;
    public float HealthAmount
    {
        get => _healthAmount;
        set
        {
            if (_healthAmount != value)
            {
                _healthAmount = value;
                UpdateHealthBar();
            }
        }
    }

    public float HealingAmount = 5f;
    public float HealingTime = 5f;
    private float nextHealingTime = 0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        {
            if (IsClient)
            {
                instantiatedHealthBar = Instantiate(healthBarPrefab, transform);
                UpdateHealthBar();
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {
        base.OnStartClient();
        {
            if (IsClient)
            {
                if (HealthAmount <= 0)
                {
                    Debug.Log("DEAD");
                    return;
                }

                if (HealthAmount < 100 && Time.time >= nextHealingTime)
                {
                    Heal(HealingAmount);
                    nextHealingTime = Time.time + HealingTime;
                }
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && IsOwner)
        {
            TakeDamageClient(20f);
        }
    }


    public void TakeDamageClient(float damage)
    {
        if (IsClient)
        {
            HealthAmount -= damage;
            HealthAmount = Mathf.Max(HealthAmount, 0);
        }
    }


    public void HealServerRpc(float healingAmount)
    {
        if (IsClient)
        {
            HealthAmount += healingAmount;
            HealthAmount = Mathf.Min(HealthAmount, 100);
        }
    }

    private void UpdateHealthBar()
    {
        if (IsClient && instantiatedHealthBar != null)
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
        if (IsClient)
        {
            TakeDamageClient(damage);
        }
    }

    public void Heal(float healingAmount)
    {
        if (IsClient)
        {
            HealServerRpc(healingAmount);
        }
    }
}