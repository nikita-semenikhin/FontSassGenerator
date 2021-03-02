using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FontSaasProcessor {
    class Program {
        static void Main(string[] args) {
            var workingDirectory = Environment.CurrentDirectory;
            var currentDir = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var fontsWoff = GetFonts(Path.Combine(currentDir, "Fonts"));
            var tst = GenerateScss("Roboto", fontsWoff["Roboto"]);
        }

        static string GenerateScss(string fontName, IEnumerable<string> files) {
            var result = "";
            var extensions = new Dictionary<string, IEnumerable<string>>();

            var grouppedBy = files.GroupBy(x => Path.GetFileNameWithoutExtension(x));
            var regex = new Regex(@"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");
            foreach(var name in grouppedBy) {
                result += "@font-face {\n";
                result += $"font-family: '{fontName}';\n";
                result += $"src: local('{name}');\n";
                var SplittedName = regex.Split(name.Key.Replace("-", "")).Where(x => !string.IsNullOrWhiteSpace(x));
                result += $"src: local('{SplittedName}');\n";
                if(name.Select(x => Path.GetExtension(x)).Contains("woff")) { 
                    
                }
                foreach(var file in name) {
                    var fileExtension = Path.GetExtension(file);
                    var fileName = Path.GetFileName(file);
                    if(fileExtension == ".eot") {
                        result += $"src: url('$current-directory+'/'+'{fileName}') format('{Format(fileExtension)}');";
                        result += $"src: url('$current-directory+'/'+'{fileName}?#iefix') format('{Format(fileExtension)}');";
                        continue;
                    }
                    result += $"src: url('$current-directory+'/'+'{fileName}') format('{Format(fileExtension)}');";
                }
            }
            return result;
        }

        public static string Format(string extension) => extension switch {
            "woff2" => "woff2",
            "woff" => "woff",
            "ttf" => "truetype",
            "eot" => "embedded-opentype",
            "svg" => "svg",
            _ => "",
        };


        static Dictionary<string, IEnumerable<string>> GetFonts(string rootPath) => Directory.EnumerateDirectories(rootPath)
                .Select(directory => new KeyValuePair<string, IEnumerable<string>>(Path.GetFileName(directory), Directory.EnumerateFiles(directory)))
                .ToDictionary(x => x.Key, x => x.Value);
    }
}