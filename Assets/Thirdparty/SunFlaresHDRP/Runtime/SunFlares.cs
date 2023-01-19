using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Collections.Generic;

namespace SunFlaresHDRP {

    [Serializable, VolumeComponentMenu("Post-processing/Sun Flares")]
    public sealed class SunFlares : CustomPostProcessVolumeComponent, IPostProcessComponent {

        #region Sun Flares

        public enum ApertureShape {
            Circular,
            Hexagonal,
            Pentagonal,
            Octogonal
        }

        [Serializable]
        public class ApertureShapeParameter : VolumeParameter<ApertureShape> {
            public ApertureShapeParameter(ApertureShape shape, bool overrideState = false)
                : base(shape, overrideState) {
            }
        }


        [Serializable]
        public class GradientParemeter : VolumeParameter<Gradient> {
            public GradientParemeter(Gradient gradient, bool overrideState = false)
                : base(gradient, overrideState) {
                if (gradient.colorKeys == null || gradient.colorKeys.Length == 0) {
                    GradientColorKey[] keys = new GradientColorKey[2];
                    keys[0] = new GradientColorKey(Color.white, 0);
                    keys[1] = new GradientColorKey(Color.white, 1);
                    gradient.colorKeys = keys;
                }
            }
        }


        [Header("General")]
        public ClampedFloatParameter sunFlaresIntensity = new ClampedFloatParameter(0.0f, 0, 1f);
        public FloatParameter sunFlaresOcclusionSpeed = new FloatParameter(50f);
        public FloatParameter sunFlaresCloudsOcclusion = new ClampedFloatParameter(0, 0, 1);
        public BoolParameter sunFlaresRaycastOcclusion = new BoolParameter(false);
        public LayerMaskParameter sunFlaresRaycastOcclusionLayerMask = new LayerMaskParameter(-1);
        public ColorParameter sunFlaresTint = new ColorParameter(new Color(1, 1, 1));
        [Tooltip("Gradient color where 0-1 refers to Sun altitude (0 = horizon and 1 = zenith)")]
        public GradientParemeter sunFlaresGradient = new GradientParemeter(new Gradient());
        public ApertureShapeParameter sunApertureShape = new ApertureShapeParameter(ApertureShape.Circular);
        [Tooltip("Enable to prevent effect rotation when looking to the Sun")]
        public BoolParameter sunFlaresRotationDeadZone = new BoolParameter(false);
        [Tooltip("Max distance threshold of Sun to center of screen where effect rotation is allowed.")]
        public FloatParameter sunFlaresRotationDeadZoneThreshold = new FloatParameter(0.01581f);
        [Header("Sun Disk")]
        public ClampedFloatParameter sunFlaresSunIntensity = new ClampedFloatParameter(0.1f, 0, 1f);
        public ClampedFloatParameter sunFlaresSunDiskSize = new ClampedFloatParameter(0.05f, 0, 1f);

        [Header("Light Diffraction")]
        public ClampedFloatParameter sunFlaresSunRayDiffractionThreshold = new ClampedFloatParameter(0.13f, 0, 1f);
        public ClampedFloatParameter sunFlaresSunRayDiffractionIntensity = new ClampedFloatParameter(3.5f, 0, 10f);
        public ClampedFloatParameter sunFlaresSolarWindSpeed = new ClampedFloatParameter(0.01f, 0, 1f);

        [Header("Corona Rays 1")]
        public ClampedFloatParameter sunFlaresCoronaRays1Length = new ClampedFloatParameter(0.02f, 0, 0.2f);
        public ClampedIntParameter sunFlaresCoronaRays1Streaks = new ClampedIntParameter(12, 2, 30);
        public ClampedFloatParameter sunFlaresCoronaRays1Spread = new ClampedFloatParameter(0.001f, 0, 0.1f);
        public ClampedFloatParameter sunFlaresCoronaRays1AngleOffset = new ClampedFloatParameter(0f, 0, 2f * Mathf.PI);
        [Min(0)] public FloatParameter sunFlaresCoronaRays1Intensity = new FloatParameter(1f);

        [Header("Corona Rays 2")]
        public ClampedFloatParameter sunFlaresCoronaRays2Length = new ClampedFloatParameter(0.05f, 0, 0.2f);
        public ClampedIntParameter sunFlaresCoronaRays2Streaks = new ClampedIntParameter(12, 2, 30);
        public ClampedFloatParameter sunFlaresCoronaRays2Spread = new ClampedFloatParameter(0.1f, 0, 0.1f);
        public ClampedFloatParameter sunFlaresCoronaRays2AngleOffset = new ClampedFloatParameter(0f, 0, 2f * Mathf.PI);
        [Min(0)] public FloatParameter sunFlaresCoronaRays2Intensity = new FloatParameter(1f);

        [Header("Ghost 1")]
        public ClampedFloatParameter sunFlaresGhosts1Size = new ClampedFloatParameter(0.03f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts1Offset = new ClampedFloatParameter(1.04f, -5f, 5f);
        public ClampedFloatParameter sunFlaresGhosts1Brightness = new ClampedFloatParameter(0.037f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts1Sharpness = new ClampedFloatParameter(200f, 1f, 500f);

        [Header("Ghost 2")]
        public ClampedFloatParameter sunFlaresGhosts2Size = new ClampedFloatParameter(0.1f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts2Offset = new ClampedFloatParameter(0.71f, -5f, 5f);
        public ClampedFloatParameter sunFlaresGhosts2Brightness = new ClampedFloatParameter(0.03f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts2Sharpness = new ClampedFloatParameter(200f, 1f, 500f);

        [Header("Ghost 3")]
        public ClampedFloatParameter sunFlaresGhosts3Size = new ClampedFloatParameter(0.24f, 0, 1f);
        public ClampedFloatParameter sunFlaresGhosts3Offset = new ClampedFloatParameter(0.31f, -5f, 5f);
        public ClampedFloatParameter sunFlaresGhosts3Brightness = new ClampedFloatParameter(0.025f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts3Sharpness = new ClampedFloatParameter(200f, 1f, 500f);

        [Header("Ghost 4")]
        public ClampedFloatParameter sunFlaresGhosts4Size = new ClampedFloatParameter(0.016f, 0f, 1f);
        public ClampedFloatParameter sunFlaresGhosts4Offset = new ClampedFloatParameter(0f, -5f, 5f);
        public ClampedFloatParameter sunFlaresGhosts4Brightness = new ClampedFloatParameter(0.017f, 0, 1f);
        public ClampedFloatParameter sunFlaresGhosts4Sharpness = new ClampedFloatParameter(200f, 1f, 500f);

        [Header("Halo")]
        public ClampedFloatParameter sunFlaresHaloOffset = new ClampedFloatParameter(0.22f, 0, 1f);
        public ClampedFloatParameter sunFlaresHaloAmplitude = new ClampedFloatParameter(15.1415f, 0, 50f);
        [Min(0)] public FloatParameter sunFlaresHaloIntensity = new FloatParameter(0.01f);

        #endregion

        static class ShaderParams {
            public static int sfSunData;
            public static int sfSunPos;
            public static int sfSunPosRightEye;
            public static int sfSunTintColor;
            public static int sfCoronaRays1;
            public static int sfCoronaRays2;
            public static int sfGhosts1;
            public static int sfGhosts2;
            public static int sfGhosts3;
            public static int sfGhosts4;
            public static int sfHalo;
            public static int sfFlareNoiseTex;
            public static int sfOcclusionTex;
            public static int sfInputTex;

            public const int PASS_FLARES = 0;
            public const int PASS_OCCLUSION = 1;

            static ShaderParams() {
                sfInputTex = Shader.PropertyToID("_InputTexture");
                sfSunData = Shader.PropertyToID("_SunData");
                sfSunPos = Shader.PropertyToID("_SunPos");
                sfSunPosRightEye = Shader.PropertyToID("_SunPosRightEye");
                sfSunTintColor = Shader.PropertyToID("_SunTint");
                sfCoronaRays1 = Shader.PropertyToID("_SunCoronaRays1");
                sfCoronaRays2 = Shader.PropertyToID("_SunCoronaRays2");
                sfGhosts1 = Shader.PropertyToID("_SunGhosts1");
                sfGhosts2 = Shader.PropertyToID("_SunGhosts2");
                sfGhosts3 = Shader.PropertyToID("_SunGhosts3");
                sfGhosts4 = Shader.PropertyToID("_SunGhosts4");
                sfHalo = Shader.PropertyToID("_SunHalo");
                sfFlareNoiseTex = Shader.PropertyToID("_FlareNoiseTex");
                sfOcclusionTex = Shader.PropertyToID("_OcclusionTex");
            }
        }


        const string SKW_HEXAGONAL_SHAPE = "SF_HEXAGONAL_SHAPE";
        const string SKW_PENTAGONAL_SHAPE = "SF_PENTAGONAL_SHAPE";
        const string SKW_OCTOGONAL_SHAPE = "SF_OCTOGONAL_SHAPE";
        const string SKW_OCCLUSION_INIT = "SF_OCCLUSION_INIT";

        class PerCamData {
            public float sunFlareCurrentIntensity;
            public Vector4 sunLastScrPos;
            public float sunLastRot;
            public float sunFlareTime;
            public RTHandle lumRT;
        }

        Material m_Material;
        static Texture2D flareNoiseTex;
        public static Transform lightTransform;
        Dictionary<Camera, PerCamData> perCamData;

        public bool IsActive() => m_Material != null && sunFlaresIntensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > HDRP Default Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        const string kShaderName = "Hidden/Shader/Sun Flares HDRP";
        PhysicallyBasedSky pbSky;

        public override void Setup() {
            if (Shader.Find(kShaderName) != null) {
                m_Material = new Material(Shader.Find(kShaderName));
            } else {
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume is unable to load.");
            }
            if (perCamData == null) {
                perCamData = new Dictionary<Camera, PerCamData>();
            } else {
                perCamData.Clear();
            }

            // Locate designated Sun light
            SunFlaresLight[] sf = FindObjectsOfType<SunFlaresLight>();
            if (sf != null && sf.Length > 0) {
                if (sf.Length > 1) {
                    Debug.LogWarning("Only one Sun Flares Light component is allowed in the scene.");
                }
                lightTransform = sf[0].transform;
            } else {
                // Fallback: find any directional light
                Light[] lights = FindObjectsOfType<Light>();
                if (Camera.main != null) { // default transform
                    lightTransform = Camera.main.transform;
                }
                foreach (Light light in lights) {
                    if (light.type == LightType.Directional) {
                        lightTransform = light.transform;
                        break;
                    }
                }
            }
            pbSky = VolumeManager.instance.stack.GetComponent<PhysicallyBasedSky>();
        }

        Vector3 GetNearPoint(Vector3 p, Vector3 o, Vector3 d) {
            float t = Vector3.Dot(p - o, d);
            return o + d * t;
        }


        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination) {
            if (m_Material == null || lightTransform == null) {
                HDUtils.BlitCameraTexture(cmd, source, destination);
                return;
            }

            // check if Sun is visible
            Camera cam = camera.camera;
            PerCamData camData;
            if (!perCamData.TryGetValue(cam, out camData)) {
                camData = new PerCamData();
                camData.lumRT = RTHandles.Alloc(2, 2, colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat);
                perCamData[cam] = camData;
            }
            Vector3 sunWorldPosition = cam.transform.position - lightTransform.forward * 1000f;
            float flareIntensity = 0;
            RenderTextureDescriptor sourceDesc = UnityEngine.XR.XRSettings.eyeTextureDesc;
            Vector3 sunScrPos = cam.WorldToViewportPoint(sunWorldPosition, sourceDesc.vrUsage == VRTextureUsage.TwoEyes ? Camera.MonoOrStereoscopicEye.Left : Camera.MonoOrStereoscopicEye.Mono);
            bool sunVisible = sunScrPos.z > 0 && sunScrPos.x >= -0.1f && sunScrPos.x < 1.1f && sunScrPos.y >= -0.1f && sunScrPos.y < 1.1f;
            if (sunVisible && sunFlaresRaycastOcclusion.value) {
                Vector3 toLightVector = sunWorldPosition - cam.transform.position;
                float maxDistance = toLightVector.magnitude;
                Ray ray = new Ray(cam.transform.position, toLightVector);
                sunVisible = !Physics.Raycast(ray, maxDistance, sunFlaresRaycastOcclusionLayerMask.value);
            }
            if (sunVisible) {
                if (sourceDesc.vrUsage == VRTextureUsage.TwoEyes) {
                    Vector3 sunScrPosRightEye = cam.WorldToViewportPoint(sunWorldPosition, Camera.MonoOrStereoscopicEye.Right);
                    m_Material.SetVector(ShaderParams.sfSunPosRightEye, sunScrPosRightEye);
                    sunVisible = sunScrPosRightEye.z > 0 && sunScrPosRightEye.x >= -0.1f && sunScrPosRightEye.x < 1.1f && sunScrPosRightEye.y >= -0.1f && sunScrPosRightEye.y < 1.1f;
                }
                if (sunVisible) {
                    Vector3 dd = sunScrPos - new Vector3(0.5f, 0.5f);
                    flareIntensity = sunFlaresIntensity.value * Mathf.Clamp01((0.6f - Mathf.Max(Mathf.Abs(dd.x), Mathf.Abs(dd.y))) / 0.6f);
                }
            }
            camData.sunFlareCurrentIntensity = Mathf.Lerp(camData.sunFlareCurrentIntensity, flareIntensity, Application.isPlaying ? 0.5f : 1f);
            if (camData.sunFlareCurrentIntensity > 0) {

                if (flareIntensity > 0) {
                    camData.sunLastScrPos = sunScrPos;
#if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        m_Material.EnableKeyword(SKW_OCCLUSION_INIT);
                    } else {
                        m_Material.DisableKeyword(SKW_OCCLUSION_INIT);
                    }
#endif
                    HDUtils.DrawFullScreen(cmd, m_Material, camData.lumRT, shaderPassId: ShaderParams.PASS_OCCLUSION);
                }

                m_Material.SetTexture(ShaderParams.sfOcclusionTex, camData.lumRT);

                Color tintColor = sunFlaresTint.value;
                // Evaluate Sun gradient tint based on Sun angle to camera or to planet surface if PBSky is present
                float lightAngle;
                if (pbSky != null) {
                    Vector3 camPos = cam.transform.position;
                    Vector3 earthPos = pbSky.planetCenterPosition.value;
                    Vector3 nearRayPos = GetNearPoint(earthPos, camPos, -lightTransform.forward);
                    Vector3 surfacePos = earthPos + (nearRayPos - earthPos).normalized * pbSky.planetaryRadius.value;
                    Vector3 surfaceToCam = (camPos - surfacePos).normalized;
                    float dt = Vector3.Dot(lightTransform.forward, surfaceToCam);
                    lightAngle = Mathf.Acos(dt) * Mathf.Rad2Deg;
                } else {
                    lightAngle = lightTransform.eulerAngles.x;
                }
                if (lightAngle > 90) lightAngle = 360 - lightAngle;
                float t = Mathf.Clamp01(lightAngle / 90f);
                Color gradient = sunFlaresGradient.value.Evaluate(t);
                tintColor.r *= gradient.r * camData.sunFlareCurrentIntensity;
                tintColor.g *= gradient.g * camData.sunFlareCurrentIntensity;
                tintColor.b *= gradient.b * camData.sunFlareCurrentIntensity;
                tintColor.a = sunFlaresOcclusionSpeed.value;
                m_Material.SetColor(ShaderParams.sfSunTintColor, tintColor);
                camData.sunLastScrPos.z = 0.5f + camData.sunFlareTime * sunFlaresSolarWindSpeed.value;
                Vector2 sfDist = new Vector2(0.5f - camData.sunLastScrPos.y, camData.sunLastScrPos.x - 0.5f);
                float rotationThreshold = sunFlaresRotationDeadZoneThreshold.value;
                if (!sunFlaresRotationDeadZone.value || sfDist.sqrMagnitude > rotationThreshold * rotationThreshold) {
                    camData.sunLastRot = Mathf.Atan2(sfDist.x, sfDist.y);
                }
                camData.sunLastScrPos.w = camData.sunLastRot;
                camData.sunFlareTime += Time.deltaTime;
                m_Material.SetVector(ShaderParams.sfSunPos, camData.sunLastScrPos);

                m_Material.SetVector(ShaderParams.sfSunData, new Vector4(sunFlaresSunIntensity.value, sunFlaresSunDiskSize.value, sunFlaresSunRayDiffractionIntensity.value, sunFlaresSunRayDiffractionThreshold.value));
                m_Material.SetVector(ShaderParams.sfCoronaRays1, new Vector4(sunFlaresCoronaRays1Length.value, Mathf.Max(sunFlaresCoronaRays1Streaks.value / 2f, 1), sunFlaresCoronaRays1Spread.value, sunFlaresCoronaRays1AngleOffset.value));
                m_Material.SetVector(ShaderParams.sfCoronaRays2, new Vector4(sunFlaresCoronaRays2Length.value, Mathf.Max(sunFlaresCoronaRays2Streaks.value / 2f, 1), sunFlaresCoronaRays2Spread.value, sunFlaresCoronaRays2AngleOffset.value));
                bool isCircular = sunApertureShape.value == ApertureShape.Circular;
                const float k = 12f / 200f;
                m_Material.SetVector(ShaderParams.sfGhosts1, new Vector4(isCircular ? Mathf.Max(1f, sunFlaresGhosts1Sharpness.value * k) : -sunFlaresGhosts1Sharpness.value, sunFlaresGhosts1Size.value, sunFlaresGhosts1Offset.value, sunFlaresGhosts1Brightness.value));
                m_Material.SetVector(ShaderParams.sfGhosts2, new Vector4(isCircular ? Mathf.Max(1f, sunFlaresGhosts2Sharpness.value * k) : -sunFlaresGhosts2Sharpness.value, sunFlaresGhosts2Size.value, sunFlaresGhosts2Offset.value, sunFlaresGhosts2Brightness.value));
                m_Material.SetVector(ShaderParams.sfGhosts3, new Vector4(isCircular ? Mathf.Max(1f, sunFlaresGhosts3Sharpness.value * k) : -sunFlaresGhosts3Sharpness.value, sunFlaresGhosts3Size.value, sunFlaresGhosts3Offset.value, sunFlaresGhosts3Brightness.value));
                m_Material.SetVector(ShaderParams.sfGhosts4, new Vector4(isCircular ? Mathf.Max(1f, sunFlaresGhosts4Sharpness.value * k) : -sunFlaresGhosts4Sharpness.value, sunFlaresGhosts4Size.value, sunFlaresGhosts4Offset.value, sunFlaresGhosts4Brightness.value));
                m_Material.SetVector(ShaderParams.sfHalo, new Vector4(sunFlaresHaloOffset.value, sunFlaresHaloAmplitude.value, sunFlaresHaloIntensity.value * 100f, sunFlaresCloudsOcclusion.value));
                switch (sunApertureShape.value) {
                    case ApertureShape.Hexagonal:
                        m_Material.DisableKeyword(SKW_PENTAGONAL_SHAPE);
                        m_Material.DisableKeyword(SKW_OCTOGONAL_SHAPE);
                        m_Material.EnableKeyword(SKW_HEXAGONAL_SHAPE);
                        break;
                    case ApertureShape.Pentagonal:
                        m_Material.DisableKeyword(SKW_HEXAGONAL_SHAPE);
                        m_Material.DisableKeyword(SKW_OCTOGONAL_SHAPE);
                        m_Material.EnableKeyword(SKW_PENTAGONAL_SHAPE);
                        break;
                    case ApertureShape.Octogonal:
                        m_Material.DisableKeyword(SKW_HEXAGONAL_SHAPE);
                        m_Material.DisableKeyword(SKW_PENTAGONAL_SHAPE);
                        m_Material.EnableKeyword(SKW_OCTOGONAL_SHAPE);
                        break;
                    default:
                        m_Material.DisableKeyword(SKW_HEXAGONAL_SHAPE);
                        m_Material.DisableKeyword(SKW_PENTAGONAL_SHAPE);
                        m_Material.DisableKeyword(SKW_OCTOGONAL_SHAPE);
                        break;
                }
                if (flareNoiseTex == null) {
                    flareNoiseTex = Resources.Load<Texture2D>("Textures/flareNoise");
                }
                m_Material.SetTexture(ShaderParams.sfFlareNoiseTex, flareNoiseTex);
                m_Material.SetTexture(ShaderParams.sfInputTex, source);
                HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: ShaderParams.PASS_FLARES);
            } else {
                HDUtils.BlitCameraTexture(cmd, source, destination);
            }
        }

        public override void Cleanup() {
            CoreUtils.Destroy(m_Material);
            foreach (PerCamData data in perCamData.Values) {
                if (data.lumRT != null) {
                    data.lumRT.Release();
                }
            }
        }
    }

}