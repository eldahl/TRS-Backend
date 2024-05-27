using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TRS_backend.Models;

namespace TRS_backend.DBModel
{
    public class TRSDbContext : DbContext
    {

        private readonly IConfiguration _configuration;

        public TRSDbContext(IConfiguration configuration, DbContextOptions<TRSDbContext> options) : base(options) {
            _configuration = configuration;
        }

        public DbSet<TblUsers> Users { get; set; }
        public DbSet<TblOpenDays> OpenDays { get; set; }
        public DbSet<TblTimeSlots> TimeSlots { get; set; }
        public DbSet<TblTables> Tables { get; set; }
        public DbSet<TblTableReservations> TableReservations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_configuration["ConnectionStrings:MySQLDB"]!);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Do conversion between TimeOnly and TimeSpan, because EF can't handle things for us.
            var timeOnlyToTimeSpanConverter = new ValueConverter<TimeOnly, TimeSpan>(
                timeOnlyValue => timeOnlyValue.ToTimeSpan(),
                timeSpanValue => TimeOnly.FromTimeSpan(timeSpanValue)
            );
            var dateOnlyToDateTimeConverter = new ValueConverter<DateOnly, DateTime>(
                dateOnlyValue => new DateTime(dateOnlyValue, new TimeOnly(0)),
                dateTimeValue => DateOnly.FromDateTime(dateTimeValue)
            );
            // Tbl Time Slots
            modelBuilder.Entity<TblTimeSlots>().Property(ts => ts.StartTime).HasConversion(timeOnlyToTimeSpanConverter);
            modelBuilder.Entity<TblTimeSlots>().Property(ts => ts.Date).HasConversion(dateOnlyToDateTimeConverter);
            
            // Tbl Open Days
            modelBuilder.Entity<TblOpenDays>().Property(od => od.OpenTime).HasConversion(timeOnlyToTimeSpanConverter);
            modelBuilder.Entity<TblOpenDays>().Property(od => od.CloseTime).HasConversion(timeOnlyToTimeSpanConverter);
            modelBuilder.Entity<TblOpenDays>().Property(od => od.Date).HasConversion(dateOnlyToDateTimeConverter);
            
            // Foreign keys so that we can seed data properly below
            modelBuilder.Entity<TblTableReservations>()
                .HasOne(tr => tr.Table)
                .WithMany()
                .HasForeignKey(tr => tr.TableId);
            modelBuilder.Entity<TblTableReservations>()
                .HasOne(tr => tr.OpenDay)
                .WithMany()
                .HasForeignKey(tr => tr.OpenDayId);
            modelBuilder.Entity<TblTableReservations>()
                .HasOne(tr => tr.TimeSlot)
                .WithMany()
                .HasForeignKey(tr => tr.TimeSlotId);

            ///////////////
            // Data seed //
            ///////////////
            modelBuilder.Entity<TblUsers>().HasData(
                new TblUsers
                {
                    Id = 1,
                    Email = "testAdmin@test.dk",
                    Username = "testAdmin",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.Now,
                    PasswordHash = new byte[] {
                        0x84, 0x7F, 0x92, 0xCB, 0xE8, 0x6D, 0xA8, 0x86, 0x5F, 0x43, 0x4C,
                        0xC4, 0x50, 0x48, 0xF9, 0xF2, 0xF8, 0x03, 0x02, 0x43, 0xAD, 0xB4,
                        0x0D, 0x4A, 0x5F, 0x1F, 0x75, 0xAC, 0xC4, 0xC8, 0x56, 0x02
                    },
                    Salt = new byte[] {
                        0xE3, 0x09, 0x10, 0x01, 0x8F, 0x3B, 0x2E, 0x5A, 0x7F, 0xE2, 0x48,
                        0x3D, 0xB2, 0xDB, 0xF1, 0xB3, 0x04, 0x2F, 0xC6, 0x84, 0xD9, 0x84,
                        0x12, 0x61, 0xFB, 0xC3, 0x60, 0xCF, 0x1B, 0x96, 0x3A, 0xCA
                    }
                },
                new TblUsers
                {
                    Id = 2,
                    Email = "testUser@test.dk",
                    Username = "testUser",
                    Role = UserRole.User,
                    CreatedAt = DateTime.Now,
                    PasswordHash = new byte[] {
                        0xFE, 0x20, 0x3C, 0xD0, 0x32, 0x54, 0x83, 0x26, 0x3B, 0x3B, 0x62,
                        0xE7, 0x5A, 0x82, 0x0B, 0x68, 0xEB, 0xA3, 0xD1, 0x75, 0x0E, 0x3F,
                        0xFA, 0x69, 0xF3, 0x09, 0x00, 0x65, 0xF2, 0xFC, 0x5C, 0x14
                    },
                    Salt = new byte[] {
                        0x4A, 0x9C, 0xCB, 0xC4, 0xCE, 0x06, 0x2F, 0xA3, 0xC8, 0xD4, 0xB1,
                        0xF5, 0x0C, 0x8A, 0x33, 0x15, 0x09, 0x65, 0xA5, 0x25, 0x9E, 0xDE,
                        0x94, 0x2B, 0x97, 0xD9, 0xDE, 0x5E, 0x92, 0x41, 0x8E, 0xE8
                    }
                }
            );

            modelBuilder.Entity<TblTables>().HasData(
                new TblTables
                {
                    Id = 1,
                    TableName = "Table 10",
                    Seats = 8
                },
                new TblTables
                {
                    Id = 2,
                    TableName = "Table 11",
                    Seats = 6
                },
                new TblTables
                {
                    Id = 3,
                    TableName = "Table 12",
                    Seats = 6
                },
                new TblTables
                {
                    Id = 4,
                    TableName = "Table 13",
                    Seats = 4
                },
                new TblTables
                {
                    Id = 5,
                    TableName = "Table 17",
                    Seats = 4
                },
                new TblTables
                {
                    Id = 6,
                    TableName = "Table 19",
                    Seats = 4
                },
                new TblTables {
                    Id = 7,
                    TableName = "Table 101",
                    Seats = 8
                },
                new TblTables
                {
                    Id = 8,
                    TableName = "Table 110",
                    Seats = 8
                },
                new TblTables
                {
                    Id = 9,
                    TableName = "Table 103",
                    Seats = 6
                }
            );

            modelBuilder.Entity<TblOpenDays>().HasData(
                new TblOpenDays
                {
                    Id = 1,
                    Date = new DateOnly(2024, 5, 25),
                    OpenTime = new TimeOnly(16, 0),
                    CloseTime = new TimeOnly(21, 0)
                },
                new TblOpenDays
                {
                    Id = 2,
                    Date = new DateOnly(2024, 5, 26),
                    OpenTime = new TimeOnly(16, 0),
                    CloseTime = new TimeOnly(21, 0)
                }
            );

            modelBuilder.Entity<TblTimeSlots>().HasData(
                new TblTimeSlots
                {
                    Id = 1,
                    Date = new DateOnly(2024, 05, 25),
                    StartTime = new TimeOnly(16, 0),
                    Duration = new TimeSpan(2, 0, 0)
                }
            );

            modelBuilder.Entity<TblTableReservations>().HasData(
                new TblTableReservations
                {
                    Id = 1,
                    OpenDayId = 1,
                    TimeSlotId = 1,
                    TableId = 1,
                    FullName = "Customer user",
                    Email = "customer@gmail.com",
                    PhoneNumber = "12345678",
                    SendReminders = true,
                    Comment = "This is a comment"
                }
            );
        }
    }
}
