using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class PosLikeEvent: BaseEvent
    {
        public PosLikeEvent(): base(nameof(PosLikeEvent))
        {
            
        }

    }
}