
using Unity.Entities;
using UnityEngine;

public struct ZombiesOleadasData : IComponentData
{
    // Velocidad de los zombies andando
    public float velocidadAndando;

    // Da�o que le hacen al generador
    public float da�oAlGenerador;
    // El numero de vida del zombie
    public float vidaZombies; 
}
