using UnityEngine;
using BlitzEcs;

namespace FlappyECS
{
    public class InputIecsSystem : IECSSystem
    {
        private Query<InputComponent> query;

        public void OnCreate(World world)
        {
            query = new Query<InputComponent>(world);
            
            // Tạo thực thể Input nếu chưa có
            if (query.MatchedEntityIds.count == 0)
            {
                world.Spawn().Add(new InputComponent { jumpPressed = false });
            }
        }

        public void OnUpdate(World world, float deltaTime)
        {
            query.ForEach((ref InputComponent input) =>
            {
                // Capture input once per frame
                input.jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
            });
        }
    }
}
