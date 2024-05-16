using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CapaColisiones
{
    Default = 1 << 0,
    Wall = 1 << 6,
    Enemigo = 1 << 8,
    Player = 1 << 9
}


public class DevolverCapa
{

    // Obtener las capas que colisiono
    public static uint ObtenerCapaTrasColision(CapaColisiones capa1, CapaColisiones capa2)
    {
        var capaRes = (uint)capa1 | (uint)capa2;

        return capaRes;
    }
}
