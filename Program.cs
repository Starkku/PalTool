using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Starkku.Utilities;
using Starkku.Utilities.FileTypes;

namespace PalTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parameters
            string filename_out = null;
            bool tool_modify = false;
            bool tool_avgalpha = false;
            bool tool_thresholdalpha = false;
            bool tool_removealpha = false;
            bool tool_sortcolors = false;
            bool tool_converttoww = false;
            bool tool_output6bit = false;
            bool tool_debuginfo = false;
            bool tool_savePNG = false;
            double avg_divider = 3.0;
            int threshold_value = -1;
            byte new_alpha_value = 1;
            int sort_start_index = 1;
            int gradient_count = 8;
            double saturation_threshold = 0.33;
            PaletteColorSortMode tool_sortmode = PaletteColorSortMode.RGB;
            PaletteColorSortMode threshold_type = PaletteColorSortMode.Saturation;

            string filename;

            if (args.Length < 1)
            {
                Console.WriteLine("Not enough parameters.");
                ShowHelp();
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(args[0]) || !File.Exists(args[0]))
                {
                    Console.WriteLine("Input file does not exist.");
                    ShowHelp();
                    return;
                }
                else
                {
                    filename = args[0];
                }
            }

            if (args.Length >= 2)
            {
                if (string.IsNullOrEmpty(args[1]) || !IsValidFilename(args[1]))
                {
                    Console.WriteLine("Invalid output file specified - using input file as output file.");
                    filename_out = filename;
                }
                else
                {
                    filename_out = args[1];
                }
            }

            int pos = Array.IndexOf(args, "-m");

            if (pos > -1)
                tool_modify = true;

            pos = Array.IndexOf(args, "-a");

            if (pos > -1)
            {
                tool_avgalpha = true;

                if (args.Length > pos + 1)
                    avg_divider = Conversion.GetDoubleFromString(args[pos + 1], avg_divider);
            }

            pos = Array.IndexOf(args, "-r");

            if (pos > -1)
                tool_removealpha = true;

            pos = Array.IndexOf(args, "-s");

            if (pos > -1)
            {
                tool_sortcolors = true;

                if (args.Length > pos + 1 && args[pos + 1] != "")
                    tool_sortmode = ParseSortMode(args[pos + 1], tool_sortmode);
            }

            pos = Array.IndexOf(args, "-t");

            if (pos > -1)
            {
                tool_thresholdalpha = true;

                if (args.Length > pos + 2 && args[pos + 2] != "")
                    threshold_type = ParseSortMode(args[pos + 1], threshold_type);

                if (args.Length > pos + 2 && args[pos + 2] != "")
                    threshold_value = Conversion.GetIntFromString(args[pos + 2], threshold_value);

                if (args.Length > pos + 3 && args[pos + 3] != "")
                {
                    new_alpha_value = Conversion.GetByteFromString(args[pos + 3], new_alpha_value);
                    if (new_alpha_value > 255)
                        new_alpha_value = 1;
                }
            }

            pos = Array.IndexOf(args, "-c");

            if (pos > -1)
                tool_converttoww = true;

            pos = Array.IndexOf(args, "-b");

            if (pos > -1)
                tool_output6bit = true;

            pos = Array.IndexOf(args, "-d");

            if (pos > -1)
                tool_debuginfo = true;

            pos = Array.IndexOf(args, "-p");
            if (pos > -1)
                tool_savePNG = true;

            pos = Array.IndexOf(args, "-g");

            if (pos > -1)
            {
                if (args.Length > pos + 1 && args[pos + 1] != "")
                    sort_start_index = Conversion.GetIntFromString(args[pos + 1], sort_start_index);

                if (args.Length > pos + 2 && args[pos + 2] != "")
                    gradient_count = Conversion.GetIntFromString(args[pos + 2], gradient_count);

                if (args.Length > pos + 3 && args[pos + 3] != "")
                    saturation_threshold = Conversion.GetDoubleFromString(args[pos + 3], saturation_threshold);
            }

            if (!tool_modify && !tool_avgalpha && !tool_removealpha && !tool_sortcolors && (!tool_thresholdalpha || (tool_thresholdalpha && threshold_value == -1)) && !tool_converttoww && !tool_output6bit && !tool_debuginfo && !tool_savePNG)
            {
                Console.WriteLine("Not enough parameters.");
                ShowHelp();
                return;
            }

            PaletteFile pal = new PaletteFile(filename, true);

            if (!pal.Initialized)
            {
                Console.WriteLine("Failed to load palette file.");
                return;
            }
            else
            {
                if (tool_debuginfo)
                {
                    Console.WriteLine("Debug Info:");
                    Console.WriteLine("------------------");
                    Console.WriteLine("Palette filename: " + Path.GetFileName(pal.Filename));
                    Console.WriteLine("Color count: " + pal.ColorCount);
                    Console.WriteLine("Duplicate color count: " + (pal.ColorCount - pal.GetDistinctColors().Length));
                    Console.WriteLine("------------------");
                }

                if (tool_modify)
                    pal.MultiplyColorsByAlpha();

                if (tool_avgalpha)
                    pal.CalculateAverageAlpha(avg_divider);

                if (tool_thresholdalpha && threshold_value >= 0 && threshold_value <= 255)
                    pal.AlphaByThreshold(threshold_type, threshold_value, new_alpha_value);

                if (tool_sortcolors)
                    pal.SortColors(tool_sortmode, sort_start_index, gradient_count, saturation_threshold);

                bool success;

                if (!tool_savePNG)
                {
                    if (!tool_converttoww)
                    {
                        if (tool_output6bit)
                        {
                            pal.MultiplyColors(0.25);
                            pal.MultiplyColors(4.0);
                        }
                        success = pal.SaveJASCPalette(filename_out, !tool_removealpha);
                    }
                    else
                        success = pal.SaveGenericPalette(filename_out);
                }
                else
                {
                    filename_out = Path.ChangeExtension(filename_out, ".png");
                    success = pal.SavePNG(filename_out);
                }

                if (!success)
                    Console.WriteLine("Could not save file '" + filename_out + "'.");
            }
        }

        private static PaletteColorSortMode ParseSortMode(string str, PaletteColorSortMode defaultValue)
        {
            string tmp = str.ToLower().Trim();

            switch (tmp)
            {
                case "hue":
                    return PaletteColorSortMode.Hue;
                case "saturation":
                    return PaletteColorSortMode.Hue;
                case "light":
                    return PaletteColorSortMode.Light;
                case "red":
                    return PaletteColorSortMode.Red;
                case "green":
                    return PaletteColorSortMode.Green;
                case "blue":
                    return PaletteColorSortMode.Blue;
                case "alpha":
                    return PaletteColorSortMode.Alpha;
                case "brightness":
                    return PaletteColorSortMode.Brightness;
                case "rgb":
                    return PaletteColorSortMode.RGB;
                case "gradients":
                    return PaletteColorSortMode.Gradients;
                default:
                    return defaultValue;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: PalTool.exe palette_file [output_palette_file] [-a, -m, -r, -c, -t, -s, -p]");
            Console.WriteLine("-m | Modify colors based on alpha.");
            Console.WriteLine("-a [number] | Calculate alpha using average of color values. Number is what the cumulative color value is divided by to get the average, defaults to 3.");
            Console.WriteLine("-t [hue|saturation|light|red|green|blue|alpha|brightness|rgb|gradients] threshold-value [new-alpha-value] | Modify alpha based on a threshold. Value listed after determines threshold type. Defaults to 'saturation' if invalid value is specified. Colors that pass the threshold value have their alpha set to the new value, rest are left untouched. No threshold is not valid, new alpha value defaults to 1. Both threshold & new alpha value should be numbers from 0 to 255.");
            Console.WriteLine("-s [hue|saturation|light|red|green|blue|alpha|brightness|rgb|gradients] | Sort colors. Value listed after determines the sort method. Defaults to 'rgb' if invalid value is specified.");
            Console.WriteLine("-g [startindex] [count] [saturation_threshold] | Sort parameters. Start color index, number of gradients and saturation split threshold for gradients sort (0-1.0), defaults to 1, 8 and 0.33 respectively.");
            Console.WriteLine("-b | Output 6-bit RGB color components instead of 8-bit (only if JASC palette).");
            Console.WriteLine("-d | Print debug info about palette to console.");
            Console.WriteLine("-r | Do not save alpha values in output palette (only if JASC palette).");
            Console.WriteLine("-c | Save output as WW palette, not JASC palette.");
            Console.WriteLine("-p | Save 256x1px PNG image containing one pixel of each of the palette colors.");
        }

        private static bool IsValidFilename(string testName)
        {
            string regexString = "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]";
            Regex containsABadCharacter = new Regex(regexString);

            if (containsABadCharacter.IsMatch(testName))
                return false;

            string pathRoot = Path.GetPathRoot(testName);

            if (pathRoot != "" && !Directory.GetLogicalDrives().Contains(pathRoot))
                return false;

            string directory = Path.GetDirectoryName(testName);

            if (directory != "" && !Directory.Exists(directory))
                return false;

            return true;
        }
    }
}
