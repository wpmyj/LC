using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class teachers_check_recordMap : EntityTypeConfiguration<teachers_check_record>
    {
        public teachers_check_recordMap()
        {
            // Primary Key
            this.HasKey(t => t.check_record_id);

            // Properties
            this.Property(t => t.check_user)
                .HasMaxLength(50);
            this.Property(t => t.check_month)
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("teachers_check_record");
            this.Property(t => t.check_record_id).HasColumnName("check_record_id");
            this.Property(t => t.teacher_id).HasColumnName("teacher_id");
            this.Property(t => t.check_time).HasColumnName("check_time");
            this.Property(t => t.total_money).HasColumnName("total_money");
            this.Property(t => t.check_rate).HasColumnName("check_rate");
            this.Property(t => t.check_user).HasColumnName("check_user");
            this.Property(t => t.check_month).HasColumnName("check_month");

            // Relationships
            this.HasOptional(t => t.SysUser)
                .WithMany(t => t.teachers_check_record)
                .HasForeignKey(d => d.check_user);
            this.HasRequired(t => t.teacher)
                .WithMany(t => t.teachers_check_record)
                .HasForeignKey(d => d.teacher_id);

        }
    }
}
