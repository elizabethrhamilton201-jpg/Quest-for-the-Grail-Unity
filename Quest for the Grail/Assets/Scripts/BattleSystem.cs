using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, Selection, Battle, Won, Loss, Run }

    [Header("Battle State")]
    [SerializeField] private BattleState state;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;

    private const string ACTION_MESSAGE = "'s Action:";
    private const string WIN_MESSAGE = "Your party won the battle!";
    private const int TURN_DURATION = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyManager = GameObject.FindAnyObjectByType<PartyManager>();
        enemyManager = GameObject.FindAnyObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        // AttackAction(allBattlers[0], allBattlers[1]);
    }

    private IEnumerator BattleRoutine()
    {
        // enemy selection menu disabled
        enemySelectionMenu.SetActive(false);
        // change our state to the battle state
        state = BattleState.Battle;
        // enable our bottom text
        bottomTextPopUp.SetActive(true);

        // loop through all our battlers
            // do their appropriate action#

            for (int i = 0; i < allBattlers.Count; i++)
            {
                switch(allBattlers[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
                        // do the attack
                        // Debug.Log(allBattlers[i].Name + " is attacking: " + allBattlers[allBattlers[i].Target].Name);
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        // run
                        break;
                    default:
                        Debug.Log("Error - incorrect battle action");
                        break;
                }
            }

        // if we haven't won or lost, repeat the loop by opening the battle menu
        if(state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }

        yield return null;
    }

    private IEnumerator AttackRoutine(int i)
    {
        // players turn
        if(allBattlers[i].IsPlayer == true)
        {
            BattleEntities currAttacker = allBattlers[i];
            BattleEntities currTarget = allBattlers[currAttacker.Target];
            // attack selected enemy (attack action)
            AttackAction(currAttacker, currTarget);
            // wait a few seconds
            yield return new WaitForSeconds(TURN_DURATION);
            // kill the enemy
            if(currTarget.CurrHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyBattlers.Remove(currTarget);
                allBattlers.Remove(currTarget);

                if(enemyBattlers.Count <= 0)
                {
                   state = BattleState.Won; 
                   bottomText.text = WIN_MESSAGE;
                   yield return new WaitForSeconds(TURN_DURATION);
                   Debug.Log("Go back to overworld scene");
                }
            }

            // if no enemies remain
            // we know we won the battle

        }
        

        // enemies turn 
        // get random party member (target)
        // attack selected party member (attack action)
        // wait a few seconds
        // kill the party member

        // if no party members remain
        // we know we lost the battle
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);

            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].MemberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();       
            
            tempBattleVisuals.SetStartingValues(currentParty[i].MaxHealth, currentParty[i].MaxHealth, currentParty[i].Level);
            tempEntity.BattleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrentEnemies();

        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();

            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);

            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].EnemyVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            tempBattleVisuals.SetStartingValues(currentEnemies[i].MaxHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Level);
            tempEntity.BattleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for (int j = 0; j < enemyBattlers.Count; j++)
        {
            enemySelectionButtons[j].SetActive(true);
            enemySelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name;
        }
        // enable buttons for each enemy
        // change the buttons text
    }

    public void SelectEnemy(int currentEnemy)
    {
        // setting current members target
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        // tell the battle system this member intends to attack
        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        // increment through party members 
        currentPlayer++;

        if(currentPlayer >= playerBattlers.Count)
        {
            // start the battle
            // Debug.Log("Start the Battle");
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }
        
    private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
    {
        // get damage (can use an algorithm such as strength * level)
        int damage = currAttacker.Strength;
        // play the attack anim
        currAttacker.BattleVisuals.PlayAttackAnimation();
        // dealing the damage
        currTarget.CurrHealth -= damage;
        // play their hit anim
        currTarget.BattleVisuals.PlayHitAnimation();
        // update the UI
        currTarget.UpdateUI();

        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.Name, currTarget.Name, damage);
    }

    private int GetRandomPartyMember()
    {
        // create a temporary list of type int (index)
        List<int> partyMembers = new List<int>();
        // find all the party members then add them to list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(allBattlers[i].IsPlayer == true)
            {
                partyMembers.Add(i);
            }
        }
        // return a random party member
        return partyMembers[Random.Range(0, partyMembers.Count)];
    }
    
    private int GetRandomEnemy()
    {
        // create temporary list of type int (index)
        List<int> enemies = new List<int>();
        // find all the enemioes then add them to the list
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(allBattlers[i].IsPlayer == false)
            {
                enemies.Add(i);
            }
            // return a random enemy
            return enemies[Random.Range(0, enemies.Count)];
        }
    }

}

[System.Serializable]
public class BattleEntities
{
    public enum Action { Attack, Run }
    public Action BattleAction;

    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int Target;

    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer)
    {
        Name = name;
        CurrHealth = currHealth;
        MaxHealth = maxHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        IsPlayer = isPlayer;
    }

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrHealth);
    }
}
