using Unity.Entities;

public struct PlayerDañoData : IComponentData
{
    public float dañoAlJugador;
    public float dañoBalaJugador;
    public float vidaJugador;

    public int nivelJugador;
    public int nivelSiguiente;
    public float experienciaActualJugador;
    public float experienciaParaProximoNivel;
    public float experienciaObtenidaPorMatarEnemigo;
}