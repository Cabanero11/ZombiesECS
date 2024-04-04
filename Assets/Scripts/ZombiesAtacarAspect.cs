using System.Globalization;
using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Zombies
{
    public readonly partial struct ZombiesAtacarAspect : IAspect
    {

        
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _localTransform;

        private readonly RefRW<ZombiesTemporizador> _zombiesTemporazidador;

        private readonly RefRO<ZombiesAtacar> _zombiesAtacar;
        private readonly RefRO<ZombiesDireccion> _zombiesDireccion;

        // Obtener los valores en variables, mas comodo
        // En mayuscula que inflinjen la nomenclatura :(
        private float DañoAlGenerador => _zombiesAtacar.ValueRO.dañoAlGenerador;

        private float AnimacionAmplitud => _zombiesAtacar.ValueRO.animacionAmplitud;

        private float FrecuenciaDeAtaque => _zombiesAtacar.ValueRO.frecuenciaDeAtaque;

        private float Direccion => _zombiesDireccion.ValueRO.direccion;



        public bool detectarSiZombiesEstaEnRadioParaAtacar(float3 posicionGenerador, float radioGenerador)
        {

            return math.distancesq(posicionGenerador, _localTransform.ValueRO.Position) <= radioGenerador - 1;

        }

        public float Temporizador
        {
            get => _zombiesTemporazidador.ValueRO.temporizador;
            set => _zombiesTemporazidador.ValueRW.temporizador = value;
        }

        public void AtacarAlGenerador(float deltaTime, EntityCommandBuffer.ParallelWriter parallelWriter, int sortingKey, Entity generadorEntidad)
        {
            // Temporizador del juego, y hacer que ataque al Generador
            Temporizador += deltaTime;

            var anguloDeComer = AnimacionAmplitud * math.sin(FrecuenciaDeAtaque * Temporizador);

            _localTransform.ValueRW.Rotation = quaternion.Euler(anguloDeComer, Direccion, 0);

            var dañoAtaque = DañoAlGenerador * deltaTime;
            var vidaGeneradorActual = new GeneradorDañoBuffer { generadorDañoBuffer = dañoAtaque };
            parallelWriter.AppendToBuffer(sortingKey, generadorEntidad, vidaGeneradorActual);
        }


    }
}
  