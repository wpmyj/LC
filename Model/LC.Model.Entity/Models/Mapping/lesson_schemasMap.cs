using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class lesson_schemasMap : EntityTypeConfiguration<lesson_schemas>
    {
        public lesson_schemasMap()
        {
            // Primary Key
            this.HasKey(t => t.lesson_schemas_id);

            // Properties
            this.Property(t => t.level_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.lesson_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("lesson_schemas");
            this.Property(t => t.lesson_schemas_id).HasColumnName("lesson_schemas_id");
            this.Property(t => t.class_type_id).HasColumnName("class_type_id");
            this.Property(t => t.level_name).HasColumnName("level_name");
            this.Property(t => t.lesson_name).HasColumnName("lesson_name");
            this.Property(t => t.sequence_num).HasColumnName("sequence_num");
            
        }
    }
}
