using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class students_logMap : EntityTypeConfiguration<students_log>
    {
        public students_logMap()
        {
            // Primary Key
            this.HasKey(t => t.log_id);

            // Properties
            this.Property(t => t.who)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.time)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("students_log");
            this.Property(t => t.log_id).HasColumnName("log_id");
            this.Property(t => t.student_id).HasColumnName("student_id");
            this.Property(t => t.who).HasColumnName("who");
            this.Property(t => t.time).HasColumnName("time");
            this.Property(t => t.log).HasColumnName("log");
        }
    }
}
