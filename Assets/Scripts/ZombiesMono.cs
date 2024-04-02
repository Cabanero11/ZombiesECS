using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class ZombiesMono : MonoBehaviour
    {
        public float velocidadSpawneo; // De salir de la tumba

        // Velocidad de los zombies andando de ZombiesOleadasData
        public float velocidadAndando;
        public float velocidadGiroAnimacion;
        public float frecuenciaAnimacion;
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
                valorVelocidadOleadas = authoring.velocidadSpawneo
            });


            // Asignar los valores del ZombiesOleadasData
            AddComponent(zombiesEntity, new ZombiesOleadasData
            {
                velocidadAndando = authoring.velocidadAndando,
                velocidadGiroAnimacion = authoring.velocidadGiroAnimacion,
                frecuenciaAnimacion = authoring.frecuenciaAnimacion
            });

            AddComponent<ZombiesSpawn>(zombiesEntity);
            
            // Esta en CementerioData
            AddComponent<ZombiesSpawnerTiempo>(zombiesEntity);
            // Esta en ZombiesOleadasData
            AddComponent<ZombiesDireccion>(zombiesEntity);
            AddComponent<ZombiesTag>(zombiesEntity);
        }
    }

}

