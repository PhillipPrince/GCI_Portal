using System.IO;
using System.Text.RegularExpressions;

class Program {
    static void Main() {
        string path = @"c:\Users\USER\Projects\GCI\GCI_Portal\GCI_Admin\Views\Home\_DashboardContent.cshtml";
        string content = File.ReadAllText(path);
        string[] patterns = new[] { "ðŸ“…", "ðŸ†•", "ðŸ“ˆ", "ðŸ‘‘", "ðŸ‘¥" };
        foreach (var pattern in patterns) {
            content = Regex.Replace(content, pattern + "\s?", "");
        }
        File.WriteAllText(path, content, new System.Text.UTF8Encoding(false));
    }
}
