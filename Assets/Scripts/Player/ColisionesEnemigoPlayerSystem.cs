using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Zombies;

[BurstCompile]
public partial struct ColisionesEnemigoPlayerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Obtener el jugador
        Entity playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        PlayerDa�oData playerDamage = entityManager.GetComponentData<PlayerDa�oData>(playerEntity);

        float distanciaDeColision = 1.0f;
        float distanciaDeColisionDropVida = 2.0f;

        // Obtener todos los enemigos
        var enemigosQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemigosPropiedades>(), ComponentType.ReadOnly<LocalTransform>());
        NativeArray<Entity> enemigos = enemigosQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> enemigosTransforms = enemigosQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        // Obtener todos los drops de vida
        var dropsVidaQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DropVidaPropiedades>(), ComponentType.ReadOnly<LocalTransform>());
        NativeArray<Entity> dropsVida = dropsVidaQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> dropsVidaTransforms = dropsVidaQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        NativeArray<DropVidaPropiedades> dropsVidaPropiedadesArray = dropsVidaQuery.ToComponentDataArray<DropVidaPropiedades>(Allocator.Temp);

        // Detectar colisiones con enemigos
        for (int i = 0; i < enemigos.Length; i++)
        {
            LocalTransform enemigoTransform = enemigosTransforms[i];

            // Comprobar colisi�n simple (puedes mejorar esto con una detecci�n de colisi�n m�s precisa)
            if (math.distance(enemigoTransform.Position, playerTransform.Position) < distanciaDeColision)
            {
                // Aplica da�o al jugador
                playerDamage.vidaJugador -= playerDamage.da�oAlJugador;
                entityManager.SetComponentData(playerEntity, playerDamage);

                // Sonido de recibirDa�o
                GameManager.Instance.PlayRecibiDa�oJugador();

                // Si la vida es menor que 0 se muere el jugador :(
                if (playerDamage.vidaJugador <= 0)
                {
                    playerDamage.jugadorMuerto = true;
                }

                // Destruir el enemigo
                entityManager.DestroyEntity(enemigos[i]);
            }
        }

        // Detectar colisiones con drops de vida
        for (int i = 0; i < dropsVida.Length; i++)
        {
            LocalTransform dropVidaTransform = dropsVidaTransforms[i];

            // Comprobar colisi�n simple (puedes mejorar esto con una detecci�n de colisi�n m�s precisa)
            if (math.distance(dropVidaTransform.Position, playerTransform.Position) < distanciaDeColisionDropVida)
            {
                // Recuperar vida del jugador
                playerDamage.vidaJugador += dropsVidaPropiedadesArray[i].vidaRecuperada;
                entityManager.SetComponentData(playerEntity, playerDamage);

                // Sonido de recibir vida
                GameManager.Instance.PlayRecuperarVida();

                // Destruir el drop de vida
                entityManager.DestroyEntity(dropsVida[i]);
            }
        }

        // Actualizar datos del jugador
        entityManager.SetComponentData(playerEntity, playerDamage);

        // Liberar arrays nativos
        enemigos.Dispose();
        enemigosTransforms.Dispose();
        dropsVida.Dispose();
        dropsVidaTransforms.Dispose();
        dropsVidaPropiedadesArray.Dispose();
    }
}
