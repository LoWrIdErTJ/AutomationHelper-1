using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

namespace AutomationHelper
{
    public class FileHelper
    {
        #region public methods
        /// <summary>
        /// Check if specified file is existing
        /// </summary>
        /// <param name="filename">File full path and name</param>
        /// <returns>true or false</returns>
        static public bool FileExists(string filename)
        {
            var ret = false;

            try
            {
                if (File.Exists(filename))
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Copy single file from source to destination
        /// </summary>
        /// <param name="source">source file full path and name</param>
        /// <param name="destination">destination file full path and name</param>
        /// <returns>true or false</returns>
        static public bool CopyFile(string source, string destination)
        {
            var ret = false;

            try
            {
                if (File.Exists(source))
                {
                    File.Copy(source, destination, true);
                    ret = true;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Delete single file
        /// </summary>
        /// <param name="filename">File full path and name</param>
        /// <returns>true or false</returns>
        static public bool DeleteFile(string filename)
        {
            var ret = false;

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="foldername"></param>
        static public void DeleteFolder(string foldername)
        {
            try
            {
                Directory.Delete(foldername, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Rename single file name
        /// </summary>
        /// <param name="filename">original file full path and name</param>
        /// <param name="newfilename">new file full path and name</param>
        /// <returns>true or false</returns>
        static public bool RenameFile(string filename, string newfilename)
        {
            var ret = false;

            try
            {
                if (File.Exists(filename))
                {
                    File.Move(filename, newfilename);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Search the text whether appear in the text file
        /// </summary>
        /// <param name="filename">File full path and name</param>
        /// <param name="text">Text string which be searched</param>
        /// <returns>The first chars position when found the text string.</returns>
        static public int SearchTextInFile(string filename, string text)
        {
            var pos = -1;

            try
            {
                if (File.Exists(filename))
                {
                    var filetext = File.ReadAllText(filename, Encoding.Default);
                    if (!string.IsNullOrEmpty(filetext))
                    {
                        pos = filetext.IndexOf(text);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return pos;
        }

        /// <summary>
        /// Create File
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="content">content in file</param>
        /// <param name="isAppend">overwrite or append</param>
        static public void CreateFile(string filename, string content, bool isAppend)
        {
            try
            {
                var sw = new StreamWriter(filename, isAppend);
                sw.WriteLine();
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// read file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public string ReadFile(string filename)
        {
            string content;

            try
            {
                var sr = new StreamReader(filename);
                content = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return content;
        }

        /// <summary>
        /// copy folder
        /// </summary>
        /// <param name="orginalfolder"></param>
        /// <param name="targetfolder"></param>
        static public void CopyFolder(string orginalfolder, string targetfolder, bool IsOverWrite)
        {
            var diSource = new DirectoryInfo(orginalfolder);
            var diTarget = new DirectoryInfo(targetfolder);

            CopyAll(diSource, diTarget, IsOverWrite);
        }

        /// <summary>
        /// Compare 2 folders
        /// </summary>
        /// <param name="expectedFolder">the expected folder as the baseline</param>
        /// <param name="targetFolder">target folder</param>
        /// <param name="binaryFileExtensions">list all binary file extension, format: .exe,.dll,.rm</param>
        /// <param name="ignoreFileExtensions">list all file extensions which the file will be ignored, format: .log,.inf,.trc </param>
        /// <param name="isScanRecursiveDirectory">also scan all sub directory</param>
        /// <param name="isCaseSensitive">case sensitive</param>
        /// <param name="isCompressWhiteSpace">compress tab, space</param>        ///
        /// <param name="ignoreRegex"></param>
        /// <param name="reportFile">save the result to XML file.</param>
        /// <param name="ignoreLineSwitchFiles">files which with line switch difference will be ginored, format: action.c,default.usr,default.c</param>
        /// <param name="skipFiles">list all files which need to be skipped during comparing</param>
        /// <returns></returns>
        public static bool CompareFolder(string expectedFolder, string targetFolder, string binaryFileExtensions, string ignoreFileExtensions,
             bool isScanRecursiveDirectory, bool isCaseSensitive, bool isCompressWhiteSpace, string ignoreRegex, string reportFile, string[] ignoreLineSwitchFiles, params string[] skipFiles)
        {
            return CompareFolder(expectedFolder, targetFolder, binaryFileExtensions, ignoreFileExtensions, isScanRecursiveDirectory, isCaseSensitive, isCompressWhiteSpace, ignoreRegex, reportFile, true, ignoreLineSwitchFiles, skipFiles);
        }

        /// <summary>
        /// Compare 2 folders
        /// </summary>
        /// <param name="expectedFolder">the expected folder as the baseline</param>
        /// <param name="targetFolder">target folder</param>
        /// <param name="binaryFileExtensions">list all binary file extension, format: .exe,.dll,.rm</param>
        /// <param name="ignoreFileExtensions">list all file extensions which the file will be ignored, format: .log,.inf,.trc </param>
        /// <param name="isScanRecursiveDirectory">also scan all sub directory</param>
        /// <param name="isCaseSensitive">case sensitive</param>
        /// <param name="isCompressWhiteSpace">compress tab, space</param>
        /// <param name="ignoreRegex"></param>
        /// <param name="reportFile">save the result to XML file.</param>
        /// <param name="alwaysGenerateReport">if need to generate report even they are equals</param>
        /// <param name="ignoreLineSwitchFiles">files which with line switch difference will be ginored, format: action.c,default.usr,default.c</param>
        /// <param name="skipFiles">list all files which need to be skipped during comparing</param>
        /// <returns></returns>
        public static bool CompareFolder(string expectedFolder, string targetFolder, string binaryFileExtensions, string ignoreFileExtensions,
             bool isScanRecursiveDirectory, bool isCaseSensitive, bool isCompressWhiteSpace, string ignoreRegex, string reportFile, bool alwaysGenerateReport, string[] ignoreLineSwitchFiles, params string[] skipFiles)
        {
            var result = true;

            var resultfile = string.IsNullOrEmpty(reportFile) ? string.Format("Result_{0:yyyyMMddhhmmss}.xml", DateTime.Now) : reportFile;

            var fileDifferences = new List<FileDiffResult>();

            var bFileExt = binaryFileExtensions == null ? null : binaryFileExtensions.Split(new[] { ',' });
            var iFileExt = ignoreFileExtensions == null ? null : ignoreFileExtensions.Split(new[] { ',' });

            foreach (var expectedFile in Directory.GetFiles(expectedFolder, "*.*", isScanRecursiveDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                var cmpType = 1;
                var ext = Path.GetExtension(expectedFile);
                var filename = Path.GetFileName(expectedFile);
                var subpath = expectedFile.Substring(expectedFolder.Length + 1, expectedFile.IndexOf(filename) - expectedFolder.Length - 1);

                // If it is an ignore type of files, skip it.
                if (iFileExt != null)
                    if (iFileExt.Contains(ext))
                        continue;

                // If it is a skip file, skip it.
                if (skipFiles != null)
                    if (skipFiles.Where(o => expectedFile.ToLower().IndexOf(o.ToLower()) > 0).Count() > 0)
                        continue;

                // If it is a binary file, compare it by Binary
                if (bFileExt != null)
                    if (bFileExt.Contains(ext))
                        cmpType = 0;

                // Combine and Get the target file.
                var targetFile = string.Format(@"{0}\{1}\{2}", targetFolder, subpath, filename).Replace(@"\\", @"\");

                var fdr = new FileDiffResult
                {
                    FileName = filename,
                    ExpectDirectory = expectedFile.Remove(expectedFile.IndexOf(filename)),
                    TargetDirectory = targetFile.Remove(targetFile.IndexOf(filename))
                };

                // If file2 does not exist, return error. Otherwise, compare them.
                if (!File.Exists(targetFile))
                {
                    fdr.Differences = new List<Differences>();
                    var diff = new Differences { Reason = DiffReason.FileMissing };
                    fdr.Differences.Add(diff);
                    fileDifferences.Add(fdr);
                }
                else
                {
                    string diffoutput;
                    result &= Fc(expectedFile, targetFile, cmpType, isCaseSensitive, isCompressWhiteSpace, out diffoutput);
                    if (!string.IsNullOrEmpty(diffoutput))
                    {
                        if (cmpType == 0)
                        {
                            fdr.Differences = new List<Differences>();
                            var diff = new Differences { Reason = DiffReason.BinaryFileLengthDifferent };
                            fdr.Differences.Add(diff);
                            fileDifferences.Add(fdr);
                        }
                        else if (cmpType == 1)
                        {
                            ParseDiffResult(diffoutput, ignoreRegex, ref fdr);
                            if (fdr.Differences.Count > 0)
                            {
                                if (ignoreLineSwitchFiles != null)
                                {
                                    if (ignoreLineSwitchFiles.Contains(filename))
                                        continue;
                                }
                                fileDifferences.Add(fdr);
                            }
                        }
                    }
                }
            }

            if (fileDifferences.Count > 0)
            {
                result = false;
                Console.WriteLine("There are {0} files different.", fileDifferences.Count);
                foreach (var fd in fileDifferences)
                    Console.WriteLine(fd.FileName);
            }
            else
            {
                result = true;
            }


            if (!result || (result && alwaysGenerateReport))
            {
                Console.WriteLine("Result File: {0}", resultfile);
                GenerateReport(resultfile,
                    result,
                    expectedFolder,
                    targetFolder,
                    binaryFileExtensions,
                    ignoreFileExtensions,
                    isScanRecursiveDirectory,
                    isCaseSensitive,
                    isCompressWhiteSpace,
                    skipFiles,
                    fileDifferences);
            }
            else
            {
                Console.WriteLine("Result: No difference in content.");
            }

            return result;
        }

        /// <summary>
        /// Use FC.exe to compare 2 files, it is dos command
        /// </summary>
        /// <param name="file1">file 1</param>
        /// <param name="file2">file 2</param>
        /// <param name="comparisionType">0 - Binaray, 1 - ASCII, 2 - Unicode</param>
        /// <param name="isCaseSensitive">Is it case sensitive?</param>
        /// <param name="isCompressWriteSpace">Does compress write space (tab/space) before comapre?</param>
        /// <param name="diffOutput">Return the differents output</param>
        /// <returns>true - same, false - different</returns>
        public static bool Fc(string file1, string file2, int comparisionType, bool isCaseSensitive, bool isCompressWriteSpace, out string diffOutput)
        {
            bool result;
            diffOutput = string.Empty;
            Process process = null;

            // check File1 and File 2 are not empty.
            if (string.IsNullOrEmpty(file1))
                throw new Exception("Path1 is empty.");

            if (string.IsNullOrEmpty(file2))
                throw new Exception("Path2 is empty.");

            #region Prepare the arguments for FC.exe
            var arguments = string.Format(" {0} {1}", file1, file2);

            switch (comparisionType)
            {
                case 0:
                    arguments += " /B";
                    break;
                case 1:
                    arguments += " /L /N";
                    break;
                case 2:
                    arguments += " /U";
                    break;
            }

            if (isCompressWriteSpace)
                arguments += " /W";

            if (!isCaseSensitive)
                arguments += " /C";
            #endregion

            // This solution is because when use UseShellExecute = false, the fc.exe will always return -1 code.             
            #region create a temp bat file, and when execute it, store the result to a temp result file.
            var runfilecontent = string.Format("{0} {1} > FC_temp_result.txt", "fc.exe", arguments);
            var fctempfile = string.Format("FC_temp_{0}.bat", TextHelper.RandomString(5));

            using (var sw = new StreamWriter(fctempfile, false))
            {
                sw.Write(runfilecontent);
                sw.Flush();
                sw.Close();
            }
            #endregion

            #region Execute the bat file to do FC.exe
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = fctempfile,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                process = Process.Start(processInfo);
                process.WaitForExit(5000);
                var exitcode = process.ExitCode;

                if (exitcode == -1)
                    throw new Exception("FC.exe execution fail.");

                if (exitcode == 0)
                    result = true;
                else
                {
                    result = false;
                    // Read the result from the temp result file.
                    using (var sr = new StreamReader("FC_temp_result.txt"))
                    {
                        diffOutput = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                }
                File.Delete(fctempfile);
            }
            #endregion

            return result;
        }

        #endregion

        #region private methods
        static private void CopyAll(DirectoryInfo source, DirectoryInfo target, bool IsOverWrite)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), IsOverWrite);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, IsOverWrite);
            }
        }

        private static void ParseDiffResult(string content, string ignoreRegex, ref FileDiffResult result)
        {
            result.Differences = new List<Differences>();

            var expFile = Path.Combine(result.ExpectDirectory, result.FileName);
            var tarFile = Path.Combine(result.TargetDirectory, result.FileName);

            var diffSection = GetDifferenceSection(content);

            var ignoreRegexArray = ignoreRegex.Split(new[] { '|' });

            foreach (var sec in diffSection)
            {
                if (!string.IsNullOrEmpty(sec))
                {
                    var diff = new Differences();

                    var expectfilediffstartpos = sec.ToLower().IndexOf(result.FileName.ToLower());
                    if (expectfilediffstartpos < 0)
                        continue;
                    expectfilediffstartpos += result.FileName.Length;
                    var expectfilediffendpos = sec.IndexOf("*****", expectfilediffstartpos);
                    if (expectfilediffendpos < 0)
                        continue;

                    diff.Expect = sec.Substring(expectfilediffstartpos, expectfilediffendpos - expectfilediffstartpos).Trim();

                    // Check the expected string in difference is in Ignore List
                    var isIgnore = false;
                    if (!string.IsNullOrEmpty(diff.Expect))
                    {
                        isIgnore = ignoreRegexArray.Any(reg => !string.IsNullOrEmpty(reg) && Regex.IsMatch(diff.Expect, reg, RegexOptions.IgnoreCase));
                    }

                    if (isIgnore)
                        continue;

                    var targetfilediffstartpos = sec.ToLower().IndexOf(result.FileName.ToLower(), expectfilediffendpos);
                    if (targetfilediffstartpos < 0)
                        continue;
                    targetfilediffstartpos += result.FileName.Length;
                    var targetexpectfilediffendpos = sec.IndexOf("*****", targetfilediffstartpos);
                    if (targetexpectfilediffendpos < 0)
                        continue;

                    diff.Target = sec.Substring(targetfilediffstartpos, targetexpectfilediffendpos - targetfilediffstartpos).Trim();

                    diff.Reason = AnalysisDiffResultType(ref diff.Expect, ref diff.Target);

                    result.Differences.Add(diff);
                }
            }

        }

        private static List<string> GetDifferenceSection(string content)
        {
            var section = new List<string>();
            var secstart = false;

            using (var sr = new StringReader(content))
            {
                string line;
                string sectxt = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith("*****") && !secstart)
                    {
                        secstart = true;
                        sectxt += string.Format("{0}\r", line.Trim());
                    }
                    else if (!string.IsNullOrEmpty(line.Trim()) && secstart)
                    {
                        sectxt += string.Format("{0}\r", line.Trim());
                    }
                    else if (string.IsNullOrEmpty(line.Trim()))
                    {
                        secstart = false;
                        section.Add(sectxt);
                        sectxt = string.Empty;
                    }
                }
            }

            return section;

        }

        private static string AnalysisDiffResultType(ref string expectDiff, ref string targetDiff)
        {
            string reason;

            if (string.IsNullOrEmpty(expectDiff))
                return DiffReason.LineCreated;

            if (string.IsNullOrEmpty(targetDiff))
                return DiffReason.LineMissing;

            var expList = ReadDiffStringLine(expectDiff);
            var tarList = ReadDiffStringLine(targetDiff);

            if (tarList.Where(o => !expList.Values.Contains(o.Value)).Count() > 0)
            {
                if (expList.Where(o => !tarList.Values.Contains(o.Value)).Count() > 0)
                {
                    expectDiff = string.Empty;
                    targetDiff = tarList.Where(o => !expList.Values.Contains(o.Value)).Aggregate(string.Empty, (current, kvp) => current + string.Format("{0}: {1}\r", kvp.Key, kvp.Value));

                    expectDiff = expList.Where(o => !tarList.Values.Contains(o.Value)).Aggregate(expectDiff, (current, kvp) => current + string.Format("{0}: {1}\r", kvp.Key, kvp.Value));

                    reason = DiffReason.LineDifferent;
                }
                else
                {
                    expectDiff = string.Empty;
                    targetDiff = tarList.Where(o => !expList.Values.Contains(o.Value)).Aggregate(string.Empty, (current, kvp) => current + string.Format("{0}: {1}\r", kvp.Key, kvp.Value));

                    reason = DiffReason.LineCreated;
                }
            }
            else if (expList.Where(o => !tarList.Values.Contains(o.Value)).Count() > 0)
            {
                expectDiff = string.Empty;
                targetDiff = string.Empty;
                expectDiff = expList.Where(o => !tarList.Values.Contains(o.Value)).Aggregate(expectDiff, (current, kvp) => current + string.Format("{0}: {1}\r", kvp.Key, kvp.Value));

                reason = DiffReason.LineMissing;
            }
            else
            {
                reason = DiffReason.LineSwitch;
            }

            return reason;
        }

        private static Dictionary<int, string> ReadDiffStringLine(string str)
        {
            var s = new Dictionary<int, string>();

            using (var sr = new StringReader(str))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        var linenum = Convert.ToInt32(line.Substring(0, line.IndexOf(":")));
                        var linecontext = line.Substring(line.IndexOf(":") + 1, line.Length - line.IndexOf(":") - 1).Trim();
                        s.Add(linenum, linecontext);
                    }
                    catch (ArgumentNullException)
                    {
                        Console.WriteLine("String is null.");
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Format exception: " + e.Message + " \nLine: " + line + " \nText: " + str);
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine(
                        "Overflow in string to int conversion.");
                    }


                }
            }

            return s;
        }

        private static void GenerateReport(string file, bool result, string expectedFolder, string targetFolder, string binaryFileExtensions, string ignoreFileExtensions,
             bool isScanRecursiveDirectory, bool isCaseSensitive, bool isCompressWhiteSpace, string[] skipFiles, List<FileDiffResult> filediff)
        {
            var doc = new XmlDocument();
            var docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            #region Root element
            var resultNode = doc.CreateElement("Comparision");
            doc.AppendChild(resultNode);

            var resultAttribute = doc.CreateAttribute("result");
            resultAttribute.Value = result.ToString();
            resultNode.Attributes.Append(resultAttribute);
            #endregion

            #region Compare Arguments
            var argumentsNode = doc.CreateElement("Arguments");
            resultNode.AppendChild(argumentsNode);
            string sfiles = null;
            if (skipFiles != null)
            {
                sfiles = skipFiles.Aggregate(sfiles, (current, sf) => current + string.Format("{0},", sf));
            }
            var skipFilesAttribute = doc.CreateAttribute("skipfiles");
            skipFilesAttribute.Value = sfiles;
            argumentsNode.Attributes.Append(skipFilesAttribute);

            var compressWhitspaceAttribute = doc.CreateAttribute("compresswhitespace");
            compressWhitspaceAttribute.Value = isCompressWhiteSpace.ToString();
            argumentsNode.Attributes.Append(compressWhitspaceAttribute);

            var caseSensitiveAttribute = doc.CreateAttribute("casesensitive");
            caseSensitiveAttribute.Value = isCaseSensitive.ToString();
            argumentsNode.Attributes.Append(caseSensitiveAttribute);

            var subDirAttribute = doc.CreateAttribute("includesubdirectory");
            subDirAttribute.Value = isScanRecursiveDirectory.ToString();
            argumentsNode.Attributes.Append(subDirAttribute);


            var ignoreFileExAttribute = doc.CreateAttribute("ignoreextension");
            ignoreFileExAttribute.Value = ignoreFileExtensions;
            argumentsNode.Attributes.Append(ignoreFileExAttribute);

            var binaryFileExtAttribute = doc.CreateAttribute("binaryextension");
            binaryFileExtAttribute.Value = binaryFileExtensions;
            argumentsNode.Attributes.Append(binaryFileExtAttribute);

            var targetAttribute = doc.CreateAttribute("actual");
            targetAttribute.Value = targetFolder;
            argumentsNode.Attributes.Append(targetAttribute);

            var expectAttribute = doc.CreateAttribute("expect");
            expectAttribute.Value = expectedFolder;
            argumentsNode.Attributes.Append(expectAttribute);
            #endregion

            #region List File Difference

            var filediffNode = doc.CreateElement("Difference");
            resultNode.AppendChild(filediffNode);

            if (filediff != null)
            {
                foreach (var fdr in filediff)
                {
                    #region File Difference Node
                    var fileNode = doc.CreateElement("File");
                    filediffNode.AppendChild(fileNode);

                    var filenameAttribute = doc.CreateAttribute("name");
                    filenameAttribute.Value = fdr.FileName;
                    fileNode.Attributes.Append(filenameAttribute);
                    #endregion

                    if (fdr.Differences != null)
                    {
                        foreach (var diff in fdr.Differences)
                        {
                            var diffNode = doc.CreateElement("Difference");
                            fileNode.AppendChild(diffNode);

                            var reasonAttribute = doc.CreateAttribute("reason");
                            reasonAttribute.Value = diff.Reason;
                            diffNode.Attributes.Append(reasonAttribute);

                            var tarNode = doc.CreateElement("Actual");
                            tarNode.InnerText = string.IsNullOrEmpty(diff.Target) ? string.Empty : diff.Target;
                            diffNode.AppendChild(tarNode);

                            var expNode = doc.CreateElement("Expect");
                            expNode.InnerText = string.IsNullOrEmpty(diff.Expect) ? string.Empty : diff.Expect;
                            diffNode.AppendChild(expNode);
                        }
                    }
                }
            }
            #endregion

            doc.Save(file);
        }
        #endregion
    }

    public class FileDiffResult
    {
        public string FileName;
        public string ExpectDirectory;
        public string TargetDirectory;
        public List<Differences> Differences;
    }

    public class Differences
    {
        public string Expect;
        public string Target;
        public string Reason;
    }

    public class DiffReason
    {
        public const string LineMissing = "Expected line is not found in target file.";
        public const string LineCreated = "A new line is existed in target file.";
        public const string LineSwitch = "Lines in target file is switched.";
        public const string LineDifferent = "The content of the line is changed in target file.";
        public const string BinaryFileLengthDifferent = "Binary file length does not match.";
        public const string FileMissing = "Expected file is not found in target folder.";
    }
}
