using TwitchToolkit;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Utils
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
