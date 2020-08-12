using Optispeech.Documentation;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

namespace Optispeech.Targets.Controllers {

    /// <summary>
    /// Controls an oscillating target, which takes two points and oscillates between them with a given frequency
    /// </summary>
    public class CurvedTargetController : TargetController {

        /// <summary>
        /// Basic elliptical trajectory parameters:
        ///     startPosition: start point for the trajectory
        ///     semiMajor, semiMinor: Semi-major (horizontal) and semi-minor (vertical) axis for the ellipse
        ///     frequency: no. of cycles per second for the target
        /// </summary>
        public Vector3 startPosition;
        public float hAmp, vAmp, frequency;

        public float angle, angularSpeed;
        public Vector3 ellipseCenter, ellipseRadius;

        public Vector3 getEllipseRadius(float hAmp, float vAmp)
        {
            return new Vector3(0f, vAmp, hAmp);
        }

        public float getAngularSpeed(float frequency)
        {
            return Mathf.PI * frequency;
        }

        public Vector3 getEllipseCenter(Vector3 startPos, float hAmp)
        {
            return startPos - new Vector3(0f, 0f, hAmp);
        }

        public float getAngle(float angularSpeed, long currTime)
        {
            float angle = (angularSpeed * currTime / 1000) % 180;
            if (angle < 90) { angle = 180 - angle; }
            return angle;
        }

        public Vector3 pointOnEllipse(Vector3 center, Vector3 axes, float angle)
        {
            //Rotation on z-y plane, angle being the arctan(y/z), i.e. angle between +z and +y in counter-clockwise direction
            return new Vector3(center.x, center.y + axes.y * Mathf.Sin(angle), center.z + axes.z * Mathf.Cos(angle));
        }

        [HideInDocumentation]
        public override long GetCycleDuration() {
            return Mathf.RoundToInt(1000 / frequency);
        }

        [HideInDocumentation]
        public override Vector3 GetTargetPosition(long currTime)
        {
            angle = getAngle(angularSpeed, currTime);
            return pointOnEllipse(ellipseCenter, ellipseRadius, angle);
        }

        [HideInDocumentation]
        public override void ApplyConfigFromString(string config) {
            base.ApplyConfigFromString(config);
            string[] values = config.Split('\t');
            if (values.Length < NUM_BASE_CONFIG_VALUES + 7)
                return;
            float.TryParse(values[NUM_BASE_CONFIG_VALUES], out startPosition.x);
            float.TryParse(values[NUM_BASE_CONFIG_VALUES + 1], out startPosition.y);
            float.TryParse(values[NUM_BASE_CONFIG_VALUES + 2], out startPosition.z);
            float.TryParse(values[NUM_BASE_CONFIG_VALUES + 3], out vAmp);
            float.TryParse(values[NUM_BASE_CONFIG_VALUES + 4], out hAmp);
            float.TryParse(values[NUM_BASE_CONFIG_VALUES + 5], out frequency);

            angularSpeed = getAngularSpeed(frequency);
            ellipseCenter = getEllipseCenter(startPosition, hAmp);
            ellipseRadius = getEllipseRadius(hAmp, vAmp);
        }

        [HideInDocumentation]
        public override string ToString() {
            return base.ToString() + "\t" + startPosition.x + "\t" + startPosition.y + "\t" + vAmp + "\t" + hAmp + "\t" + frequency;
        }
    }
}