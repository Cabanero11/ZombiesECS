using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.VisualScripting;



namespace Zombies
{
   
  
   [BurstCompile]
   [UpdateAfter(typeof(ZombiesOleadasSystem))]
   public partial struct ZombiesMoverseSystem : ISystem
   {
       [BurstCompile]
       public void OnCreate(ref SystemState state)
       {
            state.RequireForUpdate<GeneradorTag>();
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

            var generadorEntidad = SystemAPI.GetSingletonEntity<GeneradorTag>();

            var generadorEscala = SystemAPI.GetComponent<LocalTransform>(generadorEntidad).Scale;

            var generadorRadio = generadorEscala * 5f + 0.5f;

            //UnityEngine.Debug.Log("DeltaTime: " + deltaTime);
            //UnityEngine.Debug.Log("Moverse System activo");

            new ZombiesMoverseJob
            {
                DeltaTime = deltaTime,
                RadioGenerador = generadorRadio * generadorRadio,
                parallelWriter = endSimulationEntityCommandBuffer.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();

       }
   }

   // IJob Entity (Para gestionar los Jobs)

   [BurstCompile]
   public partial struct ZombiesMoverseJob : IJobEntity
   {
        public float DeltaTime;

        public float RadioGenerador;

        public EntityCommandBuffer.ParallelWriter parallelWriter;

        [BurstCompile]
        private void Execute(ZombiesMoverseAspect zombiesMoverseAspect, [ChunkIndexInQuery] int sortingKey)
        {
            zombiesMoverseAspect.Moverse(DeltaTime);

            if(zombiesMoverseAspect.detectarSiZombiesEstaEnRadioGenerador(float3.zero, RadioGenerador))
            {
                // Para si llega al radio del Generador, y emepezar  a atacar si esta en el
                parallelWriter.SetComponentEnabled<ZombiesOleadasData>(sortingKey, zombiesMoverseAspect.Entity, false);
                parallelWriter.SetComponentEnabled<ZombiesAtacar>(sortingKey, zombiesMoverseAspect.Entity, true);
            }
        }

       
   }

}


