using HarmonyLib;
using NeosModLoader;
using BaseX;
using FrooxEngine;
using System;
using System.Collections.Generic;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Operators;
using FrooxEngine.LogiX.Data;


// Thank you LeCloutPanda and everyone who helped me through testing and motivation

namespace LaserRecolorJank
{

    public class Patch : NeosMod
    {
        public override String Name => "LaserRecolorJank";

        public override String Author => "APnda";

        public override String Link => "https://github.com/Ap6661/LaserRecolorJank";

        public override String Version => "1.0.0";

        private static Uri[] DefaultCursors = { NeosAssets.Graphics.Icons.Laser.Cursor,
                                                NeosAssets.Graphics.Icons.Laser.GrabCursor,
                                                NeosAssets.Graphics.Icons.Laser.GrabHoverCursor,
                                                NeosAssets.Graphics.Icons.Laser.InteractCursor,
                                                NeosAssets.Graphics.Icons.Laser.SliderBothCursor,
                                                NeosAssets.Graphics.Icons.Laser.SliderHorizontalCursor,
                                                NeosAssets.Graphics.Icons.Laser.SliderVerticalCursor,
                                                NeosAssets.Graphics.Icons.Laser.TypingCursor};

        private static List<Sync<bool>>   ReactiveEnableds = new List<Sync<bool>>();
        private static List<Sync<color>>  RightNearColors  = new List<Sync<color>>();
        private static List<Sync<color>>  RightFarColors   = new List<Sync<color>>();
        private static List<Sync<color>>  LeftNearColors   = new List<Sync<color>>();
        private static List<Sync<color>>  LeftFarColors    = new List<Sync<color>>();
        private static List<Sync<string>> RightNearVars    = new List<Sync<string>>();
        private static List<Sync<string>> RightFarVars     = new List<Sync<string>>();
        private static List<Sync<string>> LeftNearVars     = new List<Sync<string>>();
        private static List<Sync<string>> LeftFarVars      = new List<Sync<string>>();


        public static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool>  ENABLED = new ModConfigurationKey<bool>("enabled", "Enabled", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool>  REACTIVE = new ModConfigurationKey<bool>("color_reactive", "Color Reacts to enviornment and inputs", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool>  CURSOR_ENABLED = new ModConfigurationKey<bool>("cursor_enabled", "Enables custom cursors", () => true);
        


        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> RIGHT_NEAR = new ModConfigurationKey<color>("right_near", "Right Near Color", () => new color(.25F, 1F, 1F, 1F));
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> RIGHT_FAR = new ModConfigurationKey<color>("right_far", "Right Far Color", () => new color(.25F, 1F, 1F, 1F));

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> RIGHT_NEAR_VAR = new ModConfigurationKey<string>("right_near_var", "Right Near Color Dynamic Variable", () => "");
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> RIGHT_FAR_VAR = new ModConfigurationKey<string>("right_far_var", "Right Far Color Dynamic Variable", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<float>   RIGHT_CURSOR_SCALE = new ModConfigurationKey<float>("right_cursor_scale", "The scale of the right cursor", () => 1F);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_CURSOR = new ModConfigurationKey<Uri>("right_cursor", "Right Cursor Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_GRAB = new ModConfigurationKey<Uri>("right_grab", "Right Grab Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_GRAB_HOVER = new ModConfigurationKey<Uri>("right_grab_hover", "Right Grab Hover Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_INTERACT = new ModConfigurationKey<Uri>("right_interact", "Right Interact Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_SLIDER_BOTH = new ModConfigurationKey<Uri>("right_double_slider", "Right Double Slider Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_SLIDER_HORIZONTAL = new ModConfigurationKey<Uri>("right_slider_horizontal", "Right Slider Horizontal Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_SLIDER_VERTICAL = new ModConfigurationKey<Uri>("right_slider_vertical", "Right Slider Vertical Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   RIGHT_TYPING = new ModConfigurationKey<Uri>("right_typing", "Right Typing Icon", () => null);



        
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> LEFT_NEAR = new ModConfigurationKey<color>("left_near", "Left Near Color", () => new color(.25F, 1F, 1F, 1F));
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> LEFT_FAR = new ModConfigurationKey<color>("left_far", "Left Far Color", () => new color(.25F, 1F, 1F, 1F));
        
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> LEFT_NEAR_VAR = new ModConfigurationKey<string>("left_near_var", "Left Near Color Dynamic Variable", () => "");
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<string> LEFT_FAR_VAR = new ModConfigurationKey<string>("left_far_var", "Left Far Color Dynamic Variable", () => "");

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<float>   LEFT_CURSOR_SCALE = new ModConfigurationKey<float>("left_cursor_scale", "The scale of the left cursor", () => 1F);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_CURSOR = new ModConfigurationKey<Uri>("left_cursor", "Left Cursor Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_GRAB = new ModConfigurationKey<Uri>("left_grab", "Left Grab Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_GRAB_HOVER = new ModConfigurationKey<Uri>("left_grab_hover", "Left Grab Hover Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_INTERACT = new ModConfigurationKey<Uri>("left_interact", "Left Interact Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_SLIDER_BOTH = new ModConfigurationKey<Uri>("left_double_slider", "Left Double Slider Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_SLIDER_HORIZONTAL = new ModConfigurationKey<Uri>("left_slider_horizontal", "Left Slider Horizontal Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_SLIDER_VERTICAL = new ModConfigurationKey<Uri>("left_slider_vertical", "Left Slider Vertical Icon", () => null);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Uri>   LEFT_TYPING = new ModConfigurationKey<Uri>("left_typing", "Left Typing Icon", () => null);
        






        public override void OnEngineInit()

        {
            config = GetConfiguration();
            config.Save(true);

            config.OnThisConfigurationChanged += OnConfigurationUpdate;

            Harmony harmony = new Harmony("apnda.lasercolor.jank");
            harmony.PatchAll(); }

        [HarmonyPatch(typeof(InteractionLaser))]
        class InteractionLaserJank
        {
            [HarmonyPrefix]
            [HarmonyPatch("OnAwake")]
            static void Prefix(InteractionLaser __instance,
                               FieldDrive<color> ____startColor,
                               FieldDrive<color> ____endColor,
                               FieldDrive<float3> ____directPoint,
                               FieldDrive<float3> ____actualPoint)
            {
                __instance.RunInUpdates(3, () =>
                {
                    if (!config.GetValue(ENABLED)) return;
                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;
                    
                    var Assets = __instance.Slot.AddSlot("Assets");
                    var ColE = Assets.AttachComponent<ValueField<color>>();
                    var ColS = Assets.AttachComponent<ValueField<color>>();
                    var Mesh = Assets.AttachComponent<BentTubeMesh>();
                    var Renderer = __instance.Slot.GetComponent<MeshRenderer>().Mesh;

                    Renderer.Target = Mesh;
                      Mesh.Radius.Value = 0.002f;
                      Mesh.Sides.Value = 6;
                      Mesh.Segments.Value = 16;
                      Mesh.Ends.Value = SegmentedBuilder.Ends.Capped;
                      Mesh.Shading.Value = SegmentedBuilder.Shading.Smooth;
                    
                    ____startColor.Value = ColS.Value.ReferenceID;
                    ____endColor.Value = ColE.Value.ReferenceID;

                    ____directPoint.ForceLink(Mesh.DirectTargetPoint);
                    ____actualPoint.ForceLink(Mesh.ActualTargetPoint);

                    var IsRight = __instance.Side == Chirality.Right;
                    var sc = IsRight ? config.GetValue(RIGHT_NEAR) : config.GetValue(LEFT_NEAR);
                    var ec = IsRight ? config.GetValue(RIGHT_FAR)  : config.GetValue(LEFT_FAR);
                    var sv = IsRight ? config.GetValue(RIGHT_NEAR_VAR) : config.GetValue(LEFT_NEAR_VAR);
                    var ev = IsRight ? config.GetValue(RIGHT_FAR_VAR)  : config.GetValue(LEFT_FAR_VAR);

                    Mesh.StartPointColor.Value = sc;
                    Mesh.EndPointColor.Value = ec;   

                    var ReactiveToggle = Assets.AttachComponent<ValueField<bool>>();
                      ReactiveToggle.Value.Value = config.GetValue(REACTIVE);
                      ReactiveEnableds.Add(ReactiveToggle.Value); 
                      ReactiveToggle.Disposing += (field) => {ReactiveEnableds.Remove(ReactiveToggle.Value); }; 

                    var Start = SetUpLogix(Assets, ColS.Value, Mesh.StartPointColor, sc, ReactiveToggle, true );
                    var End   = SetUpLogix(Assets, ColE.Value, Mesh.EndPointColor,   ec, ReactiveToggle, false);

                    var FStart = Start.Slot.AttachComponent<DynamicField<color>>();
                    var FEnd   = End.Slot.AttachComponent<DynamicField<color>>();

                    FStart.VariableName.Value = sv;
                    FEnd.VariableName.Value   = ev;

                    FStart.TargetField.Target = Start.Value;
                    FEnd.TargetField.Target   = End.Value;



                    if (IsRight)
                    {
                      RightNearColors.Add(Start.Value);
                      Start.Disposing += (field) => {RightNearColors.Remove(Start.Value);};
                      RightFarColors.Add(End.Value);
                      End.Disposing   += (field) => {RightFarColors.Remove(End.Value);};
                      RightNearVars.Add(FStart.VariableName);
                      Start.Disposing += (field) => {RightNearVars.Remove(FStart.VariableName);};
                      RightFarVars.Add(FEnd.VariableName);
                      End.Disposing   += (field) => {RightFarVars.Remove(FEnd.VariableName);};
                    } 
                    else 
                    {
                      LeftNearColors.Add(Start.Value);
                      Start.Disposing += (field) => {LeftNearColors.Remove(Start.Value);};
                      LeftFarColors.Add(End.Value);
                      End.Disposing   += (field) => {LeftFarColors.Remove(End.Value);};
                      LeftNearVars.Add(FStart.VariableName);
                      Start.Disposing += (field) => {LeftNearVars.Remove(FStart.VariableName);};
                      LeftFarVars.Add(FEnd.VariableName);
                      End.Disposing   += (field) => {LeftFarVars.Remove(FEnd.VariableName);};
                    }

                    __instance.Enabled = true;
                });
            }

            [HarmonyPrefix]
            [HarmonyPatch("UpdateCursor")]
            static bool Prefix( InteractionLaser __instance,
                                SyncRef<StaticTexture2D> ____cursorTexture,
                                Sync<color> ____cursorTint,
                                SyncRef<Slot> ____cursorRoot,
                                LaserCursor? cursor
                                ) 
            {
              if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return true;
              if (!config.GetValue(ENABLED)) return true;
              if (!config.GetValue(CURSOR_ENABLED)) return true;

              var IsRight = __instance.Side == Chirality.Right;

              Uri[] RightCursors =   {config.GetValue(RIGHT_CURSOR),
                                      config.GetValue(RIGHT_GRAB),
                                      config.GetValue(RIGHT_GRAB_HOVER),
                                      config.GetValue(RIGHT_INTERACT),
                                      config.GetValue(RIGHT_SLIDER_BOTH),
                                      config.GetValue(RIGHT_SLIDER_HORIZONTAL),
                                      config.GetValue(RIGHT_SLIDER_VERTICAL),
                                      config.GetValue(RIGHT_TYPING)};

              Uri[] LeftCursors =    {config.GetValue(LEFT_CURSOR),
                                      config.GetValue(LEFT_GRAB),
                                      config.GetValue(LEFT_GRAB_HOVER),
                                      config.GetValue(LEFT_INTERACT),
                                      config.GetValue(LEFT_SLIDER_BOTH),
                                      config.GetValue(LEFT_SLIDER_HORIZONTAL),
                                      config.GetValue(LEFT_SLIDER_VERTICAL),
                                      config.GetValue(LEFT_TYPING)};

              Uri[] DesiredCursors = IsRight ? RightCursors : LeftCursors; 


              var oldUri = ((cursor != null) ? cursor.GetValueOrDefault().icon : null) ?? NeosAssets.Graphics.Icons.Laser.Cursor;
              var newUri = DesiredCursors[Array.IndexOf(DefaultCursors, oldUri)] ?? oldUri;


              ____cursorTexture.Target.URL.Value = newUri; 
              ____cursorTint.Value = ((cursor != null) ? cursor.GetValueOrDefault().tint : color.White);
              float num = ((cursor != null) ? cursor.GetValueOrDefault().size : 1f);
              float3 one = float3.One;
              float3 @float = (one) * num * (IsRight ? config.GetValue(RIGHT_CURSOR_SCALE) : config.GetValue(LEFT_CURSOR_SCALE));
              ____cursorRoot.Target.LocalScale = @float;
              

              return false;
            }
          
        }

        private static ValueField<color> SetUpLogix(Slot root,
                                       IField<color> Input,
                                       Sync<color> Field,
                                       color Desired,
                                       ValueField<bool> ReactiveToggle,
                                       bool IsStart) 
        {
          // Field <= (Input / Default) * Desired
          // This should be LogiX so the color is actually networked.

          var driver = root.AddSlot(IsStart? "Start" : "End");
          var Div = driver.AttachComponent<Div_Color>();
          var Mul = driver.AttachComponent<Mul_Color>();

          var Default = driver.AttachComponent<ValueRegister<color>>();
          Default.Value.Value = new color(.25f, 1f, 1f, 1f);

          var DesiredField = driver.AttachComponent<ValueField<color>>();
          DesiredField.Value.Value = Desired;

          Div.A.Target = (IElementContent<color>)Input;
          Div.B.Target = Default;

          Mul.A.Target = Div;
          Mul.B.Target = DesiredField.Value;

          var Conditional = driver.AttachComponent<Conditional_Color>();
            Conditional.OnTrue.Target     = Mul;
            Conditional.OnFalse.Target    = DesiredField.Value;
            Conditional.Condition.Target  = ReactiveToggle.Value;
 
          IDriverNode driverNode = driver.AttachComponent<DriverNode<color>>();
            driverNode.TrySetTarget(Field);
            driverNode.TryConnectSourceTo(Conditional);

          return DesiredField;
        }


        void OnConfigurationUpdate(ConfigurationChangedEvent @event)
        {
          if (!config.GetValue(ENABLED)) 
          {
            var DefaultColor = new color(.25f,1f,1f,1f);
            foreach(Sync<bool> b in ReactiveEnableds ) { b.Value = true;         };
            foreach(Sync<color> c in RightNearColors ) { c.Value = DefaultColor; };
            foreach(Sync<color> c in RightFarColors  ) { c.Value = DefaultColor; };
            foreach(Sync<color> c in LeftNearColors  ) { c.Value = DefaultColor; };
            foreach(Sync<color> c in LeftFarColors   ) { c.Value = DefaultColor; };
            foreach(Sync<string> s in RightFarVars   ) { s.Value = null; };
            foreach(Sync<string> s in RightNearVars  ) { s.Value = null; };
            foreach(Sync<string> s in LeftFarVars    ) { s.Value = null; };
            foreach(Sync<string> s in LeftNearVars   ) { s.Value = null; };
          }
          else 
          {

            foreach(Sync<bool> b in ReactiveEnableds) { if ( !b.Value == config.GetValue(REACTIVE  ))  b.Value = config.GetValue(REACTIVE  );};
            foreach(Sync<color> c in RightNearColors) { if (!(c.Value == config.GetValue(RIGHT_NEAR))) c.Value = config.GetValue(RIGHT_NEAR);};
            foreach(Sync<color> c in RightFarColors ) { if (!(c.Value == config.GetValue(RIGHT_FAR ))) c.Value = config.GetValue(RIGHT_FAR );};
            foreach(Sync<color> c in LeftNearColors ) { if (!(c.Value == config.GetValue(LEFT_NEAR ))) c.Value = config.GetValue(LEFT_NEAR );};
            foreach(Sync<color> c in LeftFarColors  ) { if (!(c.Value == config.GetValue(LEFT_FAR  ))) c.Value = config.GetValue(LEFT_FAR  );};
            foreach(Sync<string> s in RightNearVars ) { if (!(s.Value == config.GetValue(RIGHT_NEAR_VAR))) s.Value = config.GetValue(RIGHT_NEAR_VAR);};
            foreach(Sync<string> s in RightFarVars  ) { if (!(s.Value == config.GetValue(RIGHT_FAR_VAR ))) s.Value = config.GetValue(RIGHT_FAR_VAR );}; 
            foreach(Sync<string> s in LeftNearVars  ) { if (!(s.Value == config.GetValue(LEFT_NEAR_VAR ))) s.Value = config.GetValue(LEFT_NEAR_VAR );};
            foreach(Sync<string> s in LeftFarVars   ) { if (!(s.Value == config.GetValue(LEFT_FAR_VAR  ))) s.Value = config.GetValue(LEFT_FAR_VAR  );};
          }

          }
        }
    }

