using Unity.Entities;
using UnityEngine;

public struct EnemigosData : IComponentData
{
    public Entity enemigoPrefab;
    public Entity enemigoFuertePrefab;
    public Entity enemigoRapidoPrefab;

    public int numeroDeEnemigosSpawneadosPorSegundo;
    public int incrementoDeNumeroDeEnemigosPorOleada;
    public int maximoNumeroDeEnemigos;
    public int numeroOleada;

    public float radioSpawneoEnemigos;
    public float distanciaMinimaAlJugador;

    public float cooldownSpawneoEnemigos;
    public float cooldownActualSpawneo;
}
