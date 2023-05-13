using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MusicPlayer.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Применяем все миграции, которых еще нет в базе данных
        context.Database.Migrate();
        
        FillFirstUser(context);
    }

    private static void FillFirstUser(DbContext dbContext)
    {
        if (dbContext.Set<IdentityUser>().Any()) return;
        dbContext.Set<IdentityUser>().Add(new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "astarkulov.aktan@gmail.com",
            NormalizedUserName = "ASTARKULOV.AKTAN@GMAIL.COM",
            Email = "astarkulov.aktan@gmail.com",
            NormalizedEmail = "ASTARKULOV.AKTAN@GMAIL.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAEAACcQAAAAEM9T1btPvthpRewJfQmF2HbnB2k+xSp5sJoKSoVR2dHRzDo3Z8IJbuUlXegDooF9Ig==",
            SecurityStamp = "QBMZ3SJVWTETDHP2WP6LWFBIA4VNLYCG",
            ConcurrencyStamp = "5f09a263-490f-4998-bb5a-2e9ccda3cf5f",
            LockoutEnabled = true
        });

        dbContext.SaveChanges();
    }
}