using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;


namespace Zombies
{
    public class DisparoMono : MonoBehaviour
    {
        public GameObject balaPrefab;
        public int numeroBalasPorDisparo;
        public float balasSpread;

        public float velocidadJugador;
        public float incrementoVelocidad;

        public float cooldownDisparo;
        public float temporizadorDisparo;

    }

    // Añado los valores de DisparoData a la entidad de PlayerEntity con el Baker
    public class DisparoBaker : Baker<DisparoMono>
    {
        public override void Bake(DisparoMono authoring)
        {
            var playerEntidad = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntidad, new DisparoData
            {
                balaPrefab = GetEntity(authoring.balaPrefab, TransformUsageFlags.Dynamic),
                numeroBalasPorDisparo = authoring.numeroBalasPorDisparo,
                balasSpread = authoring.balasSpread,
                velocidadJugador = authoring.velocidadJugador,
                incrementoVelocidad = authoring.incrementoVelocidad,
                cooldownDisparo = authoring.cooldownDisparo,
                temporizadorDisparo = authoring.temporizadorDisparo
            });

            // Ya que DisparoData lo uso como PlayerEntity inicializo aqui el PlayerDañoData
            AddComponent(playerEntidad, new PlayerDañoData
            {
                dañoAlJugador = 12.5f,
                dañoBalaJugador = 2f,
                vidaJugador = 100,
                nivelJugador = 1,
                nivelSiguiente = 2,
                experienciaActualJugador = 0f,
                experienciaParaProximoNivel = 100f,
                experienciaObtenidaPorMatarEnemigo = 10f
            });

        }

    }

}

