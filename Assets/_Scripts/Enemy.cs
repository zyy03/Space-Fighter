using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;
    [Header("Set Dynamically")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    protected BoundsCheck boundsCheck;
    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        materials = Util.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }
    public Vector3 pos
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
    private void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        if (boundsCheck != null && boundsCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "HeroProjectile":
                Projectile p = otherGO.GetComponent<Projectile>();
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    if(!notifiedOfDestruction){
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(gameObject);
                }
                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-projectile object");
                break;
        }
        ShowDamage();
    }
    void ShowDamage(){
        foreach (Material mat in materials)
        {
            mat.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage(){
        for(int i = 0; i < materials.Length; i++){
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
