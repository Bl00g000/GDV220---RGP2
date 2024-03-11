using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Game_Pause, 
        Game_PrepPhase,
        Game_CombatPhase,
    }

    [Header("Game States")]
    [HideInInspector] public GameState currentState;    // Prep or Combat
    [HideInInspector] public bool bIsPaused;            // Checks if game paused or not
    [SerializeField] private GameObject pauseMenu;
    private bool bCombatPlaying = false;
    private bool bTurnFinished = false;

    [Header("Game Vars")]
    public int iTurnCounter = 0;
    [SerializeField] private float fMaxTurnTime = 30.0f;

    // Turn events in case we need them for cards
    // Currently unused
    public event Action onTurnStart;
    public event Action onTurnEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bIsPaused = false;
        ChangeGameState(GameState.Game_PrepPhase);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeGameState();
        }

        if (currentState == GameState.Game_PrepPhase)
        {

        }

        if (bTurnFinished) return;
    }


    //George put this here, sorry for intruding:)
    public void StartGameRound()
    {
        //round tiemr up?

        //Player.PlayTurn

        //enemy turns


        //COMBAT PHASE
        //StartCombatPhase

        CheckWinLoss();
        
    }

    public IEnumerator PlayGameLoop()
    {
        yield return null;
    }

    //Checks if player, or all three enemies have died
    //returns 0 for no win or loss
    //Returns 1 for player loss
    //returns 2 for player victory
    //sorry, inneficient coding here...
    public int CheckWinLoss()
    {
        //PLAYER DEATH CHECKING
        if(Player.instance.iCurrentHealth <= 0)
        {
        Debug.Log("LOSS");
        return 1;     //player loss
        }

        //ENEMY DEATH CHECKING
        int enemyCounter = 0;
        int enemyDeathCounter = 0;

        if (Battlefield.instance.Enemy1) 
        {
            enemyCounter++;
            if(Battlefield.instance.Enemy1.iCurrentHealth <= 0)
            {
                enemyDeathCounter++;
            }
        }
        if (Battlefield.instance.Enemy2)
        {
            enemyCounter++;
            if (Battlefield.instance.Enemy2.iCurrentHealth <= 0)
            {
                enemyDeathCounter++;
            }
        }
        if (Battlefield.instance.Enemy3)
        {
            enemyCounter++;
            if (Battlefield.instance.Enemy3.iCurrentHealth <= 0)
            {
                enemyDeathCounter++;
            }
        }

        if(enemyDeathCounter == enemyCounter)
        {
            Debug.Log("Victory!");
            return 2;   //Player victory
        }


        return 0;   //no win or loss
    }

    public void ChangeGameState(GameState _newState = GameState.Game_Pause)
    {
        switch (_newState)
        {
            case GameState.Game_Pause:
                if (bIsPaused)  // Resume game
                {
                    Time.timeScale = 1.0f;
                    bIsPaused = false;

                    // Deactivate pause menu UI
                    pauseMenu.SetActive(false);
                }
                else            // Pause game
                {
                    Time.timeScale = 0.0f;
                    bIsPaused = true;

                    // Activate pause menu UI
                    pauseMenu.SetActive(true);
                }
                break;

            case GameState.Game_PrepPhase:
                currentState = _newState;
                bTurnFinished = false;
                iTurnCounter++;

                // Teddy playing with the card manager scale
                    // To try make it smaller during combat rather than disappearing
                    // But pivot/anchor is in the middle :')
                //CardManager.instance.transform.localScale = new Vector3(1,1,1);
                //CardManager.instance.gameObject.SetActive(true);

                Player.instance.StartTurn();

                // Still need to add enemies playing their cards (check end turn func)

                break;

            case GameState.Game_CombatPhase:
                currentState = _newState;
                bTurnFinished = true;

                // Disable clicking / activating cards
                // done; if-check in card base - teddy

                //CardManager.instance.transform.localScale *= 0.5f;
                //CardManager.instance.gameObject.SetActive(false);

                // Play battle
                StartCoroutine(Battlefield.instance.ResolveBattles(0));
                StartCoroutine(Battlefield.instance.ResolveBattles(1));
                StartCoroutine(Battlefield.instance.ResolveBattles(2));
                // Calculate damages etc

                break;

            default:
                Debug.Log("No new game state found.");
                break;
        }
    }

    public void EndTurn()
    {
        Debug.Log("Turn ended.");


        //Say Hi to Georg

        // Call function for enemies to play their turns here
        StartCoroutine(Battlefield.instance.PlayEnemyTurns());

        //state changes in the playenemyturns function!!!!!!!!
        //ChangeGameState(GameState.Game_CombatPhase);
    }
}
