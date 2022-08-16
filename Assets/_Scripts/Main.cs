using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAPON_Dict;

    [Header("Set In Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemySpawnPadding = 1.5f;
    public float gameRestartDelay = 2f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield};

    private BoundsCheck boundsCheck;
    private void Awake() {
        S = this;
        boundsCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
        WEAPON_Dict = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition varDef in weaponDefinitions)
        {
            WEAPON_Dict[varDef.type] = varDef;
        }
    }

    public void SpawnEnemy(){
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        float enemyPadding = enemySpawnPadding;
        if(go.GetComponent<BoundsCheck>() != null){
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        Vector3 pos = Vector3.zero;
        float xMin = -boundsCheck.camWidth + enemyPadding;
        float xMax = boundsCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = boundsCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy e){
        if(Random.value <= e.powerUpDropChance){
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType upType = powerUpFrequency[ndx];
            GameObject go = Instantiate(prefabPowerUp);
            PowerUp up = go.GetComponent<PowerUp>();
            up.Settype(upType);
            up.transform.position = e.transform.position;
        }
    }

    public void DelayedRestart(float delay){
        Invoke("Restart", delay);
    }
    public void Restart(){
        SceneManager.LoadScene("SampleScene");
    }
    static public WeaponDefinition GetWeaponDefinition(WeaponType weaponType){
        if(WEAPON_Dict.ContainsKey(weaponType)){
            return WEAPON_Dict[weaponType];          
        }
        return new WeaponDefinition();
    }
}
