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

        // Zombies
        private readonly RefRW<ZombiesSpawn> _zombiesSpawn;
        private readonly RefRW<ZombiesSpawnerTiempo> _zombiesSpawnTiempo;
        private int numeroZombiesSpawneados => _zombiesSpawn.ValueRO.positionValue.Value.positionValueBlob.Length;

        // Checkear que han spawneadoZombies
        public bool hanSpawneadoZombies() 
        {
            return _zombiesSpawn.ValueRO.positionValue.IsCreated && numeroZombiesSpawneados > 0;
        }


        //private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        //private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        //private const float areaGeneradorRadio = 100f;


        // Conseguir valor del tumbas a spawnear
        public int NumberOfTombstoneToSpawn => _cementerioData.ValueRO.NumberOfTombstoneToSpawn;
        public float areaGeneradorRadio => _cementerioData.ValueRO.areaGeneradorRadio;
        public Entity tumbaPrefab => _cementerioData.ValueRO.TumbaPrefab;

        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            // Calcula los límites del área de generación
            float3 minPos = new float3(-_cementerioData.ValueRO.CementeryDimesions.x * 0.25f,
                                       0f,
                                       -_cementerioData.ValueRO.CementeryDimesions.y * 0.25f);

            float3 maxPos = new float3(_cementerioData.ValueRO.CementeryDimesions.x * 0.25f,
                                       0f,
                                       _cementerioData.ValueRO.CementeryDimesions.y * 0.25f);

            // Genera una posición aleatoria dentro del área de generación
            do
            {
                randomPosition = _cementerioRandom.ValueRW.randomValue.NextFloat3(minPos, maxPos);
            } while (math.distancesq(float3.zero, randomPosition) <= areaGeneradorRadio);

            return randomPosition;
        }

        // Posicion de rotation random para las tumbas
        private quaternion GetRandomRotation() => 
            quaternion.RotateY(_cementerioRandom.ValueRW.randomValue.NextFloat(-0.5f, 0.5f)
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

        public float ZombiesSpawnTiempo 
        {
            get => _zombiesSpawnTiempo.ValueRO.tiempoSpawn;
            set => _zombiesSpawnTiempo.ValueRW.tiempoSpawn = value;
        
        }

        public bool tiempoParaSpawnearZombie => (ZombiesSpawnTiempo <= 0f);

        public float cooldownSpawneoZombies => _cementerioData.ValueRO.cooldownSpawneoZombies;

        public Entity ZombiePrefab => _cementerioData.ValueRO.ZombiePrefab;
    }
}
  