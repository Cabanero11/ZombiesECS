
using Unity.Entities;
using UnityEngine;

public struct ZombiesOleadasData : IComponentData
{
    // Velocidad de los zombies andando
    public float velocidadAndando;

    // Daņo que le hacen al generador
    public float daņoAlGenerador;
    // El numero de vida del zombie
    public float vidaZombies; 
}
