using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace Zombies
{
   // Este sistema es para el grupo de Inicializacion, y asi poder activar
   // y desactivar los sistemas
  
   [BurstCompile]
   [UpdateInGroup(typeof(InitializationSystemGroup))]
   public partial struct ZombiesInicializarSystem : ISystem
   {
       [BurstCompile]
       public void OnCreate(ref SystemState state)
       {
            //state.RequireForUpdate<GeneradorTag>();
       }

       [BurstCompile]
       public void OnDestroy(ref SystemState state)
       {

       }

       [BurstCompile]
       public void OnUpdate(ref SystemState state)
       {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            // Para cuando se crea un zombie y se le asigna el Moverse
            foreach (var zombiesMoverse in SystemAPI.Query<ZombiesMoverseAspect>().WithAll<ZombiesTag>())
            {
                entityCommandBuffer.RemoveComponent<ZombiesTag>(zombiesMoverse.Entity);
                
                // Asi desabilitamos la propiedad
                entityCommandBuffer.SetComponentEnabled<ZombiesOleadasData>(zombiesMoverse.Entity, false);
            }

            entityCommandBuffer.Playback(state.EntityManager);
       }
   }

}


