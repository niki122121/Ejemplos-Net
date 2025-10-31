using System.Data;

namespace Ejemplo_guardado
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GameDataFolder");
            Directory.CreateDirectory(appDataFolderPath);
            string persistentFilePath = Path.Combine(appDataFolderPath, "persitentfile.txt");
            Console.WriteLine("SAVE PATH:  " + persistentFilePath);

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "ALL OK");

            app.MapGet("/testSave", (HttpContext context) =>
            {
                string contentToAdd = "Data saved at " + DateTime.Now + "\n";
                File.AppendAllText(persistentFilePath, contentToAdd);

                return "Added to file";
            });
            app.MapGet("/testRead", (HttpContext context) =>
            {
                if (File.Exists(persistentFilePath))
                {
                    string fileContent = File.ReadAllText(persistentFilePath);
                    return fileContent;
                }
                else
                {
                    return "Saved data file NOT found";
                }
            });

            app.Run();
        }
    }
}
