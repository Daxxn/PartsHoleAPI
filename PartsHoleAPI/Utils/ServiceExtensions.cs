namespace PartsHoleAPI.Utils;

public static class ServiceExtensions
{
   public static void AddAbstractFactory<TInterface, TImplementation>(this IServiceCollection services)
      where TInterface : class
      where TImplementation : class, TInterface
   {
      services.AddTransient<TInterface, TImplementation>();
      services.AddSingleton<Func<TInterface>>(x => () => x.GetService<TInterface>()!);
      services.AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();
   }

   public static void ApiLog<T>(this ILogger<T> logger, LogLevel level, string method, string path, string message) =>
      logger.Log(level, "api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogInfo<T>(this ILogger<T> logger, string method, string path, string message) =>
      logger.LogInformation("api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogError<T>(this ILogger<T> logger, string method, string path, string message, Exception ex) =>
      logger.LogError(ex, "api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogWarn<T>(this ILogger<T> logger, string method, string path, string message) =>
      logger.LogWarning("api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogTrace<T>(this ILogger<T> logger, string method, string path, string message) =>
      logger.LogTrace("api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogDebug<T>(this ILogger<T> logger, string method, string path, string message) =>
      logger.LogDebug("api|{method}|{path}|{message}", method, path, message);

   public static void ApiLogCritical<T>(this ILogger<T> logger, string method, string path, string message) =>
      logger.LogCritical("api|{method}|{path}|{message}", method, path, message);
}
