using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class student_recharge_detailMap : EntityTypeConfiguration<student_recharge_detail>
    {
        public student_recharge_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.recharge_id);

            // Properties
            this.Property(t => t.recharge_user)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("student_recharge_detail");
            this.Property(t => t.recharge_id).HasColumnName("recharge_id");
            this.Property(t => t.student_id).HasColumnName("student_id");
            this.Property(t => t.amount).HasColumnName("amount");
            this.Property(t => t.inout_type).HasColumnName("inout_type");
            this.Property(t => t.incur_time).HasColumnName("incur_time");
            this.Property(t => t.recharge_user).HasColumnName("recharge_user");
            this.Property(t => t.class_record_detail_id).HasColumnName("class_record_detail_id");

            // Relationships
            this.HasRequired(t => t.student)
                .WithMany(t => t.student_recharge_detail)
                .HasForeignKey(d => d.student_id);
            this.HasOptional(t => t.SysUser)
                .WithMany(t => t.student_recharge_detail)
                .HasForeignKey(d => d.recharge_user);
            this.HasOptional(t => t.class_record_detail)
                .WithMany(t => t.student_recharge_detail)
                .HasForeignKey(d => d.class_record_detail_id);

        }
    }
}
