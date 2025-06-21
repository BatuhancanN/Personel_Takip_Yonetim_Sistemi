using Microsoft.EntityFrameworkCore;
using Personel_Takip_Yönetim_Sistemi.Models;
using Personel_Takip_Yönetim_Sistemi.Models.ViewModels;

namespace Personel_Takip_Yönetim_Sistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Hareket> Hareketler { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<HareketViewModel> HareketViewModels { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<LoginEditViewModel> LoginEditViewModels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //HareketViewModel'i keyless olarak tanımlıyoruz
            modelBuilder.Entity<HareketViewModel>().HasNoKey();
            modelBuilder.Entity<LoginEditViewModel>().HasNoKey();
        }
    }
}
