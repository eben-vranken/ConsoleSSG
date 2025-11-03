using System.Text.RegularExpressions;

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
                
                string[] tags = ((IEnumerable<object>)metadataObject["tags"]).Select(t => t.ToString()).ToArray();
                
                Page page = new Page(metadataObject["title"], DateTime.Parse(metadataObject["date"]), tags, pageContent, project.Key, metadataObject["template"], project.Value);
                
                BuildPage(page);
            }
        }
        else
        {
            _printer.WriteLineColor("No markdown files found", Program.HeaderColor);
        }
        
        Thread.Sleep(5000);
    }

    public void BuildPage(Page page)
    {
        Console.WriteLine("Searching for matching template");
        // Find correct template
        string folderPath = Path.Combine(AppContext.BaseDirectory, "Page Templates");
        string templatePath = Path.Combine(folderPath, page.Template + ".html");
        Uri path =  new Uri(templatePath);
        Console.WriteLine(path.AbsoluteUri);
        if (File.Exists(templatePath))
        {
            Console.WriteLine("Writing html file");
            // Get page
            string htmlPage = File.ReadAllText(templatePath);
            
            // Replace template elements with actual content
            htmlPage = htmlPage.Replace("{{title}}", page.Title);
            htmlPage = htmlPage.Replace("{{date}}", page.Date.ToShortDateString());
            htmlPage = htmlPage.Replace("{{tags}}", string.Join(", ", page.Tags));
            
            // Parse content
            string[] content = page.Content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            bool isOrderedList = false;
            bool isUnOrderedList = false;
            bool inParagraph = false;
            
            for (int i = 0; i < content.Length; i++)
            {
                string line = content[i];
                
                // Headings
                if (line.StartsWith("### "))
                {
                    line = "<h3>" + line.Substring(4) + "</h3>";
                } else if (line.StartsWith("## "))
                {
                    line = "<h2>" + line.Substring(3) + "</h2>";
                } else if (line.StartsWith("# "))
                {
                    line = "<h1>" + line.Substring(2) + "</h1>";
                }
                
                // Find bold
                line = Regex.Replace(line, @"\*\*(.+?)\*\*", "<b>$1</b>");
                
                // Find italic
                line = Regex.Replace(line, @"\*(.+?)\*", "<em>$1</em>");
                
                // Find lists
                // Unordered
                if (line.StartsWith("- "))
                {
                    line = Regex.Replace(line, @"^- (.+)", "<li>$1</li>");
                    if (!isUnOrderedList)
                    {
                        // Start of a list
                        line = "<ul>\n" + line;
                        isUnOrderedList = true;
                    }
                    
                } else if (isUnOrderedList)
                {
                    line = "</ul>\n" + line;
                    isUnOrderedList = false;
                }
                
                // Ordered
                var match = Regex.Match(line, @"^(\d+)\.\s+(.+)$");
                if (match.Success)
                {
                    string itemText = match.Groups[2].Value;
                    line = $"<li>{itemText}</li>";

                    if (!isOrderedList)
                    {
                        line = "<ol>\n" + line;
                        isOrderedList = true;
                    }
                }
                else
                {
                    if (isOrderedList)
                    {
                        line = "</ol>\n" + line;
                        isOrderedList = false;
                    }
                }
                
                content[i] = line;
            }
            
            string finalContent = string.Join("\n",  content);
            
            htmlPage = htmlPage.Replace("{{content}}", finalContent.Replace("\n\n", "<br>"));

            string pagePath = page.Directory + "/" + page.Title + ".html";
            
            Console.WriteLine(pagePath);
            
            Console.WriteLine("Creating html file");
            
            File.WriteAllLines(pagePath, new[] { htmlPage });
            Uri filePath =  new Uri(pagePath);
            Console.WriteLine($"Created file at {filePath.AbsoluteUri}");
        }
        else
        {
            Console.WriteLine("Template not found");
        }
    }
}