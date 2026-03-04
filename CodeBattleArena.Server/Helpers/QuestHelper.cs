using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.Threading.Tasks;

namespace CodeBattleArena.Server.Helpers
{
    public static class QuestHelper
    {
        public static string? GetParamValue(this TaskPlay task, TaskParamKey key) =>
            task.TaskPlayParams?.FirstOrDefault(p => p.ParamKey == key)?.ParamValue;

        public static int? GetIntParam(this TaskPlay task, TaskParamKey key)
        {
            var str = GetParamValue(task, key);
            return int.TryParse(str, out var val) ? val : null;
        }

        public static bool GetBoolParam(this TaskPlay task, TaskParamKey key)
        {
            var str = GetParamValue(task, key);
            return bool.TryParse(str, out var val) ? val : false;
        }
        public static string? GetStringParam(this TaskPlay task, TaskParamKey key)
        {
            var str = GetParamValue(task, key);
            return str;
        }

        public static bool TryResetIfRepeatable(this PlayerTaskPlay playerTaskPlay)
        {
            if (playerTaskPlay.IsCompleted && playerTaskPlay.IsGet)
            {
                if (playerTaskPlay.TaskPlay.RepeatAfterDays.HasValue)
                {
                    var daysSinceCompleted = (DateTime.UtcNow - playerTaskPlay.CompletedAt.Value).TotalDays;
                    if (daysSinceCompleted >= playerTaskPlay.TaskPlay.RepeatAfterDays.Value)
                        return true;
                    else
                        return false;
                }
                else if (playerTaskPlay.TaskPlay.IsRepeatable)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public static void ResetPlayerTaskPlay(PlayerTaskPlay task, string? defaultValue = null)
        {
            if (task == null) return;

            task.IsCompleted = false;
            task.ProgressValue = !string.IsNullOrEmpty(defaultValue) ? defaultValue : null;
            task.CompletedAt = null;
            task.IsGet = false;
        }

    }
}
