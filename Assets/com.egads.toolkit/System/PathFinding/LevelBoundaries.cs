using UnityEngine;
using egads.tools.utils;
using egads.system.gameManagement;

namespace egads.system.pathFinding
{
    /// <summary>
    /// max boundaries for camera and player movement
    /// </summary>
    public class LevelBoundaries : MonoBehaviour
    {
        #region Boundary Properties

        [SerializeField]
        protected Rect bounds = new Rect(0, 0, 10, 10);
        private Rect _levelBounds;
        public Rect levelBounds
        {
            get
            {
                CalculateBounds();
                return _levelBounds;
            }
        }

        #endregion

        #region Public Properties

        // Display in editor window
        public bool drawRectangle = true;
        public Color lineColor = Color.blue;

        #endregion

        #region Private Properties

        private Transform _transform;

        #endregion

        #region Unity Methods

        void Awake()
        {
            _transform = transform;

            MainBase.Instance.levelBounds = levelBounds;
        }

        void OnDrawGizmos()
        {
            CalculateBounds();

            GizmosUtilities.DrawRect(_levelBounds, lineColor);
        }

        #endregion

        #region Public Methods

        public Vector2 GetRelativePosition(Vector3 pos)
        {
            CalculateBounds();

            float x = (pos.x - _levelBounds.x) / _levelBounds.width;
            float y = (pos.y - _levelBounds.y) / _levelBounds.height;

            return new Vector2(x, y);
        }

        public void SetBounds(Rect bounds)
        {
            this.bounds = bounds;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates level bounds from position of this object + bounds
        /// </summary>
        private void CalculateBounds()
        {
            _transform = transform;

            _levelBounds.xMin = _transform.position.x + bounds.xMin;
            _levelBounds.yMin = _transform.position.y + bounds.yMin;
            _levelBounds.width = bounds.width;
            _levelBounds.height = bounds.height;
        }

        #endregion
    }

}