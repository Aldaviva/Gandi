namespace Gandi;

public class GandiException(string message, Exception cause): ApplicationException(message, cause);