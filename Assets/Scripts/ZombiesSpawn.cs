using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;



namespace Zombies
{
    public struct ZombiesSpawn : IComponentData
    {
        public BlobAssetReference<ZombiesSpawnBlob> positionValue;
    }


    // Blob (Binary Large OBject), para almacenar datos que no cambien
    public struct ZombiesSpawnBlob 
    {
        // Array (x, y , z) para almacenar los puntos de spawn de los Zombies
        public BlobArray<float3> positionValueBlob;
    }
}
