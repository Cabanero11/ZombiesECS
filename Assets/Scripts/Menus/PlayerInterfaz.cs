using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Zombies;

public class PlayerInterfaz : MonoBehaviour
{
    public TextMeshProUGUI nivelTexto;
    private EntityManager entityManager;
    private Entity playerDañoData;
    private Entity playerEntity;

    private IEnumerator Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        yield return new WaitForSeconds(0.2f);

        
    }

    private void Update()
    {
        if(entityManager == null)
        {
            return;
        }

        playerDañoData = entityManager.CreateEntityQuery(typeof(PlayerDañoData)).GetSingletonEntity();

        NativeArray<Entity> entidades = entityManager.GetAllEntities(Allocator.Temp);

        foreach (var entity in entidades)
        {
            if (entityManager.HasComponent<DisparoData>(entity))
            {
                playerEntity = entity;
            }
        }

        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);


        //Debug.Log($"Nivel: {playerDamage.nivelJugador.ToString()}");
        nivelTexto.text = "Nivel :" + playerDamage.nivelJugador.ToString();
    }
}
