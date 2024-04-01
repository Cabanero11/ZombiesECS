using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Zombies
{
    public readonly partial struct ZombiesMoverseAspect : IAspect
    {

        
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _localTransform;
        private readonly RefRW<ZombiesTemporizador> _zombiesTemporazidador;
        
        private readonly RefRO<ZombiesDireccion> _zombiesDireccion;
        private readonly RefRO<ZombiesOleadasData> _zombiesOleadasData;

        // Obtener los valores en variables, mas comodo
        // En mayuscula que inflinjen la nomenclatura :(
        private float VelocidadAndando => _zombiesOleadasData.ValueRO.velocidadAndando;
        
        private float VelocidadGiroAnimacion => _zombiesOleadasData.ValueRO.velocidadAndando;
        
        private float FrecuenciaAnimacion => _zombiesOleadasData.ValueRO.velocidadAndando;
        
        private float Direccion => _zombiesDireccion.ValueRO.direccion;

        // Comprobar si un ZombieEntro en el Radio del Generador
        public bool detectarSiZombiesEstaEnRadioGenerador(float3 posicionGenerador, float radioGenerador)
        {
            var distacia = math.distancesq(posicionGenerador, _localTransform.ValueRO.Position);

            return distacia <= radioGenerador;
        }

        public float temporizador
        {
            get => _zombiesTemporazidador.ValueRO.temporizador;
            set => _zombiesTemporazidador.ValueRW.temporizador = value;
        }

        public void Moverse(float deltaTime)
        {
            // Temporizador del juego, y mover hacia deltante al Zombie
            temporizador += deltaTime;
            _localTransform.ValueRW.Position += _localTransform.ValueRO.Forward() * VelocidadAndando * deltaTime;

            // Animacion como de andar de lado a lado

            var anguloBalanceo = VelocidadGiroAnimacion * math.sin(VelocidadAndando * temporizador);
            
            _localTransform.ValueRW.Rotation = quaternion.Euler(0, Direccion, anguloBalanceo);
        }

    }
}
  