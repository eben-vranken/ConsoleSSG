namespace ConsoleSSG;

class Program
{
    static Printer _printer = new Printer();
    static ProjectManager _projectManager = new ProjectManager();
    
    // Project settings
    const string Version = "0.0.1";
    
    // Visual Options
    const ConsoleColor HeaderColor = ConsoleColor.Yellow;
    const ConsoleColor ListKeyColor = ConsoleColor.Red;
    const ConsoleColor ListValueColor = ConsoleColor.Magenta;
    const ConsoleColor ListValueColorSecondary = ConsoleColor.Yellow;
    const ConsoleColor UserInput =  ConsoleColor.Green;
    const ConsoleColor StatusMessage =  ConsoleColor.White;
    
    const int ListPadding = 3;
    
    // List options
    private static readonly Dictionary<string, string> MenuOptions = new Dictionary<string, string>()
    {
        { "N", "New Project" },
        { "R", "Remove Project" },
        { "O", "Open Project" },
        { "E", "Exit" }
    };

    private static string _statusMessage = "";
    
    static void Main()
    {
        PrintMenu();

        string userInput = GetValidInput("Choose option:",MenuOptions.Keys.ToArray());
        
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
            }
            
            Console.Clear();
            
            PrintMenu();
            
            _printer.WriteLineColor(_statusMessage, StatusMessage);
            
            userInput = GetValidInput("Choose option:",MenuOptions.Keys.ToArray());
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
                _printer.WriteLineColor($"({project.Value})", ListValueColorSecondary);
            }
        }

        Console.WriteLine();
    }
    
    static void PrintMenu()
    {
        _printer.PrintBox("Console SSG - Version: " + Version);
        ListProjects();
        
        _printer.WriteLineColor("Options:", HeaderColor);
        for (int i = 0; i < MenuOptions.Count; i++)
        {
            KeyValuePair<string, string> menuOption = MenuOptions.ElementAt(i);
            
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
}