using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace Zombies
{
   
  
   [BurstCompile]
   [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
   [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
   public partial struct GeneradorDañarSystem : ISystem
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
            // Para que no salgan mensajes de error :O
            state.Dependency.Complete();

            foreach (var generadorAspect in SystemAPI.Query<GeneradorAspect>())
            {
                generadorAspect.GeneradorRecibirDaño();
            }
        }
   }

    
}


