using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance;

    public Bank PlayerBank;
    public Bank GoblinBank;

    [SerializeField] private List<string> _sceneNames;

    [SerializeField] private GameObject _globalUI;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _gameHUD;

    private Queue<string> _unplayedMaps;

    private int _villagersSaved;

    public int VillagersSaved{
        get=>_villagersSaved;
        set=>_villagersSaved = value;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(_mainMenu.activeSelf){
                // the main menu is open
                HideMainMenu();
            } else {
                // the main menu is currently closed
                ShowMainMenu();
            }
        }
    }
    

    void Awake(){
        // ensure there is only one instance of this object
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_globalUI);

        } else {
            Destroy(gameObject);
        }
    }


    public void LoadNextMap(){
        if(_unplayedMaps.Count > 0){
            SceneManager.LoadScene(_unplayedMaps.Dequeue(), LoadSceneMode.Single);
        }
    }

    public void PauseGame(){
        Time.timeScale = 0f;
    }

    public void ResumeGame(){
        Time.timeScale = 1f;
    }

    public void ShowMainMenu(){
        _mainMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PauseGame();
    }

    public void HideMainMenu(){
        _mainMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        ResumeGame();
    }

    public void NewGame(){

        // reset the player's resources
        PlayerBank.Reset();
        GoblinBank.Reset();
        _villagersSaved = 0;

        // build the list of maps
        _unplayedMaps = new Queue<string>();
        foreach(string name in _sceneNames){
            _unplayedMaps.Enqueue(name);
        }

        // load the first map in the queue
        LoadNextMap();

        // make the continue button and game HUD visible
        _continueButton.SetActive(true);
        _gameHUD.SetActive(true);

        // hide the main menu
        HideMainMenu();

    }

    // Start is called before the first frame update
    void Start()
    {
        // the continue button and game HUD (resource panel) will be hidden initially
        _continueButton.SetActive(false);
        _gameHUD.SetActive(false);

        // ensure the main menu is visible
        _mainMenu.SetActive(true);


    }
}
