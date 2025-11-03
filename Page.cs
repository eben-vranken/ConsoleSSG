namespace ConsoleSSG;

public class Page
{
    // Page content
    public string Title {get;set;}
    public DateTime Date {get;set;}
    public string[] Tags {get;set;}
    public string Content {get;set;}
    
    // Page properties
    public string Project {get;set;}
    public string Template { get; set; }
    public string Directory { get; set; }

    public Page(string title, DateTime date, string[] tags, string content, string project, string template, string directory)
    {
        // Page content
        Title = title;
        Date = date;
        Tags = tags;
        Content = content;
        
        // Page properties
        Project = project;
        Template = template;
        Directory = directory;
    }
}