using UnityEditor.Rendering;
using UnityEngine;
using UnityEditor;

namespace SunFlaresHDRP {
    [VolumeComponentEditor(typeof(SunFlares))]
    sealed class SunFlaresEditor : VolumeComponentEditor {

        SerializedDataParameter sunFlaresIntensity, sunFlaresOcclusionSpeed, sunFlaresRaycastOcclusion, sunFlaresRaycastOcclusionLayerMask, sunFlaresCloudsOcclusion;
        SerializedDataParameter sunFlaresTint, sunFlaresGradient;
        SerializedDataParameter sunApertureShape;

        SerializedDataParameter sunFlaresSolarWindSpeed, sunFlaresSunRayDiffractionIntensity, sunFlaresSunRayDiffractionThreshold, sunFlaresRotationDeadZone, sunFlaresRotationDeadZoneThreshold;

        SerializedDataParameter sunFlaresSunIntensity, sunFlaresSunDiskSize;

        SerializedDataParameter sunFlaresCoronaRays1Length, sunFlaresCoronaRays1Streaks, sunFlaresCoronaRays1Spread, sunFlaresCoronaRays1AngleOffset;
        SerializedDataParameter sunFlaresCoronaRays2Length, sunFlaresCoronaRays2Streaks, sunFlaresCoronaRays2Spread, sunFlaresCoronaRays2AngleOffset;

        SerializedDataParameter sunFlaresGhosts1Size, sunFlaresGhosts1Offset, sunFlaresGhosts1Brightness, sunFlaresGhosts1Sharpness;
        SerializedDataParameter sunFlaresGhosts2Size, sunFlaresGhosts2Offset, sunFlaresGhosts2Brightness, sunFlaresGhosts2Sharpness;
        SerializedDataParameter sunFlaresGhosts3Size, sunFlaresGhosts3Offset, sunFlaresGhosts3Brightness, sunFlaresGhosts3Sharpness;
        SerializedDataParameter sunFlaresGhosts4Size, sunFlaresGhosts4Offset, sunFlaresGhosts4Brightness, sunFlaresGhosts4Sharpness;

        SerializedDataParameter sunFlaresHaloOffset, sunFlaresHaloAmplitude, sunFlaresHaloIntensity;


        public override void OnEnable() {
            base.OnEnable();

            var o = new PropertyFetcher<SunFlares>(serializedObject);

            sunFlaresIntensity = Unpack(o.Find(x => x.sunFlaresIntensity));
            sunFlaresOcclusionSpeed = Unpack(o.Find(x => x.sunFlaresOcclusionSpeed));
            sunFlaresCloudsOcclusion = Unpack(o.Find(x => x.sunFlaresCloudsOcclusion));
            sunFlaresRaycastOcclusion = Unpack(o.Find(x => x.sunFlaresRaycastOcclusion));
            sunFlaresRaycastOcclusionLayerMask = Unpack(o.Find(x => x.sunFlaresRaycastOcclusionLayerMask));
            sunFlaresTint = Unpack(o.Find(x => x.sunFlaresTint));
            sunFlaresGradient = Unpack(o.Find(x => x.sunFlaresGradient));
            sunApertureShape = Unpack(o.Find(x => x.sunApertureShape));
            sunFlaresRotationDeadZone = Unpack(o.Find(x => x.sunFlaresRotationDeadZone));
            sunFlaresRotationDeadZoneThreshold = Unpack(o.Find(x => x.sunFlaresRotationDeadZoneThreshold));

            sunFlaresSolarWindSpeed = Unpack(o.Find(x => x.sunFlaresSolarWindSpeed));
            sunFlaresSunRayDiffractionIntensity = Unpack(o.Find(x => x.sunFlaresSunRayDiffractionIntensity));
            sunFlaresSunRayDiffractionThreshold = Unpack(o.Find(x => x.sunFlaresSunRayDiffractionThreshold));
            sunFlaresSunIntensity = Unpack(o.Find(x => x.sunFlaresSunIntensity));
            sunFlaresSunDiskSize = Unpack(o.Find(x => x.sunFlaresSunDiskSize));
            sunFlaresCoronaRays1Length = Unpack(o.Find(x => x.sunFlaresCoronaRays1Length));
            sunFlaresCoronaRays1Streaks = Unpack(o.Find(x => x.sunFlaresCoronaRays1Streaks));
            sunFlaresCoronaRays1Spread = Unpack(o.Find(x => x.sunFlaresCoronaRays1Spread));
            sunFlaresCoronaRays1AngleOffset = Unpack(o.Find(x => x.sunFlaresCoronaRays1AngleOffset));
            sunFlaresCoronaRays2Length = Unpack(o.Find(x => x.sunFlaresCoronaRays2Length));
            sunFlaresCoronaRays2Streaks = Unpack(o.Find(x => x.sunFlaresCoronaRays2Streaks));
            sunFlaresCoronaRays2Spread = Unpack(o.Find(x => x.sunFlaresCoronaRays2Spread));
            sunFlaresCoronaRays2AngleOffset = Unpack(o.Find(x => x.sunFlaresCoronaRays2AngleOffset));

            sunFlaresGhosts1Size = Unpack(o.Find(x => x.sunFlaresGhosts1Size));
            sunFlaresGhosts1Offset = Unpack(o.Find(x => x.sunFlaresGhosts1Offset));
            sunFlaresGhosts1Brightness = Unpack(o.Find(x => x.sunFlaresGhosts1Brightness));
            sunFlaresGhosts1Sharpness = Unpack(o.Find(x => x.sunFlaresGhosts1Sharpness));

            sunFlaresGhosts2Size = Unpack(o.Find(x => x.sunFlaresGhosts2Size));
            sunFlaresGhosts2Offset = Unpack(o.Find(x => x.sunFlaresGhosts2Offset));
            sunFlaresGhosts2Brightness = Unpack(o.Find(x => x.sunFlaresGhosts2Brightness));
            sunFlaresGhosts2Sharpness = Unpack(o.Find(x => x.sunFlaresGhosts2Sharpness));

            sunFlaresGhosts3Size = Unpack(o.Find(x => x.sunFlaresGhosts3Size));
            sunFlaresGhosts3Offset = Unpack(o.Find(x => x.sunFlaresGhosts3Offset));
            sunFlaresGhosts3Brightness = Unpack(o.Find(x => x.sunFlaresGhosts3Brightness));
            sunFlaresGhosts3Sharpness = Unpack(o.Find(x => x.sunFlaresGhosts3Sharpness));

            sunFlaresGhosts4Size = Unpack(o.Find(x => x.sunFlaresGhosts4Size));
            sunFlaresGhosts4Offset = Unpack(o.Find(x => x.sunFlaresGhosts4Offset));
            sunFlaresGhosts4Brightness = Unpack(o.Find(x => x.sunFlaresGhosts4Brightness));
            sunFlaresGhosts4Sharpness = Unpack(o.Find(x => x.sunFlaresGhosts4Sharpness));

            sunFlaresHaloOffset = Unpack(o.Find(x => x.sunFlaresHaloOffset));
            sunFlaresHaloAmplitude = Unpack(o.Find(x => x.sunFlaresHaloAmplitude));
            sunFlaresHaloIntensity = Unpack(o.Find(x => x.sunFlaresHaloIntensity));

        }

        public override void OnInspectorGUI() {

            if (SunFlares.lightTransform != null) {
                EditorGUILayout.HelpBox("Using \"" + SunFlares.lightTransform.name + "\" directional light for the Sun Flares effect. To use a different light, add a SunFlaresLight component to the desired light.", MessageType.Info);
            }
            PropertyField(sunFlaresIntensity, new GUIContent("Global Intensity"));
            PropertyField(sunFlaresOcclusionSpeed, new GUIContent("Occlusion Speed"));
            PropertyField(sunFlaresRaycastOcclusion, new GUIContent("Raycast Occlusion", "Occludes flare based on colliders"));
            if (sunFlaresRaycastOcclusion.value.boolValue) {
                PropertyField(sunFlaresRaycastOcclusionLayerMask, new GUIContent("Occluders Layer Mask", "Which occluders can block the sun flares"));
            }
            PropertyField(sunFlaresCloudsOcclusion, new GUIContent("Clouds Occlusion", "Occludes flare based on whiteness of the clouds on the sky"));
            PropertyField(sunFlaresTint, new GUIContent("Tint Color"));
            PropertyField(sunFlaresGradient, new GUIContent("Gradient Color"));
            PropertyField(sunApertureShape, new GUIContent("Aperture Shape"));
            PropertyField(sunFlaresRotationDeadZone, new GUIContent("Rotation Dead Zone"));
            if (sunFlaresRotationDeadZone.value.boolValue) {
                PropertyField(sunFlaresRotationDeadZoneThreshold, new GUIContent("Dead Zone Threshold"));
            }

            PropertyField(sunFlaresSunRayDiffractionThreshold, new GUIContent("Diffraction Threshold"));
            PropertyField(sunFlaresSunRayDiffractionIntensity, new GUIContent("Diffraction Intensity"));
            PropertyField(sunFlaresSolarWindSpeed, new GUIContent("Solar Wind Speed"));

            PropertyField(sunFlaresSunIntensity, new GUIContent("Sun Intensity"));
            PropertyField(sunFlaresSunDiskSize, new GUIContent("Disk Size"));
            PropertyField(sunFlaresCoronaRays1Length, new GUIContent("Length"));
            PropertyField(sunFlaresCoronaRays1Streaks, new GUIContent("Streaks"));
            PropertyField(sunFlaresCoronaRays1Spread, new GUIContent("Spread"));
            PropertyField(sunFlaresCoronaRays1AngleOffset, new GUIContent("Angle"));
            PropertyField(sunFlaresCoronaRays2Length, new GUIContent("Length"));
            PropertyField(sunFlaresCoronaRays2Streaks, new GUIContent("Streaks"));
            PropertyField(sunFlaresCoronaRays2Spread, new GUIContent("Spread"));
            PropertyField(sunFlaresCoronaRays2AngleOffset, new GUIContent("Angle"));
            PropertyField(sunFlaresGhosts1Size, new GUIContent("Size"));
            PropertyField(sunFlaresGhosts1Offset, new GUIContent("Offset"));
            PropertyField(sunFlaresGhosts1Brightness, new GUIContent("Brightness"));
            PropertyField(sunFlaresGhosts1Sharpness, new GUIContent("Sharpness"));
            PropertyField(sunFlaresGhosts2Size, new GUIContent("Size"));
            PropertyField(sunFlaresGhosts2Offset, new GUIContent("Offset"));
            PropertyField(sunFlaresGhosts2Brightness, new GUIContent("Brightness"));
            PropertyField(sunFlaresGhosts2Sharpness, new GUIContent("Sharpness"));
            PropertyField(sunFlaresGhosts3Size, new GUIContent("Size"));
            PropertyField(sunFlaresGhosts3Offset, new GUIContent("Offset"));
            PropertyField(sunFlaresGhosts3Brightness, new GUIContent("Brightness"));
            PropertyField(sunFlaresGhosts3Sharpness, new GUIContent("Sharpness"));
            PropertyField(sunFlaresGhosts4Size, new GUIContent("Size"));
            PropertyField(sunFlaresGhosts4Offset, new GUIContent("Offset"));
            PropertyField(sunFlaresGhosts4Brightness, new GUIContent("Brightness"));
            PropertyField(sunFlaresGhosts4Sharpness, new GUIContent("Sharpness"));
            PropertyField(sunFlaresHaloOffset, new GUIContent("Offset"));
            PropertyField(sunFlaresHaloAmplitude, new GUIContent("Amplitude"));
            PropertyField(sunFlaresHaloIntensity, new GUIContent("Intensity"));

        }

    }
}