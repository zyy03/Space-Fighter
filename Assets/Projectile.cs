using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck boundsCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    public WeaponType type{
        get{
            return _type;
        }
        set{
            SetType(value);
        }
    }
    private void Awake() {
        boundsCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }
    private void Update() {
        if(boundsCheck != null && boundsCheck.offUp){
            Destroy(gameObject);
        }
    }
    public void SetType(WeaponType eType){
        _type = eType;
        rend.material.color = Main.GetWeaponDefinition(_type).color;
    }
}
