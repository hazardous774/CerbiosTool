﻿using CerbiosTool;

try
{
    var version = "V1.1.2";
    var application = new ApplicationUI();
    application.Start(version);
}
catch (Exception ex)
{
    var now = DateTime.Now.ToString("MMddyyyyHHmmss");
    File.WriteAllText($"Crashlog-{now}.txt", ex.ToString());
    Console.WriteLine($"Error: {ex}");
}



