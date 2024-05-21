using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Zombies;
using System.Collections.Generic;
using System.Linq;

public class PlayerInterfaz : MonoBehaviour
{
    public TextMeshProUGUI nivelTexto;
    public GameObject levelUpMenu; // Referencia al submenú de elección de mejora
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;

    private EntityManager entityManager;
    private Entity playerEntity;



    public PauseMenuScript pauseMenuScript;

    [Header("Barra Vida y EXP")]
    public Slider slider;
    public Gradient colorBarraVida;
    public Image fill;
    public Slider sliderEXP;

    private List<Mejora> mejorasPersonaje;

    private void Start()
    {
        // Definimos las mejoras del personaje
        mejorasPersonaje = new List<Mejora>
        {
            new Mejora(1, "Velocidad", 0.25f),        // 25%
            new Mejora(2, "Daño", 0.30f),             // 30%
            new Mejora(3, "Vida", 0.20f),             // 25%
            new Mejora(4, "Area de Balas", 0.10f),           // 15%
            new Mejora(5, "Número de Balas", 0.05f),  // 5%
            new Mejora(7, "Reducir daño recibido", 0.10f)     // 10%
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




    private void Update()
    {
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
            ShowLevelUpMenu();
            playerDamage.nivelSiguiente++;
            entityManager.SetComponentData(playerEntity, playerDamage); // No olvides actualizar el componente en el EntityManager
        }

        // Para que cambie siempre que le baje la vida
        SetBarraVida(playerDamage.vidaJugador);
        SetBarraExperiencia(playerDamage.experienciaActualJugador);
        SetBarraExperienciaMaxima(playerDamage.experienciaParaProximoNivel);
    }

    public void ShowLevelUpMenu()
    {
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
                    disparoData.incrementoVelocidad += 1f;
                    HideLevelUpMenu();
                    break;
                case 2:
                    // Empieza con 10
                    playerDamage.dañoBalaJugador += 5f;
                    HideLevelUpMenu();
                    break;
                case 3:
                    // Empieza en 100
                    float vidaMaxima = playerDamage.vidaJugador + 10;
                    playerDamage.vidaJugador += 10;
                    SetMaximaBarraVida(vidaMaxima);
                    HideLevelUpMenu();
                    break;
                case 4:
                    // Empieza en 0.2f
                    disparoData.balasSpread += 0.1f;
                    HideLevelUpMenu();
                    break;
                case 5:
                    // Empieza en 1
                    disparoData.numeroBalasPorDisparo += 1;
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

            }

            // Actualizar los datos del jugador en EntityManager
            entityManager.SetComponentData(playerEntity, playerDamage);
            entityManager.SetComponentData(playerEntity, disparoData);
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
