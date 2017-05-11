using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class class_recordMap : EntityTypeConfiguration<class_record>
    {
        public class_recordMap()
        {
            // Primary Key
            this.HasKey(t => t.class_record_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("class_record");
            this.Property(t => t.class_record_id).HasColumnName("class_record_id");
            this.Property(t => t.schedule_id).HasColumnName("schedule_id");
            this.Property(t => t.teacher_id).HasColumnName("teacher_id");
            this.Property(t => t.teacher_check_rate).HasColumnName("teacher_check_rate");
            this.Property(t => t.assistant_id).HasColumnName("assistant_id");
            this.Property(t => t.assistant_check_rate).HasColumnName("assistant_check_rate");
            this.Property(t => t.student_number).HasColumnName("student_number");
            this.Property(t => t.student_limit).HasColumnName("student_limit");
            this.Property(t => t.amount_receivable).HasColumnName("amount_receivable");
            this.Property(t => t.actual_amount).HasColumnName("actual_amount");
            this.Property(t => t.is_checked).HasColumnName("is_checked");
            this.Property(t => t.check_record_id).HasColumnName("check_record_id");
            this.Property(t => t.assistant_is_checked).HasColumnName("assistant_is_checked");
            this.Property(t => t.assistant_check_record_id).HasColumnName("assistant_check_record_id");
            this.Property(t => t.note).HasColumnName("note");

            // Relationships
            this.HasRequired(t => t.class_schedule)
                .WithMany(t => t.classrecords)
                .HasForeignKey(d => d.schedule_id);
            this.HasOptional(t => t.teacher_check_record)
                .WithMany(t => t.classRecords)
                .HasForeignKey(d => d.check_record_id);
            this.HasOptional(t => t.assistant_check_record)
                .WithMany(t => t.assistantRecords)
                .HasForeignKey(d => d.assistant_check_record_id);

        }
    }
}
