using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class classMap : EntityTypeConfiguration<classes>
    {
        public classMap()
        {
            // Primary Key
            this.HasKey(t => t.class_id);

            // Properties
            this.Property(t => t.class_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("classes");
            this.Property(t => t.class_id).HasColumnName("class_id");
            this.Property(t => t.class_name).HasColumnName("class_name");
            this.Property(t => t.class_type).HasColumnName("class_type");
            this.Property(t => t.last_count).HasColumnName("last_count");
            this.Property(t => t.start_date).HasColumnName("start_date");
            this.Property(t => t.end_date).HasColumnName("end_date");
            this.Property(t => t.is_active).HasColumnName("is_active");

            // Relationships
            this.HasRequired(t => t.parentClassTypes)
                .WithMany(t => t.subclasses)
                .HasForeignKey(d => d.class_type);

            this.HasMany(t => t.students)
                .WithMany(t => t.classess)
                .Map(m =>
                {
                    m.ToTable("class_students");
                    m.MapLeftKey("class_id");
                    m.MapRightKey("student_id");
                });
        }
    }
}
