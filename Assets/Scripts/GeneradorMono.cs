using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class GeneradorMono : MonoBehaviour
    {
        // 200 Ta bien creo ??
        public float vidaGenerador;

    }


    public class GeneradorBaker : Baker<GeneradorMono>
    {
        public override void Bake(GeneradorMono authoring)
        {
            var generadorEntidad = GetEntity(TransformUsageFlags.Dynamic);

            //AddComponent<GeneradorTag>(generadorEntidad);
            AddComponent(generadorEntidad, new GeneradorVida { generadorVida = authoring.vidaGenerador, generadorVidaMaxima = authoring.vidaGenerador });

            //AddBuffer<GeneradorDañoBufferElemento>(generadorEntidad);
        }
    }

}

