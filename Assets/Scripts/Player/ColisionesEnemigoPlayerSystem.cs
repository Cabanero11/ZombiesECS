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
        PlayerDañoData playerDamage = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

        Entity dropSpawnerEntity = SystemAPI.GetSingletonEntity<DropVidaData>();
        DropVidaPropiedades dropVidaPropiedades = entityManager.GetComponentData<DropVidaPropiedades>(dropSpawnerEntity);

        float distanciaDeColision = 1.0f;

        // Obtener todos los enemigos
        NativeArray<Entity> enemigos = entityManager.GetAllEntities(Allocator.Temp);
        NativeArray<Entity> dropsVida = entityManager.GetAllEntities(Allocator.Temp);

        foreach (var enemigoEntity in enemigos)
        {
            // Colisiones de enemigos con el Jugador
            if (entityManager.HasComponent<EnemigosPropiedades>(enemigoEntity))
            {
                LocalTransform enemigoTransform = entityManager.GetComponentData<LocalTransform>(enemigoEntity);

                // Comprobar colisión simple (puedes mejorar esto con una detección de colisión más precisa)
                if (math.distance(enemigoTransform.Position, playerTransform.Position) < distanciaDeColision)
                {
                    // Aplica daño al jugador
                    playerDamage.vidaJugador -= playerDamage.dañoAlJugador;
                    entityManager.SetComponentData(playerEntity, playerDamage);

                    // Sonido de recibirDaño
                    GameManager.Instance.PlayRecibiDañoJugador();

                    // Si la vida es menor que 0 se muere el jugador :(
                    if (playerDamage.vidaJugador <= 0)
                    {
                        //entityManager.DestroyEntity(playerEntity);
                        playerDamage.jugadorMuerto = true;
                    }

                    // NO SE SI DESTRUIR ESE ZOMBIE
                    // SI HAY MUCHOS Y SE COMPLICA ESTARIA BIEN JHUM
                    entityManager.DestroyEntity(enemigoEntity);
                }
            }
            // Para el drop de Vida colisiones
            else if (entityManager.HasComponent<DropVidaPropiedades>(enemigoEntity))
            {
                LocalTransform dropVidaTransform = entityManager.GetComponentData<LocalTransform>(enemigoEntity);

                // Comprobar colisión simple (puedes mejorar esto con una detección de colisión más precisa)
                if (math.distance(dropVidaTransform.Position, playerTransform.Position) < distanciaDeColision)
                {

                    // Aplica daño al jugador
                    playerDamage.vidaJugador += dropVidaPropiedades.vidaRecuperada;
                    entityManager.SetComponentData(playerEntity, playerDamage);

                    // Sonido de recibirDaño
                    GameManager.Instance.PlayRecibiDañoJugador();

                    // Si el jugador toca la de vida se cura y se destruye la + de vida
                    entityManager.DestroyEntity(enemigoEntity);
                }

            }
        }

        // Borramos todos los demas, ya colisionamos :D
        enemigos.Dispose();
    }
}