using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Zombies;


// Clase que permite desde el MonoBehaviour obtener la entidad del jugador y obtener el PlayerDañoData
// Y asi mostrar los stats del jugador

public class PlayerInterfaz : MonoBehaviour
{
    public TextMeshProUGUI nivelTexto;
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
    }

    private void Update()
    {
        if (entityManager == null || playerEntity == Entity.Null)
        {
            return;
        }

        if (entityManager.HasComponent<PlayerDañoData>(playerEntity))
        {
            PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);
            nivelTexto.text = "Nivel: " + playerDamage.nivelJugador.ToString();
        }

        // Verificar si el menú de pausa está presente y habilitar las funciones de pausa/reanudación
        if (pauseMenuScript != null && Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuScript.TogglePauseMenu();
        }
    }
}