using FinalTask.Casino;
using FinalTask.Services;

var savePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "FinalTask");

var service = new FileSystemSaveLoadService(savePath);
var casino = new Casino(service);
casino.StartGame();