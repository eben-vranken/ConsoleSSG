using System.Text.Json;

namespace ConsoleSSG;

public class ProjectManager
{
    readonly Printer _printer =  new Printer();

    private Dictionary<string, string> _projects = new Dictionary<string, string>();
    private readonly string _filePath = "projects.json";

    public ProjectManager()
    {
        LoadProjects();
    }
    
    /// <summary>
    /// Initializes the projects and adds them to siteDirectories
    /// </summary>
    public void LoadProjects()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json =  File.ReadAllText(_filePath);
                _projects = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ??  new Dictionary<string, string>();
            }
            else
            {
                _projects = new Dictionary<string, string>();
                SaveProjects();
            }
            
        }
        catch (Exception e)
        {
            _printer.PrintError(e.Message);
        }
    }

    public void SaveProjects()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_projects, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception e)
        {
            _printer.PrintError(e.Message);
        }
    }
    
    /// <summary>
    /// This method creates a new project to the project manager
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="filePath">The file path the project will be stored.</param>
    /// <returns></returns>
    public bool AddProject(string name, string? filePath = null)
    {
        try
        {
            filePath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySiteGenerator", name);

            _projects.Add(name, filePath);
            SaveProjects();
            return true;
        }
        catch (Exception e)
        {
            _printer.PrintError(e.Message);
            return false;
        }
    }


    /// <summary>
    /// Removes a project based on the project name.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Whether the action succeeded or not.</returns>
    public bool RemoveProject(string key)
    {
        try
        {
            bool result = _projects.Remove(key);
            if (result)
            {
                SaveProjects();
            }
            return result;
        }
        catch (Exception e)
        {
            _printer.PrintError(e.Message);
            return false;
        }
    }

    public Dictionary<string, string> GetProjects()
    {
        return _projects;
    }
}