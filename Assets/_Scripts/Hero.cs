using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;
    [Header("Set In Inspector")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    [Header("Set Dynamically")]
    public GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;


    [SerializeField]
    private float _shieldLevel = 1;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake()-Attempt to assign second Hero.S!");
        }
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
        //fireDelegate += TemptFire;
    }
    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }
    // void TemptFire()
    // {
    //     GameObject projGo = Instantiate(projectilePrefab);
    //     projGo.transform.position = transform.position;
    //     Rigidbody rigidB = projGo.GetComponent<Rigidbody>();

    //     Projectile projectile = projGo.GetComponent<Projectile>();
    //     projectile.type = WeaponType.blaster;
    //     float tSpeed = Main.GetWel'k'laponDefinition(projectile.type).velocity;
    //     rigidB.velocity = new Vector3(0, tSpeed, 0);
    // }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered:" + go.name);
        }
    }
    public float shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(gameObject);
                Main.S.DelayedRestart(Main.S.gameRestartDelay);
            }
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp up = go.GetComponent<PowerUp>();
        switch (up.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (up.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(up.type);
                    }
                    else
                    {
                        ClearWeapons();
                        weapons[0].SetType(up.type);
                    }
                }
                break;
        }
        up.AbsorbedBy(gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}

