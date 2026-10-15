using Domain.Common;

namespace Domain.Entities;
public class SavedJob : BaseEntity<int>
{
    public int UserId { get; private set; }
    public int JobId { get; private set; }

    public User User { get; private set; } = null!;
    public Job Job { get; private set; } = null!;

    private SavedJob()
    {
    }

    public SavedJob(int userId, int jobId)
    {
        UserId = userId;
        JobId = jobId;
    }
}
