using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Zombies
{
    public readonly partial struct ZombiesOleadasAspect : IAspect
    {

        
        public readonly Entity Entity;
        public bool isGrounded => (_localTransform.ValueRO.Position.y > 0f);

        private readonly RefRW<LocalTransform> _localTransform;
        private readonly RefRO<ZombiesOleadas> _zombiesOleadas;
        
        
        public void SubirZombiesAlSuelo()
        {
            // Pide hacerlo asi que no deja cambiar un RO tal cual, tiene que ser con un RW obviamente
            var localTransform = _localTransform.ValueRO.Position;
            // TODO: hacer esto smooth
            // Recordar que en TumbasSpawnerSystem esta el offsetTumba :p
            localTransform.y = 1f;  // Altura a la que sale de la tumba
            _localTransform.ValueRW.Position = localTransform;
        }

        public void SpawnearZombies(float tiempo)
        {
            _localTransform.ValueRW.Position += math.up() * _zombiesOleadas.ValueRO.valorVelocidadOleadas * tiempo * 0.2f;
        }

        


    }
}
  