using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
}
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type;
    public string letter;
    public Color color;
    public GameObject projectilePrefab;
    public Color projectileColor;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot;
    private Renderer collarRend;
    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(_type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get
        {
            return _type;
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        //collarRend.material.color
        lastShot = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShot < def.delayBetweenShots) return;
        Projectile P;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y *= -1;
        }
        switch (type)
        {
            case WeaponType.blaster:
                P = MakeProjectile();
                P.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                P = MakeProjectile();
                P.rigid.velocity = vel;

                P = MakeProjectile();
                P.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                P.rigid.velocity = P.transform.rotation * vel;

                P = MakeProjectile();
                P.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                P.rigid.velocity = P.transform.rotation * vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "HeroProjectile";
            go.layer = LayerMask.NameToLayer("HeroProjectile");
        }
        else
        {
            go.tag = "EnemyProjectile";
            go.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile P = go.GetComponent<Projectile>();
        P.type = type;
        lastShot = Time.time;
        return P;
    }
}
