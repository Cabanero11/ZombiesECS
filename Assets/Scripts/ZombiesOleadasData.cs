
using Unity.Entities;
using UnityEngine;

public struct ZombiesOleadasData : IComponentData, IEnableableComponent
{
    // Velocidad de los zombies andando
    public float velocidadAndando;

    public float velocidadGiroAnimacion;
    public float frecuenciaAnimacion;

    // Daņo que le hacen al generador
    public float daņoAlGenerador;
    // El numero de vida del zombie
    public float vidaZombies; 
}


public struct ZombiesTemporizador : IComponentData
{
    public float temporizador;
}

public struct  ZombiesDireccion : IComponentData 
{
    public float direccion;
}


public struct ZombiesTag : IComponentData
{

}
