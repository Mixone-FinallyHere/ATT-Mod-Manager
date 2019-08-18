﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ATTTModManagerNet
{
    public partial class ATTTModManager
    {
        public partial class ModEntry
        {
            public class ModLogger
            {
                protected readonly string Prefix;
                protected readonly string PrefixError;
                protected readonly string PrefixCritical;
                protected readonly string PrefixWarning;
                protected readonly string PrefixException;

                public ModLogger(string Id)
                {
                    Prefix = $"[{Id}] ";
                    PrefixError = $"[{Id}] [Error] ";
                    PrefixCritical = $"[{Id}] [Critical] ";
                    PrefixWarning = $"[{Id}] [Warning] ";
                    PrefixException = $"[{Id}] [Exception] ";
                }

                public void Log(string str)
                {
                    ATTTModManager.Logger.Log(str, Prefix);
                }

                public void Error(string str)
                {
                    ATTTModManager.Logger.Log(str, PrefixError);
                }

                public void Critical(string str)
                {
                    ATTTModManager.Logger.Log(str, PrefixCritical);
                }

                public void Warning(string str)
                {
                    ATTTModManager.Logger.Log(str, PrefixWarning);
                }

                public void NativeLog(string str)
                {
                    ATTTModManager.Logger.NativeLog(str, Prefix);
                }

                /// <summary>
                /// [0.17.0]
                /// </summary>
                public void LogException(string key, Exception e)
                {
                    ATTTModManager.Logger.LogException(key, e, PrefixException);
                }

                /// <summary>
                /// [0.17.0]
                /// </summary>
                public void LogException(Exception e)
                {
                    ATTTModManager.Logger.LogException(null, e, PrefixException);
                }
            }
        }

        public static class Logger
        {
            const string Prefix = "[Manager] ";
            const string PrefixError = "[Manager] [Error] ";
            const string PrefixException = "[Manager] [Exception] ";

            public static readonly string filepath = Path.Combine(Path.Combine(Application.dataPath, Path.Combine("Managed", nameof(ATTTModManager))), "Log.txt");

            public static void NativeLog(string str)
            {
                NativeLog(str, Prefix);
            }

            public static void NativeLog(string str, string prefix)
            {
                Write(prefix + str, true);
            }

            public static void Log(string str)
            {
                Log(str, Prefix);
            }

            public static void Log(string str, string prefix)
            {
                Write(prefix + str);
            }

            public static void Error(string str)
            {
                Error(str, PrefixError);
            }

            public static void Error(string str, string prefix)
            {
                Write(prefix + str);
            }

            /// <summary>
            /// [0.17.0]
            /// </summary>
            public static void LogException(Exception e)
            {
                LogException(null, e, PrefixException);
            }

            /// <summary>
            /// [0.17.0]
            /// </summary>
            public static void LogException(string key, Exception e)
            {
                LogException(key, e, PrefixException);
            }

            /// <summary>
            /// [0.17.0]
            /// </summary>
            public static void LogException(string key, Exception e, string prefix)
            {
                if (string.IsNullOrEmpty(key))
                    Write($"{prefix}{e.GetType().Name} - {e.Message}");
                else
                    Write($"{prefix}{key}: {e.GetType().Name} - {e.Message}");
                Console.WriteLine(e.ToString());
            }

            private static int bufferCapacity = 100;
            private static List<string> buffer = new List<string>(bufferCapacity);
            internal static int historyCapacity = 200;
            internal static List<string> history = new List<string>(historyCapacity * 2);

            private static void Write(string str, bool onlyNative = false)
            {
                if (str == null)
                    return;

                Console.WriteLine(str);

                if (onlyNative)
                    return;

                buffer.Add(str);
                history.Add(str);

                if (history.Count >= historyCapacity * 2)
                {
                    var result = history.Skip(historyCapacity);
                    history.Clear();
                    history.AddRange(result);
                }
            }

            private static float timer;

            internal static void Watcher(float dt)
            {
                if (buffer.Count >= bufferCapacity || timer > 1f)
                {
                    WriteBuffers();
                }
                else
                {
                    timer += dt;
                }
            }

            internal static void WriteBuffers()
            {
                try
                {
                    if (buffer.Count > 0)
                    {
                        if (!File.Exists(filepath))
                        {
                            using (File.Create(filepath))
                            {; }
                        }
                        using (StreamWriter writer = File.AppendText(filepath))
                        {
                            foreach (var str in buffer)
                            {
                                writer.WriteLine(str);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                buffer.Clear();
                timer = 0;
            }

            public static void Clear()
            {
                buffer.Clear();
                history.Clear();
                if (File.Exists(filepath))
                {
                    try
                    {
                        File.Delete(filepath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}
