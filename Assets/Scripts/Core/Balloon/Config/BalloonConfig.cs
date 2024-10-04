using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Balloon.Config
{
    [CreateAssetMenu(fileName = nameof(BalloonConfig), menuName = "Configs/Core/Balloon Config")]
    public sealed class BalloonConfig : ScriptableObject
    {
        [SerializeField] private GameBalloon _prefab;
        public GameBalloon Prefab => _prefab;
        
        [SerializeField] private List<BalloonData> _balloonsData;
        private Dictionary<BalloonType, BalloonData> _balloons;
        public IReadOnlyDictionary<BalloonType, BalloonData> Balloons 
        {
            get
            {
                if (_balloons == null || _balloons.Count == 0)
                {
                    _balloons = _balloonsData.ToDictionary(x => x.Type);
                }
                
                return _balloons;
            }
        }
        
        [SerializeField] private Vector2 _balloonSize;
        public Vector2 BalloonSize => _balloonSize;
        
        [SerializeField] private Vector2 _balloonMinMaxFrequency;
        public Vector2 BalloonMinMaxFrequency => _balloonMinMaxFrequency;
        
        [SerializeField] private Vector2 _balloonMinMaxSpeed;
        public Vector2 BalloonMinMaxSpeed => _balloonMinMaxSpeed;
        
        [SerializeField] private Vector2 _balloonMinMaxAmplitude;
        public Vector2 BalloonMinMaxAmplitude => _balloonMinMaxAmplitude;

        [SerializeField] private Vector2 _delayMinMaxBeforeNextCreation; 
        public Vector2 DelayBeforeNextCreation => _delayMinMaxBeforeNextCreation;
    }
}