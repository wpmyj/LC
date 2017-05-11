using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class center_classroomsMap : EntityTypeConfiguration<center_classrooms>
    {
        public center_classroomsMap()
        {
            // Primary Key
            this.HasKey(t => t.classroom_id);

            // Properties
            this.Property(t => t.classroom_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("center_classrooms");
            this.Property(t => t.classroom_id).HasColumnName("classroom_id");
            this.Property(t => t.center_id).HasColumnName("center_id");
            this.Property(t => t.classroom_name).HasColumnName("classroom_name");
            this.Property(t => t.upper_limit).HasColumnName("upper_limit");

            this.HasRequired(t => t.parentCenter)
                .WithMany(t => t.subclassrooms)
                .HasForeignKey(d => d.center_id);
        }
    }
}
