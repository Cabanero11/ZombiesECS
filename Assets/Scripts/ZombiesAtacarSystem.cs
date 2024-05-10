using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace Zombies
{


    [BurstCompile]
    [UpdateAfter(typeof(ZombiesMoverseSystem))]
    public partial struct ZombiesAtacarSystem : ISystem
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

            // Obtener la variable Singleton asociada con la entidad de inicializacion "Begin"
            var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            var generadorEntidad = SystemAPI.GetSingletonEntity<GeneradorTag>();

            var generadorEscala = SystemAPI.GetComponent<LocalTransform>(generadorEntidad).Scale;

            var generadorRadio = generadorEscala * 5f + 1f;

            // Preparar el Job para spawnear Zombies, en el hilo Main()
            new ZombiesAtacarJob
            {
                DeltaTime = deltaTime,
                parallelWriter = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                GeneradorEntidad = generadorEntidad,
                RadioGenerador = generadorRadio
            }.ScheduleParallel();

        }
    }

    [BurstCompile]
    public partial struct ZombiesAtacarJob : IJobEntity
    {
        public float DeltaTime;

        public float RadioGenerador;

        public EntityCommandBuffer.ParallelWriter parallelWriter;

        public Entity GeneradorEntidad;

        [BurstCompile]
        private void Execute(ZombiesAtacarAspect zombiesAtacarAspect, [ChunkIndexInQuery] int sortingKey)
        {

            if (zombiesAtacarAspect.detectarSiZombiesEstaEnRadioParaAtacar(float3.zero, RadioGenerador))
            {
                zombiesAtacarAspect.AtacarAlGenerador(DeltaTime, parallelWriter, sortingKey, GeneradorEntidad);
            }
            else
            {
                // Sino esta en rango, NO acatar, SI moverse
                parallelWriter.SetComponentEnabled<ZombiesAtacar>(sortingKey, zombiesAtacarAspect.Entity, false);
                parallelWriter.SetComponentEnabled<ZombiesOleadasData>(sortingKey, zombiesAtacarAspect.Entity, true);
            }
        }


    }





}