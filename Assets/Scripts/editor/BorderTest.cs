using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using province;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace editor
{
    public class BorderTest
    {

        [Test]
        public void BorderPairTest()
        {
            var pair1 = new BorderPair(1,2);
            var pair2 = new BorderPair(2, 1);
            
            var dict = new Dictionary<BorderPair,int>();
            dict.Add(pair1,1);

            Debug.Assert(dict.ContainsKey(pair2));
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator BorderTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}