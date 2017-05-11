using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class consultant_check_recordMap : EntityTypeConfiguration<consultant_check_record>
    {
        public consultant_check_recordMap()
        {
            // Primary Key
            this.HasKey(t => t.consultant_check_record_id);

            // Table & Column Mappings
            this.ToTable("consultant_check_record");
            this.Property(t => t.consultant_check_record_id).HasColumnName("consultant_check_record_id");
            this.Property(t => t.consultant_id).HasColumnName("consultant_id");
            this.Property(t => t.check_time).HasColumnName("check_time");
            this.Property(t => t.total_money).HasColumnName("total_money");
            this.Property(t => t.check_user).HasColumnName("check_user");
            this.Property(t => t.check_month).HasColumnName("check_month");

            // Relationships
            this.HasRequired(t => t.consultant)
                .WithMany(t => t.consultant_check_record)
                .HasForeignKey(d => d.consultant_id);

        }
    }
}
