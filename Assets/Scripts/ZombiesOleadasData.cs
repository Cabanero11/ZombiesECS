
using Unity.Entities;
using UnityEngine;

public struct ZombiesOleadasData : IComponentData
{
    // Velocidad de los zombies andando
    public float velocidadAndando;

    // Daño que le hacen al generador
    public float dañoAlGenerador;
    // El numero de vida del zombie
    public float vidaZombies; 
}
