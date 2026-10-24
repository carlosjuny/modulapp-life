using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AccountStore(ApplicationDbContext db) : IAccountStore
{
    public Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public async Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken)
        => await db.Memberships
            .Include(m => m.Household)
            .Where(m => m.UserId == userId && m.Status == MembershipStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken)
        => await db.Memberships
            .ForHousehold(householdId)
            .Where(m => m.Status == MembershipStatus.Active)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task RemoveSignInDataAsync(Guid userId, CancellationToken cancellationToken)
    {
        db.UserCredentials.RemoveRange(await db.UserCredentials.Where(c => c.UserId == userId).ToListAsync(cancellationToken));
        db.ExternalLogins.RemoveRange(await db.ExternalLogins.Where(l => l.UserId == userId).ToListAsync(cancellationToken));
        db.RefreshTokens.RemoveRange(await db.RefreshTokens.Where(t => t.UserId == userId).ToListAsync(cancellationToken));
    }

    public void RemoveHousehold(Household household) => db.Households.Remove(household);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
