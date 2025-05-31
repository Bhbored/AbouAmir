using AbouAmir.Services;
using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AbouAmir
{
   
public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            InitializeDatabase(); // Initialize the database at startup
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        private static  void InitializeDatabase()

        {

            using (var dbContext = new AppDbContext())

            {
                try

                {

                      dbContext.Database.EnsureCreated();

                    Debug.WriteLine("Database initialized and tables created.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error initializing database: {ex.Message}");
                }
            }
        }
    }
}
