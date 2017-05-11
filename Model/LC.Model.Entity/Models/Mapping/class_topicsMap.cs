using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class class_topicsMap : EntityTypeConfiguration<class_topics>
    {
        public class_topicsMap()
        {
            // Primary Key
            this.HasKey(t => t.class_topic_id);

            // Properties
            this.Property(t => t.topic_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("class_topics");
            this.Property(t => t.class_topic_id).HasColumnName("class_topic_id");
            this.Property(t => t.class_type_id).HasColumnName("class_type_id");
            this.Property(t => t.unit_number).HasColumnName("unit_number");
            this.Property(t => t.topic_name).HasColumnName("topic_name");
        }
    }
}
