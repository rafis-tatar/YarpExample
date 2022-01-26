namespace YarpReversProxy
{
    public static class Routes
    {
        public static WebApplication AddRoutes(this WebApplication app)
        {
            var handlers = app.Configuration.Get<YarpReversProxy.RoutHandlers>();

            app.MapGet("/weatherforecast", handlers.GetWeatherForecast).WithName("GetWeatherForecast");
            app.MapGet("/test", () => "Hello World Test!").WithName("Test");
            return app;
        }
    }
}
