using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
            var tumbasOffset = new float3(0f, -2f, 0f); // Para que los zombies salgan debajo de la tumba

            // Spawnear Entidades, Usamos ECB en vez del EntityManager (tras realizar varias)
            // instacias se realientiza un poco, asi que usamos un EntityCommandBuffer

            // Allocator.Temp, temporal 
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);


            // Blob, hay que crear un builder y asignarle la memoria de los objetos
            var builder = new BlobBuilder(Allocator.Temp);
            ref var zombiesSpawn = ref builder.ConstructRoot<ZombiesSpawnBlob>();
            var arrayBuilder = builder.Allocate(ref zombiesSpawn.positionValueBlob, cementerio.NumberOfTombstoneToSpawn);


            for (var i = 0; i < cementerio.NumberOfTombstoneToSpawn; i++)
            {
                // Instanciamos los prefabs de las Tumbas
                var nuevaTumba = entityCommandBuffer.Instantiate(cementerio.tumbaPrefab);

                var nuevaTumbaTransform = cementerio.GetRandomTumbaTransform();

                // Pasamos al ECB la nuevaTumba creada y su posicion aleatoria
                entityCommandBuffer.SetComponent(nuevaTumba, nuevaTumbaTransform);


                // Creamos puntos de Spawn de los Zombies, y lo asignamos al arrayBuilder esas posiciones
                var puntoSpawnZombies = nuevaTumbaTransform.Position + tumbasOffset;
                arrayBuilder[i] = puntoSpawnZombies;

            }

            // Referencia al asset del Blob que usamos
            var assetBlob = builder.CreateBlobAssetReference<ZombiesSpawnBlob>(Allocator.Persistent);
            entityCommandBuffer.SetComponent(cementerioEntity, new ZombiesSpawn { positionValue = assetBlob });
            builder.Dispose();

            entityCommandBuffer.Playback(state.EntityManager);
        }


    }


}

