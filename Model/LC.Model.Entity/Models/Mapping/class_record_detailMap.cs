using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class class_record_detailMap : EntityTypeConfiguration<class_record_detail>
    {
        public class_record_detailMap()
        {
            // Primary Key
            this.HasKey(t => t.class_record_detail_id);

            // Properties
            this.Property(t => t.class_record_detail_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.student_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.attendance_status)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("class_record_detail");
            this.Property(t => t.class_record_detail_id).HasColumnName("class_record_detail_id");
            this.Property(t => t.class_record_id).HasColumnName("class_record_id");
            this.Property(t => t.student_id).HasColumnName("student_id");
            this.Property(t => t.register_time).HasColumnName("register_time");
            this.Property(t => t.attendance_status).HasColumnName("attendance_status");
            this.Property(t => t.consultants_id).HasColumnName("consultants_id");
            this.Property(t => t.consultant_check_record_id).HasColumnName("consultant_check_record_id");
            this.Property(t => t.consultant_check_rate).HasColumnName("consultant_check_rate");
            this.Property(t => t.is_checked).HasColumnName("is_checked");

            // Relationships
            this.HasOptional(t => t.class_record)
                .WithMany(t => t.class_record_detail)
                .HasForeignKey(d => d.class_record_id);
            this.HasOptional(t => t.consultant_check_record)
                .WithMany(t => t.class_record_detail)
                .HasForeignKey(d => d.consultant_check_record_id);
            this.HasOptional(t => t.consultant)
                .WithMany(t => t.class_record_detail)
                .HasForeignKey(d => d.consultants_id);
            this.HasRequired(t => t.detailStatus)
                .WithMany(t => t.classRecordDetails)
                .HasForeignKey(t => t.attendance_status);
        }
    }
}
