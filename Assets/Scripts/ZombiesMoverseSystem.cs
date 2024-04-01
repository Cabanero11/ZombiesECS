using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace Zombies
{
   
  
   [BurstCompile]
   [UpdateAfter(typeof(ZombiesOleadasSystem))]
   public partial struct ZombiesMoverseSystem : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;

            new ZombiesMoverseJob
            {
                DeltaTime = deltaTime
            }.ScheduleParallel();
       }
   }

   // IJob Entity (Para gestionar los Jobs)

   [BurstCompile]
   public partial struct ZombiesMoverseJob : IJobEntity
   {
        public float DeltaTime;

        //public EntityCommandBuffer.ParallelWriter parallelWriter;

        [BurstCompile]
        private void Execute(ZombiesMoverseAspect zombiesMoverseAspect)
        {
            zombiesMoverseAspect.Moverse(DeltaTime);
        }

       
   }

}


