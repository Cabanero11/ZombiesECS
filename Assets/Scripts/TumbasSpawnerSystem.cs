using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Zombies
{

    // Para inicializar antes del System de Grupo inicial
    
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TumbasSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CementerioData>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Para crear las tumbas al principio solo, paro el sistema al principio
            state.Enabled = false;

            var cementerioEntity = SystemAPI.GetSingletonEntity<CementerioData>();
            var cementerio = SystemAPI.GetAspect<CementerioAspect>(cementerioEntity);

            // Spawnear Entidades, Usamos ECB en vez del EntityManager (tras realizar varias)
            // instacias se realientiza un poco, asi que usamos un EntityCommandBuffer

            // Allocator.Temp, temporal 
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);



            for (var i = 0; i < cementerio.NumberOfTombstoneToSpawn; i++)
            {
                // Instanciamos los prefabs de las Tumbas
                var nuevaTumba = entityCommandBuffer.Instantiate(cementerio.tumbaPrefab);

                var nuevaTumbaTransform = cementerio.GetRandomTumbaTransform();

                // Pasamos al ECB la nuevaTumba creada y su posicion aleatoria
                entityCommandBuffer.SetComponent(nuevaTumba, nuevaTumbaTransform);
                

            }


            entityCommandBuffer.Playback(state.EntityManager);
        }


    }


}

