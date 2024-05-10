using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class ZombiesMono : MonoBehaviour
    {
        public float VelocidadSpawneo; // De salir de la tumba

        // Velocidad de los zombies andando de ZombiesOleadasData
        public float VelocidadAndando;
        public float VelocidadGiroAnimacion;
        public float FrecuenciaAnimacion;

        // ZombiesAtacar de ZombiesOleadasData
        public float DañoAlGenerador;
        public float AnimacionAmplitud;
        public float FrecuenciaDeAtaque;
    }


    public class ZombiesBaker : Baker<ZombiesMono>
    {
        public override void Bake(ZombiesMono authoring)
        {
            var zombiesEntity = GetEntity(TransformUsageFlags.Dynamic);

            // Añadir a la entidad del Cementerio los datos de CementerioData
            // para asi verlos en el editor
            AddComponent(zombiesEntity, new ZombiesOleadas
            {
                valorVelocidadOleadas = authoring.VelocidadSpawneo
            });


            // Asignar los valores del ZombiesOleadasData
            AddComponent(zombiesEntity, new ZombiesOleadasData
            {
                velocidadAndando = authoring.VelocidadAndando,
                velocidadGiroAnimacion = authoring.VelocidadGiroAnimacion,
                frecuenciaAnimacion = authoring.FrecuenciaAnimacion
            });

            // Los valores del sistema de ataque de los zombies
            AddComponent(zombiesEntity, new ZombiesAtacar
            {
                dañoAlGenerador = authoring.DañoAlGenerador,
                animacionAmplitud = authoring.AnimacionAmplitud,
                frecuenciaDeAtaque = authoring.FrecuenciaDeAtaque
            });




            // SOBRABA CREO
            AddComponent<ZombiesSpawn>(zombiesEntity);
            
            // Esta en CementerioData
            AddComponent<ZombiesSpawnerTiempo>(zombiesEntity);


            // Esta en ZombiesOleadasData
            AddComponent<ZombiesDireccion>(zombiesEntity);
            AddComponent<ZombiesTag>(zombiesEntity);
        }
    }

}

