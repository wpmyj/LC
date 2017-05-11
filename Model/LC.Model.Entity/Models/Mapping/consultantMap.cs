using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class consultantMap : EntityTypeConfiguration<consultant>
    {
        public consultantMap()
        {
            // Primary Key
            this.HasKey(t => t.consultant_id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.abbreviation)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("consultants");
            this.Property(t => t.consultant_id).HasColumnName("consultant_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.abbreviation).HasColumnName("abbreviation");
            this.Property(t => t.commission_rate).HasColumnName("commission_rate");

            // Relationships
            this.HasMany(t => t.students)
                .WithMany(t => t.consultants)
                .Map(m =>
                    {
                        m.ToTable("consultant_student");
                        m.MapLeftKey("consultant_id");
                        m.MapRightKey("student_id");
                    });


        }
    }
}
