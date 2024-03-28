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
   public partial struct ZombiesAtacarSystem : ISystem
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
            
       }
   }

   // IJob Entity (Para gestionar los Jobs)

   public partial struct ZombiesAtacarJob : IJobEntity
   {
        public float DeltaTime;

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
            //parallelWriter.SetComponentEnabled<ZombiesOleadasData>(sortingKey, zombiesOleadasAspect.Entity, true);
        }

       
   }

}


