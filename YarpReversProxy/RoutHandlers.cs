namespace YarpReversProxy
{
    internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary);   
    public class RoutHandlers
    {
        string[] summaries = new[] {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};
        internal WeatherForecast[] GetWeatherForecast()
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
               new WeatherForecast
               (
                   DateTime.Now.AddDays(index),
                   Random.Shared.Next(-20, 55),
                   summaries[Random.Shared.Next(summaries.Length)]
               ))
                .ToArray();
            return forecast;
        }
    }
}
