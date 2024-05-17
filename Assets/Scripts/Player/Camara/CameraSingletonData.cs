using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct CameraSingletonData : IComponentData
{
    public float distanciaDetrasJugador;
    public float alturaSobreJugador;
    public float velocidadCamara;
}
