using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance;

    public Bank PlayerBank;

    [SerializeField] private List<string> _sceneNames;

    [SerializeField] private GameObject _pauseMenu;

    private Queue<string> _unplayedMaps;
    

    void Awake(){
        // ensure there is only one instance of this object
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _unplayedMaps = new Queue<string>();
            foreach(string name in _sceneNames){
                _unplayedMaps.Enqueue(name);
            }
        } else {
            Destroy(gameObject);
        }
    }


    public void LoadNextMap(){
        if(_unplayedMaps.Count > 0){
            SceneManager.LoadScene(_unplayedMaps.Dequeue(), LoadSceneMode.Single);
            HidePauseMenu();
        }
    }

    public void ShowPauseMenu(){
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu(){
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
