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


        [Header("Audio Sources")]
        public AudioSource audioSource;         // El sonido que se usara
        public AudioSource audioSourceMusic;    // Este playea la musica

        [Header("Sonidos del Jugador")]
        public AudioClip disparoSonido;
        public AudioClip levelUpSonido;
        public AudioClip recibirDañoJugador;

        [Header("Musica del videojuego")]
        public AudioClip musicaMenu;
        public AudioClip musicaNivel;
        private AudioSource currentSoundSource; // Almacena la referencia al audioSource del sonido actualmente en reproducción



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

        public void MusicaDeMenu()
        {
            Debug.Log("Menu musica ok");
            PlayLevelMusic(1, 0.5f);
        }


        // PARTE SONIDOS Y MUSICA

        public void PlayDisparoSonido()
        {
            PlaySoundPitcheado(disparoSonido, 0.1f, 0.4f, 1.2f);
        }

        public void PlayLevelUpSonido()
        {
            PlaySoundPitcheado(levelUpSonido, 0.35f, 0.8f, 1.3f);
        }

        public void PlayRecibiDañoJugador()
        {
            PlaySoundPitcheado(recibirDañoJugador, 0.25f, 0.6f, 1.4f);
        }

        // Playear un sonido, que se le pasa, y tocar su volumen
        public void PlaySound(AudioClip sound, float volume)
        {
            if (audioSource != null && sound != null)
            {
                StopCurrentSoundClip(); // Detiene el sonido actual antes de reproducir uno nuevo

                audioSource.clip = sound;
                audioSource.volume = volume;
                audioSource.Play();

                currentSoundSource = audioSource; // Almacena la referencia al AudioSource del sonido actual
            }
        }

        // Parar un sonido de PlaySound() o PlaySoundPitcheado()
        public void StopCurrentSoundClip()
        {
            if (currentSoundSource != null)
            {
                currentSoundSource.Stop();
                currentSoundSource = null; // Limpia la referencia al AudioSource del sonido actual
            }
        }

        // Playear un sonido, con un pitch entre un rango, para variar el sonido cada vez :O
        // tipo -> PlaySoundPitcheado(deathSound, 1f, 0.8f, 1.2f);
        public void PlaySoundPitcheado(AudioClip sound, float volume, float pitchMin, float pitchMax)
        {
            if (sound != null)
            {
                StopCurrentSoundClip(); // Detiene el sonido actual antes de reproducir uno nuevo

                audioSource.clip = sound;

                float pitch = Random.Range(pitchMin, pitchMax);
                audioSource.pitch = pitch;
                audioSource.volume = volume;

                audioSource.Play();

                currentSoundSource = audioSource; // Almacena la referencia al AudioSource del sonido actual
            }
        }


        // LLamar al PlayLevelMusic(1, 2, 3.....) segun el nivel y como lo tengamos puesto
        // Esto lo puse en GameManager, no se funciona aun xd
        public void PlayLevelMusic(int level, float volumen)
        {
            StopCurrentLevelMusic(); // Se para la musica antes de empezar otra
            AudioClip levelMusic = GetLevelMusic(level); // Con la funcion GetLevelMusic() para ver que nivel

            if (levelMusic != null && audioSourceMusic != null)
            {
                //MusicClipManager.instance.SaveCurrentMusicClip(levelMusic);
                audioSourceMusic.clip = levelMusic;
                audioSourceMusic.loop = true;
                audioSourceMusic.volume = volumen;
                audioSourceMusic.Play();
            }
        }

        // Detiene la música de fondo del nivel actual
        public void StopCurrentLevelMusic()
        {
            if (audioSourceMusic != null)
            {
                audioSourceMusic.loop = false;
                audioSourceMusic.Stop();
            }
        }


        // Obtiene el clip de música correspondiente al nivel
        private AudioClip GetLevelMusic(int level)
        {
            // Añadir mas casos al switch segun los niveles
            switch (level)
            {
                case 1:
                    return musicaMenu;
                case 2:
                    return musicaNivel;
                default:
                    return musicaNivel;
            }
         
        }
    }

}
