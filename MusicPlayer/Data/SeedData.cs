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
        
        FillRoles(context);
        FillAdminUser(context);
    }
    
    private static void FillRoles(DbContext dbContext)
    {
        if (dbContext.Set<IdentityRole>().Any()) return;
        dbContext.Set<IdentityRole>().AddRange(new IdentityRole
        {
            Name = "Админ"
        }, new IdentityRole
        {
            Name = "Холоп"
        });
        dbContext.SaveChanges();
    }

    private static void FillAdminUser(DbContext dbContext)
    {
        if (dbContext.Set<IdentityUser>().Any()) return;
        dbContext.Set<IdentityUser>().Add(new IdentityUser
        {
            Id = "49ca44b1-12e0-4e78-a1e8-56532a20dcf1",
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

        dbContext.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
        {
            UserId = "49ca44b1-12e0-4e78-a1e8-56532a20dcf1",
            RoleId = dbContext.Set<IdentityRole>().Single(x => x.Name == "Админ").Id
        });

        dbContext.SaveChanges();
    }
}