using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Zombies
{
    public class ZombiesMono : MonoBehaviour
    {
        public float velocidadSpawneo; // De salir de la tumba
        public float velocidadAndando;


        public float dañoAlGenerador; // Daño que le hacen al generador
        public float vidaZombies; // El numero de vida del zombie
        

    }


    public class ZombiesBaker : Baker<ZombiesMono>
    {
        public override void Bake(ZombiesMono authoring)
        {
            var zombiesEntity = GetEntity(TransformUsageFlags.Dynamic);

            // Añadir a la entidad del Cementerio los datos de CementerioData
            // para asi verlos en el editor
            AddComponent(zombiesEntity, new CementerioData
            {
                
            });


            // Generar nº random de Tumbas para spawnear zombies
            AddComponent(zombiesEntity, new CementerioRandom
            {

            });

            AddComponent<ZombiesSpawn>(zombiesEntity);
            AddComponent<ZombiesSpawnerTiempo>(zombiesEntity);
        }
    }

}

