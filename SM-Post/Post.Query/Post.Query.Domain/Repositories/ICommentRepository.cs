using CQRS.Core.Domain;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task CreateAsync(CommentEntity comment);
        Task UpdateAsync(CommentEntity post);
        Task DeleteAsync(Guid commentId);
        Task<CommentEntity> GetByCommentIdAsync(Guid postId);
    }
}