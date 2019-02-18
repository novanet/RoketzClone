using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerInput
    {
        private readonly int _playerNumber;
        private Vector2 _direction = Vector2.up;
        
        public PlayerInput(int playerNumber)
        {
            _playerNumber = playerNumber;
        }

        public Vector2 GetAnalogueStickDirection()
        {
            var horizontal = GetAxis("Horizontal");
            var vertical = GetAxis("Vertical");
            
            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
                _direction = new Vector2(horizontal, vertical);
            //Debug.Log($"Hor: {horizontal} Ver: {vertical}");

            return _direction;
        }

        public bool IsBoosting()
        {
            return GetAxis("Boost") > 0.1f;
        }

        public bool IsShooting()
        {
            return GetAxis("Fire1") > 0.1f;
        }

        private float GetAxis(string axisName)
        {
            return Input.GetAxis($"Player{_playerNumber}.{axisName}");
        }
    }
}