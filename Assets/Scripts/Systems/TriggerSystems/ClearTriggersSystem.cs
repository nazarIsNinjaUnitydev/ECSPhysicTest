using Components;
using Unity.Entities;

namespace Systems.TriggerSystems
{
    [UpdateBefore(typeof(TriggerGravityFactorSystem))]
    public class ClearTriggersSystem: ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref TriggerComponent triggerComponent) =>
            {
                triggerComponent.Trigger = false;
            });
        }
    }
}