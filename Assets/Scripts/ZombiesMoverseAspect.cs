using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

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
        
        private float VelocidadGiroAnimacion => _zombiesOleadasData.ValueRO.velocidadGiroAnimacion;
        
        private float FrecuenciaAnimacion => _zombiesOleadasData.ValueRO.frecuenciaAnimacion;
        
        private float Direccion => _zombiesDireccion.ValueRO.direccion;

        // Comprobar si un ZombieEntro en el Radio del Generador
        public bool detectarSiZombiesEstaEnRadioGenerador(float3 posicionGenerador, float radioGenerador)
        {

            return math.distancesq(posicionGenerador, _localTransform.ValueRO.Position) <= radioGenerador;

        }

        public float Temporizador
        {
            get => _zombiesTemporazidador.ValueRO.temporizador;
            set => _zombiesTemporazidador.ValueRW.temporizador = value;
        }

        public void Moverse(float deltaTime)
        {
            // Temporizador del juego, y mover hacia adelante al Zombie
            Temporizador += deltaTime;
            _localTransform.ValueRW.Position += _localTransform.ValueRO.Forward() * VelocidadAndando * deltaTime;

            var anguloAndar = VelocidadGiroAnimacion * math.sin(FrecuenciaAnimacion * Temporizador);
            _localTransform.ValueRW.Rotation = quaternion.Euler(0, Direccion, anguloAndar);

            //el eppe
        }

    }
}
  