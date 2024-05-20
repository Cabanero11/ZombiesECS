using Unity.Entities;

public struct PlayerDañoData : IComponentData
{
    public int dañoAlJugador;
    public float dañoBalaJugador;
    public int vidaJugador;

    public int nivelJugador;
    public int nivelSiguiente;
    public float experienciaActualJugador;
    public float experienciaParaProximoNivel;
    public float experienciaObtenidaPorMatarEnemigo;
}