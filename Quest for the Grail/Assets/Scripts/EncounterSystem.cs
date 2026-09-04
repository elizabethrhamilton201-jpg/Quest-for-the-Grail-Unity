using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxNumEnemies;

    private EnemyManager enemyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();
        enemyManager.GenerateEnemiesByEncounter(enemiesInScene, maxNumEnemies);
    }

}

[System.Serializable]

public class Encounter
{
    public EnemyInfo Enemy;
    public int LevelMin;
    public int LevelMax;
}
