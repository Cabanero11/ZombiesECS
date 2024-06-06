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


    // Añadir las nuevas variables para las estadísticas de dificultad
    public float velocidadNormal;
    public float vidaNormal;
    public float velocidadFuerte;
    public float vidaFuerte;
    public float velocidadRapido;
    public float vidaRapido;
}
