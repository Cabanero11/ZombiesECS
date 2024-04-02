using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace Zombies
{
   
  
   [BurstCompile]
   [UpdateAfter(typeof(ZombiesSpawnerSystem))]
   public partial struct ZombiesOleadasSystem : ISystem
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

            // Al final de la fase de Simulacion
            var endSimulationEntityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            // Se crearan en diferentes hilos de forma paralela
            // Al tener en ZombiesSpawnerSystem en el OnUpdate  }.Run();
            // No tenemos orden para saber cual de estos 2 Sistemas se ejecutara antes
            // La Solucion => [UpdateAfter(typeof(ZombiesSpawnerSystem))]
            // Asi se ejecutara despues del ZombiesSpawnerSystem
            new ZombiesOleadasJob
            {
                DeltaTime = deltaTime,
                parallelWriter = endSimulationEntityCommandBuffer.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
       }
   }

   // IJob Entity (Para gestionar los Jobs)

   public partial struct ZombiesOleadasJob : IJobEntity
   {
        public float DeltaTime;

        // Un
        public EntityCommandBuffer.ParallelWriter parallelWriter;

        [BurstCompile]
        private void Execute(ZombiesOleadasAspect zombiesOleadasAspect, [ChunkIndexInQuery] int sortingKey)
        {
            zombiesOleadasAspect.SpawnearZombies(DeltaTime);

            // Sino esta en el suelo
            if(!zombiesOleadasAspect.isGrounded)
            {
                return;
            } 

            zombiesOleadasAspect.SubirZombiesAlSuelo();
            

            // Una vez conseguido elevar el zombie y sacarlo,
            // le quitamos esta Componente para añadirle otra de moverse y atacar
            parallelWriter.RemoveComponent<ZombiesOleadas>(sortingKey, zombiesOleadasAspect.Entity);
            parallelWriter.SetComponentEnabled<ZombiesOleadasData>(sortingKey, zombiesOleadasAspect.Entity, true);
        }

       
   }

}


