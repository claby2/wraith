﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour {
    public Rigidbody2D Rigidbody;
    public SpriteRenderer SpriteRenderer;
    public GameObject Crosshair;
    public GameObject Projectile;
    public AbilityFrame[] AbilityFrames;
    public AbilityIcon[] AbilityIcons;
    public AbilityCooldown[] AbilityCooldownVisual;
    public CurrentWeaponIcon CurrentWeaponIcon;
    public CurrentWeaponReload CurrentWeaponReload;
    public Inventory Inventory;
    public PlayerHealthBar PlayerHealthBar;
    public TextAsset WeaponDataJson;
    public TextAsset AbilityDataJson;
    public Sprite PlayerSprite;
    public Sprite PlayerPhasedSprite;
    public bool Phased = false;
    public Ability[] Abilities = new Ability[3];
    public int WeaponId;
    public Weapon CurrentWeapon;
    public WeaponData WeaponInformation;
    public AbilityData AbilityInformation;

    // Health
    private float maximumHealth = 100f;
    private float currentHealth;

    // Movement
    private const float moveSpeed = 10f;
    private float moveSpeedMultiplier = 1f;
    private Vector2 movementInput;

    // Attack
    private float damageMultiplier = 1f;
    private float attackCooldown = 0f;

    // Ability
    private int abilitySelected = 0;
    private float[] abilityTimers = new float[3];
    private float[] abilityCooldowns = new float[3];
    private bool[] abilityState = new bool[3];

    void Start() {
        WeaponInformation = JsonUtility.FromJson<WeaponData>(WeaponDataJson.text);
        AbilityInformation = JsonUtility.FromJson<AbilityData>(AbilityDataJson.text);
        currentHealth = maximumHealth;
        EquipWeapon(0);
        // Set abilities to id -1, empty ability
        Ability emptyAbility = new Ability(-1);
        for(int i = 0; i < 3; i++) {
            Abilities[i] = emptyAbility;
        }
        // TODO: debug, add temporary items for testing
        Inventory.AddItem(Inventory.ItemType.weapon, 1);
        Inventory.AddItem(Inventory.ItemType.weapon, 2);
        Inventory.AddItem(Inventory.ItemType.ability, 0);
        Inventory.AddItem(Inventory.ItemType.ability, 1);
        Inventory.AddItem(Inventory.ItemType.ability, 2);
        Inventory.AddItem(Inventory.ItemType.weapon, 0);
        Inventory.AddItem(Inventory.ItemType.ability, 3);
    }

    void Update() {
        HandleAbilities();
        GetMovementInput();
        if(movementInput.x > 0) {
            SpriteRenderer.flipX = true;
        } else if(movementInput.x < 0) {
            SpriteRenderer.flipX = false;
        }
        if(WeaponId != -1 && Inventory.Active == false) {
            if(((CurrentWeapon.automatic && Input.GetMouseButton(0)) || (!CurrentWeapon.automatic && Input.GetMouseButtonDown(0))) && attackCooldown <= 0) {
                attackCooldown = CurrentWeapon.cooldown;
                InstantiateProjectile();
            }
            attackCooldown -= attackCooldown < 0 ? 0 : Time.deltaTime;
            CurrentWeaponReload.SetReload(attackCooldown, CurrentWeapon.cooldown);
        }
        if(Input.GetMouseButtonDown(1)) {
            UseAbility();
        }
    }

    void FixedUpdate() {
        Move();
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        PlayerHealthBar.SetHealth(currentHealth, maximumHealth);
    }

    public void EquipWeapon(int id) {
        WeaponId = id;
        CurrentWeaponIcon.SetWeapon(WeaponId);
        if(WeaponId != -1) {
            CurrentWeapon = WeaponInformation.weapons[WeaponId];
        }
    }

    public void EquipAbility(int slot, int id) {
        if(id != -1) {
            Abilities[slot] = AbilityInformation.abilities[id];
        } else {
            abilityTimers[slot] = 0f;
            AbilityFrames[slot].Unselect();
            AbilityCooldownVisual[slot].SetCooldown(0f, 0f);
            if(abilityState[slot] == true) {
                // Ability is currently active but is about to be removed
                // Stop the ability from continuing
                StopAbility(Abilities[slot].id, slot);
            }
            Abilities[slot] = new Ability(-1);
        }
    }

    void GetMovementInput() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY);
    }

    void Move() {
        Rigidbody.velocity = new Vector2(
            movementInput.x * moveSpeed * moveSpeedMultiplier, 
            movementInput.y * moveSpeed * moveSpeedMultiplier
        );
    }

    void InstantiateProjectile() {
        GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.CrosshairPosition = Crosshair.transform.position;
        projectileScript.Speed = CurrentWeapon.projectileSpeed;
        projectileScript.Damage = CurrentWeapon.damage * damageMultiplier;
        projectileScript.SpriteId = CurrentWeapon.projectileSpriteId;
    }

    void HandleAbilities() {
        // Handle ability frame selection sprite
        int currentAbilitySelected = abilitySelected;
        if(Abilities[0].id != -1 && Input.GetKeyDown("1") && !abilityState[0]) {
            AbilityFrames[0].Select();
            abilitySelected = 0;
        } else if(Abilities[1].id != -1 && Input.GetKeyDown("2") && !abilityState[1]) {
            AbilityFrames[1].Select();
            abilitySelected = 1;
        } else if(Abilities[2].id != -1 && Input.GetKeyDown("3") && !abilityState[2]) {
            AbilityFrames[2].Select();
            abilitySelected = 2;
        }
        if(currentAbilitySelected != abilitySelected) {
            AbilityFrames[currentAbilitySelected].Unselect();
        }
        for(int ability = 0; ability < 3; ++ability) {
            // Handle ability icons
            AbilityIcons[ability].SetAbility(Abilities[ability].id);
            if(Abilities[ability].id != -1) {
                // Handle ability cooldowns
                abilityCooldowns[ability] -= abilityCooldowns[ability] > 0 ? Time.deltaTime : 0;
                AbilityCooldownVisual[ability].SetCooldown(
                    abilityCooldowns[ability], 
                    Abilities[ability].cooldown
                );
                // Handle ability timers
                if(abilityState[ability] && (abilityTimers[ability] - Time.deltaTime) <= 0) {
                    // Ability timer is about to run out
                    StopAbility(Abilities[ability].id, ability);
                    if(abilitySelected == ability) {
                        AbilityFrames[ability].Select();
                    } else {
                        AbilityFrames[ability].Unselect();
                    }
                }
                abilityTimers[ability] -= abilityTimers[ability] > 0 ? Time.deltaTime : 0;
                // Handle ability frame active sprite
                if(abilityState[ability] == true) {
                    AbilityFrames[ability].SetActive();
                }
            }
        }
    }

    void UseAbility() {
        if(abilityCooldowns[abilitySelected] <= 0) {
            switch(Abilities[abilitySelected].id) {
                case 0:
                    UseAbilityPhase();
                    break;
                case 1:
                    UseAbilitySpeedBoost();
                    break;
                case 2:
                    UseAbilityStrength();
                    break;
                case 3:
                    UseAbilityTeleport();
                    break;
            }
            abilityState[abilitySelected] = true;
        }
    }

    void StopAbility(int abilityId, int ability) {
        switch(abilityId) {
            case 0:
                StopAbilityPhase();
                break;
            case 1:
                StopAbilitySpeedBoost();
                break;
            case 2:
                StopAbilityStrength();
                break;
            case 3:
                StopAbilityTeleport();
                break;
        }
        abilityCooldowns[ability] = Abilities[ability].cooldown;
        abilityState[ability] = false;
    }

    void UseAbilityPhase() {
        abilityTimers[abilitySelected] = Abilities[abilitySelected].duration;
        SpriteRenderer.sprite = PlayerPhasedSprite;
        Phased = true;
    }

    void StopAbilityPhase() {
        SpriteRenderer.sprite = PlayerSprite;
        Phased = false;
    }

    void UseAbilitySpeedBoost() {
        abilityTimers[abilitySelected] = Abilities[abilitySelected].duration;
        moveSpeedMultiplier = 2f;
    }

    void StopAbilitySpeedBoost() {
        moveSpeedMultiplier = 1f;
    }

    void UseAbilityStrength() {
        abilityTimers[abilitySelected] = Abilities[abilitySelected].duration;
        damageMultiplier = 10f;
    }

    void StopAbilityStrength() {
        damageMultiplier = 1f;
    }

    void UseAbilityTeleport() {
        abilityTimers[abilitySelected] = Abilities[abilitySelected].duration;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, Crosshair.transform.position);
        // The Physics2D settings are set so queries start in colliders is set to false
        // This means that it is not needed to detect if the raycast hit the player itself
        if(hit.collider == null || hit.collider.CompareTag("Solid") == false) {
            transform.position = Crosshair.transform.position;
        }
    }

    void StopAbilityTeleport() {}

    [System.Serializable]
    public class WeaponData {
        public Weapon[] weapons;
    }

    [System.Serializable]
    public class Weapon {
        public int id;
        public string name;
        public float cooldown;
        public float damage;
        public bool automatic;
        public float projectileSpeed;
        public int projectileSpriteId;
        public string description;
    }

    [System.Serializable]
    public class AbilityData {
        public Ability[] abilities;
    }

    [System.Serializable]
    public class Ability {
        public int id;
        public string name;
        public float duration;
        public float cooldown;
        public string description;
        public Ability(int id) {
            this.id = id;
        }
    }
}
