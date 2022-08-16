using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set In Inspector")]
    public float rotationPerSecond = 0.1f;
    [Header("Set Dynamically")]
    public int levelShown = 0;
    Material mat;
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    private void Update()
    {
        int curLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        if (levelShown != curLevel)
        {
            levelShown = curLevel;
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }
        float rZ = -(rotationPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}

