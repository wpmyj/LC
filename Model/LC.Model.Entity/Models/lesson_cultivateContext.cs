using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using LC.Model.Entity.Models.Mapping;

namespace LC.Model.Entity.Models
{
    public partial class lesson_cultivateContext : DbContext
    {
        static lesson_cultivateContext()
        {
            Database.SetInitializer<lesson_cultivateContext>(null);
        }

        public lesson_cultivateContext()
            : base("Name=lesson_cultivateContext")
        {
        }

        public DbSet<center_classrooms> center_classrooms { get; set; }
        public DbSet<center> centers { get; set; }
        public DbSet<class_record> class_record { get; set; }
        public DbSet<class_record_detail> class_record_detail { get; set; }
        public DbSet<class_schedule> class_schedule { get; set; }
        public DbSet<class_topics> class_topics { get; set; }
        public DbSet<class_types> class_types { get; set; }
        public DbSet<classes> classes { get; set; }
        public DbSet<consultant_check_record> consultant_check_record { get; set; }
        public DbSet<consultant> consultants { get; set; }
        public DbSet<lesson_schemas> lesson_schemas { get; set; }
        public DbSet<lessons_new_vocabulary> lessons_new_vocabulary { get; set; }
        public DbSet<status> statuses { get; set; }
        public DbSet<student_recharge_detail> student_recharge_detail { get; set; }
        public DbSet<student> students { get; set; }
        public DbSet<students_log> students_log { get; set; }
        public DbSet<SysFunction> SysFunctions { get; set; }
        public DbSet<SysMenu> SysMenus { get; set; }
        public DbSet<SysModule> SysModules { get; set; }
        public DbSet<SysRight> SysRights { get; set; }
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysSubSystem> SysSubSystems { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<teacher> teachers { get; set; }
        public DbSet<teachers_check_record> teachers_check_record { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new center_classroomsMap());
            modelBuilder.Configurations.Add(new centerMap());
            modelBuilder.Configurations.Add(new class_recordMap());
            modelBuilder.Configurations.Add(new class_record_detailMap());
            modelBuilder.Configurations.Add(new class_scheduleMap());
            modelBuilder.Configurations.Add(new class_topicsMap());
            modelBuilder.Configurations.Add(new class_typesMap());
            modelBuilder.Configurations.Add(new classMap());
            modelBuilder.Configurations.Add(new consultant_check_recordMap());
            modelBuilder.Configurations.Add(new consultantMap());
            modelBuilder.Configurations.Add(new lesson_schemasMap());
            modelBuilder.Configurations.Add(new lessons_new_vocabularyMap());
            modelBuilder.Configurations.Add(new statusMap());
            modelBuilder.Configurations.Add(new student_recharge_detailMap());
            modelBuilder.Configurations.Add(new studentMap());
            modelBuilder.Configurations.Add(new students_logMap());
            modelBuilder.Configurations.Add(new SysFunctionMap());
            modelBuilder.Configurations.Add(new SysMenuMap());
            modelBuilder.Configurations.Add(new SysModuleMap());
            modelBuilder.Configurations.Add(new SysRightMap());
            modelBuilder.Configurations.Add(new SysRoleMap());
            modelBuilder.Configurations.Add(new SysSubSystemMap());
            modelBuilder.Configurations.Add(new SysUserMap());
            modelBuilder.Configurations.Add(new teacherMap());
            modelBuilder.Configurations.Add(new teachers_check_recordMap());
        }
    }
}
