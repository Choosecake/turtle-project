using UnityEngine;

namespace Code.DeathMessages
{
    [CreateAssetMenu(fileName = "DeathMessage", menuName = "DeathMessage")]
    public class DeathMessageSO : ScriptableObject
    {
        //Automatized time per letter = 029367234;
        public const float TimePerChar = .029367234f;
        
        public CauseOfDeath causeOfDeath;

        [Multiline]public string[] messageParts;
        public float[] messageDurations;

        [ContextMenu("Automatize Messages Times")]
        public void AutomatizeMessageTime()
        {
            messageDurations = new float[messageParts.Length];
            for (int i = 0; i < messageParts.Length; i++)
            {
                messageDurations[i] = messageParts[i].Length * TimePerChar;
            }
        }
    }
}