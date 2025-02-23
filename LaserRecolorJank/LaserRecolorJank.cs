using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using System;
using System.Collections.Generic;
using Elements.Core;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Operators;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes;
using FrooxEngine.FrooxEngine.ProtoFlux.CoreNodes;



// Thank you LeCloutPanda and everyone who helped me through testing and motivation

namespace LaserRecolorJank
{

	public class Patch : ResoniteMod
	{
		public override String Name => "LaserRecolorJank";

		public override String Author => "APnda";

		public override String Link => "https://github.com/Ap6661/LaserRecolorJank";

		public override String Version => "2.0.4";

		private static Uri[] DefaultCursors = { OfficialAssets.Graphics.Icons.Laser.Cursor,
			OfficialAssets.Graphics.Icons.Laser.GrabCursor,
			OfficialAssets.Graphics.Icons.Laser.GrabHoverCursor,
			OfficialAssets.Graphics.Icons.Laser.InteractCursor,
			OfficialAssets.Graphics.Icons.Laser.SliderBothCursor,
			OfficialAssets.Graphics.Icons.Laser.SliderHorizontalCursor,
			OfficialAssets.Graphics.Icons.Laser.SliderVerticalCursor,
			OfficialAssets.Graphics.Icons.Laser.TypingCursor};

		private static List<Sync<bool>>   ReactiveEnableds = new List<Sync<bool>>();
		private static List<Sync<colorX>> RightNearColors  = new List<Sync<colorX>>();
		private static List<Sync<colorX>> RightFarColors   = new List<Sync<colorX>>();
		private static List<Sync<colorX>> LeftNearColors   = new List<Sync<colorX>>();
		private static List<Sync<colorX>> LeftFarColors    = new List<Sync<colorX>>();
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
		private static ModConfigurationKey<colorX> RIGHT_NEAR = new ModConfigurationKey<colorX>("right_near", "Right Near Color", () => new colorX(.25F, 1F, 1F, 1F));
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<colorX> RIGHT_FAR = new ModConfigurationKey<colorX>("right_far", "Right Far Color", () => new colorX(.25F, 1F, 1F, 1F));

		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> RIGHT_NEAR_VAR = new ModConfigurationKey<string>("right_near_var", "Right Near Color Dynamic Variable", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> RIGHT_FAR_VAR = new ModConfigurationKey<string>("right_far_var", "Right Far Color Dynamic Variable", () => "");

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
		private static ModConfigurationKey<colorX> LEFT_NEAR = new ModConfigurationKey<colorX>("left_near", "Left Near Color", () => new colorX(.25F, 1F, 1F, 1F));
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<colorX> LEFT_FAR = new ModConfigurationKey<colorX>("left_far", "Left Far Color", () => new colorX(.25F, 1F, 1F, 1F));

		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> LEFT_NEAR_VAR = new ModConfigurationKey<string>("left_near_var", "Left Near Color Dynamic Variable", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> LEFT_FAR_VAR = new ModConfigurationKey<string>("left_far_var", "Left Far Color Dynamic Variable", () => "");

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
					FieldDrive<colorX> ____startColor,
					FieldDrive<colorX> ____endColor,
					FieldDrive<float3> ____directPoint,
					FieldDrive<float3> ____actualPoint)
			{
				__instance.RunInUpdates(3, () =>
						{
						if (!config.GetValue(ENABLED)) return;
						if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser) return;

						var Assets = __instance.Slot.AddSlot("Assets");
						var ColE = Assets.AttachComponent<ValueField<colorX>>();
						var ColS = Assets.AttachComponent<ValueField<colorX>>();
						var Mesh = Assets.AttachComponent<BentTubeMesh>();
						var Renderer = __instance.Slot.GetComponent<MeshRenderer>().Mesh;

						Renderer.Target = Mesh;
						Mesh.Radius.Value = 0.002f;
						Mesh.Sides.Value = 6;
						Mesh.Segments.Value = 16;

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

						var FStart = Start.Slot.AttachComponent<DynamicField<colorX>>();
						var FEnd   = End.Slot.AttachComponent<DynamicField<colorX>>();

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
					Sync<colorX> ____cursorTint,
					SyncRef<Slot> ____cursorRoot,
					SyncRef<Slot> ____cursorImageRoot,
					SyncRef<Slot> ____directCursorImageRoot,
					SyncRef<SegmentMesh> ____directLineMesh,
					InteractionCursor? interactionCursor
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


        __instance.CurrentCursor = interactionCursor;
        InteractionCursor? interactionCursor2 = interactionCursor;
        InteractionCursor interactionCursor3;
        if (interactionCursor2 == null)
        {
          IInteractionCursorFactory cursorFactory = __instance.CursorFactory;
          interactionCursor3 = ((cursorFactory != null) ? cursorFactory.Default(1f, null) : InteractionCursor.Default);
        }
        else
        {
          interactionCursor3 = interactionCursor2.GetValueOrDefault();
        }
        InteractionCursor interactionCursor4 = interactionCursor3;

        var oldUri = interactionCursor4.icon;;
        var newUri = DesiredCursors[Array.IndexOf(DefaultCursors, oldUri)] ?? oldUri;

        ____cursorTexture.Target.URL.Value = newUri; 
        ____cursorTint.Value = interactionCursor4.tint;
        float size = interactionCursor4.size;
        float3 one = float3.One;
        float3 @float = one * size;
        ____cursorImageRoot.Target.LocalScale = @float;
        ____directCursorImageRoot.Target.LocalScale = (@float) * 0.75f;
        ____directLineMesh.Target.Radius.Value = size * 0.05f;


        return false;
			}

		}

		private static ValueField<colorX> SetUpLogix(Slot root,
				IField<colorX> Input,
				Sync<colorX> Field,
				colorX Desired,
				ValueField<bool> ReactiveToggle,
				bool IsStart) 
		{
			// Field <= (Input / Default) * Desired
			// This should be LogiX so the color is actually networked.
			var driver = root.AddSlot(IsStart? "Start" : "End");

			var InputSource = driver.AttachComponent<ValueSource<colorX>>();
				InputSource.TrySetRootSource(Input);

			var Default = driver.AttachComponent<ValueField<colorX>>();
				Default.Value.Value = new colorX(.25f, 1f, 1f, 1f);
			var DefaultSource = driver.AttachComponent<ValueSource<colorX>>();
				DefaultSource.TrySetRootSource(Default.Value);

			var DesiredField = driver.AttachComponent<ValueField<colorX>>();
				DesiredField.Value.Value = Desired;
			var DesiredSource = driver.AttachComponent<ValueSource<colorX>>();
				DesiredSource.TrySetRootSource(DesiredField.Value);

			var ReactiveSource = driver.AttachComponent<ValueSource<bool>>();
				ReactiveSource.TrySetRootSource(ReactiveToggle.Value);

			var Div = driver.AttachComponent<ValueDiv<colorX>>();
			var Mul = driver.AttachComponent<ValueMul<colorX>>();

			Div.A.TrySet(InputSource);
			Div.B.TrySet(DefaultSource);

			Mul.A.TrySet(Div);
			Mul.B.TrySet(DesiredSource);


			var Conditional = driver.AttachComponent<ValueConditional<colorX>>();
				Conditional.OnTrue.TrySet(Mul);
				Conditional.OnFalse.TrySet(DesiredSource);
				Conditional.Condition.TrySet(ReactiveSource);


			var Driver = driver.AttachComponent<ValueFieldDrive<colorX>>();
				Driver.Value.TrySet(Conditional);
				Driver.TrySetRootTarget(Field);

			return DesiredField;


		}

		void OnConfigurationUpdate(ConfigurationChangedEvent @event)
		{
			if (!config.GetValue(ENABLED)) 
			{
				var DefaultColor = new colorX(.25f, 1f, 1f, 1f);
				foreach (Sync<bool> b in ReactiveEnableds) { b.World.RunSynchronously(delegate { b.Value = true; }); };
		    		foreach (Sync<colorX> c in RightNearColors) { c.World.RunSynchronously(delegate { c.Value = DefaultColor; }); };
				foreach (Sync<colorX> c in RightFarColors) { c.World.RunSynchronously(delegate { c.Value = DefaultColor; }); };
				foreach (Sync<colorX> c in LeftNearColors) { c.World.RunSynchronously(delegate { c.Value = DefaultColor; }); };
				foreach (Sync<colorX> c in LeftFarColors) { c.World.RunSynchronously(delegate { c.Value = DefaultColor; }); };
				foreach (Sync<string> s in RightFarVars) { s.World.RunSynchronously(delegate { s.Value = null; }); };
				foreach (Sync<string> s in RightNearVars) { s.World.RunSynchronously(delegate { s.Value = null; }); };
				foreach (Sync<string> s in LeftFarVars) { s.World.RunSynchronously(delegate { s.Value = null; }); };
				foreach (Sync<string> s in LeftNearVars) { s.World.RunSynchronously(delegate { s.Value = null; }); };
			}
			else 
			{
				foreach (Sync<bool> b in ReactiveEnableds) { if (!b.Value == config.GetValue(REACTIVE)) b.World.RunSynchronously(delegate { b.Value = config.GetValue(REACTIVE); }); };
				foreach (Sync<colorX> c in RightNearColors) { if (!(c.Value == config.GetValue(RIGHT_NEAR))) c.World.RunSynchronously(delegate { c.Value = config.GetValue(RIGHT_NEAR); });};
				foreach (Sync<colorX> c in RightFarColors) { if (!(c.Value == config.GetValue(RIGHT_FAR))) c.World.RunSynchronously(delegate { c.Value = config.GetValue(RIGHT_FAR); }); };
				foreach (Sync<colorX> c in LeftNearColors) { if (!(c.Value == config.GetValue(LEFT_NEAR))) c.World.RunSynchronously(delegate { c.Value = config.GetValue(LEFT_NEAR); }); };
				foreach (Sync<colorX> c in LeftFarColors) { if ( !(c.Value == config.GetValue(LEFT_FAR))) c.World.RunSynchronously(delegate { c.Value = config.GetValue(LEFT_FAR); }); };
				foreach (Sync<string> s in RightNearVars) { if ( !(s.Value == config.GetValue(RIGHT_NEAR_VAR))) s.World.RunSynchronously(delegate { s.Value = config.GetValue(RIGHT_NEAR_VAR); }); };
				foreach (Sync<string> s in RightFarVars) { if ( !(s.Value == config.GetValue(RIGHT_FAR_VAR))) s.World.RunSynchronously(delegate { s.Value = config.GetValue(RIGHT_FAR_VAR); }); };
				foreach (Sync<string> s in LeftNearVars) { if ( !(s.Value == config.GetValue(LEFT_NEAR_VAR))) s.World.RunSynchronously(delegate { s.Value = config.GetValue(LEFT_NEAR_VAR); }); };
				foreach (Sync<string> s in LeftFarVars) { if ( !(s.Value == config.GetValue(LEFT_FAR_VAR))) s.World.RunSynchronously(delegate { s.Value = config.GetValue(LEFT_FAR_VAR); }); };			
			}

		}
	}
}

