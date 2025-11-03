namespace ConsoleSSG;

public class BlogPost
{
    // Page content
    public string Title {get;set;}
    public DateTime Date {get;set;}
    public string[] Tags {get;set;}
    public string Content {get;set;}
    
    // Page properties
    public string template { get; set; }

    public BlogPost(string title, DateTime date, string[] tags, string content, string template)
    {
        // Page content
        this.Title = title;
        this.Date = date;
        this.Tags = tags;
        this.Content = content;
        
        // Page properties
        this.template = template;
    }
}