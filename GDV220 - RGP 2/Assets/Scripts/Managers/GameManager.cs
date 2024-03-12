using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Game_Pause, 
        Game_PrepPhase,
        Game_CombatPhase,
    }

    [Header("Game Vars")]
    [HideInInspector] public GameState currentState;    // Prep or Combat
    [HideInInspector] public bool bIsPaused;            // Checks if game paused or not
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameEndScreen;  // This is both victory and defeat

    public GameObject playerTurnSign; 
    public GameObject enemyTurnSign;  
    public GameObject combatPhaseSign;

    [HideInInspector] public bool bTurnFinished = false;
    public int iTurnCounter = 0;

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
        // Pause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeGameState();
        }

        if (bTurnFinished) return;
    }


    //George put this here, sorry for intruding:)
        // win loss checked in Battlefield.ResolveBattles() !!!!!
    //public void StartGameRound()
    //{
    //    //round tiemr up?
    //
    //    //Player.PlayTurn
    //
    //    //enemy turns
    //
    //
    //    //COMBAT PHASE
    //    //StartCombatPhase
    //
    //    CheckWinLoss();
    //}

    public IEnumerator PlayGameLoop()
    {
        yield return null;
    }

    //Checks if player, or all three enemies have died
    //returns 0 for no win or loss
    //Returns 1 for player loss
    //returns 2 for player victory
    //sorry, inneficient coding here...
        //NO GEORGE YOU AND YOUR CODE ARE BOTH GEORGEOUS
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

        if (Battlefield.instance.enemies[0]) 
        {
            enemyCounter++;
            if(Battlefield.instance.enemies[0].iCurrentHealth <= 0)
            {
                enemyDeathCounter++;
            }
        }
        if (Battlefield.instance.enemies[1])
        {
            enemyCounter++;
            if (Battlefield.instance.enemies[1].iCurrentHealth <= 0)
            {
                enemyDeathCounter++;
            }
        }
        if (Battlefield.instance.enemies[2])
        {
            enemyCounter++;
            if (Battlefield.instance.enemies[2].iCurrentHealth <= 0)
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
                playerTurnSign.SetActive(true);
                bTurnFinished = false;
                iTurnCounter++;

                Player.instance.StartTurn();

                break;

            case GameState.Game_CombatPhase:
                // Disable clicking / activating cards
                    // done; if-check in card base - teddy

                // Play battle
                StartCoroutine(Battlefield.instance.CommenceBattle());

                break;

            default:
                break;
        }
    }

    public void EndTurn()
    {
        // Do nothing if player turn is already over
        if (bTurnFinished) return;
        GameManager.instance.playerTurnSign.SetActive(false);
        bTurnFinished = true;

        //Say Hi to Georg ((HI GEORG!!!))
        // Call function for enemies to play their turns here
        StartCoroutine(Battlefield.instance.PlayEnemyTurns());

        //state changes in the playenemyturns function!!!!!!!!
            // OK YES THANK YOU SIR!!!!
        //ChangeGameState(GameState.Game_CombatPhase);
    }
    
    // Ends game if player has won/lost
    public void GameEnd(int _iEndState)
    {
        // 0 = no win/loss
        if (_iEndState == 0) return;

        // Pause but without the pause screen
        ChangeGameState();
        pauseMenu.SetActive(false);

        // Display victory/defeat overlay
        // Child 0 = defeat screen, child 1 = victory screen
        gameEndScreen.transform.GetChild(_iEndState - 1).gameObject.SetActive(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}