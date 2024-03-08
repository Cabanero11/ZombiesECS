using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;


namespace Zombies
{
    public readonly partial struct CementerioAspect : IAspect
    {
        public readonly Entity Entity;

        // Acceso solo ReadOnly para leer en paralelo
        private readonly RefRO<LocalTransform> _transform;
        private LocalTransform Transform => _transform.ValueRO;

        // Acceso ReadOnly y ReadWrite
        private readonly RefRO<CementerioData> _cementerioData;
        private readonly RefRW<CementerioRandom> _cementerioRandom;
        
        
        //private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        //private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        private const float areaGeneradorRadio = 40f;


        // Conseguir valor del tumbas a spawnear
        public int NumberOfTombstoneToSpawn => _cementerioData.ValueRO.NumberOfTombstoneToSpawn;
        public Entity tumbaPrefab => _cementerioData.ValueRO.TumbaPrefab;


        // Para crear la posicion de las tumbas de forma aleatoria, hace falta crear posicion min y max
        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            // Para que no salgan tumbas dentro del generador
            do
            {
                randomPosition = _cementerioRandom.ValueRW.randomValue.NextFloat3(minPos, maxPos);
            } while (math.distancesq(Transform.Position, randomPosition) <= areaGeneradorRadio);


            return randomPosition;
        }

        // Min -> 25% del total y el maximo un 75% de las dimensiones
        private float3 minPos => Transform.Position - new float3(
            _cementerioData.ValueRO.CementeryDimesions.x * 0.25f,
            0f,
            _cementerioData.ValueRO.CementeryDimesions.y * 0.25f
            );
        private float3 maxPos => Transform.Position - new float3(
            _cementerioData.ValueRO.CementeryDimesions.x * 1.5f,
            0f,
            _cementerioData.ValueRO.CementeryDimesions.y * 1.5f
            );

        // Posicion de rotation random para las tumbas
        private quaternion GetRandomRotation() => 
            quaternion.RotateY(_cementerioRandom.ValueRW.randomValue.NextFloat(-0.25f, 0.25f)
        );

        private float GetRandomScale() =>
            _cementerioRandom.ValueRW.randomValue.NextFloat(0.5f, 1.2f
        );
                


        public LocalTransform GetRandomTumbaTransform()
        {
            return new LocalTransform
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale()
            };
        }
    }
}
  