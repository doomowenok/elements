using UnityEngine;

namespace Core.Balloon
{
    public interface IBalloonFactory
    {
        GameBalloon Create(BalloonType type, Vector3 spawnPosition);
    }
}