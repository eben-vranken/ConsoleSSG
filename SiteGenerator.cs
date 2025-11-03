namespace ConsoleSSG;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class SiteGenerator
{
    Printer _printer = new Printer();
    
    public void BuildProject(KeyValuePair<string, string> project, string delimiter = "---")
    {
        _printer.PrintBox($"Building {project.Key}");
        
        // Scan for all md files in directory
        _printer.WriteLineColorUnderline("Scanning directory for markdown files:", Program.HeaderColor);
        var markdownFiles = Directory.GetFiles(project.Value, "*.md");

        if (markdownFiles.Length > 0)
        {
            // Initialize deserializer
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            
            foreach (var file in markdownFiles)
            {
                _printer.WriteLineColor($"{new string(' ', Program.ListPadding - 1)}Found: {file}", Program.ListKeyColor);
            }

            Console.WriteLine();

            _printer.WriteLineColorUnderline("Parse markdown files", Program.HeaderColor);
        
            foreach (var file in markdownFiles)
            {
                _printer.WriteLineColor($"{new string(' ', Program.ListPadding - 1)}{file})", Program.ListKeyColor);
                
                string fileContent = File.ReadAllText(file);
                   
                // Get metadata
                int metadataStart = fileContent.IndexOf(delimiter, StringComparison.Ordinal) + delimiter.Length;
                int metadataEnd = fileContent.LastIndexOf(delimiter, StringComparison.Ordinal);
                
                // Entire metadata
                string metadata =  fileContent.Substring(metadataStart, metadataEnd - metadataStart);

                var metadataObject = deserializer.Deserialize<dynamic>(metadata);
                
                // Get page content
                string pageContent = fileContent.Substring(metadataEnd + delimiter.Length, fileContent.Length - metadataEnd - delimiter.Length).Trim();
                
                BlogPost blogPost = new BlogPost(metadataObject["title"], DateTime.Parse(metadataObject["date"]), metadataObject["tags"], pageContent, metadataObject["template"]);
                
                BuildPage(blogPost);
            }
        }
        else
        {
            _printer.WriteLineColor("No markdown files found", Program.HeaderColor);
        }
        
        Thread.Sleep(5000);
    }

    public void BuildPage(BlogPost page)
    {
        
    }
}