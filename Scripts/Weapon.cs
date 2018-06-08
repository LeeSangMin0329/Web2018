using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class Weapon : MonoBehaviour
    {
        #region Public Properties

        [Tooltip("Weapon damege. defalut amount is 0, you must setting in each prefab.")]
        public float DamegeAmount = 30;

        #endregion
    }
}