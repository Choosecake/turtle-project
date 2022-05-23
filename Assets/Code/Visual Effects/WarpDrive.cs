using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class WarpDrive : MonoBehaviour
{
   [SerializeField] private float rate = 0.02f;
   [SerializeField] private float surroundingsDelay = 2.5f;
   [SerializeField] private VisualEffect warpSpeedVFX;
   [SerializeField] private MeshRenderer warpSurroundings;
   private bool warpActive;

   private const string AmountName = "WarpAmount";
   private const string SurroundingAmountName = "Active_";

   private void Awake()
   {
      if (warpSpeedVFX == null)
      {
         warpSpeedVFX = GetComponent<VisualEffect>();
      }
   }

   void Start()
   {
      Debug.Log(" aaaaaaa");
      warpSpeedVFX.Stop();
      warpSpeedVFX.SetFloat(AmountName, 0);
      warpSurroundings.material.SetFloat(SurroundingAmountName, 0);
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space))
      {
         warpActive = true;
         StartCoroutine(ActivateParticles());
         StartCoroutine(ActivateShader());
      }

      if (Input.GetKeyUp(KeyCode.Space))
      {
         warpActive = false;
         StartCoroutine(ActivateParticles());
         StartCoroutine(ActivateShader());

      }
   }

   IEnumerator ActivateParticles()
   {
      if (warpActive)
      {
         warpSpeedVFX.Play();

         float amount = warpSpeedVFX.GetFloat(AmountName);
         while (amount < 1 & warpActive)
         {
            amount += rate;
            warpSpeedVFX.SetFloat(AmountName, amount);
            yield return new WaitForSeconds(0.1f);
         }
      }
      else
      {
         float amount = warpSpeedVFX.GetFloat(AmountName);
         while (amount > 0 & !warpActive)
         {
            amount -= rate;
            warpSpeedVFX.SetFloat(AmountName, amount);
            yield return new WaitForSeconds(0.1f);

            if (amount <= 0 + rate)
            {
               amount = 0;
               warpSpeedVFX.SetFloat(AmountName, amount);
               warpSpeedVFX.Stop();
            }
         }
      }
   }

   IEnumerator ActivateShader()
   {
      if (warpActive)
      {
         yield return new WaitForSeconds(surroundingsDelay);

         float amount = warpSurroundings.material.GetFloat(SurroundingAmountName);
         while (amount < 1 & warpActive)
         {
            amount += rate;
            warpSurroundings.material.SetFloat(SurroundingAmountName, amount);
            yield return new WaitForSeconds(0.1f);
         }
      }
      else
      {
         float amount =  warpSurroundings.material.GetFloat(SurroundingAmountName);
         while (amount > 0 & !warpActive)
         {
            amount -= rate;
            warpSurroundings.material.SetFloat(SurroundingAmountName, amount);
            yield return new WaitForSeconds(0.1f);

            if (amount <= 0 + rate)
            {
               amount = 0;
               warpSurroundings.material.SetFloat(SurroundingAmountName, amount);
            }
         }
      }
   }
}
