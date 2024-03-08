using Unity.Entities;
using Unity.Mathematics;


namespace Zombies
{
    public struct CementerioRandom : IComponentData
    {
        // Numero random con Unity.Mathematics
        public Random randomValue;

    }

}

