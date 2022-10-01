using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggragetes
{
    public class PostAggregate: AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent {
                Id =id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
                throw new InvalidOperationException("You cannot comment inactive posts");
            if (String.IsNullOrWhiteSpace(message))
                throw new InvalidOperationException("Message cannot be empty");

            RaiseEvent(new MessageUpdateEvent() {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdateEvent @event)
        {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
                throw new InvalidOperationException("You cannot like inactive posts");

             RaiseEvent(new PosLikeEvent() {
                Id = _id
            });
        }

        public void Apply(PosLikeEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!_active)
                throw new InvalidOperationException("You cannot comment inactive posts");
            if (String.IsNullOrWhiteSpace(comment))
                throw new InvalidOperationException("Comment cannot be empty");

            RaiseEvent(new CommentAddedEvent() {
                Id = _id,
                Comment = comment,
                CommentDate = DateTime.Now,
                Username = username,
                CommentId = Guid.NewGuid()
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
                throw new InvalidOperationException("You cannot comment inactive posts");
            if (String.IsNullOrWhiteSpace(comment))
                throw new InvalidOperationException("Comment cannot be empty");
            if (!_comments[commentId].Item2.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("You are not allowed to edit comments of other users");

            RaiseEvent(new CommentUpdatedEvent() {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
                throw new InvalidOperationException("You cannot comment inactive posts");
            if (!_comments[commentId].Item2.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("You are not allowed to edit comments of other users");

            RaiseEvent(new CommentRemovedEvent() {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string username)
         {
            if (!_active)
                throw new InvalidOperationException("You cannot comment inactive posts");
            if (!_author.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("You are not allowed to remove posts of other users");

            RaiseEvent(new PostRemovedEvent() {
                Id = _id
            });
         }

         public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}