using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Name")]
    public string weaponName;
    public int animationInteger;

    [Header("Damage")]
    //Damage
    [Range(1, 10)] public int bulletDamage;

    [Header("Ammo Count")]
    public bool infinteAmmo;
    [HideInInspector] public int ammo;
    [Range(5, 180)] public int maxAmmoCount;
    [Range(5, 180)] public int startingAmmoCount;
    [Range(2, 90)] public int ammoInAmmoPack;

    [Header("Projectiles")]
    [Range(1, 10)] public int projectilesPerShot;
    [Range(0, 10)] public float angleOfSpread;

    [Header("Bullet Speed")]
    [Range(0.1f, 5)] public float bulletSpeedMultiplier;

    [Header("Fire Rate")]
    [Range(0.125f, 25)] public float bulletsPerSecond;
    [HideInInspector] public float fireRate;

    [Header("Dopamine Increase on Kill")]
    [Range(0.01f, 1)] public float dopamineIncreaseOnKill;

    [Header("Alt Fire")]
    public float altFireTime;
    //[Header("Damage")]
    //Sprite
    //public Sprite sprite;


    private void OnValidate()
    {
        fireRate = bulletsPerSecond;
        ammo = startingAmmoCount;
    }

}