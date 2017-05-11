using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class lessons_new_vocabularyMap : EntityTypeConfiguration<lessons_new_vocabulary>
    {
        public lessons_new_vocabularyMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.topic_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.vocab_type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.vocab_text)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("lessons_new_vocabulary");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.lesson_schemas_id).HasColumnName("lesson_schemas_id");
            this.Property(t => t.topic_name).HasColumnName("topic_name");
            this.Property(t => t.vocab_type).HasColumnName("vocab_type");
            this.Property(t => t.vocab_text).HasColumnName("vocab_text");

            // Relationships
            this.HasRequired(t => t.lesson_schemas)
                .WithMany(t => t.lessons_new_vocabulary)
                .HasForeignKey(d => d.lesson_schemas_id);

        }
    }
}
