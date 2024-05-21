using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Zombies
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public int numeroEscena;

        [Header("Estado de juego")]
        public GameState gameState;

        [Header("Referencias")]
        public PauseMenuScript pauseMenuScript;

        [SerializeField] private string sceneName;
        [SerializeField] public bool musicPlayedForCurrentLevel = false; // Para que solo se reproduzca una vez en el nivel actual



        public enum GameState
        {
            menu,
            PauseMenu,
            level,
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(Instance);

            pauseMenuScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenuScript>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Start is called before the first frame update
        void Start()
        {
            gameState = GameState.level;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.level)
            {
                //pauseMenuScript.TogglePauseMenu();
                //pauseMenuScript.GetComponent<PauseMenuScript>().enabled = true;
                gameState = GameState.PauseMenu;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.PauseMenu)
            {
                //pauseMenuScript.TogglePauseMenu();
                gameState = GameState.level;
            }
        }

        //Se llama desde TimerSystem
        public void EndLevel(bool win/*if player won the game else: menu exit*/)
        {
            if (win)
            {
                PlayerAudioManager.instance.PlayWinLevel();
            }

            StartCoroutine(delay());
        }
        //Cambiar de nivel (actualmente simplemente el siguiente en la lista de niveles)
        public void ChangeLevel()
        {
            //Debug.Log("vamos al nivel " + currentLevel + " de los [0.."+(levels.Length-1)+"]");
            musicPlayedForCurrentLevel = false; // cambiamos de nivel, y ya puede haber nueva musica
        }

        public void SetLevel(int level)
        {
            //Debug.Log("vamos al nivel " + currentLevel);
            //Debug.Log(levels.Length);
            musicPlayedForCurrentLevel = false;

        }

        IEnumerator delay()
        {
            yield return new WaitForSeconds(3f);
            ChangeLevel();
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Actualizar el nombre de la escena en el GameManager
            sceneName = scene.name;
            Debug.Log(sceneName);

            musicPlayedForCurrentLevel = false;
        }

        public void AlternarEntre2Escenas()
        {
            // Para que cambie entre una o la otra ajaj
            if(numeroEscena % 2 == 0)
            {
                SceneManager.LoadScene("ZombiesMain2");
                numeroEscena++;
            } 
            else if(numeroEscena % 2 == 1)
            {
                SceneManager.LoadScene("ZombiesMain");
                numeroEscena++;
            }
        }

        public void SonidosDelJugador()
        {

        }

    }

}
