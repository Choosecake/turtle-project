using System;
using System.Collections;
using UnityEngine;

namespace Code
{
    public static class UnityUtils
    {
        public static IEnumerator ExecuteRepeating(Action action, 
            float time, float repeatRate)
        {
            yield return new WaitForSeconds(time);
            while (true)
            {
                action.Invoke();
                yield return new WaitForSeconds(repeatRate);
            }
        } 
        
        // public static IEnumerable ExecuteRepeating(Action action, 
        //     float time, float repeatRate, float repeatQuantity = 1)
        // {
        //     yield return new WaitForSeconds(time);
        //     int i = 0;
        //     while (i < repeatQuantity)
        //     {
        //         action.Invoke();
        //         yield return new WaitForSeconds(repeatRate);
        //         i++;
        //     }
        // } 
    }
}