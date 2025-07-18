using Microsoft.EntityFrameworkCore;
using HRSM.Models;
using System.Data.Common;

namespace HRSM.Data
{
    public class HRSMDbContext : DbContext
    {
        public HRSMDbContext(DbContextOptions<HRSMDbContext> options)
        : base(options)
    {
    }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Virtual DbSet for Staff (uses the User table)
        public virtual DbSet<Staff> Staff => Set<Staff>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");
                entity.HasKey(e => e.User_Id);
                entity.Property(e => e.User_Id)
                    .HasColumnName("USER_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasColumnName("FIRST_NAME").IsRequired();
                entity.Property(e => e.LastName).HasColumnName("LAST_NAME").IsRequired();
                entity.Property(e => e.Contact).HasColumnName("CONTACT").IsRequired();
                entity.Property(e => e.Email).HasColumnName("EMAIL").IsRequired();
                entity.Property(e => e.Password).HasColumnName("PASSWORD").IsRequired();
                entity.Property(e => e.Role).HasColumnName("ROLE").IsRequired();
                entity.Property(e => e.ProfilePicturePath).HasColumnName("PROFILE_PICTURE_PATH");
                entity.Property(e => e.MemberSince).HasColumnName("MEMBER_SINCE");

                // Add staff-specific columns (Will only be populated for Staff users)
                entity.Property<string>("Department").HasColumnName("DEPARTMENT").IsRequired(false);
                entity.Property<DateTime?>("HireDate").HasColumnName("HIRE_DATE").IsRequired(false);

                // Add unique constraint for email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Booking table
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("BOOKING");
                entity.HasKey(e => e.BookingId);
                entity.Property(e => e.BookingId)
                    .HasColumnName("BOOKING_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.ReservationNumber).HasColumnName("RESERVATION_NUMBER").IsRequired();
                entity.Property(e => e.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(e => e.RoomNumber).HasColumnName("ROOM_NUMBER").IsRequired();
                entity.Property(e => e.RoomType).HasColumnName("ROOM_TYPE").IsRequired();
                entity.Property(e => e.RoomRate).HasColumnName("ROOM_RATE").IsRequired();
                entity.Property(e => e.CheckInDate).HasColumnName("CHECK_IN_DATE").IsRequired();
                entity.Property(e => e.CheckOutDate).HasColumnName("CHECK_OUT_DATE").IsRequired();
                entity.Property(e => e.BookingDate).HasColumnName("BOOKING_DATE").IsRequired();
                entity.Property(e => e.BookingType).HasColumnName("BOOKING_TYPE").IsRequired();
                entity.Property(e => e.NumAdults).HasColumnName("NUM_ADULTS").IsRequired();
                entity.Property(e => e.NumChildren).HasColumnName("NUM_CHILDREN").IsRequired();
                entity.Property(e => e.ExtraBeds).HasColumnName("EXTRA_BEDS");
                entity.Property(e => e.ExtraRooms).HasColumnName("EXTRA_ROOMS");
                entity.Property(e => e.SpaService).HasColumnName("SPA_SERVICE");
                entity.Property(e => e.RoomService).HasColumnName("ROOM_SERVICE");
                entity.Property(e => e.AirportTransfer).HasColumnName("AIRPORT_TRANSFER");
                entity.Property(e => e.SpecialRequests).HasColumnName("SPECIAL_REQUESTS");
                entity.Property(e => e.TotalAmount).HasColumnName("TOTAL_AMOUNT").IsRequired();
                entity.Property(e => e.Status).HasColumnName("STATUS").IsRequired();
                entity.Property(e => e.PaymentMethod).HasColumnName("PAYMENT_METHOD").IsRequired();

                // Configure relationship with User
                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Room table
            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("ROOM");
                entity.HasKey(e => e.RoomId);
                entity.Property(e => e.RoomId)
                    .HasColumnName("ROOM_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RoomNumber).HasColumnName("ROOM_NUMBER").IsRequired();
                entity.Property(e => e.RoomType).HasColumnName("ROOM_TYPE").IsRequired();
                entity.Property(e => e.Status).HasColumnName("STATUS").IsRequired();
                entity.Property(e => e.StatusUpdatedAt).HasColumnName("STATUS_UPDATED_AT").IsRequired();
                entity.Property(e => e.UpdatedBy).HasColumnName("UPDATED_BY");

                // Add unique constraint for room number
                entity.HasIndex(e => e.RoomNumber).IsUnique();
            });
            
            // Configure ServiceRequest table
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.ToTable("SERVICE_REQUEST");
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.RequestId)
                    .HasColumnName("REQUEST_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(e => e.RoomNumber).HasColumnName("ROOM_NUMBER").IsRequired();
                entity.Property(e => e.RequestType).HasColumnName("REQUEST_TYPE").IsRequired();
                entity.Property(e => e.ServiceCategory).HasColumnName("SERVICE_CATEGORY");
                entity.Property(e => e.HousekeepingNeeds).HasColumnName("HOUSEKEEPING_NEEDS");
                entity.Property(e => e.Description).HasColumnName("DESCRIPTION").IsRequired();
                entity.Property(e => e.RequestDate).HasColumnName("REQUEST_DATE").IsRequired();
                entity.Property(e => e.Status).HasColumnName("STATUS").IsRequired();

                // Configure relationship with User
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Feedback table
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("FEEDBACK");
                entity.HasKey(e => e.FeedbackId);
                entity.Property(e => e.FeedbackId)
                    .HasColumnName("FEEDBACK_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(e => e.RoomType).HasColumnName("ROOM_TYPE").IsRequired();
                entity.Property(e => e.Rating).HasColumnName("RATING").IsRequired();
                entity.Property(e => e.Comment).HasColumnName("COMMENT").IsRequired();
                entity.Property(e => e.SubmitDate).HasColumnName("SUBMIT_DATE").IsRequired();
                entity.Property(e => e.StayType).HasColumnName("STAY_TYPE").IsRequired();
                entity.Property(e => e.AdminResponse).HasColumnName("ADMIN_RESPONSE");
                entity.Property(e => e.ResponseDate).HasColumnName("RESPONSE_DATE");

                // Configure relationship with User
                entity.HasOne(f => f.User)
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configure Activity table
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("ACTIVITY");
                entity.HasKey(e => e.ActivityId);
                entity.Property(e => e.ActivityId)
                    .HasColumnName("ACTIVITY_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Message).HasColumnName("MESSAGE").IsRequired();
                entity.Property(e => e.ActivityType).HasColumnName("ACTIVITY_TYPE").IsRequired();
                entity.Property(e => e.Icon).HasColumnName("ICON").IsRequired();
                entity.Property(e => e.RelatedId).HasColumnName("RELATED_ID");
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(e => e.RelatedUrl).HasColumnName("RELATED_URL");
            });

            // Configure Event table
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("EVENT");
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.EventId)
                    .HasColumnName("EVENT_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasColumnName("TITLE").IsRequired();
                entity.Property(e => e.Location).HasColumnName("LOCATION").IsRequired();
                entity.Property(e => e.Description).HasColumnName("DESCRIPTION").IsRequired();
                entity.Property(e => e.EventDate).HasColumnName("EVENT_DATE").IsRequired();
                entity.Property(e => e.Participants).HasColumnName("PARTICIPANTS").IsRequired();
                entity.Property(e => e.Note).HasColumnName("NOTE").IsRequired(false);
                entity.Property(e => e.ImagePath).HasColumnName("IMAGE_PATH").IsRequired();
                entity.Property(e => e.DisplayOrder).HasColumnName("DISPLAY_ORDER").IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE").IsRequired();
            });

            // Configure Settings table
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.ToTable("SETTINGS");
                entity.HasKey(e => e.SettingId);
                entity.Property(e => e.SettingId)
                    .HasColumnName("SETTING_ID")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Key).HasColumnName("SETTING_KEY").IsRequired();
                entity.Property(e => e.Value).HasColumnName("SETTING_VALUE").IsRequired();
                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
                entity.Property(e => e.LastUpdated).HasColumnName("LAST_UPDATED").IsRequired();
                
                // Add unique constraint for Key
                entity.HasIndex(e => e.Key).IsUnique();
            });
        }

        public async Task EnsureDatabaseCreated()
        {
            try
            {
                // Check if database exists
                var databaseExists = await Database.CanConnectAsync();
                if (!databaseExists)
                {
                    await Database.EnsureCreatedAsync();
                }

                // Ensure Settings table exists and create if needed
                await EnsureSettingsTableExistsAsync();

                // Check if Room table exists
                bool roomTableExists = false;
                try
                {
                    // Try a different approach that won't throw an exception if the table doesn't exist
                    var result = await Database.ExecuteSqlRawAsync(
                        @"SELECT COUNT(*) FROM information_schema.tables 
                          WHERE table_schema = DATABASE() AND table_name = 'ROOM'");
                    roomTableExists = result > 0;
                }
                catch
                {
                    roomTableExists = false;
                }

                if (!roomTableExists)
                {
                    // Create Room table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS ROOM (
                            ROOM_ID INT AUTO_INCREMENT PRIMARY KEY,
                            ROOM_NUMBER VARCHAR(10) NOT NULL UNIQUE,
                            ROOM_TYPE VARCHAR(50) NOT NULL,
                            STATUS VARCHAR(50) NOT NULL,
                            STATUS_UPDATED_AT DATETIME NOT NULL,
                            UPDATED_BY VARCHAR(50)
                        )");
                    
                    // Initialize rooms with default data
                    await InitializeRoomsAsync();
                    
                    Console.WriteLine("ROOM table created and initialized successfully.");
                }
                else
                {
                    Console.WriteLine("ROOM table already exists.");
                }

                // Check if Staff columns exist in USER table
                try
                {
                    // This will throw an exception if any of these columns don't exist
                    await Database.ExecuteSqlRawAsync("SELECT DEPARTMENT, HIRE_DATE FROM USER LIMIT 1");
                    Console.WriteLine("Staff columns already exist in USER table.");
                }
                catch
                {
                    // If we get here, one or more columns don't exist
                    Console.WriteLine("Adding staff columns to USER table...");
                    
                    // Check each column individually and add if missing
                    try { await Database.ExecuteSqlRawAsync("SELECT DEPARTMENT FROM USER LIMIT 1"); } 
                    catch { await Database.ExecuteSqlRawAsync("ALTER TABLE USER ADD COLUMN DEPARTMENT VARCHAR(100) NULL"); }
                    
                    try { await Database.ExecuteSqlRawAsync("SELECT HIRE_DATE FROM USER LIMIT 1"); } 
                    catch { await Database.ExecuteSqlRawAsync("ALTER TABLE USER ADD COLUMN HIRE_DATE DATETIME NULL"); }
                    
                    Console.WriteLine("Staff columns added to USER table.");
                }

                // Check if ServiceRequest table exists
                bool serviceRequestTableExists = false;
                try
                {
                    // Check if we can query the table
                    var serviceRequestCount = await Set<ServiceRequest>().CountAsync();
                    serviceRequestTableExists = true;
                    Console.WriteLine($"SERVICE_REQUEST table exists with {serviceRequestCount} records.");
                }
                catch
                {
                    serviceRequestTableExists = false;
                }

                if (!serviceRequestTableExists)
                {
                    // Create ServiceRequest table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS SERVICE_REQUEST (
                            REQUEST_ID INT AUTO_INCREMENT PRIMARY KEY,
                            USER_ID INT NOT NULL,
                            ROOM_NUMBER VARCHAR(10) NOT NULL,
                            REQUEST_TYPE VARCHAR(20) NOT NULL,
                            SERVICE_CATEGORY VARCHAR(50),
                            HOUSEKEEPING_NEEDS VARCHAR(255),
                            DESCRIPTION TEXT NOT NULL,
                            REQUEST_DATE DATETIME NOT NULL,
                            STATUS VARCHAR(20) NOT NULL,
                            FOREIGN KEY (USER_ID) REFERENCES USER(USER_ID) ON DELETE CASCADE
                        )");
                    
                    Console.WriteLine("SERVICE_REQUEST table created successfully.");
                }
                else
                {
                    Console.WriteLine("SERVICE_REQUEST table already exists.");
                }

                // Check if Feedback table exists
                bool feedbackTableExists = false;
                try
                {
                    // Check if we can query the table
                    var feedbackCount = await Set<Feedback>().CountAsync();
                    feedbackTableExists = true;
                    Console.WriteLine($"FEEDBACK table exists with {feedbackCount} records.");
                }
                catch
                {
                    feedbackTableExists = false;
                }

                if (!feedbackTableExists)
                {
                    // Create Feedback table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS FEEDBACK (
                            FEEDBACK_ID INT AUTO_INCREMENT PRIMARY KEY,
                            USER_ID INT NOT NULL,
                            ROOM_TYPE VARCHAR(50) NOT NULL,
                            RATING INT NOT NULL,
                            COMMENT TEXT NOT NULL,
                            SUBMIT_DATE DATETIME NOT NULL,
                            STAY_TYPE VARCHAR(20) NOT NULL,
                            ADMIN_RESPONSE TEXT,
                            RESPONSE_DATE DATETIME,
                            FOREIGN KEY (USER_ID) REFERENCES USER(USER_ID) ON DELETE CASCADE
                        )");
                    
                    Console.WriteLine("FEEDBACK table created successfully.");
                }
                else
                {
                    // Check if the response columns exist and add them if they don't
                    await AddFeedbackResponseColumnsAsync();
                }

                // Check if Activity table exists
                bool activityTableExists = false;
                try
                {
                    // Check if we can query the table
                    var activityCount = await Set<Activity>().CountAsync();
                    activityTableExists = true;
                    Console.WriteLine($"ACTIVITY table exists with {activityCount} records.");
                }
                catch
                {
                    activityTableExists = false;
                }

                if (!activityTableExists)
                {
                    // Create Activity table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS ACTIVITY (
                            ACTIVITY_ID INT AUTO_INCREMENT PRIMARY KEY,
                            MESSAGE TEXT NOT NULL,
                            ACTIVITY_TYPE VARCHAR(50) NOT NULL,
                            ICON VARCHAR(50) NOT NULL,
                            RELATED_ID INT,
                            CREATED_AT DATETIME NOT NULL,
                            RELATED_URL VARCHAR(255)
                        )");
                    
                    // No longer seed sample activities as we now use real activity logging
                    
                    Console.WriteLine("ACTIVITY table created successfully.");
                }
                else
                {
                    Console.WriteLine("ACTIVITY table already exists.");
                }

                // After Activity table check, add the Notification table check
                // Check if Notification table exists
                bool notificationTableExists = false;
                try
                {
                    // Check if we can query the table
                    var notificationCount = await Set<Notification>().CountAsync();
                    notificationTableExists = true;
                    Console.WriteLine($"NOTIFICATION table exists with {notificationCount} records.");
                }
                catch
                {
                    notificationTableExists = false;
                }

                if (!notificationTableExists)
                {
                    // Create Notification table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS NOTIFICATION (
                            NOTIFICATION_ID INT AUTO_INCREMENT PRIMARY KEY,
                            USER_ID INT NOT NULL,
                            MESSAGE TEXT NOT NULL,
                            TYPE VARCHAR(50) NOT NULL,
                            ICON VARCHAR(50) NOT NULL,
                            RELATED_ID INT,
                            CREATED_AT DATETIME NOT NULL,
                            IS_READ TINYINT(1) NOT NULL DEFAULT 0,
                            RELATED_URL VARCHAR(255),
                            FOREIGN KEY (USER_ID) REFERENCES USER(USER_ID) ON DELETE CASCADE
                        )");
                    
                    Console.WriteLine("NOTIFICATION table created successfully.");
                }
                else
                {
                    Console.WriteLine("NOTIFICATION table already exists.");
                }

                // Check if Event table exists
                bool eventTableExists = false;
                try
                {
                    // Check if we can query the table
                    var eventCount = await Set<Event>().CountAsync();
                    eventTableExists = true;
                    Console.WriteLine($"EVENT table exists with {eventCount} records.");
                }
                catch
                {
                    eventTableExists = false;
                }

                if (!eventTableExists)
                {
                    // Create Event table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS EVENT (
                            EVENT_ID INT AUTO_INCREMENT PRIMARY KEY,
                            TITLE VARCHAR(100) NOT NULL,
                            LOCATION VARCHAR(100) NOT NULL,
                            DESCRIPTION VARCHAR(100) NOT NULL,
                            EVENT_DATE VARCHAR(100) NOT NULL,
                            PARTICIPANTS VARCHAR(100) NOT NULL,
                            NOTE VARCHAR(255),
                            IMAGE_PATH VARCHAR(255) NOT NULL,
                            DISPLAY_ORDER INT NOT NULL,
                            IS_ACTIVE BOOLEAN NOT NULL DEFAULT TRUE
                        )");
                    
                    // Initialize events with default data
                    await InitializeEventsAsync();
                    
                    Console.WriteLine("EVENT table created and initialized successfully.");
                }
                else
                {
                    Console.WriteLine("EVENT table already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring database and tables exist: {ex.Message}");
                throw new Exception("Error ensuring database and tables exist", ex);
            }
        }

        private async Task InitializeRoomsAsync()
        {
            Console.WriteLine("Initializing room data...");
            
            try {
                // First check if any rooms exist already
                var roomCount = await Set<Room>().CountAsync();
                if (roomCount > 0)
                {
                    Console.WriteLine($"Rooms already initialized ({roomCount} rooms found). Skipping initialization.");
                    return;
                }
                
                // Create standard rooms (301-330)
                for (int i = 301; i <= 330; i++)
                {
                    await Database.ExecuteSqlRawAsync(
                        @"INSERT INTO ROOM (ROOM_NUMBER, ROOM_TYPE, STATUS, STATUS_UPDATED_AT, UPDATED_BY) 
                          VALUES ({0}, {1}, {2}, {3}, {4})",
                        i.ToString(), "standard", "available", DateTime.UtcNow, "system");
                }

                // Create deluxe rooms (401-430)
                for (int i = 401; i <= 430; i++)
                {
                    await Database.ExecuteSqlRawAsync(
                        @"INSERT INTO ROOM (ROOM_NUMBER, ROOM_TYPE, STATUS, STATUS_UPDATED_AT, UPDATED_BY) 
                          VALUES ({0}, {1}, {2}, {3}, {4})",
                        i.ToString(), "deluxe", "available", DateTime.UtcNow, "system");
                }

                // Create suite rooms (501-530)
                for (int i = 501; i <= 530; i++)
                {
                    await Database.ExecuteSqlRawAsync(
                        @"INSERT INTO ROOM (ROOM_NUMBER, ROOM_TYPE, STATUS, STATUS_UPDATED_AT, UPDATED_BY) 
                          VALUES ({0}, {1}, {2}, {3}, {4})",
                        i.ToString(), "suite", "available", DateTime.UtcNow, "system");
                }

                // Set some initial room statuses for demonstration
                var demoRooms = new List<(string roomNumber, string status)>
                {
                    // Set some standard rooms as occupied
                    ("305", "occupied"), ("310", "occupied"), ("315", "occupied"), ("320", "occupied"),
                    
                    // Set some standard rooms as cleaning
                    ("307", "cleaning"), ("312", "cleaning"), ("325", "cleaning"),
                    
                    // Set some standard rooms as maintenance
                    ("303", "maintenance"), ("330", "maintenance"),
                    
                    // Set some deluxe rooms as occupied
                    ("405", "occupied"), ("410", "occupied"), ("415", "occupied"),
                    
                    // Set some deluxe rooms as cleaning
                    ("420", "cleaning"), ("425", "cleaning"),
                    
                    // Set some deluxe rooms as maintenance
                    ("430", "maintenance"),
                    
                    // Set some suite rooms as occupied
                    ("505", "occupied"), ("510", "occupied"),
                    
                    // Set some suite rooms as cleaning
                    ("515", "cleaning"),
                    
                    // Set some suite rooms as maintenance
                    ("520", "maintenance")
                };

                foreach (var room in demoRooms)
                {
                    await Database.ExecuteSqlRawAsync(
                        @"UPDATE ROOM SET STATUS = {0}, STATUS_UPDATED_AT = {1}, UPDATED_BY = {2} 
                          WHERE ROOM_NUMBER = {3}",
                        room.status, DateTime.UtcNow, "system", room.roomNumber);
                }
                
                Console.WriteLine("Room data initialization complete.");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error initializing room data: {ex.Message}");
                throw;
            }
        }

        private async Task InitializeEventsAsync()
        {
            Console.WriteLine("Initializing event data...");
            
            try {
                // First check if any events exist already
                var eventCount = await Set<Event>().CountAsync();
                if (eventCount > 0)
                {
                    Console.WriteLine($"Events already initialized ({eventCount} events found). Skipping initialization.");
                    return;
                }
                
                // Create default events
                var defaultEvents = new List<Event>
                {
                    new Event
                    {
                        Title = "FINE DINE",
                        Location = "Dining Area",
                        Description = "Fine Dine",
                        EventDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd"),
                        Participants = "Everyone in Hotel",
                        Note = "P.S. You Can Bring Your Love One's",
                        ImagePath = "/images/evepic1.png",
                        DisplayOrder = 1,
                        IsActive = true
                    },
                    new Event
                    {
                        Title = "FREE DRINKS",
                        Location = "Bar Area",
                        Description = "Free Drinks",
                        EventDate = DateTime.Today.AddDays(21).ToString("yyyy-MM-dd"),
                        Participants = "Everyone in Hotel",
                        Note = "P.S. You Can Bring Your Love One's",
                        ImagePath = "/images/evepic2.png",
                        DisplayOrder = 2,
                        IsActive = true
                    },
                    new Event
                    {
                        Title = "POOL PARTY",
                        Location = "Rooftop Pool",
                        Description = "Pool Party",
                        EventDate = DateTime.Today.AddDays(28).ToString("yyyy-MM-dd"),
                        Participants = "Everyone in Hotel",
                        Note = "P.S. You Can Bring Your Love One's",
                        ImagePath = "/images/evepic3.png",
                        DisplayOrder = 3,
                        IsActive = true
                    }
                };
                
                foreach (var evt in defaultEvents)
                {
                    // Create parameters array, ensuring we don't pass null for Note
                    var parameters = new object[] { 
                        evt.Title, 
                        evt.Location, 
                        evt.Description, 
                        evt.EventDate, 
                        evt.Participants, 
                        evt.Note ?? string.Empty, // Ensure Note is never null
                        evt.ImagePath, 
                        evt.DisplayOrder, 
                        evt.IsActive 
                    };
                    
                    await Database.ExecuteSqlRawAsync(@"
                        INSERT INTO EVENT (TITLE, LOCATION, DESCRIPTION, EVENT_DATE, PARTICIPANTS, NOTE, IMAGE_PATH, DISPLAY_ORDER, IS_ACTIVE)
                        VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        parameters);
                }
                
                Console.WriteLine("Event data initialization complete.");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error initializing event data: {ex.Message}");
                throw;
            }
        }

        private async Task SeedSampleActivitiesAsync()
        {
            // This method is kept for backwards compatibility but no longer used
            Console.WriteLine("Sample activity seeding is disabled as we now use real activity logging.");
            await Task.CompletedTask;
            return;
        }

        public async Task EnsureFeedbackTableExists()
        {
            bool feedbackTableExists = false;
            try
            {
                // Check if we can query the table
                var feedbackCount = await Set<Feedback>().CountAsync();
                feedbackTableExists = true;
                Console.WriteLine($"FEEDBACK table exists with {feedbackCount} records.");
            }
            catch
            {
                feedbackTableExists = false;
            }

            if (!feedbackTableExists)
            {
                // Create Feedback table
                await Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS FEEDBACK (
                        FEEDBACK_ID INT AUTO_INCREMENT PRIMARY KEY,
                        USER_ID INT NOT NULL,
                        ROOM_TYPE VARCHAR(50) NOT NULL,
                        RATING INT NOT NULL,
                        COMMENT TEXT NOT NULL,
                        SUBMIT_DATE DATETIME NOT NULL,
                        STAY_TYPE VARCHAR(20) NOT NULL,
                        ADMIN_RESPONSE TEXT,
                        RESPONSE_DATE DATETIME,
                        FOREIGN KEY (USER_ID) REFERENCES USER(USER_ID) ON DELETE CASCADE
                    )");
                
                Console.WriteLine("FEEDBACK table created successfully.");
            }
            else
            {
                // Check if the response columns exist and add them if they don't
                await AddFeedbackResponseColumnsAsync();
            }
        }
        
        public async Task AddFeedbackResponseColumnsAsync()
        {
            try
            {
                Console.WriteLine("Checking for feedback response columns...");
                
                // Check if ADMIN_RESPONSE column exists
                bool adminResponseColumnExists = false;
                try
                {
                    // Try to query the column
                    await Database.ExecuteSqlRawAsync("SELECT ADMIN_RESPONSE FROM FEEDBACK LIMIT 1");
                    adminResponseColumnExists = true;
                    Console.WriteLine("ADMIN_RESPONSE column already exists.");
                }
                catch
                {
                    Console.WriteLine("ADMIN_RESPONSE column does not exist. Adding it...");
                    // Add the column if it doesn't exist
                    await Database.ExecuteSqlRawAsync("ALTER TABLE FEEDBACK ADD COLUMN ADMIN_RESPONSE TEXT");
                    Console.WriteLine("ADMIN_RESPONSE column added successfully.");
                }
                
                // Check if RESPONSE_DATE column exists
                bool responseDateColumnExists = false;
                try
                {
                    // Try to query the column
                    await Database.ExecuteSqlRawAsync("SELECT RESPONSE_DATE FROM FEEDBACK LIMIT 1");
                    responseDateColumnExists = true;
                    Console.WriteLine("RESPONSE_DATE column already exists.");
                }
                catch
                {
                    Console.WriteLine("RESPONSE_DATE column does not exist. Adding it...");
                    // Add the column if it doesn't exist
                    await Database.ExecuteSqlRawAsync("ALTER TABLE FEEDBACK ADD COLUMN RESPONSE_DATE DATETIME");
                    Console.WriteLine("RESPONSE_DATE column added successfully.");
                }
                
                // Log the status of columns for debugging purposes
                Console.WriteLine($"Feedback columns status: ADMIN_RESPONSE={adminResponseColumnExists}, RESPONSE_DATE={responseDateColumnExists}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking or adding feedback response columns: {ex.Message}");
                throw new Exception("Error checking or adding feedback response columns", ex);
            }
        }

        public async Task EnsureSettingsTableExistsAsync()
        {
            try
            {
                // Check if Settings table exists
                bool settingsTableExists = false;
                try
                {
                    var result = await Database.ExecuteSqlRawAsync(
                        @"SELECT COUNT(*) FROM information_schema.tables 
                          WHERE table_schema = DATABASE() AND table_name = 'SETTINGS'");
                    settingsTableExists = result > 0;
                }
                catch
                {
                    settingsTableExists = false;
                }

                if (!settingsTableExists)
                {
                    // Create Settings table
                    await Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS SETTINGS (
                            SETTING_ID INT AUTO_INCREMENT PRIMARY KEY,
                            SETTING_KEY VARCHAR(50) NOT NULL UNIQUE,
                            SETTING_VALUE VARCHAR(500) NOT NULL,
                            DESCRIPTION VARCHAR(100),
                            LAST_UPDATED DATETIME NOT NULL
                        )");

                    // Add initial setting for EventPageTitle
                    await Database.ExecuteSqlRawAsync(@"
                        INSERT INTO SETTINGS (SETTING_KEY, SETTING_VALUE, DESCRIPTION, LAST_UPDATED)
                        VALUES ('EventPageTitle', 'Events', 'Title displayed on the Events page', UTC_TIMESTAMP())");
                    
                    Console.WriteLine("SETTINGS table created and initialized successfully.");
                }
                else
                {
                    // Check if EventPageTitle setting exists
                    var eventPageTitleExists = await Settings.AnyAsync(s => s.Key == "EventPageTitle");
                    if (!eventPageTitleExists)
                    {
                        // Add the EventPageTitle setting
                        Settings.Add(new Models.Settings
                        {
                            Key = "EventPageTitle",
                            Value = "Events",
                            Description = "Title displayed on the Events page",
                            LastUpdated = DateTime.UtcNow
                        });
                        await SaveChangesAsync();
                        Console.WriteLine("Added EventPageTitle setting.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring Settings table exists: {ex.Message}");
            }
        }
    }
}