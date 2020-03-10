using System;

class ErrorWriter {
    public static void Write(Symbol cause, string message) {
        Console.WriteLine(string.Format("Error at line {0}: {1}", cause.GetLine(), message));
    }

    public static void Write(Symbol cause, string template, params object[] args) {
        var message = string.Format(template, args);
        Write(cause, message);
    }
}