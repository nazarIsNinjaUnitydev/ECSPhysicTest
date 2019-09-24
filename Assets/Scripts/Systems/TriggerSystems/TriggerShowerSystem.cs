using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(TriggerGravityFactorSystem))]
    public class TriggerShowerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            string s = "";
            Entities.ForEach((ref TriggerComponent triggerComponent) =>
            {
                s +=  triggerComponent.Trigger + " " ;
            });
            Debug.Log(s);
        }
    }
}