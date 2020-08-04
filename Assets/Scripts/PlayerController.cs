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
    public Ability[] Abilities;
    public CurrentWeaponIcon CurrentWeaponIcon;
    public CurrentWeaponReload CurrentWeaponReload;
    public TextAsset WeaponDataJson;

    // Health
    private PlayerHealthBar playerHealthBarScript;
    private float maximumHealth = 100f;
    private float currentHealth;

    // Movement
    private const float moveSpeed = 10f;
    private Vector2 movementInput;

    private float attackCooldown = 0f;
    private int abilitySelected = 0;
    private int weaponId = 2;
    private WeaponData weaponData;
    private Weapon weapon;

    void Start() {
        weaponData = JsonUtility.FromJson<WeaponData>(WeaponDataJson.text);
        currentHealth = maximumHealth;
        playerHealthBarScript = PlayerHealthBar.GetComponent<PlayerHealthBar>();
        Abilities[abilitySelected].Select();
        CurrentWeaponIcon.SetWeapon(weaponId);
        weapon = weaponData.weapons[weaponId];
    }

    void Update() {
        HandleAbilities();
        GetMovementInput();
        if(movementInput.x > 0) {
            SpriteRenderer.flipX = true;
        } else if(movementInput.x < 0) {
            SpriteRenderer.flipX = false;
        }
        if(((weapon.automatic && Input.GetMouseButton(0)) || (!weapon.automatic && Input.GetMouseButtonDown(0))) && attackCooldown < 0) {
            attackCooldown = weapon.cooldown;
            InstantiateProjectile();
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
        Rigidbody.velocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
    }

    void InstantiateProjectile() {
        GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.CrosshairPosition = Crosshair.transform.position;
        projectileScript.Speed = weapon.projectileSpeed;
        projectileScript.Damage = weapon.damage;
        projectileScript.SpriteId = weapon.projectileSpriteId;
    }

    void HandleAbilities() {
        int currentAbilitySelected = abilitySelected;
        if(Input.GetKeyDown("1")) {
            Abilities[0].Select();
            abilitySelected = 0;
        } else if(Input.GetKeyDown("2")) {
            Abilities[1].Select();
            abilitySelected = 1;
        } else if(Input.GetKeyDown("3")) {
            Abilities[2].Select();
            abilitySelected = 2;
        }
        if(currentAbilitySelected != abilitySelected) {
            Abilities[currentAbilitySelected].Unselect();
        }
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
}
