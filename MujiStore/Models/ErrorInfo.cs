using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MujiStore.Models
{
    public class ErrorInfo
    {
        public string Message { get; set; }
        public string Description { get; set; }
    }
    public class LogInfo
    {
        public static string ActionName;
        public static string ControllerName;
        public static string Comments;
        public static string LogMsg;
        public static string MenuClick;
    }
 }