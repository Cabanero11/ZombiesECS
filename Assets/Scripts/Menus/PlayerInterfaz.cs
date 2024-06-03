 using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Zombies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Scenes;
using Unity.Physics;

public class PlayerInterfaz : MonoBehaviour
{
    [Header("Menus")]
    public TextMeshProUGUI nivelTexto;
    public TextMeshProUGUI puntuacionTexto;
    public TextMeshProUGUI FPStexto;
    private float deltaTime;
    public GameObject levelUpMenu;      // Referencia al submenú de elección de mejora
    public GameObject gameOverMenu;     // Referencia al submenú de GameOver
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    private bool estaEnUnMenu = false;

    private EntityManager entityManager;
    private Entity playerEntity;

    public PauseMenuScript pauseMenuScript;

    [Header("Barra Vida y EXP")]
    public Slider slider;
    public Gradient colorBarraVida;
    public Image fill;
    public Slider sliderEXP;

    private List<Mejora> mejorasPersonaje;

    // Los usare aqui por usar GameManager.Instance.PlaySonido(), es llamar a una funcion
    // estatica en Burst y eso no es correcto
    [Header("Sonidos")]
    public AudioClip disparoSonido;
    public AudioSource audioSource;
    private AudioSource currentSoundSource; // Almacena la referencia al audioSource del sonido actualmente en reproducción
    private float temporizadorDisparo = 0f;




    private void Start()
    {
        // Definimos las mejoras del personaje
        mejorasPersonaje = new List<Mejora>
        {
            new Mejora(1, "Velocidad", 0.20f),                  // 20%
            new Mejora(2, "Daño", 0.20f),                       // 20%
            new Mejora(3, "Vida", 0.20f),                       // 20%
            new Mejora(4, "Area de Balas", 0.10f),              // 10%
            new Mejora(5, "Número de Balas", 0.10f),            // 10%
            new Mejora(7, "Reducir daño recibido", 0.10f),      // 10%
            new Mejora(8, "Cadencia Disparo", 0.10f)            // 10%
        };

        StartCoroutine(InitializeAfterDelay());
    }

    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager == null)
        {
            Debug.LogError("EntityManager is not available.");
            yield break;
        }

        // Buscar la entidad del jugador una vez al inicio
        EntityQuery playerQuery = entityManager.CreateEntityQuery(typeof(PlayerDañoData));
        if (playerQuery.CalculateEntityCount() == 0)
        {
            Debug.LogError("No PlayerDañoData entity found.");
            yield break;
        }

        playerEntity = playerQuery.GetSingletonEntity();

        if (playerEntity == Entity.Null)
        {
            Debug.LogError("Player entity is null.");
            yield break;
        }

        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

        SetMaximaBarraVida(playerDamage.vidaJugador);
    }


    private void Update()
    {
        // Calcular el tiempo entre frames para mostrar los FPS luego
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = (1.0f / deltaTime);

        // Muestra los FPS en el componente Text
        FPStexto.text = string.Format("FPS: {0:0.} ", Mathf.Ceil(fps));
        //FPStexto.text = Mathf.Ceil(fps).ToString()

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager == null || playerEntity == Entity.Null)
        {
            //Debug.LogError("EntityManager or playerEntity is null.");
            return;
        }

        if (!entityManager.HasComponent<PlayerDañoData>(playerEntity))
        {
            Debug.LogError("Player entity does not have PlayerDañoData component.");
            return;
        }

        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
        DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);



        if (playerDamage.Equals(default(PlayerDañoData)))
        {
            Debug.LogError("PlayerDañoData is null or default.");
            return;
        }

        nivelTexto.text = "Nivel: " + playerDamage.nivelJugador.ToString();

        // Verificar si el menú de pausa está presente y habilitar las funciones de pausa/reanudación
        if (pauseMenuScript != null && Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuScript.TogglePauseMenu();
        }

        // Si nivel jugador 1 aumenta a 2 sera nivelSiguiente ==, entonces mostramos menu de Subir nivel + incrementar nivelSiguiente
        if (playerDamage.nivelJugador == playerDamage.nivelSiguiente)
        {
            // Mostrar menu de SubirNivel y Sonid
            GameManager.Instance.PlayLevelUpSonido();
            ShowLevelUpMenu();
            playerDamage.nivelSiguiente++;
            entityManager.SetComponentData(playerEntity, playerDamage); // No olvides actualizar el componente en el EntityManager
        }

        // Para que cambie siempre que le baje la vida
        SetBarraVida(playerDamage.vidaJugador);
        SetBarraExperiencia(playerDamage.experienciaActualJugador);
        SetBarraExperienciaMaxima(playerDamage.experienciaParaProximoNivel);

        if (playerDamage.vidaJugador <= 0)
        {
            GameOverScreen();
        }

        float vidaAnterior = playerDamage.vidaJugador;

        // Si su vida actual es menor que la anterior, es que recibio daño
        if (playerDamage.vidaJugador < vidaAnterior)
        {
            GameManager.Instance.PlayRecibiDañoJugador();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Menu de pausa
            estaEnUnMenu = true;
        }


        temporizadorDisparo -= Time.deltaTime;

        if (Input.GetMouseButton(0) && !estaEnUnMenu)
        {
            if (temporizadorDisparo <= 0f)
            {
                PlayDisparoSonido();
                temporizadorDisparo = disparoData.temporizadorDisparo;
            }
        }
        else
        {
            temporizadorDisparo = 0f; // Reiniciar el temporizador cuando se suelta el botón o está en un menú
        }
    }


    public void SetBarraVida(float vidaPersonaje)
    {
        slider.value = vidaPersonaje;

        // Para coger el valor del slider, y segun este cambiar su color
        fill.color = colorBarraVida.Evaluate(slider.normalizedValue);
    }


    public void SetMaximaBarraVida(float vidaPersonaje)
    {
        slider.maxValue = vidaPersonaje;
        //slider.value = vidaPersonaje;

        fill.color = colorBarraVida.Evaluate(1f);
    }
    public void SetBarraExperiencia(float experienciaActual)
    {
        sliderEXP.value = experienciaActual;
    }

    public void SetBarraExperienciaMaxima(float experienciaMaxima)
    {
        sliderEXP.maxValue = experienciaMaxima;
    }




    
    // Activar el menu de GameOver, pausar el juego y poner valor de puntuacion :D
    public void GameOverScreen()
    {
        estaEnUnMenu = true;
        gameOverMenu.SetActive(true);
        levelUpMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
        puntuacionTexto.text = "Puntuación: " + playerDamage.puntuacion.ToString();
    }

    public void ReiniciarJuego()
    {
        // Cargar una escena igual a esta y ir alternando para que asi se vuelvan a cargar todos los sistemas 
        // En mi mente tenia sentido
        Time.timeScale = 1f;
        SceneManager.LoadScene("ZombiesMain");
    }

    public void VolverAlMenu()
    {
        estaEnUnMenu = false;
        SceneManager.LoadScene("Menu");
    }


    public void ShowLevelUpMenu()
    {
        estaEnUnMenu = true;
        levelUpMenu.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ShowMenu");
        // Aquí podemos actualizar las opciones del menú según las probabilidades
        UpdateLevelUpOptions();
    }

    public void HideLevelUpMenu()
    {
        estaEnUnMenu = false;
        levelUpMenu.SetActive(false);
        Debug.Log("HideLevel");
        Time.timeScale = 1f; // Reanudar el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateLevelUpOptions()
    {
        // Seleccionar mejoras aleatoriamente
        List<Mejora> opcionesSeleccionadas = SeleccionarMejorasAleatoriamente(3);

        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = opcionesSeleccionadas[0].nombre;
        option1Button.onClick.RemoveAllListeners();
        option1Button.onClick.AddListener(() => SelectUpgrade(opcionesSeleccionadas[0].id));

        option2Button.GetComponentInChildren<TextMeshProUGUI>().text = opcionesSeleccionadas[1].nombre;
        option2Button.onClick.RemoveAllListeners();
        option2Button.onClick.AddListener(() => SelectUpgrade(opcionesSeleccionadas[1].id));

        option3Button.GetComponentInChildren<TextMeshProUGUI>().text = opcionesSeleccionadas[2].nombre;
        option3Button.onClick.RemoveAllListeners();
        option3Button.onClick.AddListener(() => SelectUpgrade(opcionesSeleccionadas[2].id));
    }

    // Para seleccionar las mejoras del Jugador de forma aleatoria y que usen la probabilidad que tienen
    private List<Mejora> SeleccionarMejorasAleatoriamente(int cantidad)
    {
        List<Mejora> seleccionadas = new List<Mejora>();
        float totalProbabilidad = mejorasPersonaje.Sum(m => m.probabilidad);

        for (int i = 0; i < cantidad; i++)
        {
            float randomPoint = Random.value * totalProbabilidad;
            float acumulado = 0;

            foreach (Mejora mejora in mejorasPersonaje)
            {
                acumulado += mejora.probabilidad;
                if (acumulado >= randomPoint)
                {
                    if(!seleccionadas.Contains(mejora))
                    {
                        seleccionadas.Add(mejora);
                    }

                    break;
                }
            }
        }

        return seleccionadas;
    }

    public void SelectUpgradeOption1()
    {
        SelectUpgrade(1);
    }

    public void SelectUpgradeOption2()
    {
        SelectUpgrade(2);
    }

    public void SelectUpgradeOption3()
    {
        SelectUpgrade(3);
    }

    private void SelectUpgrade(int option)
    {
        if (playerEntity == Entity.Null)
        {
            Debug.LogError("Player entity is null.");
            // Si es null la vuelvo a buscar
            EntityQuery playerQuery = entityManager.CreateEntityQuery(typeof(PlayerDañoData));
            playerEntity = playerQuery.GetSingletonEntity();
        }

        Debug.Log("SelectUpgrade antes if");

        // Implementar la lógica para aplicar la mejora seleccionada al jugador
        if (entityManager.HasComponent<PlayerDañoData>(playerEntity))
        {
            PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
            DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);
            Debug.Log("SelectUpgrade antes switch");

            switch (option)
            {
                // Se inicializa el valor en DisparoMono
                case 1:
                    // Empieza con 8
                    disparoData.incrementoVelocidad += 1.5f;
                    HideLevelUpMenu();
                    break;
                case 2:
                    // Dispara 10 balas al empezar 10*2 = 20 daño
                    playerDamage.dañoBalaJugador += 2f;
                    HideLevelUpMenu();
                    break;
                case 3:
                    // Empieza en 100
                    float vidaMaxima = playerDamage.vidaJugador + 20;
                    playerDamage.vidaJugador += 20;
                    SetMaximaBarraVida(vidaMaxima);
                    HideLevelUpMenu();
                    break;
                case 4:
                    // Empieza en 0.2f
                    disparoData.balasSpread += 0.15f;
                    HideLevelUpMenu();
                    break;
                case 5:
                    // Empieza en 1
                    disparoData.numeroBalasPorDisparo += 2;
                    HideLevelUpMenu();
                    break;
                case 7:
                    // Poder reducir el daño al jugador, 
                    // Empieza en 12,5 , -2.5 -> 5 mejoras
                    if(playerDamage.dañoAlJugador <= 0) 
                    {
                        playerDamage.dañoAlJugador = 2.5f;
                    } 
                    else if(playerDamage.dañoAlJugador > 0)
                    {
                        playerDamage.dañoAlJugador -= 2.5f;
                    }
                    
                    HideLevelUpMenu();
                    break;
                case 8:
                    // Tiempo de disparo empieza en 0.2f
                    if (disparoData.temporizadorDisparo <= 0.05f)
                    {
                        //disparoData.cooldownDisparo = 0.05f;
                        disparoData.temporizadorDisparo = 0.05f;
                    }
                    else if (disparoData.temporizadorDisparo > 0f)
                    {
                        //disparoData.cooldownDisparo -= 0.03f;
                        disparoData.temporizadorDisparo -= 0.03f;
                    }
                    HideLevelUpMenu();
                    break;

            }

            // Actualizar los datos del jugador en EntityManager
            entityManager.SetComponentData(playerEntity, playerDamage);
            entityManager.SetComponentData(playerEntity, disparoData);
        }

        
    }

    public void PlayDisparoSonido()
    {
        PlaySoundPitcheado(disparoSonido, 0.1f, 0.4f, 1.2f);
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


}

public class Mejora
{
    public int id;
    public string nombre;
    public float probabilidad;

    public Mejora(int id, string nombre, float probabilidad)
    {
        this.id = id;
        this.nombre = nombre;
        this.probabilidad = probabilidad;
    }
}
