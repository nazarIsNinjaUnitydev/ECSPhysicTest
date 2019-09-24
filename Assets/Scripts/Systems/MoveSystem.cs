using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class MoveSystem: ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref MoveComponent moveComponent, ref Translation translation) =>
            {
                float3 newPos = new float3(0, math.cos(Time.time * moveComponent.Speed) * 10f,0);
                translation.Value = newPos;
            });
        }
    }
}