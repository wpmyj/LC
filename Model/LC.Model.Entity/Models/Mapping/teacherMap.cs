using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class teacherMap : EntityTypeConfiguration<teacher>
    {
        public teacherMap()
        {
            // Primary Key
            this.HasKey(t => t.teacher_id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.mobile)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("teachers");
            this.Property(t => t.teacher_id).HasColumnName("teacher_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.mobile).HasColumnName("mobile");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.UserCode).HasColumnName("UserCode");

            // Relationships
            this.HasRequired(t => t.status1)
                .WithMany(t => t.teachers)
                .HasForeignKey(d => d.status);

        }
    }
}
