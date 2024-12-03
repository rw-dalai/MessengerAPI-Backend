namespace MessengerAPI.Models;

// Custom Exception
// - Specific exception type for domain errors
// - Helps distinguish between different error types
// - Enables specific error handling in controllers
public class MessengerException(string message) : Exception(message);