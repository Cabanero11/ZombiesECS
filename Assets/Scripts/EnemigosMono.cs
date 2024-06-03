using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



namespace Zombies
{
    public class EnemigosMono : MonoBehaviour
    {
        public GameObject enemigoPrefab;
        public GameObject enemigoFuertePrefab;
        public GameObject enemigoRapidoPrefab;

        public int numeroDeEnemigosSpawneadosPorSegundo = 50;
        public int incrementoDeNumeroDeEnemigosPorOleada = 20;
        public int maximoNumeroDeEnemigos = 150; // 5 Oleadas
        public int numeroOleada = 1;

        public float radioSpawneoEnemigos = 400f;
        public float distanciaMinimaAlJugador = 3f;

        public float cooldownSpawneoEnemigos = 1f;
        public float cooldownActualSpawneo;
    }

    // Añado los valores de DisparoData a la entidad de PlayerEntity con el Baker
    public class EnemigosBaker : Baker<EnemigosMono>
    {
        public override void Bake(EnemigosMono authoring)
        {
            var enemigoSpawner = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(enemigoSpawner, new EnemigosData
            {
                enemigoPrefab = GetEntity(authoring.enemigoPrefab, TransformUsageFlags.Dynamic),
                enemigoFuertePrefab = GetEntity(authoring.enemigoFuertePrefab, TransformUsageFlags.Dynamic),
                enemigoRapidoPrefab = GetEntity(authoring.enemigoRapidoPrefab, TransformUsageFlags.Dynamic),
                numeroDeEnemigosSpawneadosPorSegundo = authoring.numeroDeEnemigosSpawneadosPorSegundo,
                incrementoDeNumeroDeEnemigosPorOleada = authoring.incrementoDeNumeroDeEnemigosPorOleada,
                maximoNumeroDeEnemigos = authoring.maximoNumeroDeEnemigos,
                numeroOleada = authoring.numeroOleada,
                radioSpawneoEnemigos = authoring.radioSpawneoEnemigos,
                distanciaMinimaAlJugador = authoring.distanciaMinimaAlJugador,
                cooldownSpawneoEnemigos = authoring.cooldownSpawneoEnemigos,
                cooldownActualSpawneo = authoring.cooldownActualSpawneo
            });

        }

    }



}