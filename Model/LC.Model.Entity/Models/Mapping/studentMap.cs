using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class studentMap : EntityTypeConfiguration<student>
    {
        public studentMap()
        {
            // Primary Key
            this.HasKey(t => t.student_id);

            // Properties
            this.Property(t => t.moms_name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.dads_name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.students_name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.students_nickname)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.address)
                .HasMaxLength(100);

            this.Property(t => t.email)
                .HasMaxLength(50);

            this.Property(t => t.relationship)
                .HasMaxLength(50);

            this.Property(t => t.grade)
                .HasMaxLength(20);

            this.Property(t => t.moms_phone)
                .HasMaxLength(40);

            this.Property(t => t.dads_phone)
                .HasMaxLength(40);

            this.Property(t => t.school)
                .HasMaxLength(20);

            this.Property(t => t.google_contacts_id)
                .HasMaxLength(200);

            this.Property(t => t.rfid_tag)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("students");
            this.Property(t => t.student_id).HasColumnName("student_id");
            this.Property(t => t.center_id).HasColumnName("center_id");
            this.Property(t => t.moms_name).HasColumnName("moms_name");
            this.Property(t => t.dads_name).HasColumnName("dads_name");
            this.Property(t => t.students_birthdate).HasColumnName("students_birthdate");
            this.Property(t => t.students_name).HasColumnName("students_name");
            this.Property(t => t.students_nickname).HasColumnName("students_nickname");
            this.Property(t => t.address).HasColumnName("address");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.relationship).HasColumnName("relationship");
            this.Property(t => t.extra_info).HasColumnName("extra_info");
            this.Property(t => t.original_class).HasColumnName("original_class");
            this.Property(t => t.grade).HasColumnName("grade");
            this.Property(t => t.moms_phone).HasColumnName("moms_phone");
            this.Property(t => t.dads_phone).HasColumnName("dads_phone");
            this.Property(t => t.school).HasColumnName("school");
            this.Property(t => t.remaining_balance).HasColumnName("remaining_balance");
            this.Property(t => t.google_contacts_id).HasColumnName("google_contacts_id");
            this.Property(t => t.rfid_tag).HasColumnName("rfid_tag");
            this.Property(t => t.consultant_check_rate).HasColumnName("consultant_check_rate");
            this.Property(t => t.status).HasColumnName("status");

            // Relationships
            this.HasRequired(t => t.status1)
                .WithMany(t => t.students)
                .HasForeignKey(d => d.status);
        }
    }
}
