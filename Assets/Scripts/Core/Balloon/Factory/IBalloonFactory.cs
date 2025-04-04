using UnityEngine;

namespace Core.Balloon.Factory
{
    public interface IBalloonFactory
    {
        GameBalloon Create(BalloonType type, Vector3 spawnPosition);
    }
}