using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PublishExcel.WorkerService.Models
{
    public partial class RabbitMQPublishExcelDBContext : DbContext
    {
        public RabbitMQPublishExcelDBContext()
        {
        }

        public RabbitMQPublishExcelDBContext(DbContextOptions<RabbitMQPublishExcelDBContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Turkish_CI_AS");

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(e => e.Brand)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SubModel)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
