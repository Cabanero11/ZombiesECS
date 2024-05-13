using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace Zombies
{
    public struct BalasData : IComponentData
    {
        public float velocidadBala;
        public float tamañoBala;
        public float dañoBala;
    }
}

