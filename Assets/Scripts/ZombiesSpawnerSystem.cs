using System.Diagnostics;
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
    public partial struct ZombiesSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            // Obtener la variable Singleton asociada con la entidad de inicializacion "Begin"
            var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            // Preparar el Job para spawnear Zombies, en el hilo Main()
            new ZombiesSpawnJob
            {
                deltaTimeJob = deltaTime,
                entityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Run();
        }
    }

    // IJob Entity (Para gestionar los Jobs)

    [BurstCompile]
    public partial struct ZombiesSpawnJob : IJobEntity
    {
        public float deltaTimeJob; // el Time.deltaTime; de Unity
        public EntityCommandBuffer entityCommandBuffer;
        private void Execute(CementerioAspect cementerioAspect)
        {
            cementerioAspect.ZombiesSpawnTiempo -= deltaTimeJob;

            // Sino es momento de spawnear
            if(!cementerioAspect.tiempoParaSpawnearZombie)
            {
                return;
            }

            if (!cementerioAspect.hanSpawneadoZombies())
            {
                return;
            }



            cementerioAspect.ZombiesSpawnTiempo = cementerioAspect.cooldownSpawneoZombies;
            var nuevoZombie = entityCommandBuffer.Instantiate(cementerioAspect.ZombiePrefab);


            var nuevaPosicionZombie = cementerioAspect.getZombiesSpawn();

            // Le añado al nuevo zombie su nuevaPosicion con el entityCommandBuffer
            entityCommandBuffer.SetComponent(nuevoZombie, nuevaPosicionZombie); 


        }
    }

}

