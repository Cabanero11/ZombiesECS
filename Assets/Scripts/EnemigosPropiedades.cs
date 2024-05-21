using Unity.Entities;
using UnityEngine;

public struct EnemigosPropiedades : IComponentData
{
    public float vidaEnemigos;
    public float velocidadEnemigos;

    public float radioReducirVelocidad;
    public float factorReduccionVelocidad;
}
