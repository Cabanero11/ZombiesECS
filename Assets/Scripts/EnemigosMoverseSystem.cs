using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Zombies;

public partial class EnemigosMoverseSystem : SystemBase
{
    private EntityQuery playerQuery;

    protected override void OnCreate()
    {
        playerQuery = GetEntityQuery(ComponentType.ReadOnly<DisparoData>());
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        // Obtener el jugador
        var playerEntity = playerQuery.GetSingletonEntity();
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);
        var playerDamage = SystemAPI.GetComponent<PlayerDa�oData>(playerEntity);

        // Configuraci�n del job
        var job = new MoverEnemigosJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ElapsedTime = (float)SystemAPI.Time.ElapsedTime,
            PlayerPosition = playerTransform.Position,
            NivelJugador = playerDamage.nivelJugador,
            NivelSiguiente = playerDamage.nivelSiguiente
        };

        Dependency = job.ScheduleParallel(Dependency);
    }

    [BurstCompile]
    public partial struct MoverEnemigosJob : IJobEntity
    {
        public float DeltaTime;
        public float ElapsedTime;
        public float3 PlayerPosition;
        public int NivelJugador;
        public int NivelSiguiente;

        private void Execute(ref LocalTransform enemigoTransform, ref EnemigosPropiedades enemigoPropiedades)
        {
            // Mover enemigos hacia el jugador 
            float3 direccionAlJugador = math.normalize(PlayerPosition - enemigoTransform.Position);

            // Subi� de nivel el jugador, los enemigos son m�s r�pidos y tienen m�s vida
            if (NivelJugador == NivelSiguiente)
            {
                enemigoPropiedades.velocidadEnemigos += 0.33f;
                enemigoPropiedades.vidaEnemigos += 5f;
            }

            // Reducir velocidad si el enemigo est� demasiado cerca del jugador
            float distanciaAlJugador = math.distance(PlayerPosition, enemigoTransform.Position);

            if (distanciaAlJugador < enemigoPropiedades.radioReducirVelocidad)
            {
                // Reducir su velocidad, y asegurarse que no sea menor a 1.25f, bug antes de que era 0 uh?�?
                enemigoPropiedades.velocidadEnemigos = math.max(enemigoPropiedades.velocidadEnemigos * enemigoPropiedades.factorReduccionVelocidad, 1.5f);
            }

            enemigoTransform.Position += enemigoPropiedades.velocidadEnemigos * DeltaTime * direccionAlJugador;
            enemigoTransform.Position.y = 0.80f;  // Sino se quedan debajo del suelo

            float direccion = GetRotationEnemigos(enemigoTransform.Position, PlayerPosition);

            // Animaci�n de lado a lado
            float anguloAndar = 0.1f * math.sin(5f * ElapsedTime);

            enemigoTransform.Rotation = quaternion.Euler(0, direccion, anguloAndar);
        }

        public static float GetRotationEnemigos(float3 enemigoPosition, float3 jugadorPosition)
        {
            var x = enemigoPosition.x - jugadorPosition.x;
            var y = enemigoPosition.z - jugadorPosition.z;
            return math.atan2(x, y) + math.PI;
        }
    }
}
