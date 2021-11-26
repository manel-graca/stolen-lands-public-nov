﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cinemachine
{
    [SaveDuringPlay]
    [RequireComponent(typeof(CinemachineFreeLook))]
    class CinemachineFreeLookZoom : MonoBehaviour
    {
        private CinemachineFreeLook freelook;
        public CinemachineFreeLook.Orbit[] originalOrbits = new CinemachineFreeLook.Orbit[0];

        [Tooltip("The minimum scale for the orbits")]
        [Range(0.01f, 5f)]
        public float minScale = 0.5f;

        [Tooltip("The maximum scale for the orbits")]
        [Range(1f, 25f)]
        public float maxScale = 1;

        [Tooltip("The Vertical axis.  Value is 0..1.  How much to scale the orbits")]
#if CM_2_1_10_OR_EARLIER
        public AxisState zAxis = new AxisState(50f, 0.1f, 0.1f, 1, "Mouse ScrollWheel", false);
#else
        [AxisStateProperty]
        public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);
#endif
        void OnValidate()
        {
            minScale = Mathf.Max(0.01f, minScale);
            maxScale = Mathf.Max(minScale, maxScale);
        }

        void Awake()
        {
#if false // make this true for CM 2.1.10 and earlier, false otherwise
            zAxis.SetThresholds(0, 1, false);
#endif
            freelook = GetComponentInChildren<CinemachineFreeLook>();
            if (freelook != null && originalOrbits.Length == 0)
            {
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
                for (int i = 0; i < Mathf.Min(originalOrbits.Length, freelook.m_Orbits.Length); i++)
                {
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
                }
            }
        }

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (freelook != null)
            {
                if (originalOrbits.Length != freelook.m_Orbits.Length)
                {
                    originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];
                    Array.Copy(freelook.m_Orbits, originalOrbits, freelook.m_Orbits.Length);
                }
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
                for (int i = 0; i < Mathf.Min(originalOrbits.Length, freelook.m_Orbits.Length); i++)
                {
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
                }
            }
        }
    }
}
