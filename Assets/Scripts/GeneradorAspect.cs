using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;


namespace Zombies
{
    public readonly partial struct GeneradorAspect : IAspect
    {
        public readonly Entity Entity;

        // Acceso solo ReadOnly para leer en paralelo
        private readonly RefRW<LocalTransform> _localTransform;
        private LocalTransform Transform 
            => _localTransform.ValueRO;

        private readonly RefRW<GeneradorVida> _generadorVida;
        //private readonly DynamicBuffer<GeneradorDañoBufferElemento> _generadorDañoBufferElemento;
    
        private void GeneradorRecibirDaño()
        {
            /**
            foreach (var generadorDañoBufferElemento in _generadorDañoBufferElemento)
            {
                _generadorVida.ValueRW.generadorVida -= generadorDañoBufferElemento.Value;
            }

            _generadorDañoBufferElemento.Clear();

            // Escale la escala del Generador con la vida actual que tenga :O
            _localTransform.ValueRW.Scale = _generadorVida.ValueRO.generadorVida / _generadorVida.ValueRO.generadorVidaMaxima;

            */
        }

    }
}
  