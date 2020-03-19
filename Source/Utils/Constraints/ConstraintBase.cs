using TwitchToolkit;

using UnityEngine;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public class ConstraintBase
    {
        public virtual void Draw(Rect canvas)
        {
        }

        public virtual bool ShouldPurge(Viewer viewer)
        {
            return false;
        }
    }
}
