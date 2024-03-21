using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI{
    public class Intern : BaseAgent
    {
        /// <summary>
        /// Min speed to be considered stopped
        /// </summary>
        public float minStopSpeed;
        
        protected override void Start(){
            base.Start();
        }
    }
}

