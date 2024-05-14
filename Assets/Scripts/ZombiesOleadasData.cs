
using Unity.Entities;
using UnityEngine;

public struct ZombiesOleadasData : IComponentData, IEnableableComponent
{
    // Velocidad de los zombies andando
    public float velocidadAndando;

    public float velocidadGiroAnimacion;
    public float frecuenciaAnimacion;

    // Vida de los zombies
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


public struct ZombiesAtacar : IComponentData, IEnableableComponent
{
    public float dañoAlGenerador;
    public float animacionAmplitud;
    public float frecuenciaDeAtaque;
}