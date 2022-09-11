using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Other
{
    public class VFXSpawner : MonoBehaviour
    {
        public List<GameObject> spark;

        private void Start() => StartCoroutine(Spawn());

        
        //Activates a spark at random time
        IEnumerator Spawn()
        {
            foreach (var o in spark)
            {
                o.SetActive(true);
                yield return new WaitForSeconds(Random.Range(0, 2));
            }
        }
    }
}
