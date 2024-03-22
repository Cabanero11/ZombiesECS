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
        //private readonly RefRW<ZombiesOleadas> _zombiesOleadas;
        
        public void SuvbirZombiesAlSuelo()
        {
            // Pide hacerlo asi que no deja cambiar un RO tal cual, tiene que ser con un RW obviamente
            var localTransformY = _localTransform.ValueRO.Position.y;
            // TODO: hacer esto smooth
            localTransformY = 0f;
            _localTransform.ValueRW.Position = localTransformY;
        }


    }
}
  