using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour {
    public Rigidbody2D Rigidbody;
    public SpriteRenderer SpriteRenderer;
    public GameObject Crosshair;
    public GameObject Projectile;
    public GameObject PlayerHealthBar;
    public AbilityFrame[] AbilityFrames;
    public AbilityIcon[] AbilityIcons;
    public AbilityIcon[] AbilityInventoryIcons;
    public AbilityCooldown[] AbilityCooldownVisual;
    public CurrentWeaponIcon CurrentWeaponIcon;
    public CurrentWeaponIcon CurrentWeaponIconInventory;
    public CurrentWeaponReload CurrentWeaponReload;
    public TextAsset WeaponDataJson;
    public TextAsset AbilityDataJson;
    public Sprite PlayerSprite;
    public Sprite PlayerPhasedSprite;
    public bool Phased = false;

    // Health
    private PlayerHealthBar playerHealthBarScript;
    private float maximumHealth = 100f;
    private float currentHealth;

    // Movement
    private const float moveSpeed = 10f;
    private float moveSpeedMultiplier = 1f;
    private Vector2 movementInput;

    // Attack
    private float damageMultiplier = 1f;
    private float attackCooldown = 0f;
    private int weaponId = 0;
    private WeaponData weaponData;
    private Weapon weapon;

    // Ability
    private int abilitySelected = 0;
    private AbilityData abilityData;
    private Ability[] abilities = new Ability[3];
    private float[] abilityTimers = new float[3];
    private float[] abilityCooldowns = new float[3];
    private bool[] abilityState = new bool[3];

    void Start() {
        weaponData = JsonUtility.FromJson<WeaponData>(WeaponDataJson.text);
        abilityData = JsonUtility.FromJson<AbilityData>(AbilityDataJson.text);
        currentHealth = maximumHealth;
        playerHealthBarScript = PlayerHealthBar.GetComponent<PlayerHealthBar>();
        AbilityFrames[abilitySelected].Select();
        CurrentWeaponIcon.SetWeapon(weaponId);
        CurrentWeaponIconInventory.SetWeapon(weaponId);
        weapon = weaponData.weapons[weaponId];

        // TODO: debug, start with three abilities
        abilities[0] = abilityData.abilities[0];
        abilities[1] = abilityData.abilities[1];
        abilities[2] = abilityData.abilities[2];
    }

    void Update() {
        HandleAbilities();
        GetMovementInput();
        if(movementInput.x > 0) {
            SpriteRenderer.flipX = true;
        } else if(movementInput.x < 0) {
            SpriteRenderer.flipX = false;
        }
        if(((weapon.automatic && Input.GetMouseButton(0)) || (!weapon.automatic && Input.GetMouseButtonDown(0))) && attackCooldown <= 0) {
            attackCooldown = weapon.cooldown;
            InstantiateProjectile();
        }
        if(Input.GetMouseButtonDown(1)) {
            UseAbility();
        }
        attackCooldown -= attackCooldown < 0 ? 0 : Time.deltaTime;
        CurrentWeaponReload.SetReload(attackCooldown, weapon.cooldown);
    }

    void FixedUpdate() {
        Move();
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        playerHealthBarScript.SetHealth(currentHealth, maximumHealth);
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
        projectileScript.Speed = weapon.projectileSpeed;
        projectileScript.Damage = weapon.damage * damageMultiplier;
        projectileScript.SpriteId = weapon.projectileSpriteId;
    }

    void HandleAbilities() {
        // Handle ability frame selection sprite
        int currentAbilitySelected = abilitySelected;
        if(Input.GetKeyDown("1") && !abilityState[0]) {
            AbilityFrames[0].Select();
            abilitySelected = 0;
        } else if(Input.GetKeyDown("2") && !abilityState[1]) {
            AbilityFrames[1].Select();
            abilitySelected = 1;
        } else if(Input.GetKeyDown("3") && !abilityState[2]) {
            AbilityFrames[2].Select();
            abilitySelected = 2;
        }
        if(currentAbilitySelected != abilitySelected) {
            AbilityFrames[currentAbilitySelected].Unselect();
        }
        for(int ability = 0; ability < 3; ++ability) {
            // Handle ability icons
            AbilityIcons[ability].SetAbility(abilities[ability].id);
            AbilityInventoryIcons[ability].SetAbility(abilities[ability].id);
            // Handle ability cooldowns
            abilityCooldowns[ability] -= abilityCooldowns[ability] > 0 ? Time.deltaTime : 0;
            AbilityCooldownVisual[ability].SetCooldown(
                abilityCooldowns[ability], 
                abilities[ability].cooldown
            );
            // Handle ability timers
            if(abilityState[ability] && (abilityTimers[ability] - Time.deltaTime) <= 0) {
                // Ability timer is about to run out
                switch(abilities[ability].id) {
                    case 0:
                        StopAbilityPhase();
                        break;
                    case 1:
                        StopAbilitySpeedBoost();
                        break;
                    case 2:
                        StopAbilityStrength();
                        break;
                }
                abilityCooldowns[ability] = abilities[ability].cooldown;
                abilityState[ability] = false;
                if(abilitySelected == ability) {
                    AbilityFrames[ability].Select();
                } else {
                    AbilityFrames[ability].Unselect();
                }
            }
            abilityTimers[ability] -= abilityTimers[ability] > 0 ? Time.deltaTime : 0;
            // Handle ability frame active sprite
            if(abilityState[ability]) {
                AbilityFrames[ability].SetActive();
            }
        }
    }

    void UseAbility() {
        if(abilityCooldowns[abilitySelected] <= 0) {
            switch(abilities[abilitySelected].id) {
                case 0:
                    UseAbilityPhase();
                    break;
                case 1:
                    UseAbilitySpeedBoost();
                    break;
                case 2:
                    UseAbilityStrength();
                    break;
            }
            abilityState[abilitySelected] = true;
        }
    }

    void UseAbilityPhase() {
        abilityTimers[abilitySelected] = abilities[abilitySelected].duration;
        SpriteRenderer.sprite = PlayerPhasedSprite;
        Phased = true;
    }

    void StopAbilityPhase() {
        SpriteRenderer.sprite = PlayerSprite;
        Phased = false;
    }

    void UseAbilitySpeedBoost() {
        abilityTimers[abilitySelected] = abilities[abilitySelected].duration;
        moveSpeedMultiplier = 2f;
    }

    void StopAbilitySpeedBoost() {
        moveSpeedMultiplier = 1f;
    }

    void UseAbilityStrength() {
        abilityTimers[abilitySelected] = abilities[abilitySelected].duration;
        damageMultiplier = 10f;
    }

    void StopAbilityStrength() {
        damageMultiplier = 1f;
    }

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
    }
}
