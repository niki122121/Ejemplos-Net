namespace Ejemplo_scoreboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Importante: esto hacerlo con una clase o struct en vez de con dos listas
            List<string> userNames = new List<string>();
            List<int> scores = new List<int>();

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "ALL OK");

            //Importante: el server no tiene memoria persistente, es decir al apagarlo perdemos el ranking. Esto se puede arreglar guardando y cargando
            //todo en un txt
            app.MapGet("/addScore/{userName}/{score}", (HttpContext context) =>
            {
                string userName = context.Request.RouteValues["userName"].ToString();
                int score = int.Parse(context.Request.RouteValues["score"].ToString());

                userNames.Add(userName);
                scores.Add(score);

                Console.WriteLine("Added: " + userName + " with score " + score);
                return "Added";
            });

            app.MapGet("/getScores", () =>
            {
                string aux = "";
                for (int i = 0; i < scores.Count; i++)
                {
                    aux += userNames[i] + ":  " + scores[i] + "\n";
                }
                return aux;
            });

            app.MapGet("/getScoresPretty", (HttpContext context) =>
            {
                string aux = "<html><head><style>";
                aux += "p { margin: 8px 0; color: #555; font-size: 18px; }";
                aux += "</style></head><body>";
                aux += "<h1>Scores</h1>";

                for (int i = 0; i < scores.Count; i++)
                {
                    aux += "<p>" + userNames[i] + ": <strong>" + scores[i] + "</strong></p>";
                }
                aux += "</body></html>";

                context.Response.ContentType = "text/html";
                return aux;
            });

            app.Run();
        }
    }
}
