using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class class_scheduleMap : EntityTypeConfiguration<class_schedule>
    {
        public class_scheduleMap()
        {
            // Primary Key
            this.HasKey(t => t.schedule_id);

            // Table & Column Mappings
            this.ToTable("class_schedule");
            this.Property(t => t.schedule_id).HasColumnName("schedule_id");
            this.Property(t => t.class_id).HasColumnName("class_id");
            this.Property(t => t.real_date).HasColumnName("real_date");
            this.Property(t => t.start_time).HasColumnName("start_time");
            this.Property(t => t.end_date).HasColumnName("end_date");
            this.Property(t => t.teacher_id).HasColumnName("teacher_id");
            this.Property(t => t.assistant_id).HasColumnName("assistant_id");
            this.Property(t => t.classroom_id).HasColumnName("classroom_id");
            this.Property(t => t.lesson_schemas_id).HasColumnName("lesson_schemas_id");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.lesson_schemas_text).HasColumnName("lesson_schemas_text");
            this.Property(t => t.note).HasColumnName("note");

            // Relationships
            this.HasRequired(t => t.wclass)
                .WithMany(t => t.class_schedule)
                .HasForeignKey(d => d.class_id);
            this.HasOptional(t => t.schemas)
                .WithMany(t => t.class_schedule)
                .HasForeignKey(d => d.lesson_schemas_id);
            this.HasRequired(t => t.schedulestatus)
                .WithMany(t => t.class_schedule)
                .HasForeignKey(d => d.status);
            this.HasOptional(t => t.assistant)
                .WithMany(t => t.class_schedule1)
                .HasForeignKey(d => d.assistant_id);
            this.HasRequired(t => t.teacher)
                .WithMany(t => t.class_schedule)
                .HasForeignKey(d => d.teacher_id);
            this.HasRequired(t => t.classroom)
                .WithMany(t => t.schedule)
                .HasForeignKey(d => d.classroom_id);
        }
    }
}
