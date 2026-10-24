using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class HouseholdStore(ApplicationDbContext db) : IHouseholdStore
{
    public Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<Membership?> FindMembershipAsync(Guid userId, Guid householdId, CancellationToken cancellationToken)
        => db.Memberships
            .Include(m => m.Household)
            .ForHousehold(householdId)
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken)
        => await db.Memberships
            .AsNoTracking()
            .Include(m => m.Household)
            .Where(m => m.UserId == userId && m.Status == MembershipStatus.Active)
            .OrderBy(m => m.Household!.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken)
        => await db.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .ForHousehold(householdId)
            .Where(m => m.Status == MembershipStatus.Active)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<int> CountActiveAdminsAsync(Guid householdId, CancellationToken cancellationToken)
        => db.Memberships
            .ForHousehold(householdId)
            .CountAsync(m => m.Status == MembershipStatus.Active
                && (m.Role == MembershipRole.Admin || m.Role == MembershipRole.Owner), cancellationToken);

    public Task<Invitation?> FindInvitationAsync(string code, CancellationToken cancellationToken)
        => db.Invitations.Include(i => i.Household).FirstOrDefaultAsync(i => i.Code == code, cancellationToken);

    public void Add<TEntity>(TEntity entity) where TEntity : class => db.Add(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
