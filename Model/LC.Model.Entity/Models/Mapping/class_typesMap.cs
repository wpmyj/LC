using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class class_typesMap : EntityTypeConfiguration<class_types>
    {
        public class_typesMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("class_types");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.total_lessons).HasColumnName("total_lessons");
            this.Property(t => t.unit_price).HasColumnName("unit_price");
            this.Property(t => t.commission_rate_teacher).HasColumnName("commission_rate_teacher");
            this.Property(t => t.commission_rate_assistant).HasColumnName("commission_rate_assistant");
            this.Property(t => t.commission_rate_consultant).HasColumnName("commission_rate_consultant");
            this.Property(t => t.student_limit).HasColumnName("student_limit");
            this.Property(t => t.is_active).HasColumnName("is_active");
            this.Property(t => t.description).HasColumnName("description");
        }
    }
}
