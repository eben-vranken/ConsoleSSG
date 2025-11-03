namespace ConsoleSSG;

class Program
{
    static Printer _printer = new Printer();
    static ProjectManager _projectManager = new ProjectManager();
    static SiteGenerator _siteGenerator = new SiteGenerator();
    
    // Project settings
    const string Version = "0.0.1";
    
    // Visual Options
    public const ConsoleColor HeaderColor = ConsoleColor.Yellow;
    public const ConsoleColor ListKeyColor = ConsoleColor.Red;
    public const ConsoleColor ListValueColor = ConsoleColor.Magenta;
    public const ConsoleColor ListValueColorSecondary = ConsoleColor.Yellow;
    public const ConsoleColor UserInput =  ConsoleColor.Green;
    public const ConsoleColor StatusMessage =  ConsoleColor.White;
    
    public const int ListPadding = 3;
    
    // Main Menu Options
    private static readonly Dictionary<string, string> MainMenuOptions = new Dictionary<string, string>()
    {
        { "N", "New Project" },
        { "R", "Remove Project" },
        { "O", "Open Project" },
        { "E", "Exit" }
    };

    // Open Project Options
    private static readonly Dictionary<string, string> ProjectOptions = new Dictionary<string, string>()
    {
        { "O", "Open Project (Browser)" },
        { "B", "Build Project" },
        { "E", "Exit to Main Menu" }
    };
    
    private static string _statusMessage = "";
    
    static void Main()
    {
        _printer.PrintBox("Console SSG - Version: " + Version);
        ListProjects();
        
        PrintMenu(MainMenuOptions);

        string userInput = GetValidInput("Choose option:",MainMenuOptions.Keys.ToArray());
        
        while (userInput != "E")
        {
            switch (userInput)
            {
                case "N":
                    NewProject();
                    break;
                case "R":
                    RemoveProject();
                    break;
                case "O":
                    OpenProject();
                    break;
            }
            
            Console.Clear();
            
            _printer.PrintBox("Console SSG - Version: " + Version);
            ListProjects();
            PrintMenu(MainMenuOptions);
            
            _printer.WriteLineColor(_statusMessage, StatusMessage);
            userInput = GetValidInput("Choose option:",MainMenuOptions.Keys.ToArray());
        }
        
        _printer.WriteLineColor("Exiting", ConsoleColor.Red);
    }

    static void ListProjects()
    {
        Dictionary<string, string> projects = _projectManager.GetProjects();
        
        _printer.WriteLineColor("Projects:", HeaderColor);

        if (projects.Count == 0)
        {
            _printer.WriteLineColor($"{new string(' ', ListPadding-1)}No Projects!", ListKeyColor);        }
        else
        {
            for (int i = 0; i < projects.Count; i++)
            {
                KeyValuePair<string, string> project = projects.ElementAt(i);
               
                _printer.WriteColor($"{"[", ListPadding}{i+1}] ", ListKeyColor);
                _printer.WriteColor($"{project.Key} ", ListValueColor);
                var uri = new Uri(project.Value);
                _printer.WriteLineColor($"({uri.AbsoluteUri})", ListValueColorSecondary);
            }
        }

        Console.WriteLine();
    }
    
    static void PrintMenu(Dictionary<string, string> menuOptions)
    {
        _printer.WriteLineColor("Options:", HeaderColor);
        for (int i = 0; i < menuOptions.Count; i++)
        {
            KeyValuePair<string, string> menuOption = menuOptions.ElementAt(i);
            
            _printer.WriteColor($"{"[", ListPadding}{menuOption.Key}] ", ListKeyColor);
            _printer.WriteLineColor(menuOption.Value, ListValueColor);
        }

        Console.WriteLine();
    }

    static string GetValidInput(string prompt, string[] validInputs)
    {
        while (true)
        {
            _printer.WriteColor($"{prompt} ", UserInput);
            string input = Console.ReadLine() ?? "";

            if (validInputs.Contains(input) || validInputs.Contains(input.ToUpper()))
            {
                return input.ToUpper();
            }
            
            _printer.WriteLineColor("Invalid input!", ConsoleColor.Red);
        }
    }

    static string GetInputRequired(string prompt)
    {
        while (true)
        {
            _printer.WriteColor($"{prompt} ", UserInput);
            string input = Console.ReadLine() ?? "";

            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            _printer.WriteLineColor("Input cannot be empty!", ConsoleColor.Red);
        }
    }

    static string GetInput(string prompt)
    {
        _printer.WriteColor($"{prompt} ", UserInput);
        string input = Console.ReadLine() ?? "";

        return input;
    }
    
    static void NewProject()
    {
        string projectName = GetInputRequired("Project name:");
        string projectDirectory = GetInput("Project directory:");

        if (string.IsNullOrEmpty(projectDirectory))
        {
            if (_projectManager.AddProject(projectName))
            {
                _statusMessage = "Project created";
            }
        }
        else
        {
            if (_projectManager.AddProject(projectName, projectDirectory))
            {
                _statusMessage = "Project created";
            }
        }
    }

    static void RemoveProject()
    {
        Dictionary<string, string> projects = _projectManager.GetProjects();
        
        // Populate valid inputs
        string[] validInputs = Enumerable.Range(1, projects.Count).Select(i => i.ToString()).ToArray();
        
        int projectNumber = int.Parse(GetValidInput("Select project to delete:", validInputs));
        
        if (_projectManager.RemoveProject(projects.ElementAt(projectNumber - 1).Key))
        {
            _statusMessage = "Project removed";
        }
    }

    static void OpenProject()
    {
        Dictionary<string, string> projects = _projectManager.GetProjects();

        if (projects.Count == 0)
        {
            _statusMessage = "No project found";
            return;
        }
        
        // Project selector
        string[] validInputs = Enumerable.Range(1, projects.Count).Select(i => i.ToString()).ToArray();
        int projectNumber = int.Parse(GetValidInput("Select project to open:", validInputs));
        KeyValuePair<string, string> selectedProject = _projectManager.GetProject(projectNumber - 1);
        
        
        string userInput = "";
        while (userInput != "E")
        {
            Console.Clear();
            
            _printer.PrintBox(selectedProject.Key);
            _printer.WriteLineColor("Project Info:", HeaderColor);
            _printer.WriteColor($"{new string(' ', ListPadding-1)}Directory: ", ListKeyColor);
            var uri = new Uri(selectedProject.Value);
            _printer.WriteLineColor($"({uri.AbsoluteUri})", ListValueColorSecondary);

            Console.WriteLine();
            
            PrintMenu(ProjectOptions);
            
            userInput = GetValidInput("Choose option:", ProjectOptions.Keys.ToArray());

            switch (userInput)
            {
                case "B":
                    _siteGenerator.BuildProject(selectedProject);
                    break;
            }
        }
    }
}