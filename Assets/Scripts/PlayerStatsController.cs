using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Items
{
    PUNKROCKALBUMCOVER
}

public enum SandAbilities
{
    NONE = -1,
    PROJECTILE,
    SHIELD,
    GRAB
}

public class PlayerStatsController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float currentHealth;
    [SerializeField] public float currentMaxHealth;
    [SerializeField] public float endGameMaxHealth;

    [Header("Sand Meter")]
    [SerializeField] public float currentSand;
    [SerializeField] public float currentMaxSand;
    [SerializeField] public float endGameMaxSand;
    [SerializeField] public float secondsToRefillSand;
    [SerializeField] private float plannedSand;

    [Header("Sand Meter Costs")]
    //[SerializeField] float thrownCostPerDistance;
    [SerializeField] float glassCost;
    [SerializeField] float shieldCost;
    [SerializeField] float grabCost;
    [SerializeField] float grabCostPerUnit;
    float[] costs = { 0.0f, 0.0f, 0.0f };

    //[Header("Collectables")]
    //[SerializeField] int sandDollars;
    //[SerializeField] List<Items> items;

    [Header("Collectables")]
    [SerializeField] bool[] hasAbility = { true, true, true };
    bool[] inUse = { false, false, false};

    PlayerMovementController movement;
    SandAbilityManager abilMan;
    Animator anim;

    bool empty;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovementController>();
        abilMan = GetComponent<SandAbilityManager>();

        costs[0] = glassCost;
        costs[1] = shieldCost;
        costs[2] = grabCost;

        currentHealth = currentMaxHealth;
        currentSand = currentMaxSand;
        plannedSand = currentSand;
        secondsToRefillSand = 1 / secondsToRefillSand;

        empty = false;
    }

    // Update is called once per frame
    void Update()
    {
        DeterminePlannedSand();
        RechargeSand();
    }

    void DeterminePlannedSand()
    {
        plannedSand = currentMaxSand;
        for(int i = 0; i < costs.Length;i++)
        {
            if(inUse[i])
            {
                plannedSand -= costs[i];
            }
        }

        if(inUse[(int)SandAbilities.GRAB])
        {
            float dist = Vector3.Distance(transform.position, GameObject.FindObjectOfType<GrabbingSandAbility>().transform.position);
            plannedSand -= dist * grabCostPerUnit;
        }

        if(plannedSand <= 0)
        {
            empty = true;
            plannedSand = 0;
            abilMan.AttemptEndGrab();
            abilMan.AttemptEndThrow();
            abilMan.AttemptEndShield();
        }
    }

    void RechargeSand()
    {
        currentSand = Mathf.Min(currentSand + Time.deltaTime * secondsToRefillSand, plannedSand);
        if(currentMaxSand == currentSand)
        {
            empty = false;
        }
    }

    public bool ChangeHealth(float delta)
    {
        if (delta < 0 && movement.strafeTimer > movement.strafeCooldown) return false;

        currentHealth += delta;
        if(currentHealth <= 0)
        {
            Debug.Log("Player Death");
            //Destroy(this.gameObject);
        }

        if(delta < 0)
        {
            anim.SetTrigger("Hurt");
        }

        return true;
    }

    public bool ActivateSand(SandAbilities ability)
    {
        int index = (int)ability;

        if (empty || !hasAbility[index] || inUse[index] || currentSand - costs[index] < 0) return false;

        inUse[index] = true;
        currentSand -= costs[index];
        return true;
    }

    public bool DeactivateSand(SandAbilities ability)
    {
        int index = (int)ability;

        if (!hasAbility[index] || !inUse[index]) return false;

        inUse[index] = false;
        return true;
    }
}
