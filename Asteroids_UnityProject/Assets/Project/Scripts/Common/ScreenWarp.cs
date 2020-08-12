using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Physics))]
    public class ScreenWarp : MonoBehaviour
    {
        [SerializeField] private new Collider collider = null;

        public WarpZone zone = WarpZone.spawn;
        public enum WarpZone
        {
            spawn,
            play
        }

        private void LateUpdate()
        {
            if (zone == WarpZone.spawn && GameManager.Level.IsTargetWithinPlayArea(this.transform.position))
            {
                this.zone = WarpZone.play;
            }

            this.Warp();
        }

        private void Warp()
        {
            Vector3 pos = this.transform.position;
            Vector4 bounds = Vector4.zero;
            switch (this.zone)
            {
                case WarpZone.play:
                bounds = GameManager.Level.PlayerAreaCoordinates;
                break;
                default:
                bounds = GameManager.Level.SpawnAreaCoordinates;
                break;
            }


            // Object off the top of the screen
            if ((pos.z - this.collider.bounds.extents.z) > bounds.w)
            {
                // Warp to bottom of screen
                pos.z = bounds.z - this.collider.bounds.extents.z;
            }
            // Object off the bottom of the screen
            else if ((pos.z + this.collider.bounds.extents.z) < bounds.z)
            {
                // Warp to the top of the screen
                pos.z = bounds.w + this.collider.bounds.extents.z;
            }
            // Object is off the right of the screen
            else if ((pos.x - this.collider.bounds.extents.x) > bounds.y)
            {
                // Warp to the left of the screen
                pos.x = bounds.x - this.collider.bounds.extents.x;
            }
            // Object off the left of the screen
            else if ((pos.x + this.collider.bounds.extents.x) < bounds.x)
            {
                // Warp to the right of the screen
                pos.x = bounds.y + this.collider.bounds.extents.x;
            }

            this.transform.position = pos;
        }
    }
}