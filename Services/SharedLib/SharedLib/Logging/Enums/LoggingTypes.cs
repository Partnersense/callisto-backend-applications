using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Logging.Enums
{
    public enum LoggingTypes
    {
        CheckpointLog,     // Marking a checkpoint in the application's execution flow.
        StatusLog,         // Indicating the status or state of something in the application.
        PerformanceLog,    // Logging performance-related metrics or optimizations.
        SecurityLog,       // Security-related events or violations.
        ErrorLog,          // Logging errors or exceptions.
        DebugLog,          // Debugging information.
        AuditLog,          // Auditing or tracking user actions.
        IssueLog,          // Indicating potential issues that are not critical.
        InformationLog,    // General informational logs.
        CriticalLog,       // Critical errors indicating the application cannot continue.
        TraceLog           // Detailed trace logs for debugging or troubleshooting.
    }
}
