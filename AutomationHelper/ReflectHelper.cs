using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutomationHelper
{
    public class ReflectHelper
    {
        /// <summary>
        /// Create instance with class
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyFile, string interfaceName, string methodName, string className)
        {
            object assembly = null;

            if (string.IsNullOrEmpty(assemblyFile) ||
                string.IsNullOrEmpty(interfaceName) ||
                string.IsNullOrEmpty(methodName))
                throw new Exception("All parameters cannot be empty.");

            if (!FileHelper.FileExists(assemblyFile))
                throw new Exception("Assembly file does not existed.");

            try
            {
                if (assemblyFile.ToUpper().EndsWith(".DLL"))
                {
                    try
                    {
                        var ab = Assembly.LoadFrom(assemblyFile);
                        var types = ab.GetTypes();
                        foreach (var t in types.Where(o => o.GetInterface(interfaceName) != null))
                        {
                            if (!string.IsNullOrEmpty(className))
                                if (t.Name.ToLower() != className.ToLower())
                                    continue;

                            if (t.GetMethods().Where(o => o.Name.ToLower() == methodName.ToLower()).Count() > 0)
                            {
                                assembly = ab.CreateInstance(t.FullName);
                                if (assembly != null)
                                    break;
                            }                            
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assembly;
        }
       
        /// <summary>
        /// Create instance without class
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="interfaceName"></param>
        /// <param name="methodName"></param>
        /// <returns></returns> 
        public static object CreateInstance(string assemblyFile, string interfaceName, string methodName)
        {
            var assembly = CreateInstance(assemblyFile, interfaceName, methodName, string.Empty);

            return assembly;
        }

        /// <summary>
        /// get public class list
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="interfaceName"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static List<string> GetPublicClassList(string assemblyFile, string interfaceName, string nameSpace)
        {
            var classList = new List<string>();

            try
            {
                if (assemblyFile.ToUpper().EndsWith(".DLL"))
                {
                    try
                    {
                        var ab = Assembly.LoadFrom(assemblyFile);
                        var types = ab.GetTypes();
                        foreach (var t in types.Where(o=>o.IsClass && o.IsPublic))
                        {
                            if (!string.IsNullOrEmpty(interfaceName))
                                if (t.GetInterface(interfaceName) == null)
                                    continue;

                            if (!string.IsNullOrEmpty(nameSpace))
                                if (t.Namespace.ToLower() != nameSpace.ToLower())
                                    continue;

                            if (!classList.Contains(t.Name))
                            {
                                classList.Add(t.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return classList;
        }

        /// <summary>
        /// get assembly name
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="interfaceName"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static string GetAssemblyName(string assemblyFile, string interfaceName, string nameSpace)
        {
            string assemblyname = null;

            try
            {
                if (assemblyFile.ToUpper().EndsWith(".DLL"))
                {
                    try
                    {
                        var ab = Assembly.LoadFrom(assemblyFile);
                        if (!string.IsNullOrEmpty(interfaceName))
                        {
                            if (ab.GetTypes().Where(o=>o.GetInterface(interfaceName) != null).Count() == 0)
                                return null;
                        }

                        if (!string.IsNullOrEmpty(nameSpace))
                        {
                            if (ab.GetTypes().Where(o=>o.Namespace.ToLower() == nameSpace.ToLower()).Count() == 0)
                                return null;
                        }

                        assemblyname = ab.GetName().Name;                        
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assemblyname;
        }

        
    }
}
