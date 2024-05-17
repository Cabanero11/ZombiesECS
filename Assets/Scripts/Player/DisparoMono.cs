using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;


namespace Zombies
{
    public class DisparoMono : MonoBehaviour
    {
        public GameObject balaPrefab;
        public int numeroBalasPorDisparo;
        public float balasSpread;

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
            });

            // Ya que DisparoData lo uso como PlayerEntity inicializo aqui el PlayerDañoData
            AddComponent(playerEntidad, new PlayerDañoData
            {
                dañoAlJugador = 10,
                dañoBalaJugador = 10f,
                vidaJugador = 100,
                nivelJugador = 1,
                experienciaActualJugador = 0f,
                experienciaParaProximoNivel = 100f, 
                experienciaObtenidaPorMatarEnemigo = 10f 
            });

        }

    }

}

