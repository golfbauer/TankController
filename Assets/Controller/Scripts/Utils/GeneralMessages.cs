using UnityEngine;

namespace Controller.Scripts.Utils
{
    public static class GeneralMessages
    {
        public const string UpdateAll = "Update All";
        public const string Remove = "Remove";
        public const string Add = "Add";
        public const string Cancel = "Cancel";
        public const string Import = "Import";
        public const string MoveUp = "Move Up";
        public const string MoveDown = "Move Down";
        public const string NotPrefabModeWarning = "You must be in prefab mode to use this tool.";
        public const string PrefabModeWarning = "You must exit prefab mode to use this tool.";

        public const string TargetNotAssigned =
            "It seems like the target is not assigned. Try to restart prefab mode. If that does not work restart Unity.";

        public const string Type = "Type";
        public const string Transform = "Transform";
        public const string ShowLabelType = "sv_label_1";

        public static readonly Color LightRed = new(1f, 0.5f, 0.5f, 0.3f);
        public static readonly Color Orange = new(1f, 0.3f, 0f, 0.3f);
    }
}
