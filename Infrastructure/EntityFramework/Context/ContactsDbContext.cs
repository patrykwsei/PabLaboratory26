using AppCore.Interfaces;
using AppCore.Models.Contacts;
using AppCore.Models.Contacts.Enums;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ContactsDbContext()
    {
    }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=contacts.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.FullName).HasMaxLength(200);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.Property(u => u.Email).HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Status).HasConversion<string>();
        });

        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(50);
        });

        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Email).HasMaxLength(200);
            entity.Property(c => c.Phone).HasMaxLength(20);
            entity.Property(c => c.Status).HasConversion<string>();
        });

        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.FirstName).HasMaxLength(100);
            entity.Property(p => p.LastName).HasMaxLength(200);
            entity.Property(p => p.MiddleName).HasMaxLength(100);
            entity.Property(p => p.Position).HasMaxLength(100);
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
        });

        builder.Entity<Company>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(200);
            entity.Property(c => c.NIP).HasMaxLength(20);
            entity.Property(c => c.REGON).HasMaxLength(20);
            entity.Property(c => c.KRS).HasMaxLength(20);
            entity.Property(c => c.Industry).HasMaxLength(100);
            entity.Property(c => c.Website).HasMaxLength(300);
        });

        builder.Entity<Organization>(entity =>
        {
            entity.Property(o => o.Name).HasMaxLength(200);
            entity.Property(o => o.KRS).HasMaxLength(20);
            entity.Property(o => o.Website).HasMaxLength(300);
            entity.Property(o => o.Mission).HasMaxLength(1000);
            entity.Property(o => o.Type).HasConversion<string>();
        });

        builder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(c => c.Employees)
            .HasForeignKey(p => p.EmployerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Person>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Company>()
            .HasOne(c => c.PrimaryContact)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Organization>()
            .HasOne(o => o.PrimaryContact)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Note>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Content).HasMaxLength(2000);

            entity.HasOne(n => n.Contact)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.Token).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.ExpiresAt).IsRequired();
        });

        builder.Entity<Contact>().OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.PostalCode).HasMaxLength(10);
            address.Property(a => a.Country).HasMaxLength(100);
            address.Property(a => a.Type).HasConversion<string>();
        });

        var adminRoleId = "11111111-1111-1111-1111-111111111111";
        var salesRoleId = "22222222-2222-2222-2222-222222222222";
        var readOnlyRoleId = "33333333-3333-3333-3333-333333333333";

        builder.Entity<CrmRole>().HasData(
            new CrmRole
            {
                Id = adminRoleId,
                Name = UserRole.Administrator.ToString(),
                NormalizedName = UserRole.Administrator.ToString().ToUpper(),
                Description = "System administrator"
            },
            new CrmRole
            {
                Id = salesRoleId,
                Name = UserRole.SalesManager.ToString(),
                NormalizedName = UserRole.SalesManager.ToString().ToUpper(),
                Description = "Sales manager"
            },
            new CrmRole
            {
                Id = readOnlyRoleId,
                Name = UserRole.ReadOnly.ToString(),
                NormalizedName = UserRole.ReadOnly.ToString().ToUpper(),
                Description = "Read only user"
            }
        );

        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        var adamId = Guid.Parse("3D54091D-ABC8-49EC-9590-93AD3ED5458F");
        var ewaId = Guid.Parse("B4DCB17C-F875-43F8-9D66-36597895A466");

        builder.Entity<Company>().HasData(
            new Company
            {
                Id = companyId,
                Name = "WSEI",
                Industry = "edukacja",
                Phone = "123567123",
                Email = "biuro@wsei.edu.pl",
                Website = "https://wsei.edu.pl",
                Status = ContactStatus.Active,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        builder.Entity<Person>().HasData(
            new
            {
                Id = adamId,
                FirstName = "Adam",
                LastName = "Nowak",
                MiddleName = (string?)null,
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "adam@wsei.edu.pl",
                Phone = "123456789",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Programista",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EmployerId = (Guid?)companyId,
                OrganizationId = (Guid?)null
            },
            new
            {
                Id = ewaId,
                FirstName = "Ewa",
                LastName = "Kowalska",
                MiddleName = (string?)null,
                Gender = Gender.Female,
                Status = ContactStatus.Blocked,
                Email = "ewa@wsei.edu.pl",
                Phone = "123123123",
                BirthDate = new DateTime(2001, 1, 11),
                Position = "Tester",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EmployerId = (Guid?)companyId,
                OrganizationId = (Guid?)null
            }
        );

        builder.Entity<Contact>()
            .OwnsOne(c => c.Address)
            .HasData(
                new
                {
                    ContactId = adamId,
                    Street = "ul. Św. Filipa 17",
                    City = "Kraków",
                    PostalCode = "25-009",
                    Country = "Poland",
                    Type = AddressType.Correspondence
                }
            );
    }
}