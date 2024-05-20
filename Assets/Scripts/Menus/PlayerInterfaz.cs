using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Zombies;

public class PlayerInterfaz : MonoBehaviour
{
    public TextMeshProUGUI nivelTexto;
    public GameObject levelUpMenu; // Referencia al submen� de elecci�n de mejora
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;

    private EntityManager entityManager;
    private Entity playerEntity;

    public PauseMenuScript pauseMenuScript;

    private void Start()
    {
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
        EntityQuery playerQuery = entityManager.CreateEntityQuery(typeof(PlayerDa�oData));
        if (playerQuery.CalculateEntityCount() == 0)
        {
            Debug.LogError("No PlayerDa�oData entity found.");
            yield break;
        }

        playerEntity = playerQuery.GetSingletonEntity();

        if (playerEntity == Entity.Null)
        {
            Debug.LogError("Player entity is null.");
            yield break;
        }
    }



    private void Update()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager == null || playerEntity == Entity.Null)
        {
            Debug.LogError("EntityManager or playerEntity is null.");
            return;
        }

        if (!entityManager.HasComponent<PlayerDa�oData>(playerEntity))
        {
            Debug.LogError("Player entity does not have PlayerDa�oData component.");
            return;
        }

        PlayerDa�oData playerDamage = entityManager.GetComponentData<PlayerDa�oData>(playerEntity);

        if (playerDamage.Equals(default(PlayerDa�oData)))
        {
            Debug.LogError("PlayerDa�oData is null or default.");
            return;
        }

        nivelTexto.text = "Nivel: " + playerDamage.nivelJugador.ToString();

        // Verificar si el men� de pausa est� presente y habilitar las funciones de pausa/reanudaci�n
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
    }

    public void ShowLevelUpMenu()
    {
        levelUpMenu.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ShowMenu");
        // Aqu� podemos actualizar las opciones del men� seg�n las probabilidades
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
        // Aqu� puedes implementar la l�gica para elegir las opciones de mejora
        // seg�n las probabilidades que has mencionado.
        // Por ejemplo:

        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = "Mejora de Velocidad";
        option2Button.GetComponentInChildren<TextMeshProUGUI>().text = "Mejora de Da�o";
        option3Button.GetComponentInChildren<TextMeshProUGUI>().text = "Mejora de Vida";
    }

    private void SelectUpgrade(int option)
    {
        if (playerEntity == Entity.Null)
        {
            Debug.LogError("Player entity is null.");
            // Si es null la vuelvo a buscar
            EntityQuery playerQuery = entityManager.CreateEntityQuery(typeof(PlayerDa�oData));
            playerEntity = playerQuery.GetSingletonEntity();
        }

        Debug.Log("SelectUpgrade antes if");

        // Implementar la l�gica para aplicar la mejora seleccionada al jugador
        if (entityManager.HasComponent<PlayerDa�oData>(playerEntity))
        {
            PlayerDa�oData playerDamage = entityManager.GetComponentData<PlayerDa�oData>(playerEntity);
            DisparoData disparoData = entityManager.GetComponentData<DisparoData>(playerEntity);
            Debug.Log("SelectUpgrade antes switch");

            switch (option)
            {
                case 1:
                    // Se inicializa el valor en DisparoMono
                    disparoData.incrementoVelocidad += 2f;
                    HideLevelUpMenu();
                    break;
                case 2:
                    playerDamage.da�oBalaJugador += 5f;
                    HideLevelUpMenu();
                    break;
                case 3:
                    playerDamage.vidaJugador += 10;
                    HideLevelUpMenu();
                    break;
                    // A�adir m�s casos seg�n las mejoras que quieras implementar
            }

            // Actualizar los datos del jugador en EntityManager
            entityManager.SetComponentData(playerEntity, playerDamage);
            entityManager.SetComponentData(playerEntity, disparoData);
        }

        
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
}
