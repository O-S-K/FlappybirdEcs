using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class SoundIecsSystem : IECSSystem
    {
        private Query<PlaySoundRequest> query;

        public void OnCreate(World world)
        {
            query = new Query<PlaySoundRequest>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            query.ForEach((Entity entity, ref PlaySoundRequest request) =>
            {
                // Gọi AudioManager dựa trên request
                switch (request.soundName)
                {
                    case "Jump": AudioManager.Instance.PlayJump(); break;
                    case "Score": AudioManager.Instance.PlayScore(); break;
                    case "Hit": AudioManager.Instance.PlayHit(); break;
                    case "Die": AudioManager.Instance.PlayDie(); break;
                }
                
                // Xóa request sau khi đã xử lý
                entity.Remove<PlaySoundRequest>();
            });
        }
    }
}
