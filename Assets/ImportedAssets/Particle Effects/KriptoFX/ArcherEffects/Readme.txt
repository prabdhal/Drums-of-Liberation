My email is "kripto289@gmail.com"
You can contact me for any questions.
My English is not very good, and if there are any translation errors, you can let me know :)


Pack includes prefabs of main effects + prefabs of collision effects (\Assets\KriptoFX\ArcherEffects\Prefabs).
Support platforms: all platforms (PC/Consoles/VR/Mobiles/URP/HDRP)
All effects tested on Oculus Rift CV1 with single and dual mode rendering and works correctly.


------------------------------------------------------------------------------------------------------------------------------------------
IMPORTANT settings for DEFAULT built-in rendering:

Before update please remove old version of the package (to avoiding useless editor scripts)

1) You need activate HDR rendering. Edit -> ProjectSettings -> Graphics -> select current build target -> uncheck "use default" for tier1, tier2, tier3 -> set "Use HDR = true"
2) Enable HDR rendering in the current camera. MainCamera -> "AllowHDR = true"
If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "post effect msaa".

3) Add postprocessing stack package to project. Window -> Package Manager -> PostProcessing -> Instal
4) MainCamera -> AddComponent -> "Post Processing Layer". For "Layer" you should set custom layer (for example UI, or Postprocessing)
5) Create empty Gameobject and set custom layer as in the camera processing layer (for example UI). Gameobject -> AddComponent -> "Post Process Volume".
Add included postprocessing profile to "Post Process Volume" "\Assets\KriptoFX\Realistic Effects Pack v1\PostProcess Profile.asset"

------------------------------------------------------------------------------------------------------------------------------------------
IMPORTANT settings for HDRP:

Before update please remove old version of the package (to avoiding useless editor scripts)

1) Import HDRP patch "\Assets\KriptoFX\ArcherEffects\HDRP and URP patches"
2) Add "Bloom" and "tonemapping ACES" posteffects.
Camera -> Add Component -> Volume -> "IsGlobal = true" -> set the profile "\Assets\KriptoFX\Realistic Effects Pack v1\PostProcess Profile.asset"

In unity 2019.3+ added new important "Threshold" parameter in the bloom posteffect. So you need change some settings of bloom posteffect for correct intencity rendering:

Threshold 1.5
Intencity 0.2-0.5
Scatter 0.8-0.9

------------------------------------------------------------------------------------------------------------------------------------------
IMPORTANT settings for URP:

Before update please remove old version of the package (to avoiding useless editor scripts)

1) Import URP patch "\Assets\KriptoFX\ArcherEffects\HDRP and URP patches"
2) For main camera you need set "Depth texture = ON" and "Opaque texture = ON"
3) You need activate HDR rendering. Edit -> ProjectSettings -> Graphics -> select current scriptable render pipeline settings file -> Quality -> set "HDR = true"
4) Add postprocessing stack package to project. Window -> Package Manager -> PostProcessing -> Instal
5) MainCamera -> AddComponent -> "Post Processing Layer". For "Layer" you should set custom layer (for example UI, or Postprocessing)
6) Create empty Gameobject and set custom layer as in the camera processing layer (for example UI). Gameobject -> AddComponent -> "Post Process Volume".
Add included postprocessing profile to "Post Process Volume" "\Assets\KriptoFX\Realistic Effects Pack v1\PostProcess Profile.asset"

Note: included profile works only with postprocessing stack v2 (which temporary unsupported in unity 2019.3 and should be supported on unity 2019.4)
In unity 2019.3+ added new postprocessing "Volume" stack. You need create a profile with important posteffects:
1) Bloom with follow settings
	Threshold 1
	Intencity 2
	Scatter 0.9
2) Tonemapping -> "ACES"
3) MainCamera -> "PostEffects=true"

------------------------------------------------------------------------------------------------------------------------------------------
Using effects:

Video tutorial how to use effects https://www.youtube.com/watch?v=AKQCNGEeAaE


Simple using (without characters and animations):

	1) Just drag and drop prefab of effect on scene and use that (for example, bufs or projectiles).

Using with characters (bow/arrow particles, projectiles, etc):

	1) You can use "animation events" for instantiating an effects in runtime using an animation. (I use this method in the demo scene)
	https://docs.unity3d.com/Manual/animeditor-AnimationEvents.html
	2) You need set the position and the rotation for an effects. I use hand bone position (or center position of arrow) and hand bone rotation.
	3) Some effects can be apllied to bow/arrow mesh. If some prefabs of effects have the script "AE_SetMeshToEffect",
	then you need set the bow mesh (if mesh type = bow) or set the arrow mesh (if mesh type = arrow)

	The package included demo bow with string. (\Assets\KriptoFX\ArcherEffects\DemoResources\Prefabs)
	For using, you need set the "hand bone" position for "AE_BowString" and set "InHand" when the character pulls the string.

For using effects in runtime, use follow code:

	"Instantiate(prefabEffect, position, rotation);"

Using projectile collision detection:
	void Start ()
	{
		var tm = GetComponentInChildren<AE_PhysicsMotion>(true);
		if (tm!=null) tm.CollisionEnter += Tm_CollisionEnter;
	}

	private void Tm_CollisionEnter(object sender, AE_PhysicsMotion.AE_CollisionInfo e)
	{
	        Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
	}

------------------------------------------------------------------------------------------------------------------------------------------
Effect modification:


	All effects includes helpers scripts (collision behaviour, light/shader animation etc) for work out of box.
	Also you can add additional scripts for easy change of base effects settings. Just add follow scripts to prefab of effect.

	AE_EffectSettingColor - for change color of effect (uses HUE color). Can be added on any effect.
	AE_EffectSettingProjectile - for change projectile physics parameters (speed, mass, etc).
	AE_EffectQuality - for effects optimisations. When you use few effects, you can use particles limit in scene.

